using System;
using System.Drawing;
using System.Threading;

//using CLRWrapper;
///using System.Windows.Media.Media3D;
namespace MIcrotome_GUI
{
    public class CNaviManage
    {
        int Navi_x;
        int Navi_y;
        double Navi_length;
        double Navi_width;
        int Navi_mag;
        double Navi_pixel_resolu;   //um per pixel
        int Navi_image_numb;    //row row=column
        int Navi_img_total;

        public const int SEM_x_pixel = 800;
        public const int SEM_y_pixel = 600;
        public const int SEM_initial_length = 127000;  // 127000um when X1 magnification

        public bool Navi_MAP_built = false;
        public bool Navi_abort = false;

        // in SEM coordinate, um
        int[] Navi_tile_x; 
        int[] Navi_tile_y;

        Bitmap[] Navi_tile_img;
        Bitmap Navi_img;

        CStage stage;
        Main_control mainForm;

        Thread mCThread_Navi_acq;

        public delegate void BitmapHandler(Bitmap Navi_out_img);
        public delegate void eventHandler(String status);
        public event BitmapHandler EventSendNaviImg;
        public event eventHandler EventThreadStatus;

        public CNaviManage(Main_control mainForm, CStage stage)
        {
            //SEM stage home position
            Navi_x = (int)(Main_control.SEM_stageX_home / 1000);
            Navi_y = (int)(Main_control.SEM_stageY_home / 1000);

            this.stage = stage;
            this.mainForm = mainForm;
        }

        public double[] Valid_Input(double length, int mag)
        {
            // default value: 193.04um, 6
            double[] return_array = new double[5]; //number of images, magnification, length, width, pixel resolution

            // from magnification calculate um per pixel
            Navi_mag = mag;
            Navi_pixel_resolu = (double)SEM_initial_length / mag / SEM_x_pixel;  // recalculate length in um per pixel using new magnification value

            // total number of pixel for the final result image 
            double length_pixel = Math.Ceiling(length / Navi_pixel_resolu);  

            Navi_image_numb = (int)Math.Ceiling(length_pixel / SEM_x_pixel);  // deafult value: 2

            Navi_length = Navi_image_numb * SEM_x_pixel * Navi_pixel_resolu;  // default value: 185.6153846
            Navi_width = Navi_image_numb * SEM_y_pixel * Navi_pixel_resolu;  // default value: 129.9307692

            return_array[0] = Navi_image_numb;
            return_array[1] = Navi_mag;
            return_array[2] = Navi_length;
            return_array[3] = Navi_width;
            return_array[4] = Navi_pixel_resolu;
            return return_array;
        }

        // return the mean value of a bitmap
        public double get_mean(Bitmap bmp)
        {
            // Used for tally
            double r = 0;
            double g = 0;
            double b = 0;

            int total = 0;

            for (int x = 0; x < bmp.Width; x++)
            {
                for (int y = 0; y < bmp.Height; y++)
                {
                    Color clr = bmp.GetPixel(x, y);
                    r += clr.R;
                    g += clr.G;
                    b += clr.B;

                    total++;
                }
            }
            // Calculate average
            r /= total;
            g /= total;
            b /= total;
            // In our case the bitmap is in gray scale so R = B = G
            return r;
        }

        // starts the nevigation map.
        // The input specifies the center coordinates of the map
        public void Start_Navi_Map(int center_x, int center_y)
        {
            // x y in um
            Navi_x = center_x;
            Navi_y = center_y;
            Navi_img_total = Navi_image_numb * Navi_image_numb;
            // stores the (x, y) that the SEM should move to
            Navi_tile_x = new int[Navi_img_total];
            Navi_tile_y = new int[Navi_img_total];

            // calculate the top left coordinate in um
            double top_left_x = center_x + Navi_image_numb / 2.0 * SEM_x_pixel * Navi_pixel_resolu;
            double top_left_y = center_y + Navi_image_numb / 2.0 * SEM_y_pixel * Navi_pixel_resolu;
            // calculate the coordinates that SEM should move to
            for (int row = 0; row < Navi_image_numb; row++)
            {
                // convert position from um to nm   
                int y_pos = (int)Math.Round((top_left_y - (SEM_y_pixel / 2 + row * SEM_y_pixel) * Navi_pixel_resolu) * 1000);
                for (int col = 0; col < Navi_image_numb; col++)
                {
                    // convert position from um to nm   
                    int x_pos = (int)Math.Round((top_left_x - (SEM_x_pixel / 2 + col * SEM_x_pixel) * Navi_pixel_resolu) * 1000);
                    int count = row * Navi_image_numb + col;
                    Navi_tile_x[count] = x_pos;
                    Navi_tile_y[count] = y_pos;
                }
            }

            //start thread acquisition function
            if (mCThread_Navi_acq != null && mCThread_Navi_acq.IsAlive)
                mCThread_Navi_acq.Abort();
            mCThread_Navi_acq = new Thread(Thread_Navi_acq);
            mCThread_Navi_acq.Start();
        }

        public void Abort_Navi_Map()
        {
            mCThread_Navi_acq.Abort();
            EventThreadStatus("Navigation: Acquisition abort");
        }

        public void Thread_Navi_acq()
        {
            EventThreadStatus("Navigation: Acquisition start");
            bool SEM_executed = false;
            while (!SEM_executed)
            {
                int return_status = stage.Set_Mag(Navi_mag);

                if (return_status == 0)
                {
                    string ptrString = "Navigation: SEM mag set " + Navi_mag.ToString();
                    mainForm.MY_DEBUG(ptrString);
                    SEM_executed = true;
                }
                else if (return_status == 1)
                {
                    mainForm.MY_DEBUG("Navigation: Mag set SEM busy");
                    Thread.Sleep(1000);
                    SEM_executed = false;
                }
                else
                {
                    mainForm.MY_DEBUG("Navigation: SEM Mag set fail");
                    Abort_Navi_Map();
                }
            }
            SEM_executed = false;

            Navi_tile_img = new Bitmap[Navi_img_total];
            for (int row = 0; row < Navi_image_numb; row++)
            {
                for (int col = 0; col < Navi_image_numb; col++)
                {
                    int count = row * Navi_image_numb + col;
                    while (!SEM_executed)
                    {
                        int return_status = stage.Move_XYaxis(Navi_tile_x[count], Navi_tile_y[count]);

                        if (return_status == 0)
                        {
                            string ptrString = "Navigation: SEM moves to X " + Navi_tile_x[count].ToString() + ", Y " + Navi_tile_y[count].ToString();
                            mainForm.MY_DEBUG(ptrString);
                            SEM_executed = true;
                        }
                        else if (return_status == 1)
                        {
                            mainForm.MY_DEBUG("Navigation: Wait move SEM stage busy");
                            Thread.Sleep(1000);
                            SEM_executed = false;
                        }
                        else
                        {
                            mainForm.MY_DEBUG("Navigation: Move SEM stage fail");
                            Abort_Navi_Map();
                        }
                    }
                    SEM_executed = false;
                    Thread.Sleep(5000);

                    //SEM functions to adjust image parameters and perform imaging  
                    Bitmap tmp = mainForm.get_SEM_image();
                    if (tmp == null)
                    {
                        mainForm.MY_DEBUG("Image: retrieve SEM image fail");
                        Abort_Navi_Map();
                    }
                    Navi_tile_img[count] = new Bitmap(tmp, SEM_x_pixel, SEM_y_pixel);
                }
            }

            Navi_img = new Bitmap(SEM_x_pixel * Navi_image_numb, SEM_y_pixel * Navi_image_numb);

            using (Graphics gr = Graphics.FromImage(Navi_img))
            {
                gr.Clear(Color.Black);
                for (int row = 0; row < Navi_image_numb; row++)
                {
                    for (int col = 0; col < Navi_image_numb; col++)
                    {
                        int count = row * Navi_image_numb + col;
                        gr.DrawImage(Navi_tile_img[count], col * SEM_x_pixel, row * SEM_y_pixel);
                    }
                }
            }

            EventSendNaviImg(Navi_img);
            EventThreadStatus("Navigation: Acquisition success");
        }

        //utilits             
        public int[] get_ROI_position(double roi_x, double roi_y, double roi_width, double roi_height)
        {
            // center of roi x and y in um
            // calculate the pixel difference between the center of bitmap and roi for x and y
            int x_pixel_diff = (int)Math.Round((roi_x - Navi_x) / Navi_pixel_resolu);
            int y_pixel_diff = (int)Math.Round((roi_y - Navi_y) / Navi_pixel_resolu);
            // find x and y position of roi in the image bitmap
            int x_pos = Navi_image_numb * SEM_x_pixel / 2 - x_pixel_diff;
            int y_pos = Navi_image_numb * SEM_y_pixel / 2 - y_pixel_diff;
            // calculate the withd and height of the roi in bitmap coordinate
            int bitmap_width = (int)Math.Round(roi_width / Navi_pixel_resolu);
            int bitmap_height = (int)Math.Round(roi_height / Navi_pixel_resolu);

            // return top left corner and width and height
            return new int[] { x_pos - (bitmap_width / 2), y_pos - (bitmap_height / 2), bitmap_width, bitmap_height };
        }

        // check if two bitmaaps are within a residue range.
        // For example, if bitmap 1 has mean of 69, bitmap2 has mean of 72
        // input threshold is 2, then it is a fail because 72-69 > 2
        // if the difference of mean of two bitmap is within threshold, return True
        public bool residue_check(Bitmap bmp1, Bitmap bmp2, double threshold)
        {
            double mean_bmp1 = get_mean(bmp1);
            double mean_bmp2 = get_mean(bmp2);
            if (Math.Abs(mean_bmp1 - mean_bmp2) < threshold)
            {
                return true;
            }
            return false;
        }
    }
}