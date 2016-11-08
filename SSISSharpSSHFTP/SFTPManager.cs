using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Runtime;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace SSISSFTPTask110.SSIS
{
    public static class Communication
    {

        private static int _port;
        public static RecordsetHandlerObject RecordsetHandler { get; set; }
        public static string PublicKeyFilePath { get; set; }
        public static string PrivatePassPhrase { get; set; }
        public static bool EncryptionTypeKey { get; set; }
        public static IDTSComponentEvents ComponentEvents { get; set; }
        public static bool DeleteFileOnTransferCompleted { get; set; }
        public static bool RecursiveCopy { get; set; }
        public static int RecursiveDepth { get; set; }

        public static int Port
        {
            get
            {
                if (_port == 0)
                    _port = 22;

                return _port;
            }
            set
            {
                _port = value;
            }
        }

        public static readonly List<string> ActionTask = new List<string>
                             {
                                  "Send File",
                                  "Get File",
                                  "Create Remote Directory",
                                  "Create Local Directory",
                                  "Remove Remote Directory",
                                  "Remove Local Directory",
                                  "Delete Remote File",
                                  "Delete Local File",
                                  "Get Files List From the Remote Folder" 
                             };

        private static void AddEncryptionIfAvailable(Sftp sftp)
        {
            if (EncryptionTypeKey)
            {
                if (!string.IsNullOrEmpty(PublicKeyFilePath) && !string.IsNullOrEmpty(PrivatePassPhrase))
                    sftp.AddIdentityFile(PublicKeyFilePath, PrivatePassPhrase);

                if (!string.IsNullOrEmpty(PublicKeyFilePath) && string.IsNullOrEmpty(PrivatePassPhrase))
                    sftp.AddIdentityFile(PublicKeyFilePath);

                if (string.IsNullOrEmpty(PublicKeyFilePath) && string.IsNullOrEmpty(PrivatePassPhrase))
                    throw new SftpException(1, "You choosed the connection encryption type to be provided by a key file. Please specify at least the path to the file.");
            }
        }

        public static bool SendFileBySFtp(string url, string login, string password, string sourceFileName, string outputFileName)
        {
            bool retVal = false;

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;
            Sftp.RecordsetHandler = RecordsetHandler;
            Sftp.DeleteFileOnTransferCompleted = DeleteFileOnTransferCompleted;
            Sftp.RecursiveCopy = RecursiveCopy;
            Sftp.RecursiveDepth = RecursiveDepth;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();
                sftp.Put(sourceFileName, outputFileName);
                retVal = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }

        public static bool GetFileBySFtp(string url, string login, string password, string sourceFileName, string outputFileName)
        {
            bool retVal = false;

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;
            Sftp.RecordsetHandler = RecordsetHandler;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();
                sftp.Get(sourceFileName, outputFileName);
                retVal = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }

        public static bool GetFileBySFtp(string url, string login, string password, string sourceFileName, string outputFileName, bool overwrite)
        {
            bool retVal = false;

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;
            Sftp.RecordsetHandler = RecordsetHandler;
            Sftp.DeleteFileOnTransferCompleted = DeleteFileOnTransferCompleted;
            Sftp.RecursiveCopy = RecursiveCopy;
            Sftp.RecursiveDepth = RecursiveDepth;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();

                sftp.Get(sourceFileName, outputFileName);

                retVal = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }

        public static bool DeleteFileBySFtp(string url, string login, string password, string sourceFileName)
        {
            bool retVal = false;

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;
            Sftp.RecordsetHandler = RecordsetHandler;
            Sftp.RecursiveDepth = RecursiveDepth;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();
                sftp.Delete(sourceFileName);
                retVal = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }

        public static bool RemoveFolderBySFtp(string url, string login, string password, string folderPath)
        {
            bool retVal = false;

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();
                sftp.RemoveDir(folderPath);
                retVal = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }

        public static bool CreateFolderBySFtp(string url, string login, string password, string folderPath)
        {
            bool retVal = false;

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();
                sftp.Mkdir(folderPath);
                retVal = true;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }

        public static List<string> GetFileListFromSFtp(string url, string login, string password, string folderPath)
        {
            return GetFileListDepthFromSFtp(url, login, password, folderPath, 1);
        }

        public static List<string> GetFileListDepthFromSFtp(string url, string login, string password, string folderPath, int depth)
        {
            var retVal = new List<string>();

            Sftp sftp;

            if (EncryptionTypeKey)
            {
                sftp = new Sftp(url, login);
                AddEncryptionIfAvailable(sftp);
            }
            else
            {
                sftp = new Sftp(url, login, password);
            }

            sftp.ComponentEvents = ComponentEvents;

            try
            {
                sftp.NewPort = Port;
                sftp.Connect();
                retVal = Tools.SFTPTools.SFTPScanDirs(sftp, folderPath, depth);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }
            finally
            {
                if (sftp.Connected)
                    sftp.Close();
            }

            return retVal;
        }
    }
}