using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.SqlServer.Dts.Runtime;
using SSISSFTPTask110;
using SSISSFTPTask110.Tools;
using Tamir.SharpSsh.jsch;

/* 
 * Sftp.cs
 * 
 * Copyright (c) 2006 Tamir Gal, http://www.tamirgal.com, All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *
 *  	1. Redistributions of source code must retain the above copyright notice,
 *		this list of conditions and the following disclaimer.
 *
 *	    2. Redistributions in binary form must reproduce the above copyright 
 *		notice, this list of conditions and the following disclaimer in 
 *		the documentation and/or other materials provided with the distribution.
 *
 *	    3. The names of the authors may not be used to endorse or promote products
 *		derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED ``AS IS'' AND ANY EXPRESSED OR IMPLIED WARRANTIES,
 * INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE AUTHOR
 *  *OR ANY CONTRIBUTORS TO THIS SOFTWARE BE LIABLE FOR ANY DIRECT, INDIRECT,
 * INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT
 * LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA,
 * OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
 * LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE,
 * EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 **/

namespace Tamir.SharpSsh
{
    public class Sftp : SshTransferProtocolBase
    {
        private MyProgressMonitor m_monitor;
        private bool cancelled = false;
        public IDTSComponentEvents ComponentEvents { get; set; }
        public static RecordsetHandlerObject RecordsetHandler { get; set; }
        public static bool DeleteFileOnTransferCompleted { get; set; }
        public static bool RecursiveCopy { get; set; }
        public static int RecursiveDepth { get; set; }

        bool _refire = false;

        public Sftp(string sftpHost, string user, string password)
            : base(sftpHost, user, password)
        {
            Init();
        }

        public Sftp(string sftpHost, string user)
            : base(sftpHost, user)
        {
            Init();
        }

        private void Init()
        {
            m_monitor = new MyProgressMonitor(this);
        }

        protected override string ChannelType
        {
            get { return "sftp"; }
        }

        private ChannelSftp SftpChannel
        {
            get { return (ChannelSftp)m_channel; }
        }

        public override void Cancel()
        {
            cancelled = true;
        }

        //Get

        public void Get(string fromFilePath)
        {
            Get(fromFilePath, ".");
        }

        public void Get(string[] fromFilePaths)
        {
            foreach (string t in fromFilePaths)
            {
                Get(t);
            }
        }

        public void Get(string[] fromFilePaths, string toDirPath)
        {
            foreach (string t in fromFilePaths)
            {
                Get(t, toDirPath);
            }
        }

        public override void Get(string fromFilePath, string toFilePath)
        {
            #region Recordset
            if (RecordsetHandler.RecordsetEnabled)
            {

                IEnumerable<string> listOfFiles = RecordsetHandler.GetValues();
                if (listOfFiles != null)
                {
                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("Preparing to copy {0} files", listOfFiles.Count()),
                                                    string.Empty, 0, ref _refire);

                    int fileCounter = 0;
                    int lastSlashIndex = fromFilePath.LastIndexOf('/');
                    string dir = fromFilePath.Substring(0, lastSlashIndex + 1);

                    foreach (var file in listOfFiles)
                    {

                        string from = (dir.Length == 0)
                                         ? file
                                         : string.Format("{0}{1}", dir, file);

                        if (Directory.Exists(toFilePath))
                        {
                            toFilePath = (toFilePath[toFilePath.Length - 1] != '\\') ? toFilePath + "\\" : toFilePath;
                        }
                        else if (File.Exists(toFilePath))
                        {
                            toFilePath = string.Format("{0}\\", new FileInfo(toFilePath).Directory.FullName);
                        }

                        string to = string.Format("{0}{1}", toFilePath, file);

                        SftpChannel.get(from, to, m_monitor, ChannelSftp.OVERWRITE);

                        if (DeleteFileOnTransferCompleted)
                            Delete(from);

                        ComponentEvents.FireInformation(0, "SSISSFTTask",
                                    string.Format("File copied from {0} to {1}", from, to),
                                    string.Empty, 0, ref _refire);

                        fileCounter++;
                    }

                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("Total copied files {0}", fileCounter),
                                string.Empty, 0, ref _refire);
                }
            }
            #endregion

            #region File Mask
            else if (fromFilePath.Contains("*") || fromFilePath.Contains("?"))
            {
                var patternedList = SFTPTools.SFTPScanDirs(this, fromFilePath, RecursiveDepth);
                int fileCounter = 0;

                int lastBackSlashIndex = fromFilePath.LastIndexOf('/');

                string dir = fromFilePath.Substring(0, lastBackSlashIndex + 1);

                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                                string.Format("Preparing to copy {0} files", patternedList.Count),
                                                string.Empty, 0, ref _refire);

                foreach (var file in patternedList)
                {
                    //if (Directory.Exists(toFilePath))
                    //{
                    //    toFilePath = (toFilePath[toFilePath.Length - 1] != '\\') ? toFilePath + "\\" : toFilePath;
                    //}
                    //else if (File.Exists(toFilePath))
                    //{
                    //    toFilePath = string.Format("{0}\\", new FileInfo(toFilePath).Directory.FullName);
                    //}

                    string to = string.Empty;

                    SFTPTools.CreateTreeFolderFromLocal(toFilePath, dir, file, ref to);

                    SftpChannel.get(file, to, m_monitor, ChannelSftp.OVERWRITE);
                    if (DeleteFileOnTransferCompleted)
                        Delete(file);

                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("File copied from {0} to {1}", file, to),
                                string.Empty, 0, ref _refire);
                    fileCounter++;
                }

                ComponentEvents.FireInformation(0, "SSISSFTTask",
                            string.Format("Total copied files {0}", fileCounter),
                            string.Empty, 0, ref _refire);
            }
            #endregion

            #region File Only
            else if (!RecordsetHandler.RecordsetEnabled && (!fromFilePath.Contains("*") || !fromFilePath.Contains("?")))
            {
                SftpChannel.get(fromFilePath, toFilePath, m_monitor, ChannelSftp.OVERWRITE);
                if (DeleteFileOnTransferCompleted)
                    Delete(fromFilePath);
                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("File copied from {0} to {1}: {0}", fromFilePath, toFilePath),
                                string.Empty, 0, ref _refire);
            }
            #endregion
        }

        //Put

        public void Put(string fromFilePath)
        {
            Put(fromFilePath, ".");
        }

        public void Put(string[] fromFilePaths)
        {
            foreach (string t in fromFilePaths)
            {
                Put(t);
            }
        }

        public void Put(string[] fromFilePaths, string toDirPath)
        {
            foreach (string t in fromFilePaths)
            {
                Put(t, toDirPath);
            }
        }

        public override void Put(string fromFilePath, string toFilePath)
        {
            #region Recordset
            if (RecordsetHandler.RecordsetEnabled)
            {

                IEnumerable<string> listOfFiles = RecordsetHandler.GetValues();
                if (listOfFiles != null)
                {
                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("Preparing to copy {0} files", listOfFiles.Count()),
                                                    string.Empty, 0, ref _refire);

                    int fileCounter = 0;

                    foreach (var file in listOfFiles)
                    {

                        SftpChannel.put(file, toFilePath, m_monitor, ChannelSftp.OVERWRITE);
                        if (DeleteFileOnTransferCompleted)
                            File.Delete(file);

                        ComponentEvents.FireInformation(0, "SSISSFTTask",
                                    string.Format("Send file from {0} to {1}", file, toFilePath),
                                    string.Empty, 0, ref _refire);

                        fileCounter++;
                    }

                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("Total copied files {0}", fileCounter),
                                string.Empty, 0, ref _refire);
                }
            }
            #endregion

            #region FileMask
            else if (fromFilePath.Contains("*") || fromFilePath.Contains("?"))
            {
                int lastBackSlashIndex = fromFilePath.LastIndexOf('\\');

                string dir = fromFilePath.Substring(0, lastBackSlashIndex + 1);
                string pattern = fromFilePath.Substring(lastBackSlashIndex + 1, fromFilePath.Length - lastBackSlashIndex - 1);

                var filePaths = new List<string>();

                DirectoryInfo initialDir = new DirectoryInfo(dir);


                SFTPTools.LocalScanDirs(initialDir, pattern, RecursiveCopy ? RecursiveDepth : 0, ref filePaths);

                int fileCounter = 0;

                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("Preparing to copy {0} files", filePaths.Count),
                                string.Empty, 0, ref _refire);

                foreach (var filePath in filePaths)
                {
                    string fileDestination = string.Empty;
                    SFTPTools.CreateTreeFolderFromSFTP(SftpChannel, toFilePath, dir, filePath, ref fileDestination);

                    /*string fileDestination = (toFilePath[toFilePath.Length - 1] == '/')
                                                 ? toFilePath + fileInfo.Name
                                                 : toFilePath + '/' + fileInfo.Name;*/

                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("Send file from {0} to {1}", filePath, fileDestination),
                                                    string.Empty, 0, ref _refire);

                    SftpChannel.put(filePath,
                                    fileDestination,
                                    m_monitor,
                                    ChannelSftp.OVERWRITE);

                    if (DeleteFileOnTransferCompleted)
                        File.Delete(filePath);

                    fileCounter++;
                }

                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("Number of sent files: {0}", fileCounter),
                                string.Empty, 0, ref _refire);
            }
            #endregion

            #region Only the file
            else if (!RecordsetHandler.RecordsetEnabled && (!fromFilePath.Contains("*") || !fromFilePath.Contains("?")))
            {
                SftpChannel.put(fromFilePath, toFilePath, m_monitor, ChannelSftp.OVERWRITE);
                if (DeleteFileOnTransferCompleted)
                    File.Delete(fromFilePath);
                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("File sent from {0} to {1}", fromFilePath, toFilePath),
                                string.Empty, 0, ref _refire);
            }
            #endregion
        }

        //MkDir

        public override void Mkdir(string directory)
        {
            SftpChannel.mkdir(directory);
        }

        //Ls file
        public List<string> GetFileList(string path)
        {

            List<string> retList = new List<string>();

            if (path.Contains("*") || path.Contains("?"))
            {
                int lastSlashIndex = path.LastIndexOf('/');

                string dir = path.Substring(0, lastSlashIndex + 1);
                string pattern = path.Substring(lastSlashIndex + 1, path.Length - lastSlashIndex - 1);

                List<string> remoteFiles = (from ChannelSftp.LsEntry entry in SftpChannel.ls(dir)
                                            where !entry.getAttrs().isDir()
                                            select entry.getFilename()).Select(f => (string)f).ToList();

                var tmpRemoteFiles = new List<string>();

                foreach (var remoteFile in remoteFiles)
                {
                    lastSlashIndex = remoteFile.LastIndexOf('/');
                    tmpRemoteFiles.Add(remoteFile.Substring(lastSlashIndex + 1, remoteFile.Length - lastSlashIndex - 1));
                }

                retList = tmpRemoteFiles.LikeDir(pattern);
            }
            else
            {
                retList = (from ChannelSftp.LsEntry entry in SftpChannel.ls(path)
                           where !entry.getAttrs().isLink()
                           select entry.getFilename()).Select(f => (string)f).ToList();
            }

            return retList;
        }


        public IEnumerable<ChannelSftp.LsEntry> GetFileListEx(string path)
        {

            IEnumerable<ChannelSftp.LsEntry> retList = new List<ChannelSftp.LsEntry>();

            if (path.Contains("*") || path.Contains("?"))
            {
                int lastSlashIndex = path.LastIndexOf('/');

                string dir = path.Substring(0, lastSlashIndex + 1);
                string pattern = path.Substring(lastSlashIndex + 1, path.Length - lastSlashIndex - 1);

                var remoteFiles = (from ChannelSftp.LsEntry entry in SftpChannel.ls(dir)
                                   where !entry.getAttrs().isDir()
                                   select entry).ToList();

                var tmpRemoteFiles = new List<string>();

                foreach (var remoteFile in remoteFiles)
                {
                    string fileName = remoteFile.getFilename();

                    lastSlashIndex = fileName.LastIndexOf('/');
                    tmpRemoteFiles.Add(fileName.Substring(lastSlashIndex + 1, fileName.Length - lastSlashIndex - 1));
                }

                retList = from i in tmpRemoteFiles.LikeDir(pattern)
                          from j in remoteFiles
                          where j.getLongname().Equals(i)
                          select j;
            }
            else
            {
                retList = (from ChannelSftp.LsEntry entry in SftpChannel.ls(path)
                           where !entry.getAttrs().isLink()
                           select entry).ToList();
            }

            return retList;
        }

        //Ls file
        public List<string> GetFileListDepth(string path, int depth)
        {
            List<string> retList = new List<string>();

            if (path.Contains("*") || path.Contains("?"))
            {
                int lastSlashIndex = path.LastIndexOf('/');

                string dir = path.Substring(0, lastSlashIndex + 1);
                string pattern = path.Substring(lastSlashIndex + 1, path.Length - lastSlashIndex - 1);

                List<string> remoteFiles = (from ChannelSftp.LsEntry entry in SftpChannel.lsDepth(dir, depth)
                                            where !entry.getAttrs().isDir()
                                            select entry.getFilename()).Select(f => (string)f).ToList();

                var tmpRemoteFiles = new List<string>();

                foreach (var remoteFile in remoteFiles)
                {
                    lastSlashIndex = remoteFile.LastIndexOf('/');
                    tmpRemoteFiles.Add(remoteFile.Substring(lastSlashIndex + 1, remoteFile.Length - lastSlashIndex - 1));
                }

                retList = tmpRemoteFiles.LikeDir(pattern);
            }
            else
            {
                retList = (from ChannelSftp.LsEntry entry in SftpChannel.lsDepth(path, depth)
                           where !entry.getAttrs().isDir()
                           select entry.getFilename()).Select(f => (string)f).ToList();
            }

            return retList;
        }

        //Ls file
        public List<string> GetDirectoryList(string path)
        {
            List<string> retList = new List<string>();

            if (path.Contains("*") || path.Contains("?"))
            {
                int lastSlashIndex = path.LastIndexOf('/');

                string dir = path.Substring(0, lastSlashIndex + 1);
                string pattern = path.Substring(lastSlashIndex + 1, path.Length - lastSlashIndex - 1);

                List<string> remoteFiles = (from ChannelSftp.LsEntry entry in SftpChannel.ls(dir)
                                            where entry.getAttrs().isDir()
                                            select entry.getFilename()).Select(f => (string)f).ToList();

                var tmpRemoteFiles = new List<string>();

                foreach (var remoteFile in remoteFiles)
                {
                    lastSlashIndex = remoteFile.LastIndexOf('/');
                    tmpRemoteFiles.Add(remoteFile.Substring(lastSlashIndex + 1, remoteFile.Length - lastSlashIndex - 1));
                }

                retList = tmpRemoteFiles.LikeDir(pattern);
            }
            else
            {

                foreach (ChannelSftp.LsEntry entry in SftpChannel.ls(path))
                {
                    if (entry.getAttrs().isDir())
                    {
                        if (entry.getFilename() == "..")
                            retList.Add("/");
                        else
                        {
                            retList.Add(entry.getFilename());
                        }
                    }
                }

                /*retList = (from ChannelSftp.LsEntry entry in SftpChannel.ls(path)
                           where entry.getAttrs().isDir()
                           select entry.getFilename()).Select(f => (string)f).ToList();*/
            }

            return retList;
        }

        //Delete
        public void Delete(string filePath)
        {
            if (RecordsetHandler.RecordsetEnabled)
            {
                IEnumerable<string> listOfFiles = RecordsetHandler.GetValues();
                if (listOfFiles != null)
                {
                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                                    string.Format("Preparing to delete {0} files", listOfFiles.Count()),
                                                    string.Empty, 0, ref _refire);

                    int fileCounter = 0;

                    foreach (var file in listOfFiles)
                    {

                        SftpChannel.rm(file);

                        ComponentEvents.FireInformation(0, "SSISSFTTask",
                                    string.Format("File {0} deleted", file),
                                    string.Empty, 0, ref _refire);

                        fileCounter++;
                    }

                    ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("Total deleted files {0}", fileCounter),
                                string.Empty, 0, ref _refire);
                }
            }
            else if (filePath.Contains("*") || filePath.Contains("?"))
            {
                var patternList = SFTPTools.SFTPScanDirs(this, filePath, RecursiveDepth);

                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                string.Format("Number of files to delete {0}", patternList.Count),
                                string.Empty, 0, ref _refire);

                int fileCounter = 0;

                foreach (var file in patternList)
                {
                    ComponentEvents.FireInformation(0, "SSISSFTTask", string.Format("Deleting file: {0}", file), string.Empty, 0, ref _refire);
                    SftpChannel.rm(file);

                    fileCounter++;
                }

                ComponentEvents.FireInformation(0, "SSISSFTTask", string.Format("Number of deleted files: {0}", fileCounter), string.Empty, 0, ref _refire);
            }
            else if (!RecordsetHandler.RecordsetEnabled && (!filePath.Contains("*") || !filePath.Contains("?")))
            {

                ComponentEvents.FireInformation(0, "SSISSFTTask",
                                                string.Format("Deleting file: {0}", filePath),
                                                string.Empty, 0, ref _refire);
                SftpChannel.rm(filePath);
            }
        }

        //Delete
        public void RemoveDir(string dirPath)
        {
            SftpChannel.rmdir(dirPath);
        }

        #region ProgressMonitor Implementation

        private class MyProgressMonitor : SftpProgressMonitor
        {
            private long _transferred = 0;
            private long _total = 0;
            private int _elapsed = -1;
            private readonly Sftp _mSftp;
            private string _src;
            private string _dest;

            System.Timers.Timer _timer;

            public MyProgressMonitor(Sftp sftp)
            {
                _mSftp = sftp;
            }

            public override void init(int op, String src, String dest, long max)
            {
                this._src = src;
                this._dest = dest;
                this._elapsed = 0;
                this._total = max;
                _timer = new System.Timers.Timer(1000);
                _timer.Start();
                _timer.Elapsed += new System.Timers.ElapsedEventHandler(timer_Elapsed);

                string note;
                if (op.Equals(GET))
                {
                    note = "Downloading " + System.IO.Path.GetFileName(src) + "...";
                }
                else
                {
                    note = "Uploading " + System.IO.Path.GetFileName(src) + "...";
                }
                _mSftp.SendStartMessage(src, dest, (int)_total, note);
            }
            public override bool count(long c)
            {
                this._transferred += c;
                string note = ("Transfering... [Elapsed time: " + _elapsed + "]");
                _mSftp.SendProgressMessage(_src, _dest, (int)_transferred, (int)_total, note);
                return !_mSftp.cancelled;
            }
            public override void end()
            {
                _timer.Stop();
                _timer.Dispose();
                string note = ("Done in " + _elapsed + " seconds!");
                _mSftp.SendEndMessage(_src, _dest, (int)_transferred, (int)_total, note);
                _transferred = 0;
                _total = 0;
                _elapsed = -1;
                _src = null;
                _dest = null;
            }

            private void timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
            {
                this._elapsed++;
            }
        }

        #endregion ProgressMonitor Implementation
    }


}
