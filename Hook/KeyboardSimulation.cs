using static Hook.WinAPI;

namespace Hook
{
    public static class KeyboardSimulation
    {
        public static void MakeKeyEvent(int vkCode, KeyboardEventType type)
        {
            switch (type)
            {
                case KeyboardEventType.KEYDOWN:
                    keybd_event((byte)vkCode, 0x00, 0x00, 0);
                    break;
                case KeyboardEventType.KEYUP:
                    keybd_event((byte)vkCode, 0x00, 0x02, 0);
                    break;
                case KeyboardEventType.KEYCLICK:
                    keybd_event((byte)vkCode, 0x00, 0x00, 0);
                    keybd_event((byte)vkCode, 0x00, 0x02, 0);
                    break;
            }
        }
    }
}
