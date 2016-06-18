using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Hook;

namespace HookingTest
{
    public partial class Form1 : Form
    {
        KeyboardHook kh;
        MouseHook mh;

        public Form1()
        {
            InitializeComponent();

            kh = new KeyboardHook();
            mh = new MouseHook();

            kh.KeyDown += Kh_KeyDown;
            kh.KeyUp += Kh_KeyUp;
            mh.MouseDown += Mh_MouseDown;
            mh.MouseUp += Mh_MouseUp;

            kh.HookStart();
            mh.HookStart();
        }

        ~Form1()
        {
            kh.HookEnd();
            mh.HookEnd();
        }

        private bool Mh_MouseUp(MouseEventType arg1, int arg2, int arg3)
        {
            AppendText("MOUSEUP : " + arg1.ToString() + ", (" + arg2.ToString() + "," + arg3.ToString() + ")");
            return true;
        }

        private bool Mh_MouseDown(MouseEventType arg1, int arg2, int arg3)
        {
            AppendText("MOUSEDOWN : " + arg1.ToString() + ", (" + arg2.ToString() + "," + arg3.ToString() + ")");
            return true;
        }

        private bool Kh_KeyUp(Keys arg)
        {
            AppendText("KEYUP : " + arg.ToString());
            return true;
        }

        private bool Kh_KeyDown(Keys arg)
        {
            AppendText("KEYDOWN : " + arg.ToString());
            return true;
        }

        private void AppendText(string text)
        {
            this.textBox1.AppendText(text + "\r\n");
        }
    }
}
