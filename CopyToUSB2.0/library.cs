using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CopyToUSB2._0
{
    class Library
    {
        public static List<ListViewItem> listAllDifferences(string subDir, string baseFolder, string driveLetter)
        {
            List<ListViewItem> list = new List<ListViewItem>();
            try
            {
                DirectoryInfo directory = new DirectoryInfo(subDir);
                FileInfo[] files = directory.GetFiles();
                DirectoryInfo[] dirs = directory.GetDirectories();
                foreach (FileInfo f in files)
                {
                    if (!f.Attributes.HasFlag(FileAttributes.System))
                    {
                        string fullPath = f.FullName;
                        string relativePath = fullPath.Substring(subDir.IndexOf(baseFolder) + baseFolder.Length);
                        if (Directory.Exists(Path.Combine(driveLetter, baseFolder)))
                        {
                            string desPath = Path.Combine(driveLetter, baseFolder, relativePath);
                            if (System.IO.File.Exists(desPath))
                            {
                                FileInfo fi = new FileInfo(desPath);
                                if (f.LastWriteTime != fi.LastWriteTime) //Modified files !!!
                                {
                                    string[] row = { relativePath, "modified", Format.ByteSize(f.Length), f.LastWriteTime.ToString(), fi.LastWriteTime.ToString() };
                                    list.Add(new ListViewItem(row));
                                }
                            }
                            else
                            {
                                //Console.WriteLine(desPath + Directory.Exists(desPath).ToString());
                                string[] row = { relativePath, "new file", Format.ByteSize(f.Length), f.LastWriteTime.ToString(), null };
                                list.Add(new ListViewItem(row));
                            }
                        }
                        else
                        {
                            string[] row = { relativePath, "new file", Format.ByteSize(f.Length), f.LastWriteTime.ToString(), null };
                            list.Add(new ListViewItem(row));
                        }
                    }

                }
                foreach (DirectoryInfo d in dirs)
                {
                    if (!d.Attributes.HasFlag(FileAttributes.System))
                    {
                        list.AddRange(listAllDifferences(d.FullName, baseFolder, driveLetter));
                    }
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
            return list;
        }
        public static double getTotalSizeOfSelectedItems(List<ListViewItem> items)
        {
            double totalSize = 0;
            foreach (ListViewItem l in items)
            {
                if (l.Selected == true)
                {
                    string a = l.SubItems[2].Text;
                    totalSize += Format.Bytes(a);
                }
            }
            return totalSize;
        }

        public static double CopyFromTheLink(ListViewItem l, string baseFolder, string driveLetter)
        {
            string srcPath = Properties.Settings.Default["srcPath"].ToString();
            double x = Format.Bytes(l.SubItems[2].Text);
            try
            {
                // Copy the file from source to destination
                string src = Path.GetFullPath(srcPath + l.SubItems[0].Text);
                string des = Path.GetFullPath(driveLetter + "\\" + baseFolder + "\\" + l.SubItems[0].Text);
                //Console.WriteLine(src + " -------- " + des);
                if (!Directory.Exists(des))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(des));
                }
                System.IO.File.Copy(src, des, true);
                // Main task done.
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return x;
        }
    }
}
