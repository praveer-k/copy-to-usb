using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using CopyToUSB2._0.Properties;
using System.IO;

namespace CopyToUSB2._0
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }

        private void Form2_Load(object sender, EventArgs e)
        {
            srcPath.Text = Properties.Settings.Default["srcPath"].ToString();
            usbLabel.Text = Properties.Settings.Default["usbLabel"].ToString();
            startUp.Checked = Convert.ToBoolean(Properties.Settings.Default["startUp"].ToString());
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(srcPath.Text))
            {
                Properties.Settings.Default["srcPath"] = srcPath.Text.Trim();
                Properties.Settings.Default["usbLabel"] = usbLabel.Text.Trim();
                Properties.Settings.Default["startUp"] = startUp.Checked;
                SetStartup(startUp.Checked);
                Properties.Settings.Default.Save();
                this.Close();
            } else
            {
                MessageBox.Show("Source Path was not found !!!");
            }

        }

        private void SetStartup(bool doset)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (doset)
                rk.SetValue("CopyToUSB2.0", Application.ExecutablePath.ToString());
            else
                rk.DeleteValue("CopyToUSB2.0", false);
        }
    }
}
