using System;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace MIcrotome_GUI
{
    public class Knife_Controller
    {
        Main_control mainForm;

        //parameters
        private const string name_space = "Knife Control";

        //Knife communication
        private SerialPort knife_control_port;
        Thread mThreadKnifeConnect;

        //Limits
        const double voltage_max = 250;
        const double voltage_min = 0;
        const double voltage_rise_max = 500;
        const double voltage_rise_min = 0.1;
        const double voltage_fall_max = 500;
        const double voltage_fall_min = 0.1;
        const double current_max = 4.5;
        const double current_min = 0;
        const double current_rise_max = 9;
        const double current_rise_min = 0.001;
        const double current_fall_max = 9;
        const double current_fall_min = 0.001;

        //Abort command
        const string cmd_abort = "ABORt";

        //Trigger Command
        const string cmd_output = "OUTPut ON";

        //Apply command
        const string cmd_vol_cur_get = "APPL?";
        const string cmd_vol_cur_set = "APPL VOLTAGE CURRENT";

        //Voltage command
        const string cmd_vol_get = "SOUR:VOLT:LEV:IMM:AMPL?";
        const string cmd_vol_set = "SOUR:VOLT:LEV:IMM:AMPL VOLTAGE";
        const string cmd_vol_rise_slewrate_get = "SOUR:VOLT:SLEW:RIS?";
        const string cmd_vol_rise_slewrate_set = "SOUR:VOLT:SLEW:RIS SLEWRATE";
        const string cmd_vol_fall_slewrate_get = "SOUR:VOLT:SLEW:FALL?";
        const string cmd_vol_fall_slewrate_set = "SOUR:VOLT:SLEW:FALL SLEWRATE";

        //Current command
        const string cmd_cur_get = "SOUR:CURR:LEV:IMM:AMPL?";
        const string cmd_cur_set = "SOUR:CURR:LEV:IMM:AMPL VOLTAGE";
        const string cmd_cur_rise_slewrate_get = "SOUR:CURR:SLEW:RIS?";
        const string cmd_cur_rise_slewrate_set = "SOUR:CURR:SLEW:RIS SLEWRATE";
        const string cmd_cur_fall_slewrate_get = "SOUR:CURR:SLEW:FALL?";
        const string cmd_cur_fall_slewrate_set = "SOUR:CURR:SLEW:FALL SLEWRATE";

        public Knife_Controller(Main_control mainForm)
        {
            this.mainForm = mainForm;
            knife_control_port = new SerialPort();

            IsConnected = false;
            IsBusy = false;
        }

        ~Knife_Controller() 
        {
            mThreadKnifeConnect.Abort();
        }

        public bool Connect(string port_name, string baud_rate, string data_bits, string stop_bits, string parity_bits)
        {
            //Initial parameters
            if (!IsConnected)
            {
                try
                {
                    knife_control_port.PortName = port_name;
                    knife_control_port.BaudRate = Convert.ToInt32(baud_rate);
                    knife_control_port.DataBits = Convert.ToInt32(data_bits);
                    knife_control_port.StopBits = (StopBits)Enum.Parse(typeof(StopBits), stop_bits);
                    knife_control_port.Parity = (Parity)Enum.Parse(typeof(Parity), parity_bits);

                    knife_control_port.Open();
                    IsConnected = true;
                }
                catch (Exception err)
                {
                    mainForm.MY_DEBUG(err.Message, name_space);
                }
            }
            return IsConnected;
        }

        public bool Disconnect()
        {
            if (IsConnected)
            {
                try
                {
                    knife_control_port.Close();
                    IsConnected = false;
                    mainForm.MY_DEBUG("Knife Controller disconnected", name_space);
                }
                catch
                {
                    mainForm.MY_DEBUG("Knife Controller disconnect failed", name_space);
                }
            }
            return IsConnected;
        }

        private string queryData(string data_string)
        {
            string data_string_back = null;
            try
            {
                if (IsConnected)
                {
                    if (!IsBusy)
                    {
                        IsBusy = true;
                        knife_control_port.WriteLine(data_string);
                        mainForm.MY_DEBUG("Query data " + data_string + " to knife controller.", name_space);
                        data_string_back = knife_control_port.ReadLine();
                        mainForm.MY_DEBUG("Knife controller returned " + data_string_back, name_space);
                        IsBusy = false;
                    }
                    else
                    {
                        data_string_back = "Knife Controller busy";
                    }
                }
            }
            catch
            {
                IsConnected = false;
                mainForm.MY_DEBUG("Query data " + data_string + " to knife controller failed.", name_space);
                knife_control_port.Close();
            }
            return data_string_back;
        }

        private void setData(string data_string)
        {
            try
            {
                if (IsConnected)
                {
                    if (!IsBusy)
                    {
                        IsBusy = true;
                        knife_control_port.WriteLine(data_string);
                        mainForm.MY_DEBUG("Set data " + data_string + " to knife controller.", name_space);
                        IsBusy = false;
                    }
                    else
                    {
                        mainForm.MY_DEBUG("Set data " + data_string + " to knife controller failed.", name_space);
                    }
                }
            }
            catch
            {
                IsConnected = false;
                knife_control_port.Close();
            }
        }

        public void OutPut()
        {
            setData(cmd_output);
        }

        #region Abort Function
        public void Abort()
        {
            setData(cmd_abort);
        }
        #endregion

        #region Voltage And Current Functions
        public (double, double) Get_Vol_And_Cur()
        {
            string ret_data = queryData(cmd_vol_cur_get);
            string[] values = ret_data.Split(',');
            (Voltage, Current) = (Convert.ToDouble(values[0]), Convert.ToDouble(values[1]));
            return (Voltage, Current);
        }

        public void Set_Vol_And_Cur(double vol, double cur)
        {
            Voltage = vol;
            Current = cur;

            string data_str = cmd_vol_cur_set;
            data_str = data_str.Replace("VOLTAGE", Voltage.ToString());
            data_str = data_str.Replace("CURRENT", Voltage.ToString());
            setData(data_str);
        }
        #endregion

        #region Voltage Functions
        public double Get_Vol()
        {
            Voltage = Convert.ToDouble(queryData(cmd_vol_get));
            return Voltage;
        }

        public void Set_Vol(double vol)
        {
            Voltage = vol;
            string data_str = cmd_vol_set;
            data_str = data_str.Replace("VOLTAGE", Voltage.ToString());
            setData(data_str);
        }

        public double Get_Vol_Rise_SlewRate()
        {
            Voltage_Rise_SlewRate = Convert.ToDouble(queryData(cmd_vol_rise_slewrate_get));
            return Voltage_Rise_SlewRate;
        }

        public void Set_Vol_Rise_SlewRate(double slewrate)
        {
            Voltage_Rise_SlewRate = slewrate;
            string data_str = cmd_vol_rise_slewrate_set;
            data_str = data_str.Replace("SLEWRATE", Voltage_Rise_SlewRate.ToString());
            setData(data_str);
        }

        public double Get_Vol_Fall_SlewRate()
        {
            Voltage_Fall_SlewRate = Convert.ToDouble(queryData(cmd_vol_fall_slewrate_get));
            return Voltage_Fall_SlewRate;
        }

        public void Set_Vol_Fall_SlewRate(double slewrate)
        {
            Voltage_Fall_SlewRate = slewrate;
            string data_str = cmd_vol_fall_slewrate_set;
            data_str = data_str.Replace("SLEWRATE", Voltage_Fall_SlewRate.ToString());
            setData(data_str);
        }
        #endregion

        #region Current Functions
        public double Get_Cur()
        {
            Current = Convert.ToDouble(queryData(cmd_cur_get));
            return Current;
        }

        public void Set_Cur(double cur)
        {
            Current = cur;
            string data_str = cmd_cur_set;
            data_str = data_str.Replace("CURRENT", Current.ToString());
            setData(data_str);
        }

        public double Get_Cur_Rise_SlewRate()
        {
            Current_Rise_SlewRate = Convert.ToDouble(queryData(cmd_cur_rise_slewrate_get));
            return Current_Rise_SlewRate;
        }

        public void Set_Cur_Rise_SlewRate(double slewrate)
        {
            Current_Rise_SlewRate = slewrate;
            string data_str = cmd_cur_rise_slewrate_set;
            data_str = data_str.Replace("SLEWRATE", Current_Rise_SlewRate.ToString());
            setData(data_str);
        }

        public double Get_Cur_Fall_SlewRate()
        {
            Current_Fall_SlewRate = Convert.ToDouble(queryData(cmd_cur_fall_slewrate_get));
            return Current_Fall_SlewRate;
        }

        public void Set_Cur_Fall_SlewRate(double slewrate)
        {
            Current_Fall_SlewRate = slewrate;
            string data_str = cmd_cur_fall_slewrate_set;
            data_str = data_str.Replace("SLEWRATE", Current_Fall_SlewRate.ToString());
            setData(data_str);
        }
        #endregion

        #region Property Functions
        public bool IsConnected { get; private set; }

        public bool IsBusy { get; private set; }

        public double Voltage { get; set; }

        public double Voltage_Rise_SlewRate { get; set; }

        public double Voltage_Fall_SlewRate { get; set; }

        public double Current { get; set; }

        public double Current_Rise_SlewRate { get; set; }

        public double Current_Fall_SlewRate { get; set; }
        #endregion

    }
}
