using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace CSharpAutokeyboard
{
    public class SendInputs
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        static extern IntPtr PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern uint MapVirtualKey(uint uCode, uint uMapType);

        public const uint WM_CHAR = 0x0102;
        public const uint WM_KEYDOWN = 0x100;
        public const uint WM_KEYUP = 0x101;
        public const int VK_KEY_0 = 0x30;
        public const int VK_KEY_1 = 0x31;
        public const int VK_KEY_2 = 0x32;
        public const int VK_KEY_3 = 0x33;
        public const int VK_KEY_4 = 0x34;
        public const int VK_KEY_5 = 0x35;
        public const int VK_KEY_6 = 0x36;
        public const int VK_KEY_7 = 0x37;
        public const int VK_KEY_8 = 0x38;
        public const int VK_KEY_9 = 0x39;
        public const int VK_KEY_A = 0x41;
        public const int VK_KEY_B = 0x42;
        public const int VK_KEY_C = 0x43;
        public const int VK_KEY_D = 0x44;
        public const int VK_KEY_E = 0x45;
        public const int VK_KEY_F = 0x46;
        public const int VK_KEY_G = 0x47;
        public const int VK_KEY_H = 0x48;
        public const int VK_KEY_I = 0x49;
        public const int VK_KEY_J = 0x4a;
        public const int VK_KEY_K = 0x4b;
        public const int VK_KEY_L = 0x4c;
        public const int VK_KEY_M = 0x4d;
        public const int VK_KEY_N = 0x4e;
        public const int VK_KEY_O = 0x4f;
        public const int VK_KEY_P = 0x50;
        public const int VK_KEY_Q = 0x51;
        public const int VK_KEY_R = 0x52;
        public const int VK_KEY_S = 0x53;
        public const int VK_KEY_T = 0x54;
        public const int VK_KEY_U = 0x55;
        public const int VK_KEY_V = 0x56;
        public const int VK_KEY_W = 0x57;
        public const int VK_KEY_X = 0x58;
        public const int VK_KEY_Y = 0x59;
        public const int VK_KEY_Z = 0x5a;
        public const int VK_F1 = 0x70;
        public const int VK_F10 = 0x79;
        public const int VK_F11 = 0x7a;
        public const int VK_F12 = 0x7b;
        public const int VK_F13 = 0x7c;
        public const int VK_F14 = 0x7d;
        public const int VK_F15 = 0x7e;
        public const int VK_F16 = 0x7f;
        public const int VK_F17 = 0x80;
        public const int VK_F18 = 0x81;
        public const int VK_F19 = 0x82;
        public const int VK_F2 = 0x71;
        public const int VK_F20 = 0x83;
        public const int VK_F21 = 0x84;
        public const int VK_F22 = 0x85;
        public const int VK_F23 = 0x86;
        public const int VK_F24 = 0x87;
        public const int VK_F3 = 0x72;
        public const int VK_F4 = 0x73;
        public const int VK_F5 = 0x74;
        public const int VK_F6 = 0x75;
        public const int VK_F7 = 0x76;
        public const int VK_F8 = 0x77;
        public const int VK_F9 = 0x78;
        public const int VK_OEM_1 = 0xba;
        public const int VK_OEM_2 = 0xbf;
        public const int VK_OEM_3 = 0xc0;
        public const int VK_OEM_4 = 0xdb;
        public const int VK_OEM_5 = 0xdc;
        public const int VK_OEM_6 = 0xdd;
        public const int VK_OEM_7 = 0xde;
        public const int VK_OEM_8 = 0xdf;
        public const int VK_OEM_COMMA = 0xbc;
        public const int VK_OEM_MINUS = 0xbd;
        public const int VK_OEM_PERIOD = 0xbe;
        public const int VK_OEM_PLUS = 0xbb;
        public const int VK_RETURN = 0x0d;
        public const int VK_SPACE = 0x20;
        public const int VK_TAB = 0x09;

        public static void SendKeys(IntPtr hwnd, string keys)
        {
            if (keys.Equals("F1"))
                SendKey(hwnd, VK_F1);
            else if (keys.Equals("F2"))
                SendKey(hwnd, VK_F2);
            else if (keys.Equals("F3"))
                SendKey(hwnd, VK_F3);
            else if (keys.Equals("F4"))
                SendKey(hwnd, VK_F4);
            else if (keys.Equals("F5"))
                SendKey(hwnd, VK_F5);
            else if (keys.Equals("F6"))
                SendKey(hwnd, VK_F6);
            else if (keys.Equals("F7"))
                SendKey(hwnd, VK_F7);
            else if (keys.Equals("F8"))
                SendKey(hwnd, VK_F8);
            else if (keys.Equals("F9"))
                SendKey(hwnd, VK_F9);
            else if (keys.Equals("F10"))
                SendKey(hwnd, VK_F10);
            else if (keys.Equals("F11"))
                SendKey(hwnd, VK_F11);
            else if (keys.Equals("F12"))
                SendKey(hwnd, VK_F12);
            else
            {
                foreach (var key in keys)
                {
                    PostMessage(hwnd, WM_CHAR, new IntPtr(key), new IntPtr(0));
                }
                SendKey(hwnd, VK_RETURN);
            }
        }

        public static void SendKey(IntPtr hwnd, int keyCode)
        {
            PostMessage(hwnd, WM_KEYUP, new IntPtr(keyCode), new IntPtr(0));
        }
    }
}
