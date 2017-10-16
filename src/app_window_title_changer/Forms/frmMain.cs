using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace CSharpAutokeyboard
{
    public partial class frmMain : Form
    {
        public readonly List<Window> windowList = new List<Window>();

        public frmMain()
        {
            InitializeComponent();
        }

        private void UpdateWindowsList()
        {
            int selected_index = lstWindows.SelectedIndex;

            windowList.Clear();
            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {
                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                if (string.IsNullOrEmpty(proc.MainWindowTitle) || string.IsNullOrWhiteSpace(proc.MainWindowTitle))
                    continue;

                windowList.Add(
                    new Window(proc)
                );
            }

            lstWindows.DataSource = windowList;
            lstWindows.DisplayMember = "MainWindowTitle";

            lstWindows.SelectedIndex = selected_index;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            UpdateWindowsList();
        }

        private void tmrRefreshWindowList_Tick(object sender, EventArgs e)
        {
            this.UpdateWindowsList();
        }

        private void lstWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWindows.SelectedIndex < 0 || windowList == null)
                return;

            var selectedWindow = windowList[lstWindows.SelectedIndex];
        }

        private void btnStart_Click(object sender, EventArgs e)
        {

        }

        private void btnStop_Click(object sender, EventArgs e)
        {

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save Data File";
            fileDialog.Filter = "AUTOKEYS files|*.autokeys";
            fileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    StreamWriter writer = new StreamWriter(fileDialog.OpenFile());
                    writer.Dispose();
                    writer.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            Stream stream = null;
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Open Data File";
            fileDialog.Filter = "AUTOKEYS files|*.autokeys";
            fileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if ((stream = fileDialog.OpenFile()) != null)
                    {
                        using (stream)
                        {
                            // Insert code to read the stream here.
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            UpdateWindowsList();
        }
    }
}
