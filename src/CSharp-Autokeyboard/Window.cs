using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace CSharpAutokeyboard
{
    public class Window
    {
        //http://msdn.microsoft.com/en-us/library/windows/desktop/ms633546(v=vs.85).aspx
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern bool SetWindowText(IntPtr hwnd, String lpString);

        public Process Process { get; private set; }

        public IntPtr MainWindowHandle
        {
            get
            {
                return Process.MainWindowHandle;
            }
        }

        public string MainWindowTitle
        {
            get
            {
                if (Process.MainWindowHandle == IntPtr.Zero)
                    return null;

                return Process.MainWindowTitle;
            }
        }

        public string ListTitle
        {
            get
            {
                if (Process.MainWindowHandle == IntPtr.Zero)
                    return null;

                return Process.MainWindowTitle + " - " + Process.Id;
            }
        }

        public Window(Process proc)
        {
            Process = proc;
        }

        public bool SetTitle(string title)
        {
            if (string.IsNullOrEmpty(title))
                return false;

            if (MainWindowHandle == IntPtr.Zero)
                return false;

            bool result = SetWindowText(MainWindowHandle, title);
            return result;
        }
    }
}
