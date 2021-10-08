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
        //Status parameters
        public bool SmarAct_connected = false;
        public bool mMainFormRunning = false;
        public bool CoarsePositionSensorConnected = true;

        public int ZP_DIR = 1;
        public int ZN_DIR = -1;

        //Constant values
        public double SEM_X1_FOV = 1270; //X1_FOV_width 1270um in 800 pixels
        public const uint SEM_stageX_max = 100000000; //in nm, step size = 1000nm, 100mm
        public const uint SEM_stageX_min = 0;
        public const uint SEM_stageX_home = 60000000; // in nm, home position = 60mm
        public const uint SEM_stageY_max = 50000000; //in nm, step size = 1000nm, 50mm
        public const uint SEM_stageY_min = 0;
        public const uint SEM_stageY_home = 25000000; // in nm, home position = 25mm
        public const uint SEM_course_max = 16383; //in nm, step size = 1000nm, 100mm
        public const uint SEM_course_min = 0;
        public const uint SEM_fine_max = 4095; //in nm, step size = 1000nm, 50mm
        public const uint SEM_fine_min = 0;
    }
}
