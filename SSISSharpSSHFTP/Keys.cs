using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using Microsoft.SqlServer.Dts.Runtime;
using VariableDispenser = Microsoft.SqlServer.Dts.Runtime.VariableDispenser;

namespace SSISSFTPTask110
{
    internal static class Keys
    {
        public const string FTP_SERVER = "SFTPServer";

        public const string FTP_USER = "SFTPUser";
        public const string FTP_PASSWORD = "SFTPPassword";
        public const string FTP_PORT = "SFTPPort";

        public const string FTP_ACTION_LIST = "TaskAction";
        public const string FTP_LOCAL_PATH = "LocalPath";
        public const string FTP_LOCAL_PATH_SOURCE_TYPE = "LocalPathIsConnectionFileType";
        public const string FTP_LOCAL_PATH_OVERWRITE = "OverwriteLocalPath";
        public const string FTP_REMOTE_PATH = "RemotePath";

        public const string FTP_FILES_LIST = "FilesList";

        public const string FALSE = "False";
        public const string TRUE = "True";

        public const string ENCRYPTION_TYPE = "EncryptionType";
        public const string ENCRYPTION_TYPE_PASSWORD = "Password";
        public const string ENCRYPTION_TYPE_KEY = "Key";
        public const string PRIVATE_KEY_FILE = "PublicKeyFile";
        public const string PRIVATE_KEY_FILE_FROM_FILE_CONNECTION = "PublicKeyFileIsConnectionFileType";
        public const string PASS_PHRASE = "PassPhrase";

        public const string SLEEP_ON_DISCONNECT = "SleepOnDisconnect";
        public const string SLEEP_SECONDS = "SleepSeconds";

        public const string RecordsetEnabled = "RecordsetEnabled";
        public const string RecordsetVariable = "RecordsetVariable";
        public const string RecordsetColumnIndex = "RecordsetColumnIndex";
        public const string ValueIsFullPath = "ValueIsFullPath";

        public const string RecursiveCopy = "RecursiveCopy";
        public const string RecursiveCopyDepth = "RecursiveCopyDepth";

        public const string DeleteFileOnTransferCompleted = "DeleteFileOnTransferCompleted";
    }



    internal static class ExceptionMessages
    {
        public const string SLEEP_SECONDS = "SleepSeconds";
    }


    public class RecordsetHandlerObject
    {
        public bool RecordsetEnabled { get; set; }
        public string RecordsetVariable { get; set; }
        public int RecordsetColumnIndex { get; set; }
        public bool ValueIsFullPath { get; set; }
        public string RootPath { get; set; }
        public VariableDispenser VariableDispenser { get; set; }

        /// <summary>
        /// Get paths from Recordset
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetValues()
        {
            using (var oleDbDataAdapter = new OleDbDataAdapter())
            {
                using (var dataTable = new DataTable())
                {
                    oleDbDataAdapter.Fill(dataTable, EvaluateRecordsetVariable(RecordsetVariable, VariableDispenser));

                    return (from DataRow row in dataTable.Rows
                        select (ValueIsFullPath)
                            ? row[RecordsetColumnIndex].ToString()
                            : string.Format("{0}{1}", RootPath, row[RecordsetColumnIndex])).ToList();
                }
            }
        }

        private static object EvaluateRecordsetVariable(string mappedParam, VariableDispenser variableDispenser)
        {
            try
            {
                var mappedParams = mappedParam.Split(new[] { "@" }, StringSplitOptions.RemoveEmptyEntries);

                Variables variables = null;

                foreach (string t in mappedParams)
                {
                    try
                    {
                        string param = t.Split(new[] { "::" }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", string.Empty).Replace("]", string.Empty).Replace("@", string.Empty);
                        variableDispenser.LockOneForRead(param, ref variables);
                        object variableObject = variables[0].Value;
                        return variableObject;
                    }
                    catch (Exception exception)
                    {
                        throw new Exception(exception.Message);
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

            return null;
        }
    }


}