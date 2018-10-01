using static Hook.WinAPI;

namespace Hook
{
    public static class MouseSimulation
    {
        public static void MouseDown(MouseEventType type, int x, int y)
        {
            SetCursorPos(x, y);
            switch(type)
            {
                case MouseEventType.LEFT:
                    Mouse(LBDOWN);
                    break;
                case MouseEventType.RIGHT:
                    Mouse(RBDOWN);
                    break;
                case MouseEventType.WHEEL:
                    Mouse(MBDOWN);
                    break;
            }
        }

        public static void MouseUp(MouseEventType type, int x, int y)
        {
            SetCursorPos(x, y);
            switch (type)
            {
                case MouseEventType.LEFT:
                    Mouse(LBUP);
                    break;
                case MouseEventType.RIGHT:
                    Mouse(RBUP);
                    break;
                case MouseEventType.WHEEL:
                    Mouse(MBUP);
                    break;
            }
        }
        
        public static void MouseClick(MouseEventType type, int x, int y)
        {
            MouseDown(type, x, y);
            MouseUp(type, x, y);
        }

        public static void MouseScroll(MouseScrollType type)
        {
            mouse_event(WHEEL, 0, 0, (int)type, 0);
        }

        private static void Mouse(uint button)
            => mouse_event(button, 0, 0, 0, 0);
    }
}
