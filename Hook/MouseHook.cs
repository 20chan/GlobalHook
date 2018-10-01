using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using static Hook.WinAPI;

namespace Hook
{
    public delegate bool MouseEventCallback(MouseEventType type, int x, int y);
    public delegate bool MouseScrollEventCallback(MouseScrollType type);
    public class MouseHook
    {
        private static IntPtr _hookID = IntPtr.Zero;

        private static LowLevelProc _proc;

        public static event MouseEventCallback MouseDown;
        public static event MouseEventCallback MouseUp;
        public static event MouseEventCallback MouseMove;
        public static event MouseScrollEventCallback MouseScroll;
        
        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            int intw = (int)wParam;
            if (nCode >= 0 && 
                intw == WM_LBUTTONDOWN || intw == WM_RBUTTONDOWN ||
                intw == WM_LBUTTONUP || intw == WM_RBUTTONUP ||
                intw == WM_MOUSEWHEEL || intw == WM_MOUSEMOVE)
            {
                var hookStruct = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
                int x = hookStruct.pt.x, y = hookStruct.pt.y;
                bool res = true;
                switch ((int)wParam)
                {
                    case WM_LBUTTONDOWN:
                        res = MouseDown?.Invoke(MouseEventType.LEFT, x, y) ?? true;
                        break;
                    case WM_RBUTTONDOWN:
                        res = MouseDown?.Invoke(MouseEventType.RIGHT, x, y) ?? true;
                        break;
                    case WM_LBUTTONUP:
                        res = MouseUp?.Invoke(MouseEventType.LEFT, x, y) ?? true;
                        break;
                    case WM_RBUTTONUP:
                        res = MouseUp?.Invoke(MouseEventType.RIGHT, x, y) ?? true;
                        break;
                    case WM_MOUSEMOVE:
                        res = MouseMove?.Invoke(MouseEventType.NONE, x, y) ?? true;
                        break;
                    case WM_MOUSEWHEEL:
                        res = MouseScroll?.Invoke((int)hookStruct.mouseData > 0 ? MouseScrollType.UP : MouseScrollType.DOWN) ?? true;
                        break;
                }
                if (!res)
                    return (IntPtr)1;
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