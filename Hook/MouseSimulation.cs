using System.Runtime.InteropServices;

namespace Hook
{
    public class MouseSimulation
    {
        private static class API
        {
            [DllImport("user32.dll")]
            public static extern void mouse_event(uint dwFlags, uint dx, uint dy, int dwData, int dwExtraInfo);
            [DllImport("user32.dll")]
            public static extern int SetCursorPos(int x, int y);

            public const uint LBDOWN = 0x00000002; // 왼쪽 마우스 버튼 눌림
            public const uint LBUP = 0x00000004; // 왼쪽 마우스 버튼 떼어짐
            public const uint RBDOWN = 0x00000008; // 오른쪽 마우스 버튼 눌림
            public const uint RBUP = 0x000000010; // 오른쪽 마우스 버튼 떼어짐
            public const uint MBDOWN = 0x00000020; // 휠 버튼 눌림
            public const uint MBUP = 0x000000040; // 휠 버튼 떼어짐
            public const uint WHEEL = 0x00000800; //휠 스크롤
        }

        /// <summary>
        /// MOUSEDOWN 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="type">버튼 타입</param>
        /// <param name="x">좌표 x</param>
        /// <param name="y">좌표 y</param>
        public void MouseDown(MouseEventType type, int x, int y)
        {
            API.SetCursorPos(x, y);
            switch(type)
            {
                case MouseEventType.LEFT:
                    API.mouse_event(API.LBDOWN, 0, 0, 0, 0);
                    break;
                case MouseEventType.RIGHT:
                    API.mouse_event(API.RBDOWN, 0, 0, 0, 0);
                    break;
                case MouseEventType.WHEEL:
                    API.mouse_event(API.MBDOWN, 0, 0, 0, 0);
                    break;
            }
        }

        /// <summary>
        /// MOUSEUP 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="type">버튼 타입</param>
        /// <param name="x">좌표 x</param>
        /// <param name="y">좌표 y</param>
        public void MouseUp(MouseEventType type, int x, int y)
        {
            API.SetCursorPos(x, y);
            switch (type)
            {
                case MouseEventType.LEFT:
                    API.mouse_event(API.LBUP, 0, 0, 0, 0);
                    break;
                case MouseEventType.RIGHT:
                    API.mouse_event(API.RBUP, 0, 0, 0, 0);
                    break;
                case MouseEventType.WHEEL:
                    API.mouse_event(API.MBUP, 0, 0, 0, 0);
                    break;
            }
        }

        /// <summary>
        /// MOUSEDOWN, MOUSEUP 이벤트를 동시 발생시킵니다.
        /// </summary>
        /// <param name="type">버튼 타입</param>
        /// <param name="x">좌표 x</param>
        /// <param name="y">좌표 y</param>
        public void MouseClick(MouseEventType type, int x, int y)
        {
            API.SetCursorPos(x, y);
            switch (type)
            {
                case MouseEventType.LEFT:
                    API.mouse_event(API.LBDOWN, 0, 0, 0, 0);
                    API.mouse_event(API.LBUP, 0, 0, 0, 0);
                    break;
                case MouseEventType.RIGHT:
                    API.mouse_event(API.RBDOWN, 0, 0, 0, 0);
                    API.mouse_event(API.RBUP, 0, 0, 0, 0);
                    break;
                case MouseEventType.WHEEL:
                    API.mouse_event(API.MBDOWN, 0, 0, 0, 0);
                    API.mouse_event(API.MBUP, 0, 0, 0, 0);
                    break;
            }
        }

        /// <summary>
        /// MOUSESCROLL 이벤트를 발생시킵니다.
        /// </summary>
        /// <param name="value">음수면 아래로, 양수면 위로 스크롤.</param>
        public void MouseScroll(int value)
        {
            API.mouse_event(API.WHEEL, 0, 0, value, 0);
        }
    }
}
