using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Hook.WinAPI;

namespace Hook
{
    public delegate bool MouseEventCallback(MouseEventType type, int x, int y);
    public delegate bool MouseScrollEventCallback(int value);
    public class MouseHook
    {
        private static IntPtr _hookID = IntPtr.Zero;

        private static LowLevelProc _proc;

        public static event MouseEventCallback MouseDown;
        public static event MouseEventCallback MouseUp;
        public static event MouseScrollEventCallback MouseScroll;
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && ((int)wParam == WM_LBUTTONDOWN || (int)wParam == WM_RBUTTONDOWN))
            {
                var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
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
                var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
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
                var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        public static bool HookStart()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                var handle = GetModuleHandle("user32");
                _proc = HookCallback;
                _hookID = SetWindowsHookEx(WH_MOUSE_LL, _proc, handle, 0);
                return _hookID != IntPtr.Zero;
            }
        }

        public static void HookEnd()
        {
            UnhookWindowsHookEx(_hookID);
        }
    }
}