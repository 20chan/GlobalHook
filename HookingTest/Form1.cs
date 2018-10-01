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
        public Form1()
        {
            InitializeComponent();

            KeyboardHook.KeyDown += Kh_KeyDown;
            KeyboardHook.KeyUp += Kh_KeyUp;
            MouseHook.MouseDown += Mh_MouseDown;
            MouseHook.MouseUp += Mh_MouseUp;

            KeyboardHook.HookStart();
            if (!MouseHook.HookStart())
            {
                MessageBox.Show("Mouse hook failed");
            }

            FormClosing += Form1_FormClosing;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            KeyboardHook.HookEnd();
            MouseHook.HookEnd();
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
