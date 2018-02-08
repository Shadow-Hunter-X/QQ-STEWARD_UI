using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace QQMgrUI_X
{
    public partial class LoginUI : Form
    {

        private int posX = 0;
        private int posY = 0;
        private bool titelStatus = false;

        public LoginUI()
        {
            InitializeComponent();
        }

        private void loginBtn_MouseClick(object sender, MouseEventArgs e)
        {
            this.Hide();
        }

        private void CancelBtn_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        private void LoginUI_MouseDown(object sender, MouseEventArgs e)
        {
            posX = e.X;
            posY = e.Y;
            titelStatus = true; 
        }

        private void LoginUI_MouseMove(object sender, MouseEventArgs e)
        {
            if (titelStatus)
            {
                this.Top += e.Y - posY;
                this.Left += e.X - posX;
            }
        }

        private void LoginUI_MouseUp(object sender, MouseEventArgs e)
        {
            titelStatus = false;

        }
    }
}
