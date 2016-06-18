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

        private IntPtr _hookID = IntPtr.Zero;

        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);
        private LowLevelMouseProc _proc;
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

        public event Func<MouseEventType, int, int, bool> MouseDown;
        public event Func<MouseEventType, int, int, bool> MouseUp;
        public event Func<int, bool> MouseScroll;

        public MouseHook()
        {

        }

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
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

        /// <summary>
        /// 맙소사! .NET 어떤 버젼 이상부터는 WH_MOUSE_LL 후킹이 안된단다. 그래서 SetWindowsHookEX 함수는 0을 리턴해서 콜백자체가 안됨..
        /// </summary>
        public void HookStart()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = API.SetWindowsHookEx(WH_MOUSE_LL, _proc, API.GetModuleHandle(null), 0);
            }
        }

        public void HookEnd()
        {
            API.UnhookWindowsHookEx(_hookID);
        }
    }
}