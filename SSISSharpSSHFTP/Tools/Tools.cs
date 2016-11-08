using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Tamir.SharpSsh;
using System;
using Tamir.SharpSsh.jsch;

namespace SSISSFTPTask110.Tools
{
    public static class SFTPTools
    {
        public static void LocalScanDirs(DirectoryInfo dir, string pattern, int recursionLevel, ref List<string> outListFiles)
        {
            if (recursionLevel < 0)
                return;

            outListFiles.AddRange(dir.GetFiles(pattern).Select(iInfo => iInfo.FullName));

            foreach (DirectoryInfo directoryInfo in dir.GetDirectories())
            {
                try
                {
                    try
                    {
                        outListFiles.AddRange(directoryInfo.GetFiles(pattern).Select(iInfo => iInfo.FullName));
                    }
                    catch { }

                    LocalScanDirs(directoryInfo, pattern, recursionLevel - 1, ref outListFiles);
                }
                catch { }
            }
        }

        private static void SFTPScanDirs(Sftp sftpHandler, string dir, int recursionLevel, ref List<string> outListFiles)
        {
            if (recursionLevel <= 0)
                return;

            if (recursionLevel == 1)
            {
                try
                {
                    foreach (var item in from file in sftpHandler.GetFileList(dir)
                                         where !file.StartsWith(".")
                                         select string.Format("{0}/{1}", dir, file))
                    {
                        if (!outListFiles.Contains(item.Replace("//", "/")))
                        {
                            outListFiles.Add(item.Replace("//", "/"));
                        }
                    }
                }
                catch { }
            }
            else
            {
                foreach (var directoryInfo in sftpHandler.GetDirectoryList(dir).Where(directoryInfo => !directoryInfo.StartsWith(".")))
                {
                    try
                    {
                        var lDir = string.Format("{0}{1}{2}", dir, directoryInfo == "/" ? string.Empty : "/", directoryInfo);

                        if (lDir.Substring(lDir.Length - 1, 1) != "/")
                            lDir += "/";

                        try
                        {
                            foreach (var item in from file in sftpHandler.GetFileListEx(lDir)
                                                 where !file.getFilename().startsWith(".")
                                                 select file) //;string.Format("{0}{1}{2}", lDir, lDir.Substring(lDir.Length - 1, 1) == "/" ? string.Empty : "/", file))
                            {
                                if (!item.getAttrs().isDir())
                                {

                                    var filetoAdd = string.Concat(lDir, item.getFilename());
                                    if (!outListFiles.Contains(filetoAdd))
                                    {
                                        outListFiles.Add(filetoAdd);
                                    }
                                }

                                if (recursionLevel > 1)
                                    if (item.getAttrs().isDir())
                                        SFTPScanDirs(sftpHandler, string.Format("{0}{1}{2}", lDir, lDir.Substring(lDir.Length - 1, 1) == "/" ? string.Empty : "/", item.getFilename()), recursionLevel - 1, ref outListFiles);
                            }
                        }
                        catch { }
                    }
                    catch { }
                }
            }
        }

        public static List<string> SFTPScanDirs(Sftp sftpHandler, string initialPath, int recursionLevel)
        {
            var outListFiles = new List<string>();
            var pathElements = initialPath.Split('/');
            var dirpath = initialPath.Substring(0, initialPath.LastIndexOf('/'));
            if (string.IsNullOrWhiteSpace(dirpath))
                dirpath += "/";
            var pattern = pathElements[pathElements.Length - 1];

            var bHasPattern = pattern.Contains("*") || pattern.Contains("?");

            if (!bHasPattern)
                dirpath += pattern;

            SFTPScanDirs(sftpHandler, dirpath, recursionLevel, ref outListFiles);

            return (bHasPattern)
                        ? outListFiles.LikeDir(pattern)
                        : outListFiles;
        }

        public static void CreateTreeFolderFromSFTP(ChannelSftp sftpHandler, string initialFileDestination, string initialDir, string filePath, ref string fileDestination)
        {
            string[] folders = filePath.Replace(initialDir, string.Empty)
                                            .Replace('\\', '/')
                                            .Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 1;
            var treeSource = initialFileDestination;
            foreach (var folder in folders)
            {
                if (folders.Length == count)
                    break;

                treeSource = string.Format("{0}{1}/", treeSource, folder);

                if (!sftpHandler.isRemoteDir(treeSource))
                    sftpHandler.mkdir(treeSource);
                count++;
            }

            fileDestination = string.Format("{0}{1}", initialFileDestination, filePath.Replace(initialDir, string.Empty).Replace('\\', '/'));
        }

        public static void CreateTreeFolderFromLocal(string initialFileDestination, string initialDir, string filePath, ref string fileDestination)
        {
            string[] folders = filePath.Replace(initialDir, string.Empty)
                                            .Replace('/', '\\')
                                            .Split(new[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var count = 1;
            var treeSource = initialFileDestination;
            foreach (var folder in folders)
            {
                if (folders.Length == count)
                    break;

                treeSource = string.Format("{0}{1}\\", treeSource, folder);

                if (!Directory.Exists(treeSource))
                    Directory.CreateDirectory(treeSource);
                count++;
            }

            fileDestination = string.Format("{0}{1}", initialFileDestination, filePath.Replace(initialDir, string.Empty).Replace('/', '\\'));
        }
    }

    public static class StringExtensions
    {
        public static List<string> Like(this IEnumerable<string> strSource, string wildcard)
        {
            var regex = new Regex("^" + Regex.Escape(wildcard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            return strSource.Where(s => regex.IsMatch(s)).ToList();
        }

        public static List<string> LikeDir(this IEnumerable<string> strSource, string wildcard)
        {
            var regex = new Regex("^" + Regex.Escape(wildcard).Replace(@"\*", ".*").Replace(@"\?", ".") + "$", RegexOptions.IgnoreCase | RegexOptions.Singleline);

            var outSource = new List<string>();

            foreach (var str in strSource)
            {
                if (regex.IsMatch(str.Substring(str.LastIndexOf("/") + 1)))
                    outSource.Add(str);
            }

            return outSource;
        }
    }
}
