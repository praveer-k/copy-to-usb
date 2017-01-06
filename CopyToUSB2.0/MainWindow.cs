using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.IO;
using System.Security.AccessControl;
using CopyToUSB2._0.Properties;

namespace CopyToUSB2._0
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DevBroadcastVolume
    {
        public int Size;
        public int DeviceType;
        public int Reserved;
        public int Mask;
        public Int16 Flags;
    }

    public partial class MainWindow : Form
    {
        private const int WM_DEVICECHANGE = 0x219;
        private const int DBT_DEVICEARRIVAL = 0x8000;
        private const int DBT_DEVICEREMOVECOMPLETE = 0x8004;
        private const int DBT_DEVTYP_VOLUME = 0x00000002;
        private bool Flag = false;
        private string driveLetter = "";
        private string baseFolder = "";
        private Timer tmr = new Timer();
        private int sortColumn = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Settings form2 = new Settings();
            form2.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About form3 = new About();
            form3.ShowDialog();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void MainWindow_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                ShowInTaskbar = false;
                notifyIcon1.Visible = true;
                notifyIcon1.ShowBalloonTip(500, "Copy To USB", "Application has been minimized !", ToolTipIcon.Info);
            }
        }

        private void notifyIcon1_MouseClick(object sender, MouseEventArgs e)
        {
            ShowInTaskbar = true;
            notifyIcon1.Visible = false;
            WindowState = FormWindowState.Normal;
        }

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            switch (m.Msg)
            {
                case WM_DEVICECHANGE:
                    switch ((int)m.WParam)
                    {
                        case DBT_DEVICEARRIVAL:
                            int devType = Marshal.ReadInt32(m.LParam, 4);
                            if (devType == DBT_DEVTYP_VOLUME)
                            {
                                DevBroadcastVolume vol;
                                vol = (DevBroadcastVolume)Marshal.PtrToStructure(m.LParam, typeof(DevBroadcastVolume));
                                int mask = vol.Mask;
                                String binaryMask = Convert.ToString(vol.Mask, 2);
                                int str = binaryMask.Length - binaryMask.IndexOf('1') - 1;

                                driveLetter = (char)('A' + str) + ":\\"; //initiallise class variable

                                DriveInfo[] allDrives = DriveInfo.GetDrives();
                                string srcPath = Properties.Settings.Default["srcPath"].ToString();
                                string usbLabel = Properties.Settings.Default["usbLabel"].ToString();
                                foreach (DriveInfo d in allDrives)
                                {
                                    if (d.Name == driveLetter)
                                    {
                                        if(d.VolumeLabel == usbLabel || (d.VolumeLabel == "" && usbLabel == "Removable Disk") )
                                        {
                                            if (ShowInTaskbar == false)
                                            {
                                                ShowInTaskbar = true;
                                                notifyIcon1.Visible = false;
                                                WindowState = FormWindowState.Normal;
                                                TopMost = true;
                                                TopMost = false;
                                            }
                                            Flag = true;
                                            label1.Text = " > Found a new USB Flash Drive with the required label ...";
                                            if (Directory.Exists(srcPath))
                                            {
                                                char lastChar = srcPath.ElementAt(srcPath.Length - 1);
                                                srcPath = (lastChar == '\\') ? srcPath.Substring(0, srcPath.Length - 1) : srcPath;
                                                baseFolder = srcPath.Substring(srcPath.LastIndexOf("\\") + 1); // initiallise class variable.
                                                Task<List<ListViewItem>> t = Task<List<ListViewItem>>.Factory.StartNew( () => listAllDifferences(srcPath) );
                                                tmr.Interval = 500;
                                                tmr.Tick += new EventHandler(delegate (object s, EventArgs ev){
                                                    //Console.WriteLine("Background Task Status: {0}, completed: {1}", t.Status, t.IsCompleted);
                                                    if (t.IsCompleted)
                                                    {
                                                        //Console.WriteLine("Background Task completed...");
                                                        tmr.Stop();
                                                        ListViewItem[] arr = t.Result.ToArray();
                                                        listView1.BeginUpdate();
                                                        listView1.Items.AddRange(arr);
                                                        listView1.EndUpdate();
                                                        label1.Text = " > Please Select the files to be copied into the USB drive...";
                                                        t.Dispose();
                                                    }else
                                                    {
                                                        label1.Text = " > Populating the difference in contents of the directory ...";
                                                    }
                                                });
                                                tmr.Start();
                                            }
                                            else
                                            {
                                                MessageBox.Show("The Specified Source Folder Does Not Exists !!!");
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            if (Flag == true)
                            {
                                label1.Text = " > The Device has been removed";
                                listView1.Items.Clear();
                                Flag = false;
                            }                            
                            break;
                    }
                    break;
            }

        }

        private void listView1_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != sortColumn)
            {
                // Set the sort column to the new column.
                sortColumn = e.Column;
                // Set the sort order to ascending by default.
                listView1.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (listView1.Sorting == SortOrder.Ascending)
                    listView1.Sorting = SortOrder.Descending;
                else
                    listView1.Sorting = SortOrder.Ascending;
            }
            // Set the ListViewItemSorter property to a new ListViewItemComparer object.
            this.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column, listView1.Sorting);
            // Call the sort method to manually sort.
            listView1.BeginUpdate();
            listView1.Sort();
            listView1.EndUpdate();
        }

        private void updateProgressBar(Task<List<ListViewItem>> t)
        {

        }    

        private List<ListViewItem> listAllDifferences(string subDir)
        {
            List<ListViewItem> list = new List<ListViewItem>();
            try
            {
                DirectoryInfo directory = new DirectoryInfo(subDir);
                FileInfo[] files = directory.GetFiles();
                DirectoryInfo[] dirs = directory.GetDirectories();
                foreach (FileInfo f in files)
                {   
                    if (! f.Attributes.HasFlag(FileAttributes.System) )
                    {
                        string fullPath = f.FullName;
                        string relativePath = fullPath.Substring(subDir.IndexOf(baseFolder) + baseFolder.Length);
                        if (Directory.Exists(driveLetter + "\\" + baseFolder))
                        {
                            string desPath = driveLetter + baseFolder + "\\" + relativePath;
                            if (System.IO.File.Exists(desPath))
                            {
                                FileInfo fi = new FileInfo(desPath);
                                if (f.LastWriteTime != fi.LastWriteTime) //Modified files !!!
                                {
                                    string[] row = { relativePath, "modified", Format.ByteSize(f.Length), f.LastWriteTime.ToString(), fi.LastWriteTime.ToString() };
                                    var listViewItem = new ListViewItem(row);
                                    list.Add(listViewItem);
                                }
                            }
                            else
                            {
                                //Console.WriteLine(desPath + Directory.Exists(desPath).ToString());
                                string[] row = { relativePath, "new file", Format.ByteSize(f.Length), f.LastWriteTime.ToString(), null };
                                var listViewItem = new ListViewItem(row);
                                list.Add(listViewItem);
                            }
                        }
                        else
                        {
                            string[] row = { relativePath, "new file", Format.ByteSize(f.Length), f.LastWriteTime.ToString(), null };
                            var listViewItem = new ListViewItem(row);
                            list.Add(listViewItem);
                        }                        
                    }
                    
                }
                foreach (DirectoryInfo d in dirs)
                {
                    if (!d.Attributes.HasFlag(FileAttributes.System))
                    {
                        list.AddRange(listAllDifferences(d.FullName));
                    }
                }
            }
            catch(IOException e)
            {
                MessageBox.Show(e.Message); 
            }
            return list;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }
    }
}
