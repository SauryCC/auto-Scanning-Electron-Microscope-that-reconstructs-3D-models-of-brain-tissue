using System;
using System.IO;
using System.IO.Ports;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace MIcrotome_GUI
{
    public partial class Main_control : Form
    {
        public CCoarsePositioner mCCoarsePositioner;
        public CSEM_SU3500 mCSEM_TCPIP;
        //public CSEMImage_analog mCSEM_Image;
        public CStage mStageControl;
        public Knife_Controller mKnifeController;
        public double Coarse_Max_StepSize_nm = 1783;
        public double Coarse_Max_DACValue_Bit = 4095;
        public int default_stageSpeed = 500;

        public COVManage OVManage;
        public CCoordinate CoordinateManage;
        public CNaviManage NaviMange;

        private Bitmap minimap_background;
        private Bitmap minimap_background_with_ROI;
        private const int residualImageNumber = 5;
        private const double residualDiffThreshold = 2;
        private Dictionary<uint, Queue<Bitmap>> residueBitmaps;

        private static Mutex Navi_lock = new Mutex();

        public Main_control()
        {
            InitializeComponent();
            mMainFormRunning = true;
            mCCoarsePositioner = new CCoarsePositioner(CoarsePositionSensorConnected, Coarse_Max_StepSize_nm, Coarse_Max_DACValue_Bit);
            //mCSEM_Image = new CSEMImage();
            mCSEM_TCPIP = new CSEM_SU3500(this);
            SEM_Image_Initialize();
            OVManage = new COVManage();
            CoordinateManage = new CCoordinate();
            mStageControl = new CStage(this, mCCoarsePositioner, mCSEM_TCPIP);
            NaviMange = new CNaviManage(this, mStageControl);
            mKnifeController = new Knife_Controller(this);
            residueBitmaps = new Dictionary<uint, Queue<Bitmap>>();
            MY_DEBUG("Program Initialized");
            BackgroundThread_Initialization();

            // wait for connections to establish and update the UI
            Thread.Sleep(1000);
            mStageControl.UpdateSEMValues();
            numericUpDown_Xpos.Value = mStageControl.Get_Xaxis();
            numericUpDown_Ypos.Value = mStageControl.Get_Yaxis();
            NumericUpDown_Mag.Value = mStageControl.Get_Mag();
            NumericUpDown_accV.Value = mStageControl.Get_AccVol();
            NumericUpDown_Focus.Value = mStageControl.Get_Focus(false);
            NumericUpDown_WD.Value = mStageControl.Get_WD();
            NumericUpDown_MagROI.Value = mStageControl.Get_Mag();
            NumericUpDown_FocusROI.Value = mStageControl.Get_Focus(false);
        }

        private void Main_control_Load(object sender, EventArgs e)
        {
            string[] ports = SerialPort.GetPortNames();
            comboBox_COMPort.Items.AddRange(ports);
        }

        public void MY_DEBUG(string inf, string ns = "Main Control")
        {
            inf = ns + ": " + inf;
            System.Diagnostics.Debug.WriteLine(inf);

            //Trace.WriteLine
            if (listBox_MainForm.InvokeRequired)
            {
                Action<string> actionDelegate = delegate (string txt) { listBox_MainForm.Items.Add(txt); };
                listBox_MainForm.BeginInvoke(actionDelegate, inf);
            }
            else
                listBox_MainForm.Items.Add(inf);
        }

        #region MainForm Topbar functions
        private void StageConnect_CheckedChanged(object sender, EventArgs e)
        {
            if (check_StageConnect.Checked == true)
            {
                SmarAct_connected = true;
                if (mStageControl.Initialize() == 0) check_StageConnect.BackColor = Color.Green;
                else check_StageConnect.BackColor = Color.Red;
            }
            else
            {
                mStageControl.Disconnect();
                SmarAct_connected = false;
                check_StageConnect.BackColor = Color.Transparent;
            }
        }

        private void checkBox_SEMimage_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox_SEMimage.Checked == true)
            {
                bool result = SEM_Image_Streaming();
                checkBox_SEMimage.Checked = result;
                //button_buildNavi.Enabled = true;
            }
            else
            {
                bool result = SEM_Image_Stop();
                checkBox_SEMimage.Checked = !result;
                //button_buildNavi.Enabled = false;
            }
        }

        private void TabControl_Main_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabControl_Main.SelectedTab == tabPage_SEM)
            {
                comboBox_roiSEM.DataSource = CoordinateManage.ROIs.ToArray();
                comboBox_roiSEM.SelectedIndex = comboBox_ROIset.SelectedIndex;
                NumericUpDown_MagROI.Value = mStageControl.Get_Mag();
                NumericUpDown_FocusROI.Value = mStageControl.Get_Focus()[1];
            }
            else if (TabControl_Main.SelectedTab == tabPage_AutoCycle)
            {
                listBox_AutoCycleROI.DataSource = CoordinateManage.ROIs.ToArray();
                listBox_AutoCycleROI.SelectedIndex = -1;
                textBoxSaveDir.Text = Directory.GetCurrentDirectory();
            }
            else if (TabControl_Main.SelectedTab == tabPage_ROI)
            {
                comboBox_ROIset.DataSource = CoordinateManage.ROIs.ToArray();
                comboBox_ROIset.SelectedIndex = -1;
                label_roiMagnification.Text = mStageControl.Get_Mag().ToString();
                label_roiFocus.Text = mStageControl.Get_Focus(false).ToString();
                NumericUpDownROI_XLoc.Value = (decimal)Math.Round((double)mStageControl.Get_Xaxis() / 1000, 0);
                NumericUpDownROI_YLoc.Value = (decimal)Math.Round((double)mStageControl.Get_Yaxis() / 1000, 0);
            }
            else if (TabControl_Main.SelectedTab == tabPage_Manual)
            {
                numericUpDown_Xpos.Value = mStageControl.Get_Xaxis();
                numericUpDown_Ypos.Value = mStageControl.Get_Yaxis();
            }
        }
        #endregion

        #region Manual Control -> Stage functions
        private void check_encoderEnable_CheckedChanged(object sender, EventArgs e)
        {
            if (check_encoderEnable.Checked == true)
                mCCoarsePositioner.SetSensorMode(CSmarAct.SA_SENSOR_POWERSAVE);
            else
                mCCoarsePositioner.SetSensorMode(CSmarAct.SA_SENSOR_DISABLED);
        }

        #region XY Button Click Functions
        private void button_XP_Click(object sender, EventArgs e)
        {
            int stepSize = (int)(Math.Round(numericUpDown_stepSize.Value / 1000, 0) * 1000);
            int current_x = mStageControl.Get_Xaxis();
            int current_y = mStageControl.Get_Yaxis();
            mStageControl.Move_XYaxis(Math.Max(current_x - stepSize, (int)SEM_stageX_min), current_y);
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }

        private void button_XN_Click(object sender, EventArgs e)
        {
            int stepSize = (int)(Math.Round(numericUpDown_stepSize.Value / 1000, 0) * 1000);
            int current_x = mStageControl.Get_Xaxis();
            int current_y = mStageControl.Get_Yaxis();
            mStageControl.Move_XYaxis(Math.Min(current_x + stepSize, (int)SEM_stageX_max), current_y);
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }

        private void button_YP_Click(object sender, EventArgs e)
        {
            int stepSize = (int)(Math.Round(numericUpDown_stepSize.Value / 1000, 0) * 1000);
            int current_x = mStageControl.Get_Xaxis();
            int current_y = mStageControl.Get_Yaxis();
            mStageControl.Move_XYaxis(current_x, Math.Min(current_y + stepSize, (int)SEM_stageY_max));
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }

        private void button_YN_Click(object sender, EventArgs e)
        {
            int stepSize = (int)(Math.Round(numericUpDown_stepSize.Value / 1000, 0) * 1000);
            int current_x = mStageControl.Get_Xaxis();
            int current_y = mStageControl.Get_Yaxis();
            mStageControl.Move_XYaxis(current_x, Math.Max(current_y - stepSize, (int)SEM_stageY_min));
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }
        #endregion


        #region Z Button Click Functions
        private void button_ZP_Click(object sender, EventArgs e)
        {
            double stepSize = Convert.ToDouble(numericUpDown_stepSize.Text);
            mStageControl.Move_Zaxis(ZP_DIR, stepSize);
        }

        private void button_ZN_Click(object sender, EventArgs e)
        {
            double stepSize = Convert.ToDouble(numericUpDown_stepSize.Text);
            mStageControl.Move_Zaxis(ZN_DIR, stepSize);
        }
        #endregion

        private void button_manualMove_Click(object sender, EventArgs e)
        {
            int x_pos = Convert.ToInt32(numericUpDown_Xpos.Text);
            int y_pos = Convert.ToInt32(numericUpDown_Ypos.Text);
            int z_pos = Convert.ToInt32(numericUpDown_Zpos.Text);
            // check if xyz within the coordinate limit
            mCCoarsePositioner.MoveDistance_Absolute(CSmarAct.SA_Z_AXIS, z_pos);
            mStageControl.Move_XYaxis(x_pos, y_pos);
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }
        private void button_home_Click(object sender, EventArgs e)
        {
            // Set the initial position to 0.
            mCCoarsePositioner.MoveDistance_Absolute(CSmarAct.SA_Z_AXIS, 0);
            mStageControl.Move_XYaxis((int)SEM_stageX_home, (int)SEM_stageY_home);
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }
        #endregion

        #region ROI/Grid Setup -> ROIs Functions
        private void comboBox_ROIset_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox_ROIset.SelectedIndex != -1)
            {
                uint roi_id = (uint)comboBox_ROIset.SelectedIndex;
                label_roiMagnification.Text = CoordinateManage.get_ROI_mag(roi_id).ToString();
                label_roiFocus.Text = CoordinateManage.get_ROI_foc(roi_id).ToString();
                double x, y;
                (x, y) = CoordinateManage.get_ROI_pos(roi_id);
                NumericUpDownROI_XLoc.Value = (decimal)x;
                NumericUpDownROI_YLoc.Value = (decimal)y;
            }
        }

        private void label_roiMagnification_TextChanged(object sender, EventArgs e)
        {
            double width = CNaviManage.SEM_initial_length / Double.Parse(label_roiMagnification.Text);
            double height = CNaviManage.SEM_initial_length / (double)CNaviManage.SEM_x_pixel * CNaviManage.SEM_y_pixel / Double.Parse(label_roiMagnification.Text);
            label_roiSize.Text = String.Format("{0:F2} x {1:F2}", width, height);
        }

        private void button_roiSave_Click(object sender, EventArgs e)
        {
            if (comboBox_ROIset.SelectedValue is null)
            {
                MY_DEBUG("You have not selected a ROI");
                return;
            }

            uint roi_id = (uint)comboBox_ROIset.SelectedValue;
            int selected_idx = comboBox_ROIset.SelectedIndex;
            int x = (int)NumericUpDownROI_XLoc.Value;
            int y = (int)NumericUpDownROI_YLoc.Value;

            if (!CoordinateManage.modify_ROI_pos(roi_id, x, y))
            {
                MY_DEBUG("Cannot modify the selected ROI position");
                return;
            }

            comboBox_ROIset.DataSource = CoordinateManage.ROIs.ToArray();
            comboBox_ROIset.SelectedIndex = selected_idx;
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }

        private void button_NewROI_Click(object sender, EventArgs e)
        {
            int x = (int)NumericUpDownROI_XLoc.Value;
            int y = (int)NumericUpDownROI_YLoc.Value;
            int mag = Int32.Parse(label_roiMagnification.Text);
            int focus = Int32.Parse(label_roiFocus.Text);

            int new_idx = CoordinateManage.new_ROI(x, y, mag, focus);
            comboBox_ROIset.DataSource = CoordinateManage.ROIs.ToArray(); 
            comboBox_ROIset.SelectedIndex = new_idx;
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }

        private void button_DelROI_Click(object sender, EventArgs e)
        {
            if (comboBox_ROIset.SelectedValue is null)
            {
                MY_DEBUG("You have not selected a ROI");
                return;
            }

            uint roi_id = (uint)comboBox_ROIset.SelectedValue;
            if (!CoordinateManage.remove_ROI(roi_id))
                MY_DEBUG("Cannot delete the selected ROI");
            else
            {
                comboBox_ROIset.DataSource = CoordinateManage.ROIs.ToArray();
                comboBox_ROIset.SelectedIndex = -1;
                comboBox_ROIset.Text = "";
            }
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }
        #endregion

        #region SEM Control -> SEM Setting Functions
        private void button_SEMget_Click(object sender, EventArgs e)
        {
            if (mStageControl.UpdateSEMValues() != 0)
            {
                MY_DEBUG("Cannot get SEM values, check if SEM is connected.");
                return;
            }
            int focus = mStageControl.Get_Focus(course: false);
            int magnification = mStageControl.Get_Mag();
            int accVol = mStageControl.Get_AccVol();
            int WD = mStageControl.Get_WD();

            NumericUpDown_Focus.Value = focus;
            NumericUpDown_accV.Value = accVol;
            NumericUpDown_Mag.Value = magnification;
            NumericUpDown_WD.Value = WD;
        }

        private void button_HV_Click(object sender, EventArgs e)
        {
            if (mStageControl.Set_HV(true) != 0) MY_DEBUG("Cannot set SEM HV values, check if SEM is connected.");
        }

        private void button_HV_OFF_Click(object sender, EventArgs e)
        {
            if (mStageControl.Set_HV(false) != 0) MY_DEBUG("Cannot set SEM HV values, check if SEM is connected.");
        }

        private void button_setSEM_Click(object sender, EventArgs e)
        {
            if (radioButton_Mag.Checked)
            {
                mStageControl.Set_Mag((int)NumericUpDown_Mag.Value);
                if (minimap_background != null) post_new_Navi_img(minimap_background);
            }
            else if (radioButton_accV.Checked)
            {
                mStageControl.Set_AccVol((int)NumericUpDown_accV.Value);
            }
            else if (radioButton_Focus.Checked)
            {
                mStageControl.Set_AccVol((int)NumericUpDown_Focus.Value);
            }
            else if (radioButton_WD.Checked)
            {
                mStageControl.Set_AccVol((int)NumericUpDown_WD.Value);
            }
            else
            {
                MY_DEBUG("Nothing has chosen.");
            }
        }
        #endregion

        #region SEM Control -> ROI/Tile Focus Setting Functions
        private void button_setROIsem_Click(object sender, EventArgs e)
        {
            if (comboBox_roiSEM.SelectedValue is null)
            {
                MY_DEBUG("You have not selected a ROI");
                return;
            }

            uint roi_id = (uint)comboBox_roiSEM.SelectedValue;
            int selected_idx = comboBox_roiSEM.SelectedIndex;

            if (radioButton_MagROI.Checked)
            {
                int mag = (int)NumericUpDown_MagROI.Value;
                CoordinateManage.modify_ROI_mag(roi_id, mag);
                comboBox_roiSEM.DataSource = CoordinateManage.ROIs.ToArray();
                comboBox_roiSEM.SelectedIndex = selected_idx;
                if (minimap_background != null) post_new_Navi_img(minimap_background);
            }
            else if (radioButton_FocusROI.Checked)
            {
                int foc = (int)NumericUpDown_FocusROI.Value;
                CoordinateManage.modify_ROI_foc(roi_id, foc);
                comboBox_roiSEM.DataSource = CoordinateManage.ROIs.ToArray();
                comboBox_roiSEM.SelectedIndex = selected_idx;
                if (minimap_background != null) post_new_Navi_img(minimap_background);
            }
            else
            {
                MY_DEBUG("Please specify one of the magnification or focus you want to set");
            }
        }

        private void button_goROI_Click(object sender, EventArgs e)
        {
            if (comboBox_roiSEM.SelectedValue is null)
            {
                MY_DEBUG("You have not selected a ROI");
                return;
            }
            
            uint roi_id = (uint)comboBox_roiSEM.SelectedValue;
            // x and y in um
            double x;
            double y;
            (x, y) = CoordinateManage.get_ROI_pos(roi_id);
            mStageControl.Move_XYaxis((int)(x * 1000), (int)(y * 1000));
            if (minimap_background != null) post_new_Navi_img(minimap_background);
        }
        #endregion

        #region Manual Control -> Knife
        private void button_KnifeConnect_Click(object sender, EventArgs e)
        {
            if (mKnifeController.Connect(comboBox_COMPort.Text, comboBox_BaudRate.Text, comboBox_DataBits.Text, comboBox_StopBits.Text, comboBox_ParityBits.Text))
                label_KnifeController.BackColor = Color.Green;
            else
                label_KnifeController.BackColor = SystemColors.Control;
        }

        private void button_KnifeDisconnect_Click(object sender, EventArgs e)
        {
            if (mKnifeController.Disconnect()) label_KnifeController.BackColor = Color.Green;
            else label_KnifeController.BackColor = SystemColors.Control;
        }

        private void button_GetKnifeInfo_Click(object sender, EventArgs e)
        {
            if (mKnifeController.IsConnected)
            {
                int state = 0;
                try
                {
                    NumericUpDown_knifeStart.Value = (decimal)mKnifeController.Get_Vol();
                    state++;
                    NumericUpDown_knifeEnd.Value = NumericUpDown_knifeStart.Value;
                    NumericUpDown_cutSpeed.Value = (decimal)mKnifeController.Get_Vol_Rise_SlewRate();
                    state++;
                    NumericUpDown_retractSpeed.Value = (decimal)mKnifeController.Get_Vol_Fall_SlewRate();
                    state++;
                }
                catch
                {
                    if (state == 0)
                        MY_DEBUG("Cannot call Get_Vol()");
                    else if (state == 1)
                        MY_DEBUG("Cannot call Get_Vol_Rise_SlewRate()");
                    else if (state == 2)
                        MY_DEBUG("Cannot call Get_Vol_Fall_SlewRate()");
                    else
                        MY_DEBUG("Unknown error happens");
                }
            }
            else
                MY_DEBUG("Knife is not connected.");
        }

        private void button_knifeCut_Click(object sender, EventArgs e)
        {
            double rise_slewrate = Convert.ToDouble(NumericUpDown_cutSpeed.Value);
            double fall_slewrate = Convert.ToDouble(NumericUpDown_retractSpeed.Value);
            double start_voltage = Convert.ToDouble(NumericUpDown_knifeStart.Value);
            double end_voltage = Convert.ToDouble(NumericUpDown_knifeEnd.Value);
            mKnifeController.Set_Vol_Rise_SlewRate(rise_slewrate);
            mKnifeController.Set_Vol_Fall_SlewRate(fall_slewrate);
            mKnifeController.Set_Vol(start_voltage);
            mKnifeController.OutPut();
            Thread.Sleep(2000);
            mKnifeController.Set_Vol(end_voltage);
            mKnifeController.OutPut();
        }

        private void button_Sweep_Click(object sender, EventArgs e)
        {
            mStageControl.Move_Zaxis(-1, 10);
            double rise_slewrate = Convert.ToDouble(NumericUpDown_cutSpeed.Value);
            double fall_slewrate = Convert.ToDouble(NumericUpDown_retractSpeed.Value);
            double start_voltage = Convert.ToDouble(NumericUpDown_knifeStart.Value);
            double end_voltage = Convert.ToDouble(NumericUpDown_knifeEnd.Value);
            mKnifeController.Set_Vol_Rise_SlewRate(rise_slewrate);
            mKnifeController.Set_Vol_Fall_SlewRate(fall_slewrate);
            mKnifeController.Set_Vol(start_voltage);
            mKnifeController.Set_Vol(end_voltage);
            mKnifeController.Set_Vol(start_voltage);
            mStageControl.Move_Zaxis(1, 10);
        }
        #endregion

        #region Auto Cycle -> Auto Cycle Setup functions
        private void button_AutoCycleStart_Click(object sender, EventArgs e)
        {
            int total_iteration = (int)numericUpDown_AutoCycleTotalItr.Value;
            double z_step_size = (double)numericUpDown_AutoCycleZStep.Value;
            String save_dir = textBoxSaveDir.Text;
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/jpeg");
            Encoder encoder = Encoder.Quality;
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(encoder, 25L);
            myEncoderParameters.Param[0] = myEncoderParameter;

            for (int i = 0; i < total_iteration; i++)
            {
                foreach (Coordinate roi in CoordinateManage.ROIs)
                {
                    // set magnification
                    while (true)
                    {
                        int return_status = mStageControl.Set_Mag(roi.magification);

                        if (return_status == 0)
                        {
                            break;
                        }
                        else if (return_status == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            MY_DEBUG("AutoCycle: Set SEM magnification fail");
                            return;
                        }
                    }
                    // set focus
                    while (true)
                    {
                        int return_status = mStageControl.Set_Focus(roi.focus, false);

                        if (return_status == 0)
                        {
                            break;
                        }
                        else if (return_status == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            MY_DEBUG("AutoCycle: Set SEM focus fail");
                            return;
                        }
                    }
                    while (true)
                    {
                        int return_status = mStageControl.Move_XYaxis((int)Math.Round(roi.x * 1000), (int)Math.Round(roi.y * 1000));

                        if (return_status == 0)
                        {
                            break;
                        }
                        else if (return_status == 1)
                        {
                            Thread.Sleep(1000);
                        }
                        else
                        {
                            MY_DEBUG("AutoCycle: Move SEM stage fail");
                            return;
                        }
                    }
                    Thread.Sleep(3000);  // wait for the image to be stable
                    Bitmap SEM_img = get_SEM_image();
                    // Save referrence images for each roi and get the reference image
                    if (!residueBitmaps.ContainsKey(roi.id))
                        residueBitmaps.Add(roi.id, new Queue<Bitmap>(residualImageNumber));

                    Bitmap referenceImage = null;
                    if (residueBitmaps[roi.id].Count == residualImageNumber)
                        referenceImage = residueBitmaps[roi.id].Dequeue();
                    else if (residueBitmaps[roi.id].Count > 0)
                        referenceImage = residueBitmaps[roi.id].Peek();
                    else
                        referenceImage = SEM_img;

                    // Compare the current image and the referrence image to check if there is any residuals.
                    SEM_img.Save(Path.Combine(save_dir, "sem_tmp.jpg"), myImageCodecInfo, myEncoderParameters);
                    referenceImage.Save(Path.Combine(save_dir, "reference_tmp.jpg"), myImageCodecInfo, myEncoderParameters);
                    Bitmap SEM_compare = new Bitmap(Path.Combine(save_dir, "sem_tmp.jpg"));
                    Bitmap reference_compare = new Bitmap(Path.Combine(save_dir, "reference_tmp.jpg"));
                    // TODO: might want to change to while loop
                    if (NaviMange.residue_check(SEM_compare, reference_compare, residualDiffThreshold))
                    {
                        // Clean it using dimand knife and take the photo again.
                        mStageControl.Move_Zaxis(ZP_DIR, -10);
                        Thread.Sleep(2000);  // wait for the move to finish
                        
                        // Swap the knife
                        // mKnifeController.Set_Vol(100);
                        // Thread.Sleep(2000);
                        // mKnifeController.Set_Vol(0);
                        // Thread.Sleep(2000);

                        mStageControl.Move_Zaxis(ZP_DIR, 10);
                        Thread.Sleep(2000);  // wait for the move to finish

                        SEM_img = get_SEM_image();
                        SEM_img.Save(Path.Combine(save_dir, "sem_tmp.jpg"), myImageCodecInfo, myEncoderParameters);
                        SEM_compare = new Bitmap(Path.Combine(save_dir, "sem_tmp.jpg"));
                    }
                    residueBitmaps[roi.id].Enqueue(SEM_img);

                    SEM_img.Save(Path.Combine(save_dir, string.Format("ROI_{0}_itr_{1}.jpg", roi.id, i)), myImageCodecInfo, myEncoderParameters);
                }

                // cut sample once
                // mKnifeController.Set_Vol(100);
                // Thread.Sleep(2000);
                // mKnifeController.Set_Vol(0);
                // Thread.Sleep(2000);

                // move one step in z direction
                mStageControl.Move_Zaxis(ZP_DIR, z_step_size);
            }
            MY_DEBUG("AutoCycle Done");
            File.Delete(Path.Combine(save_dir, "sem_tmp.jpg"));
            File.Delete(Path.Combine(save_dir, "reference_tmp.jpg"));
        }
        #endregion

        #region Navigation functions
        private void button_buildNavi_Click(object sender, EventArgs e)
        {
            Form_NavigationBuild Build_Navi = new Form_NavigationBuild(NaviMange, mStageControl);
            NaviMange.EventSendNaviImg -= post_new_Navi_img;
            NaviMange.EventSendNaviImg += post_new_Navi_img;
            Build_Navi.Show();
        }

        public void post_new_Navi_img(Bitmap inputImg)
        {
            Navi_lock.WaitOne();
            // update the background image and resize it to fit to picture box
            minimap_background = new Bitmap(inputImg);
            // check if there is any existing ROIs. 
            // calculate their position and update the map
            Brush brush = new SolidBrush(Color.Red);
            Pen pen = new Pen(brush);
            Font bigFont = new Font(DefaultFont.FontFamily, 14, FontStyle.Regular);
            using (Graphics gr = Graphics.FromImage(inputImg))
            {
                foreach (Coordinate roi in CoordinateManage.ROIs)
                {
                    int[] rect = NaviMange.get_ROI_position(roi.x, roi.y, roi.width, roi.height);
                    gr.DrawRectangle(pen, rect[0], rect[1], rect[2], rect[3]);
                    gr.DrawString(roi.name, bigFont, brush, rect[0]+1, rect[1]+1);
                }
            }

            // update background with ROI to let thread handle SEM position
            minimap_background_with_ROI = new Bitmap(inputImg);

            // update the navigation picture box
            pictureBox_Navi.Image = new Bitmap(inputImg, pictureBox_Navi.Width, pictureBox_Navi.Height);
            Navi_lock.ReleaseMutex();
        }
        #endregion

        #region Util functions
        private static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                    return encoders[j];
            }
            return null;
        }
        #endregion
    }
}
