using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Hook
{
    public class MouseHook
    {
        #region WinAPI
        private const int WH_MOUSE_LL = 14;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_MOUSEWHEEL = 0x020A;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;

        private static IntPtr _hookID = IntPtr.Zero;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelMouseProc _proc;
        private class API
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct POINT
            {
                public int x;
                public int y;
            }
            [StructLayout(LayoutKind.Sequential)]
            public struct MSLLHOOKSTRUCT
            {
                public POINT pt;
                public uint mouseData;
                public uint flags;
                public uint time;
                public IntPtr dwExtraInfo;
            }

            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelMouseProc lpfn, IntPtr hMod, uint dwThreadId);
            [DllImport("user32.dll")]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);
            [DllImport("user32.dll")]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
        }
        #endregion

        public static event Func<MouseEventType, int, int, bool> MouseDown;
        public static event Func<MouseEventType, int, int, bool> MouseUp;
        public static event Func<int, bool> MouseScroll;
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && ((int)wParam == WM_LBUTTONDOWN || (int)wParam == WM_RBUTTONDOWN))
            {
                API.MSLLHOOKSTRUCT hookStruct = (API.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(API.MSLLHOOKSTRUCT));
                int x = hookStruct.pt.x, y = hookStruct.pt.y;
                switch ((int)wParam)
                {
                    case WM_LBUTTONDOWN:
                        if (MouseDown?.Invoke(MouseEventType.LEFT, x, y) == false) return _hookID;
                        break;
                    case WM_RBUTTONDOWN:
                        if (MouseDown?.Invoke(MouseEventType.RIGHT, x, y) == false) return _hookID;
                        break;
                }
            }
            if (nCode >= 0 && ((int)wParam == WM_LBUTTONUP || (int)wParam == WM_RBUTTONUP))
            {
                API.MSLLHOOKSTRUCT hookStruct = (API.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(API.MSLLHOOKSTRUCT));
                int x = hookStruct.pt.x, y = hookStruct.pt.y;
                switch ((int)wParam)
                {
                    case WM_LBUTTONUP:
                        if (MouseUp?.Invoke(MouseEventType.LEFT, x, y) == false) return _hookID;
                        break;
                    case WM_RBUTTONUP:
                        if (MouseUp?.Invoke(MouseEventType.RIGHT, x, y) == false) return _hookID;
                        break;
                }
            }
            if(nCode >= 0 && (int)wParam == WM_MOUSEWHEEL)
            {
                API.MSLLHOOKSTRUCT hookStruct = (API.MSLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(API.MSLLHOOKSTRUCT));
            }
            return API.CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public static bool HookStart()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                var handle = API.GetModuleHandle("user32");
                _proc = HookCallback;
                _hookID = API.SetWindowsHookEx(WH_MOUSE_LL, _proc, handle, 0);
                return _hookID != IntPtr.Zero;
            }
        }

        public static void HookEnd()
        {
            API.UnhookWindowsHookEx(_hookID);
        }
    }
}