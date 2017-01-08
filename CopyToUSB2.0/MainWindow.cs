using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
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
        private bool DeviceFlag = false;

        private string srcPath = "";
        private string driveLetter = "";
        private string baseFolder = "";

        private int sortColumn = 0;

        private CancellationTokenSource cts;
        
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
            // Call the sort method to manually sort.
            listView1.BeginUpdate();
            listView1.Sort();
            listView1.EndUpdate();
            // Set the ListViewItemSorter property to a new ListViewItemComparer object.
            this.listView1.ListViewItemSorter = new ListViewItemComparer(e.Column, listView1.Sorting);
            listView1.Refresh();
        }

        private void button_Click(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();
            int totalSelected = listView1.SelectedItems.Count;
            if (button.Enabled == true && button.Text == "Copy" && totalSelected>0)
            {
                button.Enabled = false;
                double totalSize = Library.getTotalSizeOfSelectedItems(listView1.SelectedItems.Cast<ListViewItem>().ToList());
                double curSize = 0;
                CancellationToken token = cts.Token;
                progressBar1.Value = 0;
                int taskIndex = 0;
                List<Task<double>> tasks = new List<Task<double>>();
                label1.Text = " > Initiallizing Copying process... Please wait ";
                foreach (ListViewItem l in listView1.SelectedItems)
                {
                    tasks.Add(Task<double>.Factory.StartNew(() => {
                        Interlocked.Increment(ref taskIndex);
                        return Library.CopyFromTheLink(l, baseFolder, driveLetter);
                    }, token));

                    label1.Text = " > Copying file " + taskIndex.ToString() + " Out of " + totalSelected.ToString();

                    tasks[tasks.Count - 1].ContinueWith((t) => {
                        if (!token.IsCancellationRequested)
                        {
                            curSize += t.Result;
                            int per = (int)(curSize / totalSize * 100);
                            progressBar1.Value = per;
                            listView1.Items.Remove(l);
                            label1.Text = " > " + taskIndex.ToString() + " files copied out of " + totalSelected.ToString();
                            if (taskIndex == totalSelected)
                            {
                                cts.Dispose();
                                button.Text = "Copy";
                                if (listView1.Items.Count > 0)
                                {
                                    label1.Text = " > Please Select the files to be copied into the USB drive...";
                                    button.Enabled = true;
                                }
                            }
                        }
                        else
                        {
                            cts.Dispose();
                            progressBar1.Value = 0;
                            label1.Text = " > Copying of files was cancelled";
                            button.Text = "Copy";
                            button.Enabled = true;
                        }
                    }, TaskScheduler.FromCurrentSynchronizationContext());
                }
                button.Text = "Cancel";
                button.Enabled = true;
            }
            else if (button.Text=="Cancel")
            {
                button.Enabled = false;
                cts.Cancel();
            }
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
                                string src = Properties.Settings.Default["srcPath"].ToString().Trim();
                                string usbLabel = Properties.Settings.Default["usbLabel"].ToString().Trim();
                                foreach (DriveInfo d in allDrives)
                                {
                                    if (d.Name == driveLetter)
                                    {
                                        if (d.VolumeLabel == usbLabel || (d.VolumeLabel == "" && usbLabel == "Removable Disk"))
                                        {
                                            if (ShowInTaskbar == false)
                                            {
                                                ShowInTaskbar = true;
                                                notifyIcon1.Visible = false;
                                                WindowState = FormWindowState.Normal;
                                                TopMost = true;
                                                TopMost = false;
                                            }
                                            DeviceFlag = true;
                                            label1.Text = " > Found a new USB Flash Drive with the required label ...";
                                            if (Directory.Exists(src))
                                            {
                                                srcPath = Path.GetFullPath(src);
                                                baseFolder = srcPath.Replace(Path.GetPathRoot(srcPath), ""); // initiallise class variable.
                                                //Console.WriteLine(srcPath + " ~~~~ " + baseFolder);
                                                Task<List<ListViewItem>> task = Task<List<ListViewItem>>.Factory.StartNew(() => { return Library.listAllDifferences(srcPath, baseFolder, driveLetter); });
                                                label1.Text = " > Populating the differences b/w source and destination...";
                                                task.ContinueWith((t) => {
                                                    listView1.BeginUpdate();
                                                    listView1.Items.AddRange(t.Result.ToArray());
                                                    listView1.EndUpdate();
                                                    label1.Text = " > Please Select the files to be copied into the USB drive...";
                                                    button.Text = "Copy";
                                                    button.Enabled = true;
                                                }, TaskScheduler.FromCurrentSynchronizationContext());
                                            }
                                            else
                                            {
                                                MessageBox.Show("The Specified Source Folder Does Not Exists !!!");
                                                break;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                            break;
                        case DBT_DEVICEREMOVECOMPLETE:
                            if (DeviceFlag == true)
                            {
                                label1.Text = " > The Device has been removed";
                                listView1.Items.Clear();
                                progressBar1.Value = 0;
                                button.Enabled = false;
                                button.Text = "Copy";
                                DeviceFlag = false;
                            }
                            break;
                    }
                    break;
            }

        }
    }
}
