using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QQMgrUI_X
{
    public partial class QQMgrUI : Form
    {
           
        /// <summary>
        /// 记忆窗口在移动过程中的坐标
        /// </summary>
        private int posX = 0;
        private int posY = 0;
        private bool titelStatus = false;

        /// <summary>
        /// 功能切换标签状态，及对应处理函数
        /// </summary>
        private int SelectedItemTag = 1;
        private int PreSelectedItemTag = 1;
        private void AdjustFuncStatus()
        {
            if(PreSelectedItemTag == 1)
            {
                title1.Image = global::QQMgrUI_X.Properties.Resources.title1h;
                //Detail1Panel.Visible = false;

            }
            else if(PreSelectedItemTag == 2)
            {
                title2.Image = global::QQMgrUI_X.Properties.Resources.title2;
                //Detail2Panel.Visible = false;
            }
            else if(PreSelectedItemTag == 3)
            {
                title3.Image = global::QQMgrUI_X.Properties.Resources.title3;
                //Detail3Panel.Visible = false;
            }
            else if(PreSelectedItemTag == 4)
            {
                title4.Image = global::QQMgrUI_X.Properties.Resources.title4;
                //Detail4Panel.Visible = false;
            }
            else if(PreSelectedItemTag == 5)
            {
                title5.Image = global::QQMgrUI_X.Properties.Resources.title5;
            }
            else if(PreSelectedItemTag == 6)
            {
                title6.Image = global::QQMgrUI_X.Properties.Resources.title6;
            }
        }

        /// <summary>
        /// title3上的转态记忆数组
        /// </summary>
        private int[] states = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

        /// <summary>
        /// title4上的状态记忆数组
        /// </summary>
        private int[] T4Status = { 0,0,0,0,0,0};

        public QQMgrUI()
        {
            InitializeComponent();

           
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        // ---- 扫描按钮的三态相应
        private void ScanControl_MouseClick(object sender, MouseEventArgs e)
        {
              
        }

        private void ScanControl_MouseEnter(object sender, EventArgs e)
        {
            this.ScanControl.Image = global::QQMgrUI_X.Properties.Resources.scanH;
        }

        private void ScanControl_MouseLeave(object sender, EventArgs e)
        {
            this.ScanControl.Image = global::QQMgrUI_X.Properties.Resources.scan;
        }

        // ----- 模拟窗口标题栏移动
        private void TitlePanel_MouseDown(object sender, MouseEventArgs e)
        {
            posX = e.X;
            posY = e.Y;
            titelStatus = true; 
        }

        private void TitlePanel_MouseUp(object sender, MouseEventArgs e)
        {
            titelStatus = false;
        }

        private void TitlePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if(titelStatus)
            {
                this.Top += e.Y - posY;
                this.Left += e.X - posX;
            }
        }

        // --- 关闭按钮三态,及点击事件
        private void CloseControl_MouseEnter(object sender, EventArgs e)
        {
            //CloseControl.Image = global::QQMgrUI_X.Properties.Resources.
            this.CloseControl.Image = global::QQMgrUI_X.Properties.Resources.close_hover;
        }

        private void CloseControl_MouseLeave(object sender, EventArgs e)
        {
            this.CloseControl.Image = null;
        }

        private void CloseControl_MouseClick(object sender, MouseEventArgs e)
        {
            Application.Exit();
        }

        // ---- 最小化控件中的三态及点击响应
        private void MinControl_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void MinControl_MouseEnter(object sender, EventArgs e)
        {

        }

        private void MinControl_MouseLeave(object sender, EventArgs e)
        {

        }

        // ---- title1的三态及点击响应处理
        private void title1_Click(object sender, EventArgs e)
        {
            if( SelectedItemTag != 1)
            {
                title1.Image = global::QQMgrUI_X.Properties.Resources.titel1;
                totalinforlabel.Text = "48天未体检，风险上升";
                PreSelectedItemTag = SelectedItemTag;
                SelectedItemTag = 1;
                AdjustFuncStatus();
                Detail1Panel.Visible = true;
                Detail2Panel.Visible = false;
            }

        }

        private void title1_MouseEnter(object sender, EventArgs e)
        {
            if ( SelectedItemTag != 1)
            {
                title1.Image = global::QQMgrUI_X.Properties.Resources.titel1;
            }

        }

        private void title1_MouseLeave(object sender, EventArgs e)
        {
            if( SelectedItemTag != 1 )
            {
                title1.Image = global::QQMgrUI_X.Properties.Resources.title1h;
            }

        }

        // --- title2的三态及点击响应
        private void title2_MouseClick(object sender, MouseEventArgs e)
        {
            if(SelectedItemTag != 2)
            {
                title2.Image = global::QQMgrUI_X.Properties.Resources.title2h;
                totalinforlabel.Text = "定期杀毒，清除风险";
                PreSelectedItemTag = SelectedItemTag;
                SelectedItemTag = 2;
                AdjustFuncStatus();
                Detail2Panel.Visible = true;
                Detail3Panel.Visible = false;
            }
        }

        private void title2_MouseEnter(object sender, EventArgs e)
        {
            if (SelectedItemTag != 2)
                title2.Image = global::QQMgrUI_X.Properties.Resources.title2h;
        }

        private void title2_MouseLeave(object sender, EventArgs e)
        {
            if (SelectedItemTag != 2)
                title2.Image = global::QQMgrUI_X.Properties.Resources.title2;

        }

        // --- title3的三态相应及单击事件
        private void title3_MouseClick(object sender, MouseEventArgs e)
        {
            if(SelectedItemTag != 3)
            {
                title3.Image = global::QQMgrUI_X.Properties.Resources.title3h;
                totalinforlabel.Text = "常清理，电脑也轻盈";
                PreSelectedItemTag = SelectedItemTag;
                SelectedItemTag = 3;
                AdjustFuncStatus();
                Detail3Panel.Visible = true;
                Detail4Panel.Visible = false;
            }
        }

        private void title3_MouseEnter(object sender, EventArgs e)
        {
            if (SelectedItemTag != 3)
                title3.Image = global::QQMgrUI_X.Properties.Resources.title3h;
                
        }

        private void title3_MouseLeave(object sender, EventArgs e)
        {
            if (SelectedItemTag != 3)
                title3.Image = global::QQMgrUI_X.Properties.Resources.title3;

        }

        // --- title4的三态及点击响应事件
        private void title4_MouseClick(object sender, MouseEventArgs e)
        {
            if(SelectedItemTag != 4)
            {
                title4.Image = global::QQMgrUI_X.Properties.Resources.title4h;
                totalinforlabel.Text = "常加速，电脑更给力";
                PreSelectedItemTag = SelectedItemTag;
                SelectedItemTag = 4;
                AdjustFuncStatus();
                Detail4Panel.Visible = true;
                //Detail3Panel.Visible = false;
                //Detail2Panel.Visible = false;
                //Detail1Panel.Visible = false;

            }
        }

        private void title4_MouseEnter(object sender, EventArgs e)
        {
            if (SelectedItemTag != 4)
                title4.Image = global::QQMgrUI_X.Properties.Resources.title4h;

        }

        private void title4_MouseLeave(object sender, EventArgs e)
        {
            if (SelectedItemTag != 4)
                title4.Image = global::QQMgrUI_X.Properties.Resources.title4;

        }

        // ---- title5的三态及点击响应
        private void title5_MouseClick(object sender, MouseEventArgs e)
        {
            if(SelectedItemTag != 5)
            {
                title5.Image = global::QQMgrUI_X.Properties.Resources.title5h;
                totalinforlabel.Text = "一键分析软件问题";
                PreSelectedItemTag = SelectedItemTag;
                SelectedItemTag = 5;
                AdjustFuncStatus();
            }

        }

        private void title5_MouseEnter(object sender, EventArgs e)
        {
            if (SelectedItemTag != 5)
                title5.Image = global::QQMgrUI_X.Properties.Resources.title5h;

        }

        private void title5_MouseLeave(object sender, EventArgs e)
        {
            if (SelectedItemTag != 5)
                title5.Image = global::QQMgrUI_X.Properties.Resources.title5;
        }

        // --- title6的三态及点击响应
        private void title6_MouseClick(object sender, MouseEventArgs e)
        {
            if (SelectedItemTag != 6)
            {
                title6.Image = global::QQMgrUI_X.Properties.Resources.title6h;
                totalinforlabel.Text = "你的电脑有软件更新";
                PreSelectedItemTag = SelectedItemTag;
                SelectedItemTag = 6;
                AdjustFuncStatus();
            }
        }

        private void title6_MouseEnter(object sender, EventArgs e)
        {
            if (SelectedItemTag != 6)
                title6.Image = global::QQMgrUI_X.Properties.Resources.title6h;

        }

        private void title6_MouseLeave(object sender, EventArgs e)
        {
            if (SelectedItemTag != 6)
                title6.Image = global::QQMgrUI_X.Properties.Resources.title6;
        }

        // --- func111 三态相应
        private void func111_MouseClick(object sender, MouseEventArgs e)
        {
            //func111.Image = global::QQMgrUI_X.Properties.Resources._111h;
        }

        private void func111_MouseEnter(object sender, EventArgs e)
        {
            func111.Image = global::QQMgrUI_X.Properties.Resources._111h;
        }

        private void func111_MouseLeave(object sender, EventArgs e)
        {
            func111.Image = global::QQMgrUI_X.Properties.Resources._111;
        }

        // --- func112三态
        private void func112_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void func112_MouseEnter(object sender, EventArgs e)
        {
            func112.Image = global::QQMgrUI_X.Properties.Resources._112h;
        }

        private void func112_MouseLeave(object sender, EventArgs e)
        {
            func112.Image = global::QQMgrUI_X.Properties.Resources._112;
        }

        // func121 三态
        private void func121_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void func121_MouseEnter(object sender, EventArgs e)
        {
            func121.Image = global::QQMgrUI_X.Properties.Resources._121h;
        }

        private void func121_MouseLeave(object sender, EventArgs e)
        {
            func121.Image = global::QQMgrUI_X.Properties.Resources._121;
        }

        // func122 三态
        private void func122_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void func122_MouseEnter(object sender, EventArgs e)
        {
            func122.Image = global::QQMgrUI_X.Properties.Resources._122h;
        }

        // --- func123 三态
        private void func123_MouseClick(object sender, MouseEventArgs e)
        {
        }

        private void func123_MouseEnter(object sender, EventArgs e)
        {
            func123.Image = global::QQMgrUI_X.Properties.Resources._123h;
        }

        private void func123_MouseLeave(object sender, EventArgs e)
        {
            func123.Image = global::QQMgrUI_X.Properties.Resources._123;
        }

        // --- func124
        private void func124_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void func124_MouseEnter(object sender, EventArgs e)
        {
            func124.Image = global::QQMgrUI_X.Properties.Resources._124h;
        }

        private void func124_MouseLeave(object sender, EventArgs e)
        {
            func124.Image = global::QQMgrUI_X.Properties.Resources._124;
        }

        private void func122_MouseLeave(object sender, EventArgs e)
        {
            func122.Image = global::QQMgrUI_X.Properties.Resources._122;
        }

        // --- func211
        private void func211_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void func211_MouseEnter(object sender, EventArgs e)
        {
            func211.Image = global::QQMgrUI_X.Properties.Resources._211h;
        }

        private void func211_MouseLeave(object sender, EventArgs e)
        {
            func211.Image = global::QQMgrUI_X.Properties.Resources._211;
        }
        // --- func212
        private void func212_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void func212_MouseEnter(object sender, EventArgs e)
        {
            func212.Image = global::QQMgrUI_X.Properties.Resources._212h;
        }

        private void func212_MouseLeave(object sender, EventArgs e)
        {
            func212.Image = global::QQMgrUI_X.Properties.Resources._212;
        }

        // --- func213
        private void func213_MouseClick(object sender, MouseEventArgs e)
        {
            
        }

        private void func213_MouseEnter(object sender, EventArgs e)
        {
            func213.Image = global::QQMgrUI_X.Properties.Resources._213h;
        }

        private void func213_MouseLeave(object sender, EventArgs e)
        {
            func213.Image = global::QQMgrUI_X.Properties.Resources._213;
        }
         // --- func214
        private void pictureBox2_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox2_MouseEnter(object sender, EventArgs e)
        {
            pictureBox2.Image = global::QQMgrUI_X.Properties.Resources._214h;
        }

        private void pictureBox2_MouseLeave(object sender, EventArgs e)
        {
            pictureBox2.Image = global::QQMgrUI_X.Properties.Resources._214;
        }

        private void fun311_MouseClick(object sender, MouseEventArgs e)
        {
            if (states[0] == 0)
            {
                fun311.Image = global::QQMgrUI_X.Properties.Resources._311h;
                states[0] = 1;
            }
            else
            { 
                fun311.Image = global::QQMgrUI_X.Properties.Resources._311;
                states[0] = 0;
            }
           
        }

        private void fun311_Click(object sender, EventArgs e)
        {
           

        }

        private void fun312_MouseClick(object sender, MouseEventArgs e)
        {
            if (states[1] == 0)
            {
                fun312.Image = global::QQMgrUI_X.Properties.Resources._312h;
                states[1] = 1;
            }
            else
            {
                fun312.Image = global::QQMgrUI_X.Properties.Resources._312;
                states[1] = 0;
            }
        }

        private void fun313_MouseClick(object sender, MouseEventArgs e)
        {
            if( states[2] == 0)
            {
                fun313.Image = global::QQMgrUI_X.Properties.Resources._313h;
                states[2] = 1;
            }
            else
            {
                fun313.Image = global::QQMgrUI_X.Properties.Resources._313;
                states[2] = 0;
            }

        }

        private void fun314_MouseClick(object sender, MouseEventArgs e)
        {
            if( states[3] == 0)
            {
                fun314.Image = global::QQMgrUI_X.Properties.Resources._314h;
                states[3] = 1;
            }
            else
            {
                fun314.Image = global::QQMgrUI_X.Properties.Resources._314;
                states[3] = 0;
            }

        }

        private void fun315_MouseClick(object sender, MouseEventArgs e)
        {
            if( states[4] == 0)
            {
                fun315.Image = global::QQMgrUI_X.Properties.Resources._315h;
                states[4] = 1;
            }
            else
            {
                fun315.Image = global::QQMgrUI_X.Properties.Resources._315;
                states[4] = 0;
            }

        }

        private void func321_MouseClick(object sender, MouseEventArgs e)
        {
            if(states[5] == 0)
            {
                func321.Image = global::QQMgrUI_X.Properties.Resources._316h;
                states[5] = 1;
            }
            else
            {
                func321.Image = global::QQMgrUI_X.Properties.Resources._316;
                states[5] = 0;
            }
        }

        private void func322_MouseClick(object sender, MouseEventArgs e)
        {
            if( states[6] == 0)
            {
                func322.Image = global::QQMgrUI_X.Properties.Resources._322h;
                states[6] = 1;
            }
            else
            {
                func322.Image = global::QQMgrUI_X.Properties.Resources._322;
                states[6] = 0;
            }

        }

        private void func323_MouseClick(object sender, MouseEventArgs e)
        {
            if( states[7] == 0)
            {
                func323.Image = global::QQMgrUI_X.Properties.Resources._323h;
                states[7] = 1;
            }
            else
            {
                func323.Image = global::QQMgrUI_X.Properties.Resources._323;
                states[7] = 0;
            }

        }

        private void func324_MouseClick(object sender, MouseEventArgs e)
        {
            if(states[8] == 0)
            {
                func324.Image = global::QQMgrUI_X.Properties.Resources._324h;
                states[8] = 1;
            }
            else
            {
                func324.Image = global::QQMgrUI_X.Properties.Resources._324;
                states[8] = 0;
            }
        }

        private void func325_MouseClick(object sender, MouseEventArgs e)
        {
            if(states[9] == 0)
            {
                func325.Image = global::QQMgrUI_X.Properties.Resources._325h;
                states[9] = 1;
            }
            else
            {
                func325.Image = global::QQMgrUI_X.Properties.Resources._325;
                states[9] = 0;
            }

        }

        private void allselect_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void Detail4Panel_VisibleChanged(object sender, EventArgs e)
        {
            //MessageBox.Show("A");
        }

        private void func411_MouseClick(object sender, MouseEventArgs e)
        {
            if(T4Status[0] == 0)
            {
                func411.Image = global::QQMgrUI_X.Properties.Resources._411h;
                T4Status[0] = 1;
            }
            else
            {
                func411.Image = global::QQMgrUI_X.Properties.Resources._411;
                T4Status[0] = 0;
            }
        }

        private void func412_MouseClick(object sender, MouseEventArgs e)
        {
            if( T4Status[1] == 0)
            {
                func412.Image = global::QQMgrUI_X.Properties.Resources._412h;
                T4Status[1] = 1;
            }
            else
            {
                func412.Image = global::QQMgrUI_X.Properties.Resources._412;
                T4Status[1] = 0;
            }
        }

        private void func413_MouseClick(object sender, MouseEventArgs e)
        {
            if(T4Status[2] == 0)
            {
                func413.Image = global::QQMgrUI_X.Properties.Resources._413h;
                T4Status[2] = 1;
            }
            else
            {
                func413.Image = global::QQMgrUI_X.Properties.Resources._413;
                T4Status[2] = 0;
            }
        }

        private void func421_MouseClick(object sender, MouseEventArgs e)
        {
            if( T4Status[3] == 0)
            {
                func421.Image = global::QQMgrUI_X.Properties.Resources._421h;
                T4Status[3] = 1;
            }
            else
            {
                func421.Image = global::QQMgrUI_X.Properties.Resources._421;
                T4Status[3] = 0;
            }
        }

        private void func422_MouseClick(object sender, MouseEventArgs e)
        {
            if( T4Status[4] == 0)
            {
                func422.Image = global::QQMgrUI_X.Properties.Resources._422h;
                T4Status[4] = 1;
            }
            else
            {
                func422.Image = global::QQMgrUI_X.Properties.Resources._422;
                T4Status[4] = 0;
            }
        }

        private void func423_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void QQMgrUI_Load(object sender, EventArgs e)
        {
            if(this.WindowState == FormWindowState.Normal)
            {
                this.Show();
                
                LoginWnd.ShowDialog();
            }
        }

    }
}
