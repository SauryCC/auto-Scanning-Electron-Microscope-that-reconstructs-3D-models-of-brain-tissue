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
using System.Net;
using System.Net.Sockets;

//using CLRWrapper;
///using System.Windows.Media.Media3D;
using System.IO;
namespace MIcrotome_GUI
{
    public class COVManage
    {
        struct OV_Data
        {
            public uint ID;

            //default in SEM stage coordinate
            public int x;
            public int y;

            //public int Mag;
            //public double npp;
            //public int dwellTime;
            //public double WD;

            public OV_Data(uint ID, int x, int y)
            {
                this.ID = ID;
                this.x = x;
                this.y = y;
            }
        }

        List<OV_Data> OVs;

        public uint num_ov;


        public COVManage()
        {
            OVs = new List<OV_Data>();
            num_ov = 0;
        }

        public int add_OV(int ov_x, int ov_y)
        {
            uint ID = num_ov;
            if (!OVs.Exists(x => x.ID == ID))
            {
                OVs.Add(new OV_Data(ID, ov_x, ov_y));
                num_ov += 1;
                return (int)ID;
            }
            else
                return -1;
        }
        /*
        #region Navigation position mangement
        public bool modify_navi_s(int navi_x, int navi_y)
        {
            Navigation.x = navi_x;
            Navigation.y = navi_y;
            return true;
        }

        public (Int32, Int32) get_navi_s()
        {
            return (Navigation.x, Navigation.y);
        }
        #endregion

        #region Overviews position mangement
        public bool add_OV_s(uint ov_id, int ov_x, int ov_y)
        {
            if (!OVs.Exists(x => x.ID == ov_id))
            {
                OVs.Add(new Coordinate(ov_id, ov_x, ov_y));
                num_ov += 1;
                return true;
            }
            else
                return false;
        }

        public bool modify_OV_s(uint ov_id, int ov_x, int ov_y)
        {
            if (OVs.Exists(x => x.ID == ov_id))
            {
                var temp = OVs.Find(x => x.ID == ov_id);
                temp.x = ov_x;
                temp.y = ov_y;
                return true;
            }
            else
                return false;
            
        }

        public bool remove_OV_s(uint ov_id)
        {
            if (OVs.Exists(x => x.ID == ov_id))
            {
                OVs.RemoveAll(x => x.ID == ov_id);
                num_ov -= 1;
                return true;
            }
            else
                return false;
        }

        public (Int32, Int32) get_OV_s(uint ov_id)
        {
            if (OVs.Exists(x => x.ID == ov_id))
            {
                var temp = OVs.Find(x => x.ID == ov_id);
                return (temp.x, temp.y);
            }
            else
                return (-1, -1);
        }
        #endregion

        #region Grids position mangement
        public bool add_grid_s(uint grid_id, int grid_x, int grid_y)
        {
            if (!Grids.Exists(x => x.ID == grid_id))
            {
                Grids.Add(new Coordinate(grid_id, grid_x, grid_y));
                num_grid += 1;
                return true;
            }
            else
                return false;
        }

        public bool modify_grid_s(uint grid_id, int grid_x, int grid_y)
        {
            if (Grids.Exists(x => x.ID == grid_id))
            {
                var temp = Grids.Find(x => x.ID == grid_id);
                temp.x = grid_x;
                temp.y = grid_y;
                return true;
            }
            else
                return false;

        }

        public bool remove_grid_s(uint grid_id)
        {
            if (Grids.Exists(x => x.ID == grid_id))
            {
                Grids.RemoveAll(x => x.ID == grid_id);
                num_grid -= 1;
                return true;
            }
            else
                return false;
        }

        public (Int32, Int32) get_grid_s(uint grid_id)
        {
            if (Grids.Exists(x => x.ID == grid_id))
            {
                var temp = Grids.Find(x => x.ID == grid_id);
                return (temp.x, temp.y);
            }
            else
                return (-1, -1);
        }
        #endregion
        */
    }
}