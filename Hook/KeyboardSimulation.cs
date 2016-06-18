using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Hook
{
    public static class KeyboardSimulation
    {
        private static class API
        {
            [DllImport("User32.dll")]
            public static extern void keybd_event(uint vk, uint scan, uint flags, uint extraInfo);
        }

        public static void MakeKeyEvent(Keys key, KeyboardEventType type)
        {
            switch (type)
            {
                case KeyboardEventType.KEYDOWN:
                    API.keybd_event((byte)key, 0x00, 0x00, 0);
                    break;
                case KeyboardEventType.KEYUP:
                    API.keybd_event((byte)key, 0x00, 0x02, 0);
                    break;
                case KeyboardEventType.KEYCLICK:
                    API.keybd_event((byte)key, 0x00, 0x00, 0);
                    API.keybd_event((byte)key, 0x00, 0x02, 0);
                    break;
            }
        }
    }
}
