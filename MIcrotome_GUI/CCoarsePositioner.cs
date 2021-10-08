using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MIcrotome_GUI
{

    public class CCoarsePositioner
    {
        //system control
        uint mSystemIndex = 0;
        private int mIsIdle = 0;// for delay checkstatus
        private bool bCP_Busy;
        private bool _IsConnected_;
        bool moving;

        CThread mThreadCPConnect;

        //sensor control
        private bool mSensorConnected;
        uint mSensorType; //Apendix 5.4
        uint mSensorMode;
        //bool mSensorAutoOff;

        //Channel control
        const int mNumberOfChannel = 1;

        public double mMax_StepSize_nm = 1783;//1500.0;// motion distance in nm
        public double mMax_DACValue_Bit = 4095;// 2^12-1, 12 bit DAC

       public CCoarsePositioner(bool sensor_connected,double Coarse_Max_StepSize_nm, double Coarse_Max_DACValue_Bit)
        {
            // if changed smaract actuator, this value mMax_StepSize_nm,  mMax_DACValue_Bit must be changed. 
            //mMax_StepSize_nm nm is the step length of smaract coarse positioner, this value must be renewed if new coarse positioner is designed.
            //4095 is the max DAC output of SmarAct controller.
            bCP_Busy = false;
            _IsConnected_ = false;
            //mSensorAutoOff = true;

            mMax_StepSize_nm = Coarse_Max_StepSize_nm;
            mMax_DACValue_Bit = Coarse_Max_DACValue_Bit;

            mSensorConnected = sensor_connected;


            mDirection = new double[6] { -1, -1, -1, -1, -1, -1 };
            mFrequency_ClosedLoop = new uint[6] { 0, 0, 0, 0, 0, 0 };
            mPositionStore = new int[6] { 0, 0, 0, 0, 0, 0 };
            mSpeedStore = new uint[6] { 0, 0, 0, 0, 0, 0 };
            // -1 negative, 
            //+1 positive, move away from cable
            //mDirection[X_CP_AXIS] = -1;
            //mDirection[Y_CP_AXIS] = -1;
            //mDirection[Z_CP_AXIS] = -1;
            moving = false;
            mThreadCPConnect = new CThread();
            //mThreadBackground_CP.SetName("coarse positioner:");
            mThreadCPConnect.SetName(null);
        }

        ~CCoarsePositioner() { Disconnect(); }

        public uint Initialize()
        {
            _IsConnected_ = false;
            try
            {
                if (bCP_Busy == false)
                {
                    bCP_Busy = true;
                    CSmarAct.SA_ReleaseSystems();
                    Thread.Sleep(100);
                    mResult = CSmarAct.SA_InitSystems(CSmarAct.SA_SYNCHRONOUS_COMMUNICATION);	 //+ CSmarAct.SA_HARDWARE_RESET init systems
                    if (mResult != CSmarAct.SA_OK)
                    {
                        MY_DEBUG("Coarse positioner Initialization error");
                        return mResult;
                    }
                    _IsConnected_ = true;
                    MY_DEBUG("Coarse positioner SA_InitSystems SA_HARDWARE_RESET:" + mResult.ToString());
                    Thread.Sleep(100);

                    mResult = CSmarAct.SA_GetSystemID((uint)mSystemIndex, ref mID);
                    bCP_Busy = false;
                    MY_DEBUG("SA_GetSystemID:" + mID.ToString());
                    Thread.Sleep(5);
                }
                else
                {
                    MY_DEBUG("Nanopositioner isn't initialized: busy");
                    return CSmarAct.SA_INITIALIZATION_ERROR;
                }       
            }
            catch (Exception ex)
            {
                MY_DEBUG("Coarse positioner Initialization error!");
                return CSmarAct.SA_INITIALIZATION_ERROR;
            }
            if (bCP_Busy == false)
            {
                //bCP_Busy = true;
                default_set();
                //bCP_Busy = false;
            }        
            MY_DEBUG("Coarse positioner is initialized");
            //_IsConnected_ = true;
            return CSmarAct.SA_OK;
        }

        void default_set()
        {
            bCP_Busy = true;
            CSmarAct.SA_SetAccumulateRelativePositions_S((uint)mSystemIndex, Z_CP_AXIS, CSmarAct.SA_NO_ACCUMULATE_RELATIVE_POSITIONS);
            bCP_Busy = false;
            //Stop(X_CP_AXIS);
            //Stop(Y_CP_AXIS);
            //Stop(Z_CP_AXIS);
            if (mSensorConnected == true)
            {
                //SetSpeedClosedLoop(X_CP_AXIS, 0);// default speed
                //SetSpeedClosedLoop(Y_CP_AXIS, 0);
                //SetSpeedClosedLoop(Z_CP_AXIS, 0);
                int position = 0;
                mSensorType = 1; //1 is linear positioner with nano sensor
                bCP_Busy = true;
                for (uint channelIndex = X_CP_AXIS; channelIndex < X_CP_AXIS + 3; channelIndex++)
                {

                    CSmarAct.SA_SetSensorType_S(mSystemIndex, channelIndex, mSensorType);   // Set Sensor Type
                    //CSmarAct.SA_SetClosedLoopMaxFrequency_S(mSystemIndex, channelIndex, 100);   // Set Frequency
                    CSmarAct.SA_SetPosition_S(mSystemIndex, channelIndex, position);   // Set position to 0
                }
                bCP_Busy = false;
                SetSensorModeEnable();
            }
            else
            {
                SetSensorModeDisable();
                SetSpeedOpenLoop(1000);
            }
            //SetSensorModeDisable();
            //SetSensorModeEnable();
            //SetSensorModePowerSave();

            //SetChannelVoltage(X_CP_AXIS, 0);
            //SetChannelVoltage(Y_CP_AXIS, 0);
            //SetChannelVoltage(Z_CP_AXIS, 0);
        }

        public void Disconnect()
        {
            //mResult = CSmarAct.SA_CloseSystem(mSystemIndex);//, loc1, "sync,open-timeout 1500");//"async,reset");
            
            bCP_Busy = true;
            mResult = CSmarAct.SA_ReleaseSystems();
            Thread.Sleep(5);
            bCP_Busy = false;

            if (mResult != CSmarAct.SA_OK)
            {
                MY_DEBUG("Nanopositioner SA_ReleaseSystems error!");
            }
            else
                _IsConnected_ = false;
        }

        /// <summary>
        /// ///////////////////////////////////////////////////
        /// </summary>
        double NM2STEPS(double x) { return ((x) / mMax_StepSize_nm * mMax_DACValue_Bit); }
        double STEPS2NM(double x) { return ((x) * mMax_StepSize_nm / mMax_DACValue_Bit); }

        public void MY_DEBUG(string inf)
        {
            if (string.IsNullOrEmpty(inf) == false)
                System.Diagnostics.Debug.WriteLine(inf);
        }
        ///////////////////////////////////////////////////  
        public bool IsBusy { get { return bCP_Busy; } }

        public bool IsConnected { get { return _IsConnected_; } }

        public bool IsMoving { get { return moving; } }

        //public void SetSensorAutoOff(bool auto_off) { mSensorAutoOff = auto_off; }
        
   
        const int NUM_OF_AXIS = (3);
        const int X_AXIS = (0);
        const int Y_AXIS = (1);
        const int Z_AXIS = (2);
        //const int T_AXIS = (3);
        const int AXIS_DELTA = 0;// (3);     //0
        const int X_CP_AXIS = (AXIS_DELTA + X_AXIS);
        const int Y_CP_AXIS = (AXIS_DELTA + Y_AXIS);
        const int Z_CP_AXIS = (AXIS_DELTA + Z_AXIS);

        // Math.Sign(x)  ((x > 0) ? 1 : ((x < 0) ? -1 : 0))      
        //uint mcsHandle1 = 0;
        uint mID = 0;
        
        uint mResult;
        //uint mStatus;
        uint mFrequency_OpenLoop;
        uint[] mFrequency_ClosedLoop;
        int[] mPositionStore;
        uint[] mSpeedStore;
        double[] mDirection;// +-1, control the direction of each axis

        #region Postion sensor value readout and sensor mode setting functions
        public void SetSensorMode(uint mode)
        {
            if (_IsConnected_ == true)
                SetSensorModeHardWare(mode);
        }

        void SetSensorModeHardWare(uint mode)
        {
            if (bCP_Busy == false)
            {
                MY_DEBUG("CP SetSensorModeHardWare: " + mode.ToString());
                bCP_Busy = true;
                mIsIdle++;
                mResult = CSmarAct.SA_SetSensorEnabled_S((uint)mSystemIndex, mode);
                bCP_Busy = false;
                if (mResult == CSmarAct.SA_OK)
                {
                    mSensorMode = mode;
                }
                else
                {
                    _IsConnected_ = false;
                    MY_DEBUG("SetSensorModeHardWare error, mode:" + mode.ToString());
                }
            }
            else
                MY_DEBUG("coarse busy: SetSensorModeHardWare");
        }

        // Check Coarse positioner connection first //
        public void SetSensorModeEnable()
        {
            if (mSensorConnected == true)
                SetSensorMode(CSmarAct.SA_SENSOR_ENABLED);
        }

        public void SetSensorModePowerSave()
        {
            if (mSensorConnected == true)
                SetSensorMode(CSmarAct.SA_SENSOR_POWERSAVE);
        }

        public void SetSensorModeDisable()
        {
            SetSensorMode(CSmarAct.SA_SENSOR_DISABLED);
        }

        // Direct set hardware sensor mode without connection check//
        public void SetSensorModeHardWareEnable()
        {
            if (mSensorConnected == true)
                SetSensorModeHardWare(CSmarAct.SA_SENSOR_ENABLED);
        }

        public void SetSensorModeHardWarePowerSave()
        {
            if (mSensorConnected == true)
                SetSensorModeHardWare(CSmarAct.SA_SENSOR_POWERSAVE);
        }

        public void SetSensorModeHardWareDisable()
        {
            SetSensorModeHardWare(CSmarAct.SA_SENSOR_DISABLED);
        }

        // Position Sensor readout
        public int GetStorePosition(uint channelIndex)
        {
            return mPositionStore[channelIndex];
        }

        public int GetPosition(uint channelIndex) //return status, -1 system is not connected, 0 invalid readout, 1 valid readout
        {
            if (_IsConnected_ == false) return -1;
            if (mSensorMode == CSmarAct.SA_SENSOR_DISABLED)
                return 0;

            int position = 0;

            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mResult = CSmarAct.SA_GetPosition_S((uint)mSystemIndex, (uint)channelIndex, ref position);
                Thread.Sleep(5); 
                bCP_Busy = false;

                if (mResult != CSmarAct.SA_OK)
                {
                    _IsConnected_ = false;
                    MY_DEBUG("GetPosition error:" + mResult.ToString());
                    mSensorMode = CSmarAct.SA_SENSOR_DISABLED;
                    //Initialize();
                }
                position *= (int)mDirection[(uint)channelIndex];
            }
            //else
            //MY_DEBUG("coarse busy: GetPosition");
            mPositionStore[channelIndex] = position;
            return 1;
        }
        #endregion

        #region Movement functions
        public void MoveDistance(uint channel, double distance)
        {
            //uint status=0;
            //GetStatus(channel, ref status);
            //if (status != CSmarAct.SA_STOPPED_STATUS)
            //{
            //    MY_DEBUG("CoarsePositioiner MoveDistance drop:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + frequency.ToString());
            //    return; 
            //}

            mThreadCPConnect.Start(
                delegate ()
                {
                    if (mSensorMode == CSmarAct.SA_SENSOR_DISABLED || mSensorConnected == false)
                        MoveDistance_OpenLoop(channel, distance);
                    else
                    {
                        //if (mSensorAutoOff == true)
                        //{
                        //    SetSensorModeHardWareEnable();
                        //    Thread.Sleep(300);
                        //}
                        MoveDistance_CloseLoop(channel, distance);
                        //if (mSensorAutoOff == true)
                        //    SetSensorModeHardWareDisable();
                    }
                });
        }

        public void MoveDistance_Absolute(uint channel, double distance)
        {
            mThreadCPConnect.Start(
                delegate ()
                {
                    if (mSensorMode == CSmarAct.SA_SENSOR_DISABLED || mSensorConnected == false)
                        MoveDistance_OpenLoop(channel, distance);
                    else
                    {
                        //if (mSensorAutoOff == true)
                        //{
                        //    SetSensorModeHardWareEnable();
                        //    Thread.Sleep(300);
                        //}
                        MoveDistance_CloseLoop_Absolute(channel, distance);
                        //if (mSensorAutoOff == true)
                        //    SetSensorModeHardWareDisable();
                    }
                });
        }
        #endregion

        #region Close-loop movement functions, relative and absolute
        public void MoveDistance_CloseLoop(uint channel, double distance)//uint channelIndex, int stepsize, int speed)// close loop
        {
            distance *= mDirection[(uint)channel];

            // added 20161121
            int k = 5;
            while (k-- > 0)
            {
                if (bCP_Busy == false)
                    break;
                Thread.Sleep(5);
            }
            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                CSmarAct.SA_GotoPositionRelative_S(mSystemIndex, channel, (int)distance, 1000);
                Thread.Sleep(5);
                bCP_Busy = false;

                MY_DEBUG("CoarsePositioiner MoveDistance_CloseLoop:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + mFrequency_ClosedLoop[channel].ToString());
            }
            else
                MY_DEBUG("coarse busy: MoveDistance_CloseLoop, abort");
        }

        public void MoveDistance_CloseLoop_Absolute(uint channel, double distance)//uint channelIndex, int stepsize, int speed)// close loop
        {
            distance *= mDirection[(uint)channel];
            int k = 5;
            while (k-- > 0)
            {
                if (bCP_Busy == false)
                    break;
                Thread.Sleep(5);
            }
            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                CSmarAct.SA_GotoPositionAbsolute_S(mSystemIndex, channel, (int)distance, 1000);
                Thread.Sleep(5);
                bCP_Busy = false;

                MY_DEBUG("CoarsePositioiner MoveDistance_CloseLoop_Absolute:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + mFrequency_ClosedLoop[channel].ToString());
            }
            else
                MY_DEBUG("coarse busy: MoveDistance_CloseLoop, abort");
        }
        #endregion

        public void SetSpeedOpenLoop(uint frequency)
        {
            if (frequency < 1)
                frequency = 1;
            if (frequency > 18500)
                frequency = 18500;

            mFrequency_OpenLoop = frequency;
        }

        public void MoveDistance_OpenLoop(uint channel, double distance)
        {
            MoveDistance_OpenLoop_F((uint)channel, distance, mFrequency_OpenLoop);
        }

        public void MoveDistance_OpenLoop_Rough_F(uint channel, double distance, uint frequency, bool wait_until_finish = true)
        {
            mFrequency_OpenLoop = frequency;

            distance *= mDirection[(uint)channel];
            if (moving == true)
            {
                MY_DEBUG("Coarse positioner busy, abort.");
                return;
            }

            moving = true;
            mIsIdle++;

            MY_DEBUG("MoveDistance_OpenLoop_Rough_F:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + frequency.ToString());

            distance = NM2STEPS(distance);
            int number_of_steps = 0, step_left = (int)distance;
            int dir = Math.Sign(distance);
            distance = Math.Abs(distance);

            double STEP_MAX = (NM2STEPS(1000));
            double STEP_MIN = (NM2STEPS(50));

            double STEP_Prefer = (NM2STEPS(700));

            if (distance <= STEP_MIN)
            {
                MoveFineSteps((uint)channel, 1 * dir, (uint)STEP_MIN, frequency);
            }

            if (distance < STEP_MAX && distance > STEP_MIN)
            {
                MoveFineSteps((uint)channel, 1 * dir, (uint)distance, frequency);
            }

            if (distance < STEP_MAX * 2 && distance > STEP_MAX)
            {
                MoveFineSteps((uint)channel, 2 * dir, (uint)(distance / 2), frequency);
            }

            if (distance > STEP_MAX * 2)
            {
                number_of_steps = (int)(distance / STEP_Prefer-1);// try to move less, no exceed


                MoveFineSteps((uint)channel, number_of_steps * dir, (uint)STEP_Prefer, frequency);
                if (wait_until_finish == true)
                {
                    WaitForIdle((uint)channel);
               
                    Thread.Sleep((int)(number_of_steps * 1000 / frequency + 500)); 
                }
            }
            moving = false;
        }


        /// <summary>
        /// for UI move coarse
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="distance"></param>
        public void MoveDistance_Steps_OpenLoop(uint channel, double distance,uint frequency)
        {
            //uint frequency=500;

            distance *= mDirection[(uint)channel];
            //if (moving == true)
            //{
            //    MY_DEBUG("MoveDistance_Steps_OpenLoop busy, abort.");
            //    return;
            //}

            //moving = true;
            mIsIdle++;

            MY_DEBUG("MoveDistance_Steps_OpenLoop:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + frequency.ToString());

            distance = NM2STEPS(distance);

            int dir = Math.Sign(distance);
            distance = Math.Abs(distance);

            double STEP_MAX = (NM2STEPS(mMax_StepSize_nm));
            double STEP_MIN = (NM2STEPS(50));

            double amplitude=((STEP_MAX+STEP_MIN)/2);

            double fsteps = distance / amplitude;
            int steps = (int)fsteps;
            if (fsteps - steps > 0.5)
                steps++;
            steps *= dir;


            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                mResult = CSmarAct.SA_StepMove_S((uint)mSystemIndex, (uint)channel, steps, (uint)amplitude, frequency);

                Thread.Sleep(5); bCP_Busy = false;
                if (mResult != CSmarAct.SA_OK)
                {
                    _IsConnected_ = false;
                    ///MainWindow.MY_DEBUG("MoveDistance_Steps_OpenLoop error!\n", new double[] { channel, steps, amplitude, frequency });

                }
            }
            else
                MY_DEBUG("coarse busy: MoveFineSteps");
        }

        /// <summary>
        /// for UI move coarse
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="steps"></param>
        /// <param name="amplitude"></param>
        /// <param name="frequency"></param>
        public void MoveCoarse_OpenLoop(uint channel, int steps, uint amplitude, uint frequency)
        {
            mResult = CSmarAct.SA_StepMove_S((uint)mSystemIndex, (uint)channel, steps, (uint)amplitude, frequency);

            if (mResult != CSmarAct.SA_OK)
            {
                _IsConnected_ = false;
                ///MainWindow.MY_DEBUG("MoveDistance_Steps_OpenLoop error!\n", new double[] { channel, steps, amplitude, frequency });
            }
        }

        public void MoveDistance_OpenLoop_F(uint channel, double distance, uint frequency, bool wait_until_finish=true)
        {
            mFrequency_OpenLoop = frequency;

            distance *= mDirection[(uint)channel];
            if (moving == true)
            {
                MY_DEBUG("Coarse positioner busy, abort.");
                return;
            }

            moving = true;
            mIsIdle++;

            MY_DEBUG("MoveDistance_OpenLoop_F:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + frequency.ToString());

            distance = NM2STEPS(distance);
            int number_of_steps = 0, step_left = (int)distance;
            int dir = Math.Sign(distance);
            distance = Math.Abs(distance);

            double STEP_MAX = (NM2STEPS(mMax_StepSize_nm)); 
            double STEP_MIN = (NM2STEPS(50));
            if (distance <= STEP_MIN)
            {
                MoveFineSteps((uint)channel, 1 * dir, (uint)STEP_MIN, frequency);
            }

            if (distance < STEP_MAX && distance > STEP_MIN)
            {
                MoveFineSteps((uint)channel, 1 * dir, (uint)distance, frequency);
            }

            if (distance < STEP_MAX * 2 && distance > STEP_MAX)
            {
                MoveFineSteps((uint)channel, 2 * dir, (uint)(distance / 2), frequency);
            }

            if (distance > STEP_MAX * 2)
            {
                number_of_steps = (int)(distance / STEP_MAX - 1);
                step_left = (int)(distance - STEP_MAX * (double)(number_of_steps));
                step_left /= 2;

                MoveFineSteps((uint)channel, number_of_steps * dir, (uint)STEP_MAX, frequency);
                if (wait_until_finish == true)
                {
                    WaitForIdle((uint)channel); 
                }
                
                Thread.Sleep((int)(number_of_steps * 1000 / frequency + 500));
               
                //20160209
                //WaitForIdle can not block the controller.
                // here sleep time must be long enough, otherwise the previous command will be ignored.
                MoveFineSteps((uint)channel, 2 * dir, (uint)step_left, frequency);
            }
            //SetChannelVoltage((uint)channel, 0);
            moving = false;
        }

        //void MoveToPosition(double x, double y, double z, double t)
        //{

        //}

        //void MoveToPosition(int channel, double position)
        //{

        //}

        //double GetPosition(int axis)
        //{
        //    return 0;
        //}

        


        public void Stop(uint axis)
        {
            //int k = 0;
            //for (  k = 0; k < 1; k++)
            //{
            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                CSmarAct.SA_Stop_S((uint)mSystemIndex, axis);
                Thread.Sleep(5);
                Thread.Sleep(5); bCP_Busy = false;
                //break;
            }
            else
                MY_DEBUG("coarse busy: Stop");

            //}      
            //if (k==10)
            //    MY_DEBUG("coarse busy: Stop missed");
        }

        //////////////////////////////////////

        void MoveToFinePosition(uint channel, uint position, uint speed)
        {
            if (position > 4095 || position < 0)
            {
                MY_DEBUG("MoveToFinePosition input exceeding error!\n");
            }
            if (position > 4095)
                position = 4095;
            if (position < 0)
                position = 0;

            mIsIdle++;
            CSmarAct.SA_ScanMoveAbsolute_S((uint)mSystemIndex, (uint)channel, position, speed);
        }

        void MoveFineDistance(int channel, int distance, uint speed)
        {
            //CSmarAct.SA_Stop_S((uint)mSystemIndex,(uint)channel);
            mIsIdle++;
            mResult = CSmarAct.SA_ScanMoveRelative_S((uint)mSystemIndex, (uint)channel, distance, speed);

            if (mResult != CSmarAct.SA_OK)
            {
                _IsConnected_ = false;
                MY_DEBUG("MoveFineDistance error!\n");
            }
        }

        public void MoveAtSpeed(uint channelIndex, double dir, double step_size, double frequency)
        {
            dir *= 30000;
            MoveFineSteps(channelIndex, (int)dir, (uint)Math.Abs(step_size), (uint)Math.Abs(frequency));
            MY_DEBUG("coarse MoveAtSpeed step_size: " + step_size.ToString() + ", frequency:" + frequency.ToString());
        }
        public void MoveFineSteps(uint channelIndex, int steps, uint amplitude, uint frequency)
        {
            // minimum step size=100, otherwise will mResult in error,
            // step=0-4095
            ///steps = MainWindow.LIMIT_MAX_MIN(steps, 30000, -30000);
            ///amplitude = MainWindow.LIMIT_MAX_MIN(amplitude, 4095, 2500);//500.0/1500.0*4095.0=1365
            ///frequency = MainWindow.LIMIT_MAX_MIN(frequency, 18500, 1);

            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                mResult = CSmarAct.SA_StepMove_S((uint)(uint)mSystemIndex, (uint)(uint)channelIndex, steps, amplitude, frequency);

                Thread.Sleep(5); bCP_Busy = false;
                if (mResult != CSmarAct.SA_OK)
                {
                    _IsConnected_ = false;
                    ///MainWindow.MY_DEBUG("MoveFineSteps error!\n", new double[] { channelIndex, steps, amplitude, frequency });

                }
            }
            else
                MY_DEBUG("coarse busy: MoveFineSteps");
        }

        /// <summary>
        /// amplitude: 2500->1064 nm, 4095->1783nm
        /// </summary>
        /// <param name="channelIndex"></param>
        /// <param name="steps"></param>
        /// <param name="amplitude"></param>
        //public void MoveOneStep(uint channel, double step_size_nm)
        //{

        //    step_size_nm *= mDirection[(uint)channel];
        //    if (moving == true)
        //    {
        //        MY_DEBUG("Coarse positioner busy, abort.");
        //        return;
        //    }

        //    moving = true;
        //    mIsIdle++;

        //    MY_DEBUG("MoveOneStep:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + step_size_nm.ToString() );
  
        //    int dir = Math.Sign(step_size_nm);
        //    step_size_nm = Math.Abs(step_size_nm);
 

        //    step_size_nm = MainWindow.LIMIT_MAX_MIN(step_size_nm, 1783.0, 1064.0);

        //    //amplitude is the DAC value of SmarAct controller
        //    uint amplitude = Convert.ToUInt32((step_size_nm - 1064.0) * (4095.0 - 2500.0) + 2500.0);
               
        //    //amplitude  2500->1064 nm, 4095->1783nm
        //    //here if amplitude< 2500, the final motion distance is still 1064 nm;

        //    uint frequency = 1;

  
        //        mResult = CSmarAct.SA_StepMove_S((uint)mSystemIndex, (uint)channel, dir, amplitude, frequency);

        //        Thread.Sleep(5);  
        //        if (mResult != CSmarAct.SA_OK)
        //        {
        //            mConnected = false;
        //            MY_DEBUG("MoveOneStep error!\n");

        //        }
 
        //}



        void MoveWait(uint channelIndex, int stepsize)
        {
            // close loop inside the controller, close loop for this function
            mIsIdle++;
            CSmarAct.SA_GotoPositionRelative_S((uint)mSystemIndex, (uint)channelIndex, stepsize, 1000);
            WaitForIdle(mSystemIndex);
        }



        public bool CheckConnection(uint channelIndex)
        {
            uint status = 0;
            return GetStatus(channelIndex, ref status);
        }

        public bool GetStatus(uint channelIndex, ref uint status)
        {
            if (mIsIdle > 0)
            {
                mIsIdle--;
                return true;
            }
            //MY_DEBUG("CP GetStatus check:"+mIsIdle.ToString());

            status = CSmarAct.SA_STOPPED_STATUS;
            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mResult = CSmarAct.SA_GetStatus_S((uint)mSystemIndex, (uint)channelIndex, ref status);		  // get mStatus
                Thread.Sleep(5); bCP_Busy = false;

                if (mResult != CSmarAct.SA_OK)
                {
                    _IsConnected_ = false;
                    MY_DEBUG("coarse GetStatus error:" + mResult.ToString() + "_" + status.ToString());
                }
            }
            else
                MY_DEBUG("coarse busy: GetStatus");
            return mResult == CSmarAct.SA_OK;
        }

        void WaitForIdle(uint channelIndex)
        {
            mResult = 0;
            int k = 100;
            do
            {
                //pin_ptr<uint *> pinnedPtr =  (uint * )&mStatus;
                uint pR = 0;
                GetStatus(channelIndex, ref pR);
                if (pR != CSmarAct.SA_STOPPED_STATUS)
                {
                    if (mSensorConnected == true)
                        MY_DEBUG("coarse positioner is busy: " + k.ToString());
                }

                Thread.Sleep(10);
                if (k-- < 0)
                    break;
            }
            while (mResult != CSmarAct.SA_TARGET_STATUS);	   // until target reach.	 // old was ==,20170228
        }

        // void  MoveDistance(uint channelIndex, int  stepsize)
        // {	
        // 	// close loop inside the controller, open loop for this function
        // 	SetSpeedCloseLoop((uint)channelIndex,0);
        // 	 mResult = CSmarAct.SA_GotoPositionRelative_S((uint)mSystemIndex, (uint)channelIndex, stepsize, 1000);	
        // 	if (mResult != CSmarAct.SA_OK) 
        // 	{
        // 		mConnected = false;
        // 		MY_DEBUG("MoveWait GetStatus error!\n");
        // 	}
        // }
        // 

        //  goon, use a Thread to move



        // for Z, distance>0, move up, but sensor position readout increase.

        //-----without setting frequency---------------------------------------------
        

        
        //-----without setting frequency---------------------------------------------


        //----- setting frequency---------------------------------------------
        public void MoveDistance_F(uint channel, double distance, uint frequency)
        {
            //uint status=0;
            //GetStatus(channel, ref status);
            //if (status != CSmarAct.SA_STOPPED_STATUS)
            //{
            //    MY_DEBUG("CoarsePositioiner MoveDistance drop:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + frequency.ToString());
            //    return; 
            //}

            mThreadCPConnect.Start(
                delegate()
                {
                    if (mSensorMode == CSmarAct.SA_SENSOR_DISABLED)
                        //MoveDistance_OpenLoop_F(channel, distance, frequency);
                        MoveDistance_Steps_OpenLoop(channel, distance,frequency);
                    else
                    {
                        //if (mSensorAutoOff == true)
                        //{
                        //    SetSensorModeHardWareEnable();
                        //    Thread.Sleep(300);
                        //}
                        MoveDistance_CloseLoop_F(channel, distance, frequency);
                        //if (mSensorAutoOff == true)
                        //    SetSensorModeHardWareDisable();
                    }
                });
        }
        public void MoveDistance_CloseLoop_F(uint channel, double distance, uint frequency)//uint channelIndex, int stepsize, int speed)// close loop
        {
            distance *= mDirection[(uint)channel];

            // added 20161121
            int k = 5;
            while (k-- > 0)
            {
                if (bCP_Busy == false)
                    break;
                Thread.Sleep(5);
            }
            SetSpeedClosedLoop(channel, frequency);
            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                CSmarAct.SA_GotoPositionRelative_S(mSystemIndex, channel, (int)distance, 1000);
                Thread.Sleep(5);
                bCP_Busy = false;

                MY_DEBUG("CoarsePositioiner MoveDistance_CloseLoop_F:\t" + ((char)(channel + (uint)'x')).ToString() + "\t" + distance.ToString() + "\t" + frequency.ToString());
            }
            else
                MY_DEBUG("coarse busy: MoveDistance_CloseLoop, abort");
        }


        //---set speed----------
        //void SetSpeedCloseLoop(uint channel, uint speed)
        //{
        //    //if (speed > 18500)
        //    //    speed = 18500;
        //    if (speed < 50)
        //        speed = 50;

        //    CSmarAct.SA_SetClosedLoopMoveSpeed_S(mSystemIndex, channel, speed);
        //    mResult = CSmarAct.SA_SetClosedLoopMaxFrequency_S(mSystemIndex, channel, speed * 10);

        //    if (mResult != CSmarAct.SA_OK)
        //    {
        //        mConnected = false;
        //        MY_DEBUG("Setfrequency error!\n");
        //    }
        //}


        public void SetSpeedClosedLoop(uint channel, uint speed)
        {
            //uint speed=(uint)Math.Abs(frequency * 1000 * 50);
            if (speed < 50)
                speed = 50;
            //MY_DEBUG(speed.ToString());

            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                CSmarAct.SA_SetClosedLoopMoveSpeed_S(mSystemIndex, channel, speed);
                mFrequency_ClosedLoop[channel] = speed;
                Thread.Sleep(10);
                //mResult = CSmarAct.SA_SetClosedLoopMaxFrequency_S(mSystemIndex, channel, speed * 10);
                Thread.Sleep(10);
                bCP_Busy = false;
            }
            else
                MY_DEBUG("coarse busy: SetSpeedClosedLoop, abort");
        }
       public void SetSpeedClosedLoop_All( uint frequency)
        {
            SetSpeedClosedLoop(X_CP_AXIS, frequency);
            SetSpeedClosedLoop(Y_CP_AXIS, frequency);
            SetSpeedClosedLoop(Z_CP_AXIS, frequency);
       }
        //------------------------------------------------

        public void SetPosition(uint channelIndex, int position)
        {

            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mIsIdle++;
                mResult = CSmarAct.SA_SetPosition_S((uint)mSystemIndex, (uint)channelIndex, position);
                Thread.Sleep(5); bCP_Busy = false;
                if (mResult != CSmarAct.SA_OK)
                {
                    _IsConnected_ = false;
                    MY_DEBUG("SetPosition error!\n");
                }
            }
            else
                MY_DEBUG("coarse busy: SetPosition");

        }

        public uint GetFinePosition(uint channelIndex)
        {
            uint level = 0;
            mResult = CSmarAct.SA_GetVoltageLevel_S((uint)mSystemIndex, (uint)channelIndex, ref level);
            if (mResult != CSmarAct.SA_OK)
            {
                _IsConnected_ = false;
                MY_DEBUG("GetFinePosition error!\n");
            }
            return level;
        }

        

        public uint GetStoreSpeed(uint channelIndex)
        {
            return mSpeedStore[channelIndex];
        }
        public uint GetSpeed(uint channelIndex)
        {
            if (_IsConnected_ == false) return 0;
            //if (mSensorMode[channelIndex] == CSmarAct.SA_SENSOR_DISABLED)
            //    return 0;

            uint speed = 0;

            if (bCP_Busy == false)
            {
                bCP_Busy = true;
                mResult = CSmarAct.SA_GetClosedLoopMoveSpeed_S((uint)mSystemIndex, (uint)channelIndex, ref speed);
                Thread.Sleep(6); bCP_Busy = false;

                if (mResult != CSmarAct.SA_OK)
                {
                    _IsConnected_ = false;
                    MY_DEBUG("GetSpeed error:" + mResult.ToString());
                    //Initialize();
                }
            }
            //else
            //MY_DEBUG("coarse busy: GetPosition");
            mSpeedStore[channelIndex] = speed;
            return speed;
        }
        // 
        // int  ResetFinePosition(void)
        // {
        // 	//CSmarAct.SA_StepMove_S(pM->(uint)mSystemIndex, Z_CP_AXIS,1,1,50);       // use this command to reset the positioner voltage to 2047
        // 	double temp_stepV = 100;
        // 	double temp_stepNM = temp_stepV*2*1535.0/4096;
        // 	int p=0,pnm1=0,pnm2=0;
        // 	int k=0;
        // 	for (k=0;k<10;k++)
        // 	{
        // 		pnm1= GetPosition(Z_CP_AXIS);
        // 		MoveWait(Z_CP_AXIS, temp_stepNM);
        // 		//MoveFineDistance(Z_CP_AXIS, -temp_stepV, Math.Abs(temp_stepV)*1000);
        // 		Thread.Sleep(20, 1);
        // 		MoveFineSteps(Z_CP_AXIS, -2, temp_stepV, temp_stepV*10); // here we should at least move 2 steps
        // 		Thread.Sleep(20, 1);
        // 		p= GetFinePosition(Z_CP_AXIS);
        // 		pnm2= GetPosition(Z_CP_AXIS);
        // 		if (abs(pnm1-pnm2)>20)
        // 		{
        // 			MoveWait(Z_CP_AXIS, pnm1-pnm2);
        // 			continue;
        // 		}
        // 
        // 		if (p==2047)
        // 			break;
        // 	} 
        // 	if (p!=2047)
        // 	{
        // 		sprintf(inf, "times: %d\t servo initial value: %d\n",k,p);
        // 		MY_DEBUG(inf);
        // 	}
        // 	return p;
        // }
        //
        int redo_count = 0;
        public void SetChannelVoltage(uint channelIndex, uint V_0_150)
        {
            // minimum step size=100, otherwise will mResult in error,
            // step=0-4095  SA_StepMove_S
            //mResult = CSmarAct.SA_GotoGripperOpeningRelative_S((uint)(uint)mSystemIndex, (uint)(uint)channelIndex, V_0_4095, 1);
            if (V_0_150 > 150) V_0_150 = 150;
            V_0_150 *= (uint)(4095.0 / 150.0);

            mIsIdle++;

            mResult = CSmarAct.SA_ScanMoveAbsolute_S((uint)mSystemIndex, (uint)(uint)channelIndex, V_0_150, 100000);

            if (mResult != CSmarAct.SA_OK)
            {
                _IsConnected_ = false;
                MY_DEBUG("SetChannelVoltage error!\n");
                //Initialize();

                //MY_DEBUG("redo initialize=" + redo_count.ToString());
                //if (redo_count++ < 2)
                //{
                //    SetChannelVoltage(channelIndex, V_0_150);
                //}
            }
            redo_count = 0;
        }
    }
}













//// this is for new controller
//string loc1 = @"usb:id:4065026041";
//uint bufferSize = 4096;
//int c = 0;
//while (c++ < 100)
//{
//    //CSmarAct.SA_GetSystemID(0, ref ID );

//mResult = CSmarAct.SA_FindSystems("", ref loc1, ref bufferSize);
//    mSystemIndex = 0;
//    mResult = CSmarAct.SA_CloseSystem(mSystemIndex);
//    Thread.Sleep(5);
//    MY_DEBUG("Nanopositioner SA_CloseSystem error:" + mResult.ToString() + "_" + c.ToString());
//mResult = CSmarAct.SA_OpenSystem(ref mSystemIndex, loc1, "sync,reset,open-timeout 3000");//reset,sync,"                      "async,reset");
//    Thread.Sleep(5);
//    if (mResult == CSmarAct.SA_OK)
//    {
//        MY_DEBUG("Nanopositioner SA_OpenSystem successfull:" + mResult.ToString() + "_" + c.ToString());
//        break;
//    }
//    MY_DEBUG("Nanopositioner SA_OpenSystem error:" + mResult.ToString() + "_" + c.ToString());
//}


// test  fine steps
//for (int k = 0; k < 10; k++)
//{ 
//    uint ch = 2;
//    double p1 = mCCoarsePositioner.GetPosition(ch);
//    Thread.Sleep(50);

//    This.mCCoarsePositioner.MoveDistance_OpenLoop_F(ch, -1000,1);
//    //This.mCCoarsePositioner.MoveFineSteps(ch, -2, step, f);

//    Thread.Sleep(2000);
//    double p2 = mCCoarsePositioner.GetPosition(ch);
//    MY_DEBUG2( p1.ToString() + "\t" + p2.ToString() + "\t" + (p2 - p1).ToString() + "\t");
//    Thread.Sleep(50);
//}



//uint []s=new uint[]{50,1000,2000,2500,3000,3500,4095};
//for (uint q = 0; q <s.Length; q++)

//    for (int k = 0; k < 10; k++)
//    {
//        uint f = 1;
//        uint ch = 2;
//        double p1 = mCCoarsePositioner.GetPosition(ch);
//        Thread.Sleep(50);
//        uint step = s[q];
//        This.mCCoarsePositioner.MoveDistance_OpenLoop_F(ch, 2000,1);
//        //This.mCCoarsePositioner.MoveFineSteps(ch, -2, step, f);

//        Thread.Sleep(500);
//        double p2 = mCCoarsePositioner.GetPosition(ch);
//        MY_DEBUG2(f.ToString() + "\t" + step.ToString() + "\t" + p1.ToString() + "\t" + p2.ToString() + "\t" + (p2 - p1).ToString() + "\t");
//        Thread.Sleep(50);
//    }

////
//for (uint q = 0; q < s.Length; q++)

//for (int k = 0; k < 10; k++)
//{
//    uint f = 1000;
//    uint ch = 2;
//    double p1 = mCCoarsePositioner.GetPosition(ch);
//    Thread.Sleep(50);
//    uint step = s[q];
//    This.mCCoarsePositioner.MoveFineSteps(ch, -1, step, f);
//    Thread.Sleep(500);
//    double p2 = mCCoarsePositioner.GetPosition(ch);
//    MY_DEBUG2(f.ToString() + "\t" + step.ToString() + "\t" + p1.ToString() + "\t" + p2.ToString() + "\t" + (p2 - p1).ToString() + "\t");
//    Thread.Sleep(50);
//}
