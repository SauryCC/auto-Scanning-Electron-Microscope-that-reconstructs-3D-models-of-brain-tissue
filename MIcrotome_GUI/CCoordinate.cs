using System;
using System.Collections.Generic;

//using CLRWrapper;
///using System.Windows.Media.Media3D;
namespace MIcrotome_GUI
{
    public class CCoordinate
    {
        public List<Coordinate> ROIs;
        List<Coordinate> Grids;
         
        public int num_roi;
        public List<uint> available_roi_ids;
        public int num_grid;

        public CCoordinate()
        {
            ROIs = new List<Coordinate>();
            available_roi_ids = new List<uint>();
            available_roi_ids.Add(0);
            num_roi = 0;

            Grids = new List<Coordinate>();
            num_grid = 0;
        }

        #region ROI mangement
        public int new_ROI(double roi_x, double roi_y)
        {
            ROIs.Add(new Coordinate(available_roi_ids[0], roi_x, roi_y));
            available_roi_ids.RemoveAt(0);
            num_roi += 1;
            if (available_roi_ids.Count == 0)
                available_roi_ids.Add((uint)num_roi);
            return ROIs.Count - 1;
        }

        public int new_ROI(double roi_x, double roi_y, int roi_mag)
        {
            ROIs.Add(new Coordinate(available_roi_ids[0], roi_x, roi_y, roi_mag));
            available_roi_ids.RemoveAt(0);
            num_roi += 1;
            if (available_roi_ids.Count == 0)
                available_roi_ids.Add((uint)num_roi);
            return ROIs.Count - 1;
        }

        public int new_ROI(double roi_x, double roi_y, int roi_mag, int roi_focus)
        {
            ROIs.Add(new Coordinate(available_roi_ids[0], roi_x, roi_y, roi_mag, roi_focus));
            available_roi_ids.RemoveAt(0);
            num_roi += 1;
            if (available_roi_ids.Count == 0)
                available_roi_ids.Add((uint)num_roi);
            return ROIs.Count - 1;
        }

        public bool modify_ROI_pos(uint roi_id, double roi_x, double roi_y)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                var temp = ROIs.Find(x => x.id == roi_id);
                temp.x = roi_x;
                temp.y = roi_y;
                return true;
            }
            else
                return false;

        }

        public (double, double) get_ROI_pos(uint roi_id)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                var temp = ROIs.Find(x => x.id == roi_id);
                return (temp.x, temp.y);
            }
            else
                return (-1, -1);
        }

        public bool modify_ROI_mag(uint roi_id, int roi_mag)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                var temp = ROIs.Find(x => x.id == roi_id);
                temp.magification = roi_mag;
                temp.width = CNaviManage.SEM_initial_length / (double)roi_mag;
                temp.height = CNaviManage.SEM_initial_length / (double)CNaviManage.SEM_x_pixel * CNaviManage.SEM_y_pixel / roi_mag;
                return true;
            }
            else
                return false;

        }

        public int get_ROI_mag(uint roi_id)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                var temp = ROIs.Find(x => x.id == roi_id);
                return temp.magification;
            }
            else
                return 0;
        }

        public bool modify_ROI_foc(uint roi_id, int roi_foc)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                var temp = ROIs.Find(x => x.id == roi_id);
                temp.focus = roi_foc;
                return true;
            }
            else
                return false;

        }

        public int get_ROI_foc(uint roi_id)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                var temp = ROIs.Find(x => x.id == roi_id);
                return temp.focus;
            }
            else
                return -1;
        }

        public bool remove_ROI(uint roi_id)
        {
            if (ROIs.Exists(x => x.id == roi_id))
            {
                ROIs.RemoveAll(x => x.id == roi_id);
                available_roi_ids.Add(roi_id);
                num_roi -= 1;
                return true;
            }
            else
                return false;
        }
        #endregion

        #region Grids position mangement
        public bool set_grid_pos(uint grid_id, int grid_x, int grid_y)
        {
            if (!Grids.Exists(x => x.id == grid_id))
            {
                Grids.Add(new Coordinate(grid_id, grid_x, grid_y));
                num_grid += 1;
                return true;
            }
            else
            {
                var temp = Grids.Find(x => x.id == grid_id);
                temp.x = grid_x;
                temp.y = grid_y;
                return true;
            }
        }

        public bool modify_grid_pos(uint grid_id, int grid_x, int grid_y)
        {
            if (Grids.Exists(x => x.id == grid_id))
            {
                var temp = Grids.Find(x => x.id == grid_id);
                temp.x = grid_x;
                temp.y = grid_y;
                return true;
            }
            else
                return false;

        }

        public bool remove_grid(uint grid_id)
        {
            if (Grids.Exists(x => x.id == grid_id))
            {
                Grids.RemoveAll(x => x.id == grid_id);
                num_grid -= 1;
                return true;
            }
            else
                return false;
        }

        public (double, double) get_grid_pos(uint grid_id)
        {
            if (Grids.Exists(x => x.id == grid_id))
            {
                var temp = Grids.Find(x => x.id == grid_id);
                return (temp.x, temp.y);
            }
            else
                return (-1, -1);
        }
        #endregion
    }
}