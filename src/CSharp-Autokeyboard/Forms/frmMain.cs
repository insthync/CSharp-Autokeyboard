using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using LitJson;
using System.Runtime.InteropServices;
using System.Text;

namespace CSharpAutokeyboard
{
    public partial class frmMain : Form
    {
        // Refs
        // https://msdn.microsoft.com/en-us/library/ms644950(VS.85).aspx
        // 
        private const int WM_SETTEXT = 0x000C;
        [DllImport("User32.dll")]
        private static extern Int32 SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, StringBuilder lParam);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        private List<Window> windowList = null;
        private Window selectedWindow = null;
        private Window runningWindow = null;
        private Timer updateTimer = new Timer();
        private bool isRunning = false;
        private bool dirtyIsRunning = false;
        private int currentIndex;
        private int repeatKeysCount = 0;
        private bool startDelayCount = false;
        private long delayStartTime = 0;
        private int repeatCount = 0;

        public frmMain()
        {
            InitializeComponent();
            updateTimer.Interval = 50;  // 50 ms
            updateTimer.Tick += new EventHandler(UpdateLogic);
            updateTimer.Start();
        }

        private void ResetUpdateStates()
        {
            runningWindow = null;
            currentIndex = 0;
            repeatKeysCount = 0;
            startDelayCount = false;
            delayStartTime = 0;
            repeatCount = 0;
        }

        private void IncreaseUpdateIndex()
        {
            ++currentIndex;
            repeatKeysCount = 0;
            startDelayCount = false;
            delayStartTime = 0;
            repeatCount = 0;
            if (currentIndex >= gvKeyList.RowCount - 1)
                currentIndex = 0;
        }

        private KeyDataEntry GetKeyDataEntry(int index)
        {
            // Skip last row it's always empty
            if (index < 0 || index >= gvKeyList.RowCount - 1)
                return null;
            var row = gvKeyList.Rows[index];
            var result = new KeyDataEntry();
            result.enabled = Convert.ToBoolean(row.Cells[0].Value);
            result.keys = Convert.ToString(row.Cells[1].Value);
            result.repeatKeys = Convert.ToInt32(row.Cells[2].Value);
            result.delay = Convert.ToInt32(row.Cells[3].Value);
            result.repeat = Convert.ToInt32(row.Cells[4].Value);
            return result;
        }

        private void UpdateLogic(object sender, System.EventArgs e)
        {
            if (isRunning && isRunning != dirtyIsRunning)
            {
                ResetUpdateStates();
                runningWindow = selectedWindow;
            }
            dirtyIsRunning = isRunning;

            btnStart.Enabled = !isRunning;
            btnStop.Enabled = isRunning;

            if (!isRunning)
                return;

            if (runningWindow == null)
            {
                isRunning = false;
                return;
            }

            var keyDataEntry = GetKeyDataEntry(currentIndex);
            if (keyDataEntry != null)
            {
                if (!keyDataEntry.enabled)
                {
                    IncreaseUpdateIndex();
                    return;
                }
                if (repeatKeysCount < keyDataEntry.repeatKeys)
                {
                    ++repeatKeysCount;
                    IntPtr windowPtr = runningWindow.MainWindowHandle;
                    if (runningWindow.MainWindowHandle == IntPtr.Zero)
                    {
                        isRunning = false;
                        return;
                    }
                    SetForegroundWindow(windowPtr);
                    SendKeys.Send(keyDataEntry.keys);
                    Console.WriteLine("Send Keys " + keyDataEntry.keys + " count " + repeatKeysCount);
                }
                else
                {
                    var currentTicks = DateTime.Now.Ticks;
                    if (!startDelayCount)
                    {
                        delayStartTime = currentTicks;
                        startDelayCount = true;
                        Console.WriteLine("Starting delay at " + delayStartTime);
                        return;
                    }
                    var differenceTicks = currentTicks - delayStartTime;
                    if (differenceTicks / TimeSpan.TicksPerSecond >= keyDataEntry.delay)
                    {
                        Console.WriteLine("Delay ended at " + currentTicks + " difference ticks " + differenceTicks);
                        ++repeatCount;
                        if (repeatCount >= keyDataEntry.repeat)
                        {
                            Console.WriteLine("Repeated " + repeatCount);
                            IncreaseUpdateIndex();
                        }
                        else
                        {
                            repeatKeysCount = 0;
                            delayStartTime = 0;
                            startDelayCount = false;
                        }
                    }
                }
            }
        }

        private void RefreshWindowsList()
        {
            Console.WriteLine("RefreshWindowsList");
            int selectedIndex = lstWindows.SelectedIndex;

            List<Window> windowList = new List<Window>();
            Process[] processes = Process.GetProcesses();
            foreach (Process proc in processes)
            {
                if (proc.MainWindowHandle == IntPtr.Zero)
                    continue;

                if (string.IsNullOrEmpty(proc.MainWindowTitle) || string.IsNullOrWhiteSpace(proc.MainWindowTitle))
                    continue;

                windowList.Add(new Window(proc));
            }

            lstWindows.DataSource = windowList;
            lstWindows.DisplayMember = "ListTitle";
            lstWindows.SelectedIndex = selectedIndex;
            this.windowList = windowList;
        }

        private void frmMain_Load(object sender, EventArgs e)
        {
            RefreshWindowsList();
        }

        private void lstWindows_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstWindows.SelectedIndex < 0 || windowList == null || windowList.Count == 0)
            {
                selectedWindow = null;
                return;
            }

            selectedWindow = windowList[lstWindows.SelectedIndex];
        }

        private void gvKeyList_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(IntegerColumn_KeyPress);
            if (gvKeyList.CurrentCell.ColumnIndex == 2 ||   // Repeat Keys
                gvKeyList.CurrentCell.ColumnIndex == 3 ||   // Delay
                gvKeyList.CurrentCell.ColumnIndex == 4      // Repeat
                )
            {
                TextBox tb = e.Control as TextBox;
                if (tb != null)
                {
                    tb.KeyPress += new KeyPressEventHandler(IntegerColumn_KeyPress);
                }
            }
        }

        private void IntegerColumn_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            isRunning = true;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            isRunning = false;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            List<KeyDataEntry> entries = new List<KeyDataEntry>();
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Title = "Save Data File";
            fileDialog.Filter = "AUTOKEYS files|*.autokeys";
            fileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    // Don't add last empty row
                    for (var i = 0; i < gvKeyList.RowCount - 1; ++i)
                    {
                        var row = gvKeyList.Rows[i];
                        var newEntry = new KeyDataEntry();
                        newEntry.enabled = Convert.ToBoolean(row.Cells[0].Value);
                        newEntry.keys = Convert.ToString(row.Cells[1].Value);
                        newEntry.repeatKeys = Convert.ToInt32(row.Cells[2].Value);
                        newEntry.delay = Convert.ToInt32(row.Cells[3].Value);
                        newEntry.repeat = Convert.ToInt32(row.Cells[4].Value);
                        entries.Add(newEntry);
                    }
                    var json = JsonMapper.ToJson(entries);
                    File.WriteAllText(fileDialog.FileName, json);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: Could not read file from disk. Original error: " + ex.Message);
                }
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Title = "Open Data File";
            fileDialog.Filter = "AUTOKEYS files|*.autokeys";
            fileDialog.InitialDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var json = File.ReadAllText(fileDialog.FileName);
                    var entries = JsonMapper.ToObject<List<KeyDataEntry>>(json);
                    foreach (var entry in entries)
                    {
                        var newData = new DataGridViewRow();
                        gvKeyList.Rows.Add(entry.enabled, 
                            entry.keys, 
                            entry.repeatKeys.ToString(), 
                            entry.delay.ToString(),
                            entry.repeat.ToString());
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
            RefreshWindowsList();
        }
    }
}
