using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;


namespace MIcrotome_GUI
{
    public partial class Main_control : Form
    {
        Thread mCThread_UI_Update;
        Thread mCThread_SEM_Navi_Update;
        bool SYS_TCPIP_Connected = true;
        public const uint mCaxis_z = 0;


        void BackgroundThread_Initialization()
        {
            
            if (SYS_TCPIP_Connected == true)
            {
                if (mCSEM_TCPIP.IsConnected == false)
                    mCSEM_TCPIP.Connect();
                //mCThread_UI_Update = new CThread("UI Update");
                //mCThread_UI_Update.mDelegateFunction = new CThread.DelegateFunction(ThreadFunction_UI_Update);
                //mCThread_UI_Update.Start(ThreadFunction_UI_Update);
            }
            //update Smaract encoder value
            mCThread_UI_Update = new Thread(Thread_UI_Update_Smaract);
            mCThread_UI_Update.Start();

            //update SEM Navigation map position
            mCThread_SEM_Navi_Update = new Thread(Thread_Update_SEM_Position);
            mCThread_SEM_Navi_Update.Start();
        }

        double mCoarsePositionX = 0;
        double mCoarsePositionY = 0;
        

        uint mCoarseSpeedZ = 0;
        void Thread_UI_Update_Smaract() // update the SmarAct Coarse encoder values
        {
            double mCoarsePositionZ = 0;
            while (true)
            {
                Thread.Sleep(100);
                try
                {
                    if (mStageControl.CPConnected)
                    {
                        mCoarsePositionZ = mStageControl.Get_Zaxis();
                        if (label_zEncoder.InvokeRequired)
                        {
                            Action<string> actionDelegate = delegate (string txt) { label_zEncoder.Text = txt; };
                            label_zEncoder.BeginInvoke(actionDelegate, mCoarsePositionZ.ToString("f3"));
                        }
                        else
                            label_zEncoder.Text = mCoarsePositionZ.ToString("f3");
                    }
                    else
                    {
                        if (label_zEncoder.InvokeRequired)
                        {
                            Action<string> actionDelegate = delegate (string txt) { label_zEncoder.Text = txt; };
                            label_zEncoder.BeginInvoke(actionDelegate, "--");
                        }
                        else
                            label_zEncoder.Text = "--";
                    }
                }
                catch
                {
                    MY_DEBUG("error caught: Thread_UI_Update_Smaract", "Form Background Thread");
                    return;
                }
                Thread.Sleep(1000 * Convert.ToInt32(mCCoarsePositioner.IsBusy));// wait more if busy
            }
        }

        
        void Thread_Update_SEM_Position() // update parameters of SEM
        {
            while (true)
            {
                try
                {
                    if (mCSEM_TCPIP.IsConnected && (minimap_background_with_ROI != null))
                    {
                        Navi_lock.WaitOne();
                        int iteration = 0;
                        while (mStageControl.UpdateStageValue() != 0 && iteration < 10) iteration++;  // keep trying until successful retrieve value
                        Bitmap inputImage = new Bitmap(minimap_background_with_ROI);
                        using (Graphics gr = Graphics.FromImage(inputImage))
                        {
                            // update the current SEM position
                            Pen pen = new Pen(new SolidBrush(Color.Blue));
                            int[] SEM_pos = NaviMange.get_ROI_position(mStageControl.Get_Xaxis() / 1000.0, mStageControl.Get_Yaxis() / 1000.0, mStageControl.Get_SEM_wdith(), mStageControl.Get_SEM_height());
                            gr.DrawRectangle(pen, SEM_pos[0], SEM_pos[1], SEM_pos[2], SEM_pos[3]);
                        }
                        // update the navigation picture box
                        pictureBox_Navi.Image = new Bitmap(inputImage, pictureBox_Navi.Width, pictureBox_Navi.Height);
                        Navi_lock.ReleaseMutex();
                    }

                    // update minimap every 3 second
                    Thread.Sleep(3000);
                }
                catch
                {
                    MY_DEBUG("error caught: Thread_Update_SEM_Position", "Form Background Thread");
                    return;
                }
            }
        }
    }
}
