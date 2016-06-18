using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hook
{
    public class KeyboardHook
    {
        #region WinAPI
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x101;
        private const int WM_SYSTEMKEYUP = 0x0105;
        private const int WM_SYSKEYDOWN = 0x104;
        public struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public int dwExtraInfo;
        }
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        private LowLevelKeyboardProc _proc;
        private IntPtr _hookID = IntPtr.Zero;
        private static class API
        {
            [DllImport("user32.dll")]
            public static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);
            [DllImport("user32.dll")]
            public static extern bool UnhookWindowsHookEx(IntPtr hhk);
            [DllImport("user32.dll")]
            public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
            [DllImport("kernel32.dll")]
            public static extern IntPtr GetModuleHandle(string lpModuleName);
        }
        #endregion
        /// <summary>
        /// 리턴값이 FALSE 이면 키 잠금.
        /// </summary>
        public bool IsGetSystemKeyEvent { get; set; }
        public event Func<Keys, KeyboardEventType, bool> KeyEvented;

        public event Func<Keys, bool> KeyDown;
        public event Func<Keys, bool> KeyUp;

        public KeyboardHook(bool getSysKeyEvent = true)
        {
            _proc = HookCallback;
            IsGetSystemKeyEvent = getSysKeyEvent;
        }
        
        private IntPtr HookCallback(int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (nCode >= 0)
            {
                if (wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
                {
                    int vkCode = lParam.vkCode;
                    int flags = lParam.flags;
                    Keys key = (Keys)vkCode;
                    if (KeyEvented?.Invoke(key, KeyboardEventType.KEYDOWN) == false) return (IntPtr)1;
                    if (KeyDown?.Invoke(key) == false) return (IntPtr)1;
                }
                else if (wParam == (IntPtr)WM_KEYUP || wParam == (IntPtr)WM_SYSTEMKEYUP)
                {
                    int vkCode = lParam.vkCode;
                    int flags = lParam.flags;
                    Keys key = (Keys)vkCode;
                    if (KeyEvented?.Invoke(key, KeyboardEventType.KEYUP) == false) return (IntPtr)1;
                    if (KeyUp?.Invoke(key) == false) return (IntPtr)1;
                }
            }
            
            return API.CallNextHookEx(_hookID, nCode, wParam, ref lParam);
        }

        /// <summary>
        /// 키보드 후킹을 시작합니다.
        /// </summary>
        public void HookStart()
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                _hookID = API.SetWindowsHookEx(WH_KEYBOARD_LL, _proc, API.GetModuleHandle("user32"), 0);
            }
        }

        /// <summary>
        /// 키보드 후킹을 끝냅니다.
        /// </summary>
        public void HookEnd()
        {
            API.UnhookWindowsHookEx(_hookID);
        }
    }
}
