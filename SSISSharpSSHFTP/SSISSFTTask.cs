using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using DTSExecResult = Microsoft.SqlServer.Dts.Runtime.DTSExecResult;
using DTSProductLevel = Microsoft.SqlServer.Dts.Runtime.DTSProductLevel;
using VariableDispenser = Microsoft.SqlServer.Dts.Runtime.VariableDispenser;

namespace SSISSFTPTask110.SSIS
{
    [DtsTask(
        DisplayName = "SFTP Task",
        UITypeName = "SSISSFTPTask110.SSISSFTTaskUIInterface" +
        ",SSISSFTPTask110," +
        "Version=2.0.0.30," +
        "Culture=Neutral," +
        "PublicKeyToken=f7871de73e053501",
        IconResource = "SSISSFTPTask110.sftp.ico",
        TaskContact = "cosmin.vlasiu@gmail.com",
        RequiredProductLevel = DTSProductLevel.None
        )]
    public class SSISSFTTask : Task, IDTSComponentPersist
    {
        #region Constructor
        public SSISSFTTask()
        {
            _vars = null;
        }

        #endregion

        #region Public Properties

        #region Connector
        [Category("Connection"), Description("SFTP Server")]
        public string SFTPServer { get; set; }
        [Category("Connection"), Description("SFTP Username")]
        public string SFTPUser { get; set; }
        [Category("Connection"), Description("SFTP Password")]
        public string SFTPPassword { get; set; }
        [Category("Connection"), Description("SFTP Port / By default 22")]
        public string SFTPPort { get; set; }

        [Category("Connection"), Description("Encryption Type (Password/Key)")]
        public string EncryptionType { get; set; }

        [Category("Connection"), Description("The Path to the Private Key File")]
        public string PublicKeyFile { get; set; }
        [Category("Connection"), Description("Private Key File Is ConnectionFile Type")]
        public string PublicKeyFileIsConnectionFileType { get; set; }
        [Category("Connection"), Description("Pass Phrase")]
        public string PassPhrase { get; set; }

        [Category("Connection"), Description("Sleep on disconnect / To avoid overload connections. Give it the time to disconnect completely.")]
        public string SleepOnDisconnect { get; set; }
        [Category("Connection"), Description("Sleep seconds")]
        public string SleepSeconds { get; set; }

        #endregion

        #region Recordset

        [Category("Recordset"), Description("Enable Recordset")]
        public bool RecordsetEnabled { get; set; }
        [Category("Recordset"), Description("Recordset Variable")]
        public string RecordsetVariable { get; set; }
        [Category("Recordset"), Description("Recordset Column Index")]
        public int RecordsetColumnIndex { get; set; }
        [Category("Recordset"), Description("Values Is FullPath?")]
        public bool ValueIsFullPath { get; set; }
        #endregion

        #region Action

        [Category("File Transfer"), Description("Action list"), ReadOnly(true)]
        public string TaskAction { get; set; }

        [Category("File Transfer"), Description("Local File")]
        public string LocalPath { get; set; }

        [Category("File Transfer"), Description("Source from File Connector or from Variable/Expression")]
        public string LocalPathIsConnectionFileType { get; set; }

        [Category("File Transfer"), Description("Remote File")]
        public string RemotePath { get; set; }

        [Category("File Transfer"), Description("Get File List From an Remote Path // Object type variable")]
        public string FilesList { get; set; }

        [Category("File Transfer"), Description("Overwrite local file (for the GetFile action)")]
        public string OverwriteLocalPath { get; set; }

        [Category("File Transfer"), Description("Delete the file on Transfer Completed")]
        public string DeleteFileOnTransferCompleted { get; set; }

        [Category("File Transfer"), Description("Indicates if the copy will be recursive")]
        public bool RecursiveCopy { get; set; }

        [Category("File Transfer"), Description("If RecursiveCopy is TRUE, please indicate de depth...(0 = current folder)")]
        public int RecursiveCopyDepth { get; set; }

        #endregion

        #endregion

        #region Private Properties

        Variables _vars;

        #endregion

        #region Validate

        /// <summary>
        /// Validate local properties
        /// </summary>
        public override DTSExecResult Validate(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log)
        {
            bool isBaseValid = true;

            if (base.Validate(connections, variableDispenser, componentEvents, log) != DTSExecResult.Success)
            {
                componentEvents.FireError(0, "SSISSFTTask", "Base validation failed", "", 0);
                isBaseValid = false;
            }

            #region Check connection
            if (string.IsNullOrEmpty(SFTPServer))
            {
                componentEvents.FireError(0, "SSISSFTTask", "SFTP Server is required.", "", 0);
                isBaseValid = false;
            }

            if (string.IsNullOrEmpty(SFTPUser))
            {
                componentEvents.FireError(0, "SSISSFTTask", "SFTP User is required.", "", 0);
                isBaseValid = false;
            }

            if (EncryptionType == Keys.ENCRYPTION_TYPE_PASSWORD)
            {
                if (string.IsNullOrEmpty(SFTPPassword))
                {
                    componentEvents.FireError(0, "SSISSFTTask", "SFTP User is required.", "", 0);
                    isBaseValid = false;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(PublicKeyFile))
                {
                    componentEvents.FireError(0, "SSISSFTTask", "PublicKeyFile is required.", "", 0);
                    isBaseValid = false;
                }
            }

            #endregion

            #region TaskAction Check
            if (string.IsNullOrEmpty(TaskAction))
            {
                componentEvents.FireError(0, "SSISSFTTask", "No action choosed.", "", 0);
                isBaseValid = false;
            }

            if (TaskAction == Communication.ActionTask[0] || TaskAction == Communication.ActionTask[1])
            {
                if (string.IsNullOrEmpty(LocalPath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a source file or source folder for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }

                if (string.IsNullOrEmpty(RemotePath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please chosse a destination file or destination folder for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[2])
            {
                if (string.IsNullOrEmpty(RemotePath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a remote folder for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[3])
            {
                if (string.IsNullOrEmpty(LocalPath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a local folder for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[4])
            {
                if (string.IsNullOrEmpty(RemotePath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a remote folder for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[5])
            {
                if (string.IsNullOrEmpty(LocalPath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a local folder for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[6])
            {
                if (string.IsNullOrEmpty(RemotePath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a remote path for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[7])
            {
                if (string.IsNullOrEmpty(LocalPath))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please choose a local file for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            if (TaskAction == Communication.ActionTask[8])
            {
                if (string.IsNullOrEmpty(FilesList))
                {
                    componentEvents.FireError(0, "SSISSFTTask", string.Format("Please specify an Object Variable for action {0}.", TaskAction), "", 0);
                    isBaseValid = false;
                }
            }

            #endregion

            #region Recordset
            if (RecordsetEnabled)
            {
                if (string.IsNullOrEmpty(RecordsetVariable))
                {
                    componentEvents.FireError(0, "SSISSFTTask", "Please specify an Object Variable for Recordset's source.", "", 0);
                    isBaseValid = false;
                }
            }

            #endregion

            //bool refire = true;
            //var transaction = new object();
            //ExecutesSFTPCommands(componentEvents, refire, variableDispenser, connections, log, transaction);

            return isBaseValid ? DTSExecResult.Success : DTSExecResult.Failure;
        }

        #endregion

        #region Execute

        /// <summary>
        /// This method is a run-time method executed dtsexec.exe
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="variableDispenser"></param>
        /// <param name="componentEvents"></param>
        /// <param name="log"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public override DTSExecResult Execute(Connections connections, VariableDispenser variableDispenser, IDTSComponentEvents componentEvents, IDTSLogging log, object transaction)
        {
            bool refire = true;
            var dtExecuteResult = DTSExecResult.Failure;
            componentEvents.FireInformation(0, "SSISSFTTask", "START SFTP Task", string.Empty, 0, ref refire);
            System.Threading.Tasks.Task task = System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                dtExecuteResult = ExecutesSFTPCommands(componentEvents,
                                     refire,
                                     variableDispenser,
                                     connections,
                                     log,
                                     transaction);
            });

            System.Threading.Tasks.Task.WaitAll(task);

            return dtExecuteResult;
        }

        private DTSExecResult ExecutesSFTPCommands(IDTSComponentEvents componentEvents, bool refire, VariableDispenser variableDispenser, Connections connections, IDTSLogging log, object transaction)
        {
            //GetNeededVariables(variableDispenser);

            string localPathEx = string.Empty;
            Communication.ComponentEvents = componentEvents;
            Communication.RecursiveDepth = RecursiveCopyDepth;
            Communication.RecursiveCopy = RecursiveCopy;

            try
            {
                #region Initilization

                Communication.RecordsetHandler = new RecordsetHandlerObject
                {
                    RecordsetColumnIndex = RecordsetColumnIndex,
                    RecordsetEnabled = RecordsetEnabled,
                    RecordsetVariable = RecordsetVariable,
                    VariableDispenser = variableDispenser,
                    ValueIsFullPath = ValueIsFullPath
                };

                Communication.DeleteFileOnTransferCompleted = DeleteFileOnTransferCompleted == Keys.TRUE;

                if (!string.IsNullOrEmpty(LocalPath))
                {

                    localPathEx = LocalPathIsConnectionFileType == Keys.TRUE
                                      ? connections[LocalPath].ConnectionString
                                      : EvaluateExpression(LocalPath, variableDispenser).ToString();

                    componentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("The rendered local path is: {0}", localPathEx),
                                                    string.Empty, 0, ref refire);
                }


                componentEvents.FireInformation(0, "SSISSFTTask",
                                                string.Format("SFTP Server: \"{0}\", User \"{1}\"",
                                                              EvaluateExpression(SFTPServer, variableDispenser),
                                                              EvaluateExpression(SFTPUser, variableDispenser)),
                                                string.Empty, 0, ref refire);

                Communication.EncryptionTypeKey = EncryptionType == Keys.ENCRYPTION_TYPE_KEY;

                int port = 22;
                Int32.TryParse(EvaluateExpression(SFTPPort, variableDispenser).ToString(), out port);
                Communication.Port = port;

                componentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("SFTP port: {0}", port),
                                                    string.Empty, 0, ref refire);

                if (Communication.EncryptionTypeKey)
                {
                    componentEvents.FireInformation(0, "SSISSFTTask",
                                                    "The SFTP operation will be encrypted by a private key file",
                                                    string.Empty, 0, ref refire);


                    var keyPath = LocalPathIsConnectionFileType == Keys.TRUE
                                      ? connections[PublicKeyFile].ConnectionString
                                      : EvaluateExpression(PublicKeyFile, variableDispenser).ToString();

                    componentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("The path to the private key file is: {0}", keyPath),
                                string.Empty, 0, ref refire);

                    Communication.PublicKeyFilePath = EvaluateExpression(keyPath, variableDispenser).ToString();

                    componentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("The Pass Phrase is: {0}", EvaluateExpression(PassPhrase, variableDispenser)),
                                string.Empty, 0, ref refire);

                    Communication.PrivatePassPhrase = EvaluateExpression(PassPhrase, variableDispenser).ToString();
                }

                #endregion

                #region Send File

                if (TaskAction == Communication.ActionTask[0]) //SEND FILE
                {
                    if (RecordsetEnabled)
                        componentEvents.FireInformation(0, "SSISSFTTask", "Preparing to SEND some file(s) within a recordset", string.Empty, 0, ref refire);
                    else
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("Preparing to copy the file(s) {0} to {1}",
                                                                      ResolveLocalPath(localPathEx),

                                                                     ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);

                    Communication.RecordsetHandler.RootPath = ResolveLocalPath(localPathEx);

                    if (Communication.RecursiveDepth == null || Communication.RecursiveDepth <= 0)
                        Communication.RecursiveDepth = 1;

                    if (Communication.SendFileBySFtp(EvaluateExpression(SFTPServer, variableDispenser).ToString(),
                                                     EvaluateExpression(SFTPUser, variableDispenser).ToString(),
                                                     EvaluateExpression(SFTPPassword, variableDispenser).ToString(),
                                                     ResolveLocalPath(localPathEx),
                                                     ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())))
                    {
                        if (RecordsetEnabled)
                            componentEvents.FireInformation(0, "SSISSFTTask", "End of the SEND operation", string.Empty, 0, ref refire);
                        else
                            componentEvents.FireInformation(0, "SSISSFTTask",
                                                            string.Format("The file {0} has been copied to {1}",
                                                                          ResolveLocalPath(localPathEx),
                                                                          ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);
                    }
                    else
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format("The file {0} has not been copied to {1}",
                                                                ResolveLocalPath(localPathEx),
                                                                ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0);
                    }
                }

                #endregion

                #region Get File

                if (TaskAction == Communication.ActionTask[1]) //GET FILE
                {
                    if (RecordsetEnabled)
                        componentEvents.FireInformation(0, "SSISSFTTask", "Start of GET operation within a Recordset", string.Empty, 0, ref refire);
                    else
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("Preparing to copy the file {0} to {1}",
                                                                      ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString()),
                                                                      ResolveLocalPath(localPathEx)),
                                                        string.Empty, 0, ref refire);

                    Communication.RecordsetHandler.RootPath = ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString());

                    if (Communication.GetFileBySFtp(EvaluateExpression(SFTPServer, variableDispenser).ToString(),
                                                    EvaluateExpression(SFTPUser, variableDispenser).ToString(),
                                                    EvaluateExpression(SFTPPassword, variableDispenser).ToString(),
                                                    ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString()),
                                                    ResolveLocalPath(localPathEx),
                                                    OverwriteLocalPath == Keys.TRUE))
                    {
                        if (RecordsetEnabled)
                            componentEvents.FireInformation(0, "SSISSFTTask", "End of GET operation within a Recordset", string.Empty, 0, ref refire);
                        else
                            componentEvents.FireInformation(0, "SSISSFTTask",
                                                            string.Format("The file {0} has been copied to {1}",
                                                                          ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString()),
                                                                          ResolveLocalPath(localPathEx)),
                                                            string.Empty, 0, ref refire);
                    }
                    else
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format("The file {0} has not been copied to {1}",
                                                                ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString()),
                                                                ResolveLocalPath(localPathEx)),
                                                  string.Empty, 0);
                    }
                }

                #endregion

                #region create RemoteDir

                if (TaskAction == Communication.ActionTask[2]) // CREATE REMOTE DIR
                {

                    componentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("Preparing to create the remote folder {0} ",
                                                                  ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);

                    if (Communication.CreateFolderBySFtp(EvaluateExpression(SFTPServer, variableDispenser).ToString(),
                                                         EvaluateExpression(SFTPUser, variableDispenser).ToString(),
                                                         EvaluateExpression(SFTPPassword, variableDispenser).ToString(),
                                                         ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())))
                    {
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("The folder {0} has been created",
                                                                      ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);
                    }
                    else
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format("The folder {0} has not been created",
                                                                ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0);
                    }
                }

                #endregion

                #region Create Local Dir

                if (TaskAction == Communication.ActionTask[3]) // CREATE LOCAL DIR
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(localPathEx))
                        {

                            componentEvents.FireInformation(0, "SSISSFTTask",
                                                            string.Format("Preparing to create the local folder {0}: ",
                                                                          ResolveLocalPath(localPathEx)), string.Empty,
                                                            0, ref refire);

                            DirectoryInfo directoryInfo = Directory.CreateDirectory(localPathEx);
                            componentEvents.FireInformation(0, "SSISSFTTask",
                                                            directoryInfo.Exists
                                                                ? string.Format("The folder {0} has been created", ResolveLocalPath(localPathEx))
                                                                : string.Format("The folder {0} has not been created", ResolveLocalPath(localPathEx)),
                                                            string.Empty, 0, ref refire);
                        }
                    }
                    catch (Exception exception)
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format("The folder {0} has not been created. Exception detail {1} - {2}",
                                                                ResolveLocalPath(localPathEx), exception.Message, exception.Source),
                                                  string.Empty, 0);
                    }
                }

                #endregion

                #region Remove Remote Dir

                if (TaskAction == Communication.ActionTask[4]) //REMOVE REMOTE DIR
                {

                    componentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("Preparing to remove the remote folder {0} ",
                                                                  ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);

                    if (Communication.RemoveFolderBySFtp(EvaluateExpression(SFTPServer, variableDispenser).ToString(),
                                                         EvaluateExpression(SFTPUser, variableDispenser).ToString(),
                                                         EvaluateExpression(SFTPPassword, variableDispenser).ToString(),
                                                         ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())))
                    {
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("The remote folder {0} has been removed",
                                                                      ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);
                    }
                    else
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format("The remote folder {0} has not been removed",
                                                                ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0);
                    }
                }

                #endregion

                #region Remove local Dir

                if (TaskAction == Communication.ActionTask[5]) //REMOVE LOCAL FOLDER
                {
                    try
                    {
                        if (string.IsNullOrEmpty(localPathEx))
                        {
                            componentEvents.FireInformation(0, "SSISSFTTask",
                                                            string.Format("Preparing to remove the local folder {0} ",
                                                                          ResolveLocalPath(localPathEx)), string.Empty,
                                                            0, ref refire);
                            Directory.Delete(localPathEx);
                            componentEvents.FireInformation(0, "SSISSFTTask",
                                                            string.Format("The local folder {0} has been removed",
                                                                          ResolveLocalPath(localPathEx)),
                                                            string.Empty, 0, ref refire);
                        }
                    }
                    catch (Exception exception)
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format(
                                                      "The local folder {0} has not been removed. Exception detail {1} - {2}",
                                                      ResolveLocalPath(localPathEx), exception.Message, exception.Source),
                                                  string.Empty, 0);
                    }
                }

                #endregion

                #region Delete Remote File

                if (TaskAction == Communication.ActionTask[6]) //DELETE REMOTE FILE
                {
                    if (RecordsetEnabled)
                        componentEvents.FireInformation(0, "SSISSFTTask", "Start of the DELETE operation within a Recordset", string.Empty, 0, ref refire);
                    else
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("Preparing to delete the remote file {0}",
                                                                      ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);

                    Communication.RecordsetHandler.RootPath = ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString());

                    if (Communication.DeleteFileBySFtp(EvaluateExpression(SFTPServer, variableDispenser).ToString(),
                                                       EvaluateExpression(SFTPUser, variableDispenser).ToString(),
                                                       EvaluateExpression(SFTPPassword, variableDispenser).ToString(),
                                                       ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())))
                    {
                        if (RecordsetEnabled)
                            componentEvents.FireInformation(0, "SSISSFTTask", "END of DELETE operation within a Recordset", string.Empty, 0, ref refire);
                        else
                            componentEvents.FireInformation(0, "SSISSFTTask", string.Format("The remote file(s) {0} has been deleted", ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0, ref refire);
                    }
                    else
                    {
                        componentEvents.FireError(0, "SSISSFTTask", string.Format("The remote file {0} has not been deleted", ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())), string.Empty, 0);
                    }
                }

                #endregion

                #region Delete Local File

                if (TaskAction == Communication.ActionTask[7]) //DELETE LOCAL FILE
                {
                    try
                    {
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("Preparing to delete the local file {0}",
                                                                      ResolveLocalPath(localPathEx)), string.Empty, 0,
                                                        ref refire);
                        File.Delete(ResolveLocalPath(localPathEx));
                        componentEvents.FireInformation(0, "SSISSFTTask",
                                                        string.Format("The local file {0} has been deleted",
                                                                      ResolveLocalPath(localPathEx)), string.Empty, 0,
                                                        ref refire);
                    }
                    catch (Exception exception)
                    {
                        componentEvents.FireError(0, "SSISSFTTask",
                                                  string.Format(
                                                      "The local file {0} has not been deleted. Exception detail {1} - {2}",
                                                      ResolveLocalPath(localPathEx), exception.Message, exception.Source),
                                                  string.Empty, 0);
                    }
                }

                #endregion

                #region File List


                if (TaskAction == Communication.ActionTask[8]) //FILE LIST
                {
                    componentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format(
                                                        "Preparing to get the files list from the remote folder {0}",
                                                        ResolveRemotePath(EvaluateExpression(RemotePath, variableDispenser).ToString())),
                                                    string.Empty, 0, ref refire);

                    if (Communication.RecursiveDepth == null || Communication.RecursiveDepth <= 0)
                        Communication.RecursiveDepth = 1;

                    var retValue = Communication.GetFileListDepthFromSFtp(EvaluateExpression(SFTPServer, variableDispenser).ToString(),
                                                          EvaluateExpression(SFTPUser, variableDispenser).ToString(),
                                                          EvaluateExpression(SFTPPassword, variableDispenser).ToString(),
                                                          ResolveRemotePathEx(EvaluateExpression(RemotePath, variableDispenser).ToString()),
                                                          Communication.RecursiveDepth);

                    componentEvents.FireInformation(0, "SSISSFTTask", retValue.Count + " file(s) found:", string.Empty,
                                                    0, ref refire);
                    foreach (var file in retValue)
                    {
                        componentEvents.FireInformation(0, "SSISSFTTask", file, string.Empty, 0, ref refire);
                    }

                    var mappedParams = FilesList.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (string t in mappedParams)
                    {
                        try
                        {
                            string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                            variableDispenser.LockOneForWrite(param, ref _vars);
                        }
                        catch
                        {
                            //We will continue...
                        }
                    }

                    //variableDispenser.GetVariables(ref _vars);

                    _vars[GetVariableFromNamespaceContext(FilesList)].Value = retValue;

                    componentEvents.FireInformation(0, "SSISSFTTask",
                                                    GetVariableFromNamespaceContext(FilesList) + " has obtained the list of files", string.Empty, 0, ref refire);
                }

                #endregion

                #region End of process

                if (!string.IsNullOrEmpty(SleepOnDisconnect))
                {
                    componentEvents.FireInformation(0, "SSISSFTTask", string.Format("Sleep on disconnect? {0}", SleepOnDisconnect), string.Empty, 0, ref refire);

                    if (SleepOnDisconnect == Keys.TRUE)
                    {
                        componentEvents.FireInformation(0, "SSISSFTTask", "Preparing to sleep...", string.Empty, 0, ref refire);

                        if (!string.IsNullOrEmpty(SleepSeconds))
                        {
                            int seconds = Int32.TryParse(SleepSeconds,
                                                         NumberStyles.Integer,
                                                         CultureInfo.CurrentCulture,
                                                         out seconds)
                                              ? seconds
                                              : 0;
                            if (seconds > 0)
                            {
                                componentEvents.FireInformation(0, "SSISSFTTask", string.Format("It's time to sleep for {0} seconds", seconds), string.Empty, 0, ref refire);
                                Thread.Sleep(seconds * 1000);
                                componentEvents.FireInformation(0, "SSISSFTTask", "Wake-up...", string.Empty, 0, ref refire);
                            }
                            else
                            {
                                componentEvents.FireInformation(0, "SSISSFTTask", "The SleepSeconds value must be greater then 0!", string.Empty, 0, ref refire);
                            }
                        }
                    }
                }

                #endregion

                componentEvents.FireInformation(0, "SSISSFTTask", "SFTP Task ended succesfully", string.Empty, 0, ref refire);
            }
            catch (Exception ex)
            {
                componentEvents.FireError(0, "SSISSFTTask", string.Format("Error : {0}", ex.Message), "", 0);
                //componentEvents.FireError(0, "SSISSFTTask", string.Format("Problem & Source : {0} - {1} - {2}", ex.Message, ex.Source, ex.StackTrace), "", 0);
            }
            finally
            {
                if (_vars != null)
                    if (_vars.Locked != null)
                        if (_vars.Locked)
                            _vars.Unlock();
            }

            return base.Execute(connections, variableDispenser, componentEvents, log, transaction);
        }

        #endregion

        #region Methods
        /// <summary>
        /// This method evaluate expressions like @[User::DestinationFile] + @[System::FileNumber] or any other operation created using 
        /// ExpressionBuilder
        /// </summary>
        /// <param name="mappedParam"></param>
        /// <param name="variableDispenser"></param>
        /// <returns></returns>
        private static object EvaluateExpression(string mappedParam, VariableDispenser variableDispenser)
        {
            object variableObject;

            try
            {
                var expressionEvaluatorClass = new ExpressionEvaluator
                {
                    Expression = mappedParam
                };

                expressionEvaluatorClass.Evaluate(DtsConvert.GetExtendedInterface(variableDispenser), out variableObject, false);
            }
            catch (Exception) // for already initialized values
            {
                variableObject = mappedParam;
            }

            return variableObject;
        }

        /// <summary>
        /// Determines whether [is variable in lock for read or write] [the specified lock for read].
        /// </summary>
        /// <param name="lockForReadWrite"></param>
        /// <param name="variable">The variable.</param>
        /// <returns>
        /// 	<c>true</c> if [is variable in lock for read or write] [the specified lock for read]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsVariableInLockForReadOrWrite(List<string> lockForReadWrite, string variable)
        {
            bool retVal = lockForReadWrite.Contains(variable);

            if (!retVal)
            {
                lockForReadWrite.Add(variable);
            }

            return retVal;
        }

        /// <summary>
        /// Gets the needed variables.
        /// </summary>
        /// <param name="variableDispenser">The variable dispenser.</param>
        private void GetNeededVariables(VariableDispenser variableDispenser)
        {

            var lockForReadWrite = new List<string>();

            {
                var mappedParams = SFTPServer.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        if (!IsVariableInLockForReadOrWrite(lockForReadWrite, param))
                            variableDispenser.LockForRead(param);
                    }
                    catch (Exception)
                    {
                        //We will continue...
                    }
                }
            }

            {
                var mappedParams = SFTPUser.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        if (!IsVariableInLockForReadOrWrite(lockForReadWrite, param))
                            variableDispenser.LockForRead(param);
                    }
                    catch
                    {
                        //We will continue...
                    }
                }
            }

            {
                var mappedParams = SFTPPassword.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        if (!IsVariableInLockForReadOrWrite(lockForReadWrite, param))
                            variableDispenser.LockForRead(param);
                    }
                    catch
                    {
                        //We will continue...
                    }
                }
            }

            {
                var mappedParams = LocalPath.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        if (!IsVariableInLockForReadOrWrite(lockForReadWrite, param))
                            variableDispenser.LockForRead(param);
                    }
                    catch
                    {
                        //We will continue...
                    }
                }
            }

            {
                var mappedParams = RemotePath.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        if (!IsVariableInLockForReadOrWrite(lockForReadWrite, param))
                            variableDispenser.LockForRead(param);
                    }
                    catch
                    {
                        //We will continue...
                    }
                }
            }

            {
                var mappedParams = FilesList.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        if (!IsVariableInLockForReadOrWrite(lockForReadWrite, param))
                            variableDispenser.LockOneForWrite(param, ref _vars);
                    }
                    catch
                    {
                        //We will continue...
                    }
                }
            }

            //variableDispenser.GetVariables(ref _vars);
        }

        private static string GetVariableFromNamespaceContext(string ssisVariable)
        {
            return ssisVariable.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
        }

        private static string ResolveRemotePath(string path)
        {
            var stringBuilder = new StringBuilder();

            const string slash = "/";
            if (!path.StartsWith(slash))
                stringBuilder.Append(slash);

            stringBuilder.Append(path.Replace(@"\", slash));

            return stringBuilder.ToString().Trim();
        }

        private static string ResolveRemotePathEx(string path)
        {
            const string slash = "/";
            string retVal;
            if (path.Length > 0)
            {
                var stringBuilder = new StringBuilder();

                if (!path.StartsWith(slash))
                    stringBuilder.Append(slash);

                stringBuilder.Append(path);

                if (path.Contains("*") || path.Contains("?"))
                {

                }
                else
                    if (!path.EndsWith(slash))
                        stringBuilder.Append(slash);

                retVal = stringBuilder.ToString().Trim();
            }
            else
            {
                retVal = slash;
            }
            return retVal;
        }

        private static string ResolveLocalPath(string path)
        {
            //StringBuilder stringBuilder = new StringBuilder();

            //const string slash = "/";

            //stringBuilder.Append(Path);

            //if (!Path.EndsWith(slash))
            //    stringBuilder.Append(slash);

            //return stringBuilder.ToString();
            return path.Trim();
        }

        #endregion

        #region Implementation of IDTSComponentPersist

        public void SaveToXML(XmlDocument doc, IDTSInfoEvents infoEvents)
        {
            XmlElement taskElement = doc.CreateElement(string.Empty, "SSISSFTPTask", string.Empty);

            XmlAttribute sftpServer = doc.CreateAttribute(string.Empty, Keys.FTP_SERVER, string.Empty);
            sftpServer.Value = SFTPServer;

            XmlAttribute sftpUser = doc.CreateAttribute(string.Empty, Keys.FTP_USER, string.Empty);
            sftpUser.Value = SFTPUser;

            XmlAttribute sftpPassword = doc.CreateAttribute(string.Empty, Keys.FTP_PASSWORD, string.Empty);
            sftpPassword.Value = SFTPPassword;

            XmlAttribute sftpPort = doc.CreateAttribute(string.Empty, Keys.FTP_PORT, string.Empty);
            sftpPort.Value = SFTPPort;

            XmlAttribute sftpSourceFile = doc.CreateAttribute(string.Empty, Keys.FTP_LOCAL_PATH, string.Empty);
            sftpSourceFile.Value = LocalPath;

            XmlAttribute sftpSourceFileType = doc.CreateAttribute(string.Empty, Keys.FTP_LOCAL_PATH_SOURCE_TYPE, string.Empty);
            sftpSourceFileType.Value = LocalPathIsConnectionFileType;

            XmlAttribute sftpDestinationFile = doc.CreateAttribute(string.Empty, Keys.FTP_REMOTE_PATH, string.Empty);
            sftpDestinationFile.Value = RemotePath;

            XmlAttribute sftpActionLists = doc.CreateAttribute(string.Empty, Keys.FTP_ACTION_LIST, string.Empty);
            sftpActionLists.Value = TaskAction;

            XmlAttribute sftpFileLists = doc.CreateAttribute(string.Empty, Keys.FTP_FILES_LIST, string.Empty);
            sftpFileLists.Value = FilesList;

            XmlAttribute overWriteLocalPath = doc.CreateAttribute(string.Empty, Keys.FTP_LOCAL_PATH_OVERWRITE, string.Empty);
            overWriteLocalPath.Value = OverwriteLocalPath;

            XmlAttribute deleteFileOnTransferCompleted = doc.CreateAttribute(string.Empty, Keys.DeleteFileOnTransferCompleted, string.Empty);
            deleteFileOnTransferCompleted.Value = DeleteFileOnTransferCompleted;

            XmlAttribute sleepSeconds = doc.CreateAttribute(string.Empty, Keys.SLEEP_SECONDS, string.Empty);
            sleepSeconds.Value = SleepSeconds;

            XmlAttribute sleepOnDisconnect = doc.CreateAttribute(string.Empty, Keys.SLEEP_ON_DISCONNECT, string.Empty);
            sleepOnDisconnect.Value = SleepOnDisconnect;

            XmlAttribute encryptionType = doc.CreateAttribute(string.Empty, Keys.ENCRYPTION_TYPE, string.Empty);
            encryptionType.Value = EncryptionType;

            XmlAttribute publicKeyFile = doc.CreateAttribute(string.Empty, Keys.PRIVATE_KEY_FILE, string.Empty);
            publicKeyFile.Value = PublicKeyFile;

            XmlAttribute publicKeyFileIsConnectionFileType = doc.CreateAttribute(string.Empty, Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION, string.Empty);
            publicKeyFileIsConnectionFileType.Value = PublicKeyFileIsConnectionFileType;

            XmlAttribute passPhrase = doc.CreateAttribute(string.Empty, Keys.PASS_PHRASE, string.Empty);
            passPhrase.Value = PassPhrase;

            XmlAttribute recordsetColumnIndex = doc.CreateAttribute(string.Empty, Keys.RecordsetColumnIndex, string.Empty);
            recordsetColumnIndex.Value = RecordsetColumnIndex.ToString();

            XmlAttribute recordsetEnabled = doc.CreateAttribute(string.Empty, Keys.RecordsetEnabled, string.Empty);
            recordsetEnabled.Value = RecordsetEnabled.ToString();

            XmlAttribute recordsetVariable = doc.CreateAttribute(string.Empty, Keys.RecordsetVariable, string.Empty);
            recordsetVariable.Value = RecordsetVariable;

            XmlAttribute valueIsFullPath = doc.CreateAttribute(string.Empty, Keys.ValueIsFullPath, string.Empty);
            valueIsFullPath.Value = ValueIsFullPath.ToString();

            XmlAttribute recursiveCopy = doc.CreateAttribute(string.Empty, Keys.RecursiveCopy, string.Empty);
            recursiveCopy.Value = RecursiveCopy.ToString();

            XmlAttribute recursiveCopyDepth = doc.CreateAttribute(string.Empty, Keys.RecursiveCopyDepth, string.Empty);
            recursiveCopyDepth.Value = RecursiveCopyDepth.ToString();

            taskElement.Attributes.Append(sftpServer);
            taskElement.Attributes.Append(sftpUser);
            taskElement.Attributes.Append(sftpPassword);
            taskElement.Attributes.Append(sftpPort);

            taskElement.Attributes.Append(encryptionType);
            taskElement.Attributes.Append(publicKeyFile);
            taskElement.Attributes.Append(publicKeyFileIsConnectionFileType);
            taskElement.Attributes.Append(passPhrase);

            taskElement.Attributes.Append(sftpSourceFile);
            taskElement.Attributes.Append(sftpSourceFileType);
            taskElement.Attributes.Append(overWriteLocalPath);
            taskElement.Attributes.Append(deleteFileOnTransferCompleted);
            taskElement.Attributes.Append(sftpDestinationFile);
            taskElement.Attributes.Append(sftpActionLists);
            taskElement.Attributes.Append(sftpFileLists);
            taskElement.Attributes.Append(sleepSeconds);
            taskElement.Attributes.Append(sleepOnDisconnect);

            taskElement.Attributes.Append(recordsetColumnIndex);
            taskElement.Attributes.Append(recordsetEnabled);
            taskElement.Attributes.Append(recordsetVariable);
            taskElement.Attributes.Append(valueIsFullPath);

            taskElement.Attributes.Append(recursiveCopy);
            taskElement.Attributes.Append(recursiveCopyDepth);

            doc.AppendChild(taskElement);
        }

        public void LoadFromXML(XmlElement node, IDTSInfoEvents infoEvents)
        {
            if (node.Name != "SSISSFTPTask")
            {
                throw new Exception("Unexpected task element when loading task.");
            }

            try
            {
                SFTPServer = node.Attributes.GetNamedItem(Keys.FTP_SERVER).Value;
                SFTPUser = node.Attributes.GetNamedItem(Keys.FTP_USER).Value;
                SFTPPassword = node.Attributes.GetNamedItem(Keys.FTP_PASSWORD).Value;
                SFTPPort = node.Attributes.GetNamedItem(Keys.FTP_PORT).Value;

                EncryptionType = node.Attributes.GetNamedItem(Keys.ENCRYPTION_TYPE).Value;
                PublicKeyFile = node.Attributes.GetNamedItem(Keys.PRIVATE_KEY_FILE).Value;
                PublicKeyFileIsConnectionFileType = node.Attributes.GetNamedItem(Keys.PRIVATE_KEY_FILE_FROM_FILE_CONNECTION).Value;
                PassPhrase = node.Attributes.GetNamedItem(Keys.PASS_PHRASE).Value;

                TaskAction = node.Attributes.GetNamedItem(Keys.FTP_ACTION_LIST).Value;
                LocalPath = node.Attributes.GetNamedItem(Keys.FTP_LOCAL_PATH).Value;
                LocalPathIsConnectionFileType = node.Attributes.GetNamedItem(Keys.FTP_LOCAL_PATH_SOURCE_TYPE).Value;
                OverwriteLocalPath = node.Attributes.GetNamedItem(Keys.FTP_LOCAL_PATH_OVERWRITE).Value;
                DeleteFileOnTransferCompleted = node.Attributes.GetNamedItem(Keys.DeleteFileOnTransferCompleted).Value;
                RemotePath = node.Attributes.GetNamedItem(Keys.FTP_REMOTE_PATH).Value;
                FilesList = node.Attributes.GetNamedItem(Keys.FTP_FILES_LIST).Value;
                SleepOnDisconnect = node.Attributes.GetNamedItem(Keys.SLEEP_ON_DISCONNECT).Value;
                SleepSeconds = node.Attributes.GetNamedItem(Keys.SLEEP_SECONDS).Value;

                RecursiveCopy = Convert.ToBoolean(node.Attributes.GetNamedItem(Keys.RecursiveCopy).Value);
                RecursiveCopyDepth = Convert.ToInt32(node.Attributes.GetNamedItem(Keys.RecursiveCopyDepth).Value);

                //if (RecursiveCopyDepth == 0)
                //    RecursiveCopyDepth = 1;

                RecordsetColumnIndex = Convert.ToInt32(node.Attributes.GetNamedItem(Keys.RecordsetColumnIndex).Value);
                RecordsetVariable = node.Attributes.GetNamedItem(Keys.RecordsetVariable).Value;

                bool b = false;

                bool.TryParse(node.Attributes.GetNamedItem(Keys.RecordsetEnabled).Value, out b);
                RecordsetEnabled = b;

                bool.TryParse(node.Attributes.GetNamedItem(Keys.ValueIsFullPath).Value, out b);
                ValueIsFullPath = b;

            }
            catch (Exception)
            {

            }
        }

        #endregion
    }
}
