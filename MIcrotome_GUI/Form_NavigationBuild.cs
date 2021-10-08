using System;
using System.Windows.Forms;

namespace MIcrotome_GUI
{
    public partial class Form_NavigationBuild : Form
    {
        public CNaviManage NaviManage;

        public delegate void DeckLinkMessageHandler(string printedStr);

        public event DeckLinkMessageHandler EventPrintedStr;

        public Form_NavigationBuild(CNaviManage NaviManage, CStage stage)
        {
            InitializeComponent();
            this.NaviManage = NaviManage;
            this.NaviManage.EventThreadStatus += Navi_Thread_Handler;
            numericalUpDown_XCen.Value = stage.Get_Xaxis() / 1000;
            numericalUpDown_YCen.Value = stage.Get_Yaxis() / 1000;
            numericalUpDown_Mag.Value = stage.Get_Mag();
        }

        private void numericalUpDown_Length_ValueChanged(object sender, EventArgs e)
        {
            button_Navi_start.Enabled = false;
            double length_in = Convert.ToDouble(numericalUpDown_Length.Value);
            double width_in = length_in * 0.7;
            numericalUpDown_Width.Value = (decimal)width_in;
            button_Navi_start.Enabled = false;
        }

        private void numericalUpDown_Width_ValueChanged(object sender, EventArgs e)
        {
            button_Navi_start.Enabled = false;
            double width_in = Convert.ToDouble(numericalUpDown_Width.Value);
            double length_in = width_in / 0.7;
            numericalUpDown_Length.Value = (decimal)length_in;
            button_Navi_start.Enabled = false;
        }

        private void numericalUpDown_Pixel_ValueChanged(object sender, EventArgs e)
        {
            double um_per_pixel = Convert.ToDouble(numericalUpDown_Pixel.Value) / 1000;
            int mag = (int)Math.Ceiling(CNaviManage.SEM_initial_length / um_per_pixel / CNaviManage.SEM_x_pixel);
            numericalUpDown_Mag.Value = mag;
            button_Navi_start.Enabled = false;
        }

        private void numericalUpDown_Mag_ValueChanged(object sender, EventArgs e)
        {
            int mag = (int)numericalUpDown_Mag.Value;
            double um_per_pixel = (double)CNaviManage.SEM_initial_length / mag / CNaviManage.SEM_x_pixel;
            numericalUpDown_Pixel.Value = (int)(um_per_pixel * 1000);
            button_Navi_start.Enabled = false;
        }

        private void button_NaviValid_Click(object sender, EventArgs e)
        {
            double width_in = Convert.ToDouble(numericalUpDown_Width.Value);
            double length_in = Convert.ToDouble(numericalUpDown_Length.Value);
            double pixel_in = Convert.ToDouble(numericalUpDown_Pixel.Value);
            int mag_in = Convert.ToInt32(numericalUpDown_Mag.Value);
            if ((width_in * length_in * pixel_in) > 0)
            {
                double[] result = NaviManage.Valid_Input(length_in, mag_in);
                string output = String.Format("Image grids: {0:F0} x {0:F0}, Area: {1:F2} x {2:F2} um, Pixel : {3:F2} nm", result[0], result[2], result[3], result[4]*1000);
                label_validResult.Text = String.Format("Image grids: {0:F0} x {0:F0}", result[0]);
                label_area.Text = String.Format("Area: {0:F2} x {1:F2} um", result[2], result[3]);
                label_resolu.Text = String.Format("Pixel resolution: {0:F2}", result[4]*1000);
                button_Navi_start.Enabled = true;
            }
            else
            {
                button_Navi_start.Enabled = false;
            }

        }

        private void button_Navi_start_Click(object sender, EventArgs e)
        {
            button_Navi_start.Enabled = false;
            int x_pos = Convert.ToInt32(numericalUpDown_XCen.Value);
            int y_pos = Convert.ToInt32(numericalUpDown_YCen.Value);
            NaviManage.Start_Navi_Map(x_pos, y_pos);
            
        }

        private void button_Navi_abort_Click(object sender, EventArgs e)
        {
            NaviManage.Abort_Navi_Map();
        }

        private void Navi_Thread_Handler(string status)
        {
            if(status == "Navigation: Acquisition start")
            {
                button_safe_Enable(button_Navi_abort, true);
                MY_DEBUG(status);
            }
            else if(status == "Navigation: Acquisition abort")
            {
                button_safe_Enable(button_Navi_abort, false);
                button_safe_Enable(button_Navi_start, true);
                MY_DEBUG(status);
            }
            else if (status == "Navigation: Acquisition success")
            {
                MY_DEBUG(status);
            }
        }

        //utilits 
        void MY_DEBUG(string meassage)
        {
            EventPrintedStr?.Invoke(meassage);
        }

        void button_safe_Enable(Button button, bool status)
        {
            if (button.InvokeRequired)
            {
                Action actionDelegate = delegate () { button.Enabled = status; };
                button.BeginInvoke(actionDelegate);
            }
            else
                button.Enabled = status;
        }
    }
}
