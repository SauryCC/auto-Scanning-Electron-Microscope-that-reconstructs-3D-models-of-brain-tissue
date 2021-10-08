using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using DeckLinkAPI;
// using Emgu;


namespace MIcrotome_GUI
{
    public partial class Main_control : Form
    {

        CDeckLink SEMImaging;
        //Input frame from the decklink capture
        const int DECK_WIDTH = 1920;  
        const int DECK_HEIGHT = 1080;
        //Cropped frame displayed in the interface
        const int SEM_WIDTH = 800;
        const int SEM_HEIGHT = 560;

        Bitmap image_convert_RGB2Bitmap_ImageShow_SEM = null;
        Bitmap mSEMImage_show_scale;

        public void SEM_Image_Initialize()
        {
            Thread.Sleep(1000);
            SEMImaging = new CDeckLink();
        }

        public bool SEM_Image_Streaming()
        {
            
            if (SEMImaging.deckLink != null)
            {
                bool result = SEMImaging.StartCapture();
                if (result == true)
                {
                    MY_DEBUG("SEM Image: Start streaming.");
                    SEMImaging.EventArrivedFrame += ImageShow_SEM;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                MY_DEBUG("SEM Image: Streaming device is not detected.");
                return false;
            }
        }

        public bool SEM_Image_Stop()
        {
            bool result = SEMImaging.StopCapture();
            if (result == true)
            {
                SEMImaging.EventArrivedFrame -= ImageShow_SEM;
                MY_DEBUG("SEM Image: Stop streaming.");
                return true;
            }
            else
                return false;
        }

        public Bitmap get_SEM_image()
        {
            if (SEMImaging.deckLink != null)
            {   
                if (SEMImaging.IsCapturing)
                {
                    return image_convert_RGB2Bitmap_ImageShow_SEM;
                }
                else
                {
                    MY_DEBUG("SEM Imaging is not starting."); 
                    return null;
                }
            }
            else
            {
                MY_DEBUG("SEM Image: Streaming device is not detected.");
                return null;
            }
        }

        void ImageShow_SEM(byte[] RGB_ImageSEM)
        {
            byte[] data_RGB;
            convert_bitRGB_To_byteRGB(RGB_ImageSEM, out data_RGB);
            convert_RGB2Bitmap_ImageShow_SEM(data_RGB);
        }

        public Bitmap convert_RGB2Bitmap_ImageShow_SEM(byte[] imageData)
        {
            // Need to copy our 8 bit greyscale image into a 32bit layout.
            // Choosing 32bit rather than 24 bit as its easier to calculate stride etc.
            // This will be slow enough and isn't the most efficient method.
            unsafe
            {
                fixed (byte* ptr = imageData)
                {
                    // Craete a bitmap wit a raw pointer to the data
                    image_convert_RGB2Bitmap_ImageShow_SEM = new Bitmap(SEM_WIDTH, SEM_HEIGHT, SEM_WIDTH * 3, PixelFormat.Format24bppRgb, new IntPtr(ptr));
                    // And save it.
                    // image.Save(Path.ChangeExtension(fileName, ".bmp"));
                    //int scale = pictureBox_SEMImage.Width;

                    //Rectangle rect = new Rectangle(239, 0, 1440, 1080);
                    //mSEMImage_show = image_convert_RGB2Bitmap_ImageShow_SEM.Clone(rect, PixelFormat.Format32bppRgb);
                    //mSEMImage_show_scale = new Bitmap(mSEMImage_show, width / scale, height / scale);

                    // using window Form pictureBox
                    mSEMImage_show_scale = new Bitmap(image_convert_RGB2Bitmap_ImageShow_SEM, pictureBox_SEMImage.Width, pictureBox_SEMImage.Height);
                    pictureBox_SEMImage.Image = mSEMImage_show_scale;

                    // open cv imageBox
                    // if (This.Enabled == true)
                    //     This.CV_Show_SEM_Image(This.mSEMImage_show);
                }
            }
            return image_convert_RGB2Bitmap_ImageShow_SEM;
        }

        void convert_bitRGB_To_byteRGB(byte[] data_RGB_ImageSEM, out byte[] data_RGB)
        {
            data_RGB = new byte[SEM_WIDTH * SEM_HEIGHT * 3];
            uint R_Hi, R_Lo, G_Hi, G_Lo, B_Hi, B_Lo;
            uint R, G, B;
            uint G_ex = 0b_0000_1111;
            uint B_ex = 0b_0000_0011;
            int i = 0;
            int y_offset = 280;
            try
            {
                for (int r = y_offset; r < (y_offset+ SEM_HEIGHT); r++)
                    for (int c = 320; c < 1120; c++)
                    {
                        int j = (r * DECK_WIDTH + c) * 4;    //DECK_WIDTH_OFF
                        //Blue
                        B_Hi = (B_ex & (uint)data_RGB_ImageSEM[j + 2]) << 8;
                        B_Lo = (uint)data_RGB_ImageSEM[j + 3];
                        B = (B_Hi | B_Lo) >> 2;
                        data_RGB[i] = (byte)B;
                        //Green
                        G_Hi = (G_ex & (uint)data_RGB_ImageSEM[j + 1]) << 6;
                        G_Lo = (uint)data_RGB_ImageSEM[j + 2] >> 2;
                        G = (G_Hi | G_Lo) >> 2;
                        data_RGB[i + 1] = (byte)G;
                        //Red
                        R_Hi = (uint)data_RGB_ImageSEM[j] << 4;
                        R_Lo = (uint)data_RGB_ImageSEM[j + 1] >> 4;
                        R = (R_Hi | R_Lo) >> 2;
                        data_RGB[i + 2] = (byte)R;

                        i += 3;
                    }
            }
            catch (Exception e)
            {
                MY_DEBUG("SEM Image: unvalid data readout.");
            }
        }

        /*public byte[] data_YUV_ImageSEM;
        void convert_byteYUV_To_byteRGB(byte[] data_YUV_ImageSEM, out byte[] data_RGB, int DECK_WIDTH, int DECK_HEIGHT)
        {
            data_RGB = new byte[DECK_HEIGHT * DECK_WIDTH * 4];
            int j;
            int u, y, r, c;
            try
            {
                for (r = 200; r < 740; r++)
                    for (c = 580; c < 1380; c += 2)
                    {
                        j = (r * DECK_WIDTH + c) * 2;    //DECK_WIDTH_OFF
                        u = data_YUV_ImageSEM[j];
                        y = data_YUV_ImageSEM[j + 1];

                        data_RGB[r * DECK_WIDTH + c] = (byte)(1.0 * y + 1.772 * (u - 128) + 0);        // b

                        y = data_YUV_ImageSEM[j + 3];
                        data_RGB[r * DECK_WIDTH + c + 1] = (byte)(1.0 * y + 1.772 * (u - 128) + 0);
                    }
            }
            catch (Exception e)
            { }
        }*/
    }
}
