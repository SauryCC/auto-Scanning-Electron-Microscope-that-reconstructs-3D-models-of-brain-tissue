//using CLRWrapper;
using System.Threading;
///using System.Windows.Media.Media3D;
namespace MIcrotome_GUI
{
    public class CStage
    {
        CCoarsePositioner Z_stage;
        Main_control mainForm;
        CSEM_SU3500 xy_stage;
        public double Coarse_Max_StepSize_nm ;
        public double Coarse_Max_DACValue_Bit;
        public bool CPConnected;

        public const uint Caxis_z = 0;
        public double z_pos;

        //parameters
        private const string name_space = "CStage";


        public CStage(Main_control mainForm, CCoarsePositioner mCCoarsePositioner, CSEM_SU3500 SEM)
        {
            Z_stage = mCCoarsePositioner;
            xy_stage = SEM;
            this.mainForm = mainForm;

            Coarse_Max_StepSize_nm = Z_stage.mMax_StepSize_nm;
            Coarse_Max_DACValue_Bit = Z_stage.mMax_DACValue_Bit;
            CPConnected = false;
        }

        ~CStage() { Disconnect(); }

        // update all system values, 0 means success, 1 means busy, and 2 means failed
        public int UpdateSEMValues()
        {
            int result = xy_stage.UpdateHV();
            // UpdateHV test
            if (result == 0)
            { 
                mainForm.MY_DEBUG("SEM HV updated", name_space);
            }
            else if (result == 1) 
            {
                mainForm.MY_DEBUG("SEM HV update busy", name_space);
            }
            else 
            { 
                mainForm.MY_DEBUG("SEM update HV failed", name_space); 
                return 2; 
            }
            // UpdateFocus test
            result = xy_stage.UpdateFocus();
            if (result == 0) 
            {
                mainForm.MY_DEBUG("SEM Focus updated", name_space);
            }
            else if (result == 1) 
            {
                mainForm.MY_DEBUG("SEM Focus update busy", name_space);
            }
            else 
            { 
                mainForm.MY_DEBUG("SEM update Focus failed", name_space); 
                return 2; 
            }
            // UpdateMagnification test
            result = xy_stage.UpdateMagnification();
            if (result == 0) 
            { 
                mainForm.MY_DEBUG("SEM Magnification updated", name_space);
            }
            else if (result == 1)
            {
                 mainForm.MY_DEBUG("SEM Magnification update busy", name_space);
            }
            else 
            { 
                mainForm.MY_DEBUG("SEM update Magnification failed", name_space); 
                return 2; 
            }
            // UpdateStage test
            result = xy_stage.UpdateStage();
            if (result == 0) 
            {
                mainForm.MY_DEBUG("SEM Stage updated", name_space);
            }
            else if (result == 1) 
            {
                mainForm.MY_DEBUG("SEM Stage update busy", name_space);
            }
            else 
            { 
                mainForm.MY_DEBUG("SEM update Stage failed", name_space); 
                return 2; 
            }
            // UpdateVoltage test
            result = xy_stage.UpdateVoltage();
            if (result == 0) 
            {
                mainForm.MY_DEBUG("SEM Voltage updated", name_space);
            }
            else if (result == 1) 
            {
                mainForm.MY_DEBUG("SEM Voltage update busy", name_space);
            }
            else 
            { 
                mainForm.MY_DEBUG("SEM update Voltage failed", name_space); 
                return 2; 
            }
            // UpdateWD test
            result = xy_stage.UpdateWD();
            if (result == 0) 
            {
                mainForm.MY_DEBUG("SEM WD updated", name_space);
            }
            else if (result == 1) 
            {
                mainForm.MY_DEBUG("SEM WD update busy", name_space);
            }
            else 
            { 
                mainForm.MY_DEBUG("SEM update WD failed", name_space); 
                return 2; 
            }

            return 0;
        }

        public int UpdateStageValue()
        {
            return xy_stage.UpdateStage();
        }


        // Initializes the SEM to ready for operation
        // Must be called at the beginning
        //0 means success, 1 means busy, and 2 means failed
        public int Initialize()
        {
            uint status = Z_stage.Initialize();
            if (status == 0)
            {
                CPConnected = true;
                mainForm.MY_DEBUG("Z Stage Initialized", name_space);
            }
            else
            {
                mainForm.MY_DEBUG("Z Stage not able to initialize", name_space);
            }

            //SEM
            if (xy_stage.IsConnected)
            {
                mainForm.MY_DEBUG("SEM ready", name_space);
                if (UpdateSEMValues() != 0)
                {
                    mainForm.MY_DEBUG("Cannot retrieve SEM values", name_space);
                    return 2;
                }
            }
            else
            {
                // try to connect and wait for 1 second to let connection established
                xy_stage.Connect();
                Thread.Sleep(1000);

                if (xy_stage.IsConnected)
                {
                    mainForm.MY_DEBUG("SEM ready", name_space);
                    if (UpdateSEMValues() != 0)
                    {
                        mainForm.MY_DEBUG("Cannot retrieve SEM values", name_space);
                        return 2;
                    }
                }
                else
                {
                    mainForm.MY_DEBUG("Failed to connect to SEM", name_space);
                    return 2;
                }
            }
            return (int)status;
        }

        // disconnect with hardware
        public void Disconnect()
        {
            Z_stage.Disconnect();
            xy_stage.Disconnect();
        }

        #region MCS Z-axis functions
        public void Move_Zaxis(int direction, double distance)
        {
            double stepSize = direction * distance;
            Z_stage.MoveDistance(CSmarAct.SA_Z_AXIS, stepSize);
        }

        //get current z axis position
        public double Get_Zaxis()
        {
            int status = Z_stage.GetPosition(Caxis_z);
            if (status == 1)
            {
                double Position = Z_stage.GetStorePosition(Caxis_z);
                z_pos = Position / 1000;    // in um
                return z_pos;
            }
            else
                return 0;
        }
        #endregion

        #region SEM XY-axis functions
        
        // move SEM to specific X,Y position
        // the input position must be within the range stated below
        //if move successful, return true, if fail return false
        // input is the customed xy coordinates
        // position is [X,Y,Z,T,R]
        //X : 0 to 100000000 (nm) 
        //Y : 0 to 50000000 (nm) (1000 nm Step) 
        public int Move_XYaxis(int x, int y)
        {
            int[] position = new int[5];
            position[0] = x;
            position[1] = y;
            position[2] = xy_stage.StageZ;
            position[3] = xy_stage.StageT;
            position[4] = xy_stage.StageR;

            return xy_stage.SetStage(position);
        }

        // get current x axis value as an int
        public int Get_Xaxis() { return xy_stage.StageX; }

        // get current y axis value as an int
        public int Get_Yaxis() { return xy_stage.StageY; }
        #endregion

        #region SEM width and height function
        // return the current SEM width in um
        public double Get_SEM_wdith()
        {
            return CNaviManage.SEM_initial_length / (double)xy_stage.Magnification;
        }

        // return the current SEM height in um
        public double Get_SEM_height()
        {
            return CNaviManage.SEM_initial_length / (double)CNaviManage.SEM_x_pixel * CNaviManage.SEM_y_pixel / xy_stage.Magnification;
        }
        #endregion

        #region SEM Focus functions
        // sets the focus of the SEM microscope
        //0 means success, 1 means busy, and 2 means failed
        // focus[0] = SEM_focus_Coarse;
        // focus[1] = SEM_focus_fine;
        public int Set_Focus(int[] focus)
        {
            return xy_stage.SetFocus(focus);
        }
        
        // sets the focus of the SEM microscope, with specified course value. 
        // Input regulations stated below
        // if course == true set course focus, else set fine focus
        //0 means success, 1 means busy, and 2 means failed
        // focus[0] = SEM_focus_Coarse;
        // focus[1] = SEM_focus_fine;
        // Course value：0～16383
        public int Set_Focus(int focus, bool course)
        {
            if (course) return xy_stage.SetFocus(new int[] { focus, xy_stage.Focus[1] });
            else return xy_stage.SetFocus(new int[] { xy_stage.Focus[0], focus });
        }

        // return how many percentage (%) current focus is, based on maximum value
        // f[0] /= SEM_focus_max_coarse;
        // f[1] /= SEM_focus_max_fine;
        public double[] Get_Focus_percentage()
        {  
            double[] persentages = new double[] { 0, 0 };
            persentages[0] = (double)xy_stage.Focus[0] / Main_control.SEM_course_max;
            persentages[1] = (double)xy_stage.Focus[1] / Main_control.SEM_fine_max;
            return persentages;
        }

        // return the focus information of the SEM
        // the returned data follow the data as below
        // f[0] = focus_Coarse;
        // f[1] = SEM_focus_fine;
        // returns actual values of coarse and fine
        // Course value：0～65535
        // Fine value：0～65535
        public int[] Get_Focus()
        { 
            return xy_stage.Focus;
        }

        public int Get_Focus(bool course)
        {
            if (course) return xy_stage.Focus[0];
            else return xy_stage.Focus[1];
        }
        #endregion

        #region SEM Magnification functions
        // sets the magnification value for the SEM
        // 0 means success, 1 means busy, and 2 means failed
        // magnification : 5～8000000 (×5～×8,000 k)
        public int Set_Mag(int magnification)
        {
            return xy_stage.SetMagnification(magnification);
        }

        // returns the magnification value for the SEM
        // returned magnification range is 5～8000000 (×5～×8,000 k)
        public int Get_Mag()
        {
            return xy_stage.Magnification;
        }
        #endregion

        #region SEM WD functions
        // sets the WD value for the SEM
        // 0 means success, 1 means busy, and 2 means failed
        // wd : 4000～70000 (4.0～70.00 mm)
        public int Set_WD(int wd)
        {
            return xy_stage.SetWD(wd);
        }

        // returns the current SEM WD
        // WD : 4000～70000 (4.0～70.00 mm)
        public int Get_WD()
        {
            return xy_stage.WD;
        }
        #endregion

        #region SEM HV functions
        // sets the HV value for the SEM
        // 0 means success, 1 means busy, and 2 means failed
        public int Set_HV(bool on_off)
        {
            return xy_stage.SetHV(on_off);
        }

        // gets the HV value for the SEM
        // true means on, false means off
        public bool Get_HV()
        {
            return xy_stage.HV;
        }
        #endregion

        #region SEM Voltage functions
        // sets the acceleration voltage for the SEM
        // 0 means success, 1 means busy, and 2 means failed
        // acceleration_voltage : 300～30000 (0.30～30.0 kV)
        public int Set_AccVol(int acceleration_voltage)
        {
            return xy_stage.SetVoltage(acceleration_voltage);
        }

        // returns the acceleration voltage for the SEM
        public int Get_AccVol()
        {
            return xy_stage.Voltage[0];
        }
        #endregion
    }
}