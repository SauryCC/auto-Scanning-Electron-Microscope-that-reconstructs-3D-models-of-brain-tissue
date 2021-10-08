using System;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;

//using CLRWrapper;
///using System.Windows.Media.Media3D;
namespace MIcrotome_GUI
{
    //public partial class MainWindow : Form
    //{
    public class CSEM_SU3500
    {
        Main_control mainForm;

        //SEM communication
        TcpListener tcpl;
        TcpClient tcpc;
        Thread mThreadSEMConnect;
        byte[] byteReadStream;

        //State parameters
        private bool bTryingConnection;
        private const string name_space = "SEM Controller";
        private const int center_x = (int)Main_control.SEM_stageX_home;  // 60mm
        private const int center_y = (int)Main_control.SEM_stageY_home;  // 25mm
        private const int radius_limit = 102000000;  // 102mm

        //Limits
        const uint voltage_max = 30000;
        const uint voltage_min = 300;
        const uint mag_max = 8000000;
        const uint mag_min = 6;
        public const uint coarse_max = 16383;
        public const uint coarse_min = 0;
        public const uint fine_max = 4095;
        public const uint fine_min = 0;

        readonly int[] stageMax = { 100000000, 50000000, 70000000, 90000, 359900 };
        readonly int[] stageMin = { 0, 0, 5000000, -20000, 0 };
        const uint stageX_max = 100000000; //in nm, step size = 1000nm, 100mm
        const uint stageX_min = 0;
        const uint stageY_max = 50000000; //in nm, step size = 1000nm, 50mm
        const uint stageY_min = 0;
        const uint stageZ_max = 70000000; //in nm, step size = 10 or 100 um, 70mm
        const uint stageZ_min = 5000000; //5mm
        const int stageT_max = 90000; //in 1/1000 deg, step size = 0.1 degree, 90 deg
        const int stageT_min = -20000; //-20 deg
        const int stageR_max = 359900; //in 1/1000 deg, step size = 0.1 degree, 359.9 deg
        const int stageR_min = 0;

        //parameter reading commands
        const string str_hv_get = "0300 0303 0000 Get HVONOFF ALL\r\n";
        const string str_stage_get = "0300 0303 0000 Get STAGEUNIT MOVEXYZTR\r\n";
        const string str_mag_get = "0300 0303 0000 Get MAGNIFICATION NOW\r\n";
        const string str_vacc_get = "0300 0303 0000 Get HVCONTROL VACC\r\n";
        const string str_focus_get = "0300 0303 0000 Get FOCUS NOW\r\n";
        const string str_wd_get = "0300 0303 0000 Get WD NOW\r\n";
        const string str_acc_get = "0300 0303 0000 Get HVCONTROL VACC\r\n";
        //parameter seting commands
        const string str_mag_set = "0300 0303 0000 Set MAGNIFICATION EXECUTE ";//30000(CR)(LF)
        const string str_stage_set = "0300 0303 0000 Set STAGEUNIT MOVEXYZTR ";
        const string str_focus_set = "0300 0303 0000 Set FOCUS ALL ";//30000(CR)(LF)
        const string str_HV_On_set = "0300 0303 0000 Set HVONOFF EXECUTE ON\r\n";//(CR)(LF)"
        const string str_HV_Off_set = "0300 0303 0000 Set HVONOFF EXECUTE OFF\r\n";//(CR)(LF)"
        const string str_wd_set = "0300 0303 0000 Set WD HM ";
        const string str_acc_set = "0300 0303 0000 Set HVCONTROL VACC ";

        //SEM focus limit
        const double SEM_focus_max_coarse = 16383.0f;
        const double SEM_focus_max_fine = 4095.0f;

        public CSEM_SU3500(Main_control mainForm)
        {
            this.mainForm = mainForm;
            PixelSize_nmpp = 1;
            HV = false;
            Magnification = 6;
            Focus = new int[] { 0, 0 };
            Voltage = new int[] { 0, 0 };
            WD = 4000;
            StageX = (int)Main_control.SEM_stageX_home;
            StageY = (int)Main_control.SEM_stageY_home;
            StageZ = 0;
            StageT = 0;
            StageR = 0;

            IsConnected = false;
            bTryingConnection = false;
            IsBusy = false;
        }

        ~CSEM_SU3500() {
            Disconnect();
        }

        public void Connect()
        {
            if (bTryingConnection == false)// to avoid multi connection
            {
                IsConnected = false;
                bTryingConnection = true;
                mThreadSEMConnect = new Thread(Thread_Connect);
                mThreadSEMConnect.Start();
            }
            else
            {
                mainForm.MY_DEBUG("Program is trying to connect to SEM", name_space);
                return;
            }
        }

        public void Disconnect()
        {
            tcpl.Stop();
            mThreadSEMConnect.Abort();
        }

        void Thread_Connect()
        {
            IPAddress Addr = IPAddress.Any;
            tcpl = new TcpListener(Addr, 3000);
            tcpl.Server.ReceiveTimeout = 2000;
            tcpl.Server.SendTimeout = 2000;

            mainForm.MY_DEBUG("TCPIP start.", name_space);
    
            try
            {
                tcpl.Start(); // block application until data and connection
                tcpc = tcpl.AcceptTcpClient();
                IsConnected = true;
                mainForm.MY_DEBUG("SEM TCPIP established", name_space);
                byteReadStream = new byte[1000]; //allocate space
            }
            catch
            {
                IsConnected = false;
                mainForm.MY_DEBUG("SEM TCPIP AcceptTcpClient caught", name_space);
            }
            bTryingConnection = false;
        }

        private string SendData(string data_string)
        {
            string data_string_back = null;
            try
            {
                if (IsConnected)
                {
                    if (IsBusy == false)
                    {
                        IsBusy = true;
                        byte[] data_byte = Encoding.ASCII.GetBytes(data_string);

                        tcpc.GetStream().Write(data_byte, 0, data_byte.Length);
                        Thread.Sleep(20);
                        tcpc.GetStream().Read(byteReadStream, 0, tcpc.Available);
                        //Console.WriteLine(Encoding.Default.GetString(byteReadStream) + "\n");
                        data_string_back = Encoding.Default.GetString(byteReadStream);
                        IsBusy = false;
                    }
                    else
                    {
                        mainForm.MY_DEBUG("SEM: SEM busy miss", name_space);
                        data_string_back = "SEM busy";
                    }           
                }
            }
            catch
            {
                IsConnected = false;
                tcpl.Stop();
                mainForm.MY_DEBUG("SEM ethernet disconnected. try to reconnect.", name_space);
            }
            return data_string_back;
        }

        #region Property Functions
        public bool IsConnected { get; private set; }

        public bool IsBusy { get; private set; }

        public double PixelSize_nmpp { get; private set; }
        #endregion

        #region SEM HV functions
        public int UpdateHV()
        {
            if (IsConnected)
            {
                string return_data = SendData(str_hv_get); //"0300 0303 0000 Get HVONOFF ALL\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0300 0303 0000 Get HVONOFF ALL 0 IDLE\r\n"
                int start = return_data.LastIndexOf("ALL") + 4;
                int end = return_data.IndexOf(" IDLE");
                if (end == -1)
                {
                    end = return_data.LastIndexOf(" PROCESSING");
                }
                HV = (Convert.ToInt16(return_data.Substring(start, end - start)) == 1);
                return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Get HV not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int SetHV(bool on_off)
        {
            if (IsConnected)
            {
                if (on_off != HV)
                {
                    string return_data;
                    if (on_off)
                        return_data = SendData(str_HV_On_set); //"0303 0300 0000 Set HVONOFF EXECUTE ON\r\n"
                    else
                        return_data = SendData(str_HV_Off_set); //"0303 0300 0000 Set HVONOFF EXECUTE Off\r\n"

                    if (return_data == null)
                        return 2;
                    else if (return_data == "SEM busy")
                        return 1;
                    //Retrun example "0303 0300 0000 Set HVONOFF EXECUTE ON OK\r\n"
                    if (return_data.Contains("OK"))
                    {
                        mainForm.MY_DEBUG("Set HV executed", name_space);
                        HV = on_off;
                        return 0;
                    }
                    else if (return_data.Contains("NG"))
                        mainForm.MY_DEBUG("Set HV not exectued: SEM is busy", name_space);
                    else if (return_data.Contains("PARAMERROR"))
                        mainForm.MY_DEBUG("Set HV not exectued: Wrong parameter", name_space);
                    else
                        mainForm.MY_DEBUG("Set HV not exectued: invalid return value", name_space);
                    return 2;
                }
                else
                    return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Set HV not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public bool HV { get; private set; }
        #endregion

        #region SEM Stage functions
        public int UpdateStage()
        {
            if (IsConnected)
            {
                string return_data = SendData(str_stage_get); //"0300 0303 0000 Get STAGEUNIT MOVEXYZTR\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Get STAGEUNIT MOVEXYZTR 2000,2000,2000,0,0 IDLE\r\n"
                int start = return_data.LastIndexOf("MOVEXYZTR") + 10;
                int end = return_data.IndexOf(" IDLE");
                if (end == -1)
                {
                    end = return_data.LastIndexOf(" PROCESSING");
                }
                string sub_string = return_data.Substring(start, end - start);
                string[] split_str = sub_string.Split(',');
                StageX = Convert.ToInt32(split_str[0]);
                StageY = Convert.ToInt32(split_str[1]);
                StageZ = Convert.ToInt32(split_str[2]);
                StageT = Convert.ToInt32(split_str[3]);
                StageR = Convert.ToInt32(split_str[4]);
                return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Get Stage not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int SetStage(int[] position)
        {
            if (position.Length != 5)
            {
                mainForm.MY_DEBUG("Set Stage not exectued: invalid input length", name_space);
                return 2;
            }

            if (((position[0] - center_x) ^ 2 + (position[1] - center_y) ^ 2) > (radius_limit ^ 2))
            {
                mainForm.MY_DEBUG("Set Stage not exectued: x y position out of radius", name_space);
                return 2;
            }

            if (IsConnected)
            {
                string str_send = str_stage_set; //"0300 0303 0000 Set STAGEUNIT MOVEXYZTR "
                str_send = str_send + position[0].ToString() + "," + position[1].ToString() + "," + position[2].ToString() + ",";
                str_send = str_send + position[3].ToString() + "," + position[4].ToString() + "\r\n";
                //"0300 0303 0000 Set STAGEUNIT MOVEXYZTR 20000,20000,0,0,0\r\n"
                string return_data = SendData(str_send);
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0300 0303 0000 Set STAGEUNIT MOVEXYZTR 20000,20000,0,0,0 OK\r\n"
                if (return_data.Contains("OK"))
                {
                    mainForm.MY_DEBUG("Set Stage executed", name_space);
                    StageX = position[0];
                    StageY = position[1];
                    StageZ = position[2];
                    StageT = position[3];
                    StageR = position[4];
                    return 0;
                }
                else if (return_data.Contains("NG"))
                {
                    mainForm.MY_DEBUG("Set Stage not exectued: SEM is busy", name_space);
                    return 1;
                }
                else if (return_data.Contains("PARAMERROR"))
                {
                    mainForm.MY_DEBUG("Set Stage not exectued: Wrong parameter", name_space);
                }
                else
                    mainForm.MY_DEBUG("Set Stage not exectued: invalid return value", name_space);
                return 2;
            }
            else
            {
                mainForm.MY_DEBUG("Set Stage not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int StageX { get; private set; }

        public int StageY { get; private set; }

        public int StageZ { get; private set; }

        public int StageT { get; private set; }

        public int StageR { get; private set; }
        #endregion

        #region SEM Magnification functions
        public int UpdateMagnification()
        {
            if (IsConnected)
            {
                string return_data = SendData(str_mag_get); //"0303 0300 0000 Get MAGNIFICATION NOW\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Get MAGNIFICATION NOW 0,1500 IDLE\r\n"

                int start = return_data.LastIndexOf("NOW") + 4;
                int end = return_data.IndexOf(" IDLE");
                if (end == -1)
                {
                    end = return_data.LastIndexOf(" PROCESSING");
                }
                string sub_string = return_data.Substring(start, end - start);
                string[] split_str = sub_string.Split(',');
                Magnification = Convert.ToInt32(split_str[1]);
                return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Get Magnification not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int SetMagnification(int mag)
        {
            if (IsConnected)
            {
                string return_data = SendData(str_mag_set + mag.ToString() + "\r\n"); //"0300 0303 0000 Set MAGNIFICATION EXECUTE 1500\r\n
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Set MAGNIFICATION EXECUTE 1500 OK\r\n"

                if (return_data.Contains("OK"))
                {
                    mainForm.MY_DEBUG("Set Magnification executed", name_space);
                    Magnification = mag;
                    return 0;
                }
                else if (return_data.Contains("NG"))
                    mainForm.MY_DEBUG("Set Magnification not exectued: SEM is busy", name_space);
                else if (return_data.Contains("PARAMERROR"))
                    mainForm.MY_DEBUG("Set Magnification not exectued: Wrong parameter", name_space);
                else
                    mainForm.MY_DEBUG("Set Magnification not exectued: invalid return value", name_space);
                return 2;
            }
            else
            {
                mainForm.MY_DEBUG("Set Magnification not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int Magnification { get; private set; }
        #endregion

        #region SEM Focus functions
        public int UpdateFocus()
        {
            if (IsConnected)
            {
                string return_data = SendData(str_focus_get); //"0303 0300 0000 Get FOCUS NOW\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Get FOCUS NOW 100,100 IDLE\r\n"

                int start = return_data.LastIndexOf("NOW") + 4;
                int end = return_data.IndexOf(" IDLE");
                if (end == -1)
                {
                    end = return_data.LastIndexOf(" PROCESSING");
                }
                string sub_string = return_data.Substring(start, end - start);
                string[] split_str = sub_string.Split(',');
                Focus[0] = Convert.ToInt32(split_str[0]);  // get the Course value
                Focus[1] = Convert.ToInt32(split_str[1]);  // get the Fine value
                return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Get Focus not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int SetFocus(int[] focus)
        {
            if (focus.Length != 2)
            {
                mainForm.MY_DEBUG("Set Focus not exectued: invalid input length", name_space);
                return 2;
            }

            if (IsConnected)
            {
                string str_send = str_focus_set + focus[0].ToString() + "," + focus[1].ToString() + "\r\n";
                string return_data = SendData(str_send); //"0300 0303 0000 Set FOCUS ALL 1500,1500\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0300 0303 0000 Set FOCUS ALL 1500,1500 OK\r\n"

                if (return_data.Contains("OK"))
                {
                    mainForm.MY_DEBUG("Set Focus executed", name_space);
                    Focus = focus;
                    return 0;
                }
                else if (return_data.Contains("NG"))
                    mainForm.MY_DEBUG("Set Focus not exectued: SEM is busy", name_space);
                else if (return_data.Contains("PARAMERROR"))
                    mainForm.MY_DEBUG("Set Focus not exectued: Wrong parameter", name_space);
                else
                    mainForm.MY_DEBUG("Set Focus not exectued: invalid return value", name_space);
                return 2;
            }
            else
            {
                mainForm.MY_DEBUG("Set Focus not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int[] Focus { get; private set; }
        #endregion

        #region SEM WD functions
        // Current value of WD 4000～70000：4.0～70.00 mm
        public int UpdateWD()
        {
            if (IsConnected)
            {
                string return_data = SendData(str_wd_get); //"0303 0300 0000 Get WD NOW\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Get WD NOW 10000 IDLE\r\n"

                int start = return_data.LastIndexOf("NOW") + 4;
                int end = return_data.IndexOf(" IDLE");
                if (end == -1)
                {
                    end = return_data.LastIndexOf(" PROCESSING");
                }
                string sub_string = return_data.Substring(start, end - start);
                WD = Convert.ToInt32(sub_string);
                return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Get WD not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int SetWD(int wd)
        {
            if (IsConnected)
            {
                string return_data = SendData(str_wd_set + wd.ToString() + "\r\n"); //"0300 0303 0000 Set WD HM 1500\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0300 0303 0000 Set WD HM 1500 OK\r\n"

                if (return_data.Contains("OK"))
                {
                    mainForm.MY_DEBUG("Set WD executed", name_space);
                    WD = wd;
                    return 0;
                }
                else if (return_data.Contains("NG"))
                    mainForm.MY_DEBUG("Set WD not exectued: SEM is busy", name_space);
                else if (return_data.Contains("PARAMERROR"))
                    mainForm.MY_DEBUG("Set WD not exectued: Wrong parameter", name_space);
                else
                    mainForm.MY_DEBUG("Set WD not exectued: invalid return value", name_space);
                return 2;
            }
            else
            {
                mainForm.MY_DEBUG("Set WD not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int WD { get; private set; }
        #endregion

        #region SEM Acceleration Voltage functions
        // num = [Vacc, Vdec]
        // (Vacc:0～30000：0.0～30ｋV) Vdec=0
        public int UpdateVoltage()
        {
            if (IsConnected)
            {
                string return_data = SendData(str_acc_get); //"0303 0300 0000 Get HVCONTROL VACC\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Get HVCONTROL VACC 3000,0 IDLE\r\n"

                int start = return_data.LastIndexOf("VACC") + 5;
                int end = return_data.IndexOf(" IDLE");
                if (end == -1)
                {
                    end = return_data.LastIndexOf(" PROCESSING");
                }
                string sub_string = return_data.Substring(start, end - start);
                string[] split_str = sub_string.Split(',');
                Voltage[0] = Convert.ToInt32(split_str[0]);  // get the acceleration value
                Voltage[1] = Convert.ToInt32(split_str[1]);  // get the deceleration value
                return 0;
            }
            else
            {
                mainForm.MY_DEBUG("Get Voltage not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int SetVoltage(int voltage)
        {
            if (IsConnected)
            {
                string return_data = SendData(str_acc_set + voltage.ToString() + "\r\n"); //"0303 0300 0000 Set HVCONTROL VACC 3000\r\n"
                if (return_data == null)
                    return 2;
                else if (return_data == "SEM busy")
                    return 1;
                //Retrun example "0303 0300 0000 Set HVCONTROL VACC 3000 OK\r\n"

                if (return_data.Contains("OK"))
                {
                    mainForm.MY_DEBUG("Set Voltage executed", name_space);
                    Voltage[0] = voltage;
                    return 0;
                }
                else if (return_data.Contains("NG"))
                    mainForm.MY_DEBUG("Set Voltage not exectued: SEM is busy", name_space);
                else if (return_data.Contains("PARAMERROR"))
                    mainForm.MY_DEBUG("Set Voltage not exectued: Wrong parameter", name_space);
                else
                    mainForm.MY_DEBUG("Set Voltage not exectued: invalid return value", name_space);
                return 2;
            }
            else
            {
                mainForm.MY_DEBUG("Set Voltage not exectued: SEM is not connected", name_space);
            }
            return 2;
        }

        public int[] Voltage { get; private set; }
        #endregion

        #region Unused Functions
        public double Read_PixelSize(double adjust)
        {
            //  d=[
            //    300000	0.5291666
            //    16000	9.921875
            //    6500    24.42308
            //    1200	132.2917
            //    12 13229.17
            //]
            //a=d(:,1);
            //b=d(:,2);
            //mean(b.*a)=1.587500160000000e+05

            double mag = Magnification;
            const double rate = 1.587500160000000e+05;
            double nm_per_pixel = rate / mag;
            PixelSize_nmpp = nm_per_pixel * adjust;
            return PixelSize_nmpp;
        }

        public void Zooming(double factor)
        {
            double mag = Magnification * factor;
            Thread.Sleep(50);
            SetMagnification((int)mag);
        }
        #endregion
    }
}