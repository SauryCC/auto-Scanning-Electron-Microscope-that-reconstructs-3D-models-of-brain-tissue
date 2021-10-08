using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Drawing.Imaging;
//using CLRWrapper;
///using System.Windows.Media.Media3D;
using System.IO;
namespace MIcrotome_GUI
{
    //public partial class MainWindow : Form
    //{
        public class CThread
        {
            private void OutputDebugString(string inf)
            { 
                //#if __ENABLE_DEBUG_OUTPUT_STRING__
                System.Diagnostics.Debug.WriteLine(inf); 
                //#endif
            }
            //public delegate void CallbackHandler();

            public delegate void DelegateFunction();
            public DelegateFunction mDelegateFunction;

            Thread mThread = null;
            bool mBusy = false;
            string mName = "";
            public void SetName(string str) { mName = str; }
            public CThread()
            {
                mThread = new Thread(Thread_function);

            }
            public CThread(string str_name)
            {
                mThread = new Thread(Thread_function);
                SetName(str_name);
            }
            //use mCThread_Background.Start(delegate(){.......}); to call
            public void Start(DelegateFunction callback)
            {
                mDelegateFunction = new DelegateFunction(callback);
                mThread = new Thread(Thread_function);
                //mThread.IsBackground = true;
                mThread.Start();
            }
            private void Thread_function()
            {
                if (mBusy == false)
                {
                    if (mName != null)
                        OutputDebugString(mName + " Background thread start");
                    mDelegateFunction();
                    if (mName != null)
                        OutputDebugString(mName + " Background thread end");

                }
                else
                {
                    if (mName != null)
                        OutputDebugString(mName + " Background thread busy");
                }
                //mDelegateFunction = null;
            }

            public void Stop()
            {
                mThread.Abort();
            }
        }
    //}
}