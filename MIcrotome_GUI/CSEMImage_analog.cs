#if SEM_Analog_Input
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Matrox.MatroxImagingLibrary;


namespace MIcrotome_GUI
{

    public class CSEMImage_analog
    {
        private const int DEFAULT_IMAGE_SIZE_X = 1280;
        private const int DEFAULT_IMAGE_SIZE_Y = 960;
        private const int DEFAULT_IMAGE_SIZE_BAND = 1;

        MIL_ID MilApplication;  // MIL Application identifier.
        MIL_ID MilSystem;       // MIL System identifier.
        MIL_ID MilDisplay;      // MIL Display identifier.
        MIL_ID MilDigitizer;    // MIL Digitizer identifier.
        MIL_ID MilImage;        // MIL Image buffer identifier.

        MIL_INT BufSizeX;
        MIL_INT BufSizeY;
        MIL_INT BufSizeBand;

        public CSEMImage_analog()
        {
            MilApplication = MIL.M_NULL;  // MIL Application identifier.
            MilSystem = MIL.M_NULL;       // MIL System identifier.
            MilDisplay = MIL.M_NULL;      // MIL Display identifier.
            MilDigitizer = MIL.M_NULL;    // MIL Digitizer identifier.
            MilImage = MIL.M_NULL;        // MIL Image buffer identifier.

            BufSizeX = DEFAULT_IMAGE_SIZE_X;
            BufSizeY = DEFAULT_IMAGE_SIZE_Y;
            BufSizeBand = DEFAULT_IMAGE_SIZE_BAND;
        }

        ~CSEMImage_analog() { Disconnect(); }

        public void StartStream(IntPtr UserWindowHandle)
        {

            MIL.MappAlloc(MIL.M_DEFAULT, ref MilApplication);  // Allocate a MIL application.
            MIL.MsysAlloc(MIL.M_SYSTEM_MORPHIS, MIL.M_DEFAULT, MIL.M_DEFAULT, ref MilSystem);  // Allocate a MIL system.
            MIL.MdispAlloc(MilSystem, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_WINDOWED, ref MilDisplay);  // Allocate a MIL display

            if (MIL.MsysInquire(MilSystem, MIL.M_DIGITIZER_NUM, MIL.M_NULL) > 0)
            {
                //MIL.MdigAlloc(MilSystem, MIL.M_DEFAULT, "M_640X480_YUV16", MIL.M_DEFAULT, ref MilDigitizer);
                MIL.MdigAlloc(MilSystem, MIL.M_DEFAULT, "M_DEFAULT", MIL.M_DEFAULT, ref MilDigitizer);
                MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_X, ref BufSizeX);
                MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_Y, ref BufSizeY);
                //MIL.MdigInquire(MilDigitizer, MIL.M_SIZE_BAND, ref BufSizeBand);
            }

            if (MIL.MdispInquire(MilDisplay, MIL.M_DISPLAY_MODE, MIL.M_NULL) == MIL.M_WINDOWED)
            {
                // Allocate a MIL buffer.
                long Attributes = MIL.M_IMAGE + MIL.M_DISP;
                if (MilDigitizer != MIL.M_NULL)
                {
                    // Add M_GRAB attribute if a digitizer is allocated.
                    Attributes |= MIL.M_GRAB;
                }
                MIL.MbufAlloc2d(MilSystem, BufSizeX, BufSizeY, 8 + MIL.M_UNSIGNED, Attributes, ref MilImage);

                // Clear the buffer.
                MIL.MbufClear(MilImage, 0);

                // Select the MIL buffer to be displayed in the user-specified window.
                MIL.MdispSelectWindow(MilDisplay, MilImage, UserWindowHandle);

                // Grab in the user window if supported.
                if (MilDigitizer != MIL.M_NULL)
                {
                    // Grab continuously.
                    MIL.MdigGrabContinuous(MilDigitizer, MilImage);
                }
            }
        }

        public void Disconnect()
        {
            // Stop continuous grab.
            MIL.MdigHalt(MilDigitizer);

            // Remove the MIL buffer from the display.
            //MIL.MdispSelect(MilDisplay, MIL.M_NULL);

            // Free allocated objects.
            //MIL.MbufFree(MilImage);

            MIL.MappFreeDefault(MilApplication, MilSystem, MilDisplay, MilDigitizer, MilImage);


            MilApplication = MIL.M_NULL;  // MIL Application identifier.
            MilSystem = MIL.M_NULL;       // MIL System identifier.
            MilDisplay = MIL.M_NULL;      // MIL Display identifier.
            MilDigitizer = MIL.M_NULL;    // MIL Digitizer identifier.
            MilImage = MIL.M_NULL;        // MIL Image buffer identifier.

            BufSizeX = DEFAULT_IMAGE_SIZE_X;
            BufSizeY = DEFAULT_IMAGE_SIZE_Y;
            BufSizeBand = DEFAULT_IMAGE_SIZE_BAND;

        }

        public void PauseStream()
        {
            // Stop continuous grab.
            MIL.MdigHalt(MilDigitizer);

            // Remove the MIL buffer from the display.
            //MIL.MdispSelect(MilDisplay, MIL.M_NULL);

            // Free allocated objects.
            //MIL.MbufFree(MilImage);
        }

        public void ResumeStream()
        {
            if (MilDigitizer != MIL.M_NULL)
            {
                // Grab continuously.
                MIL.MdigGrabContinuous(MilDigitizer, MilImage);
            }
            else
            {
                MY_DEBUG("MilDigitizer is NULL");
            }
        }

        public void MY_DEBUG(string inf)
        {
            if (string.IsNullOrEmpty(inf) == false)
                System.Diagnostics.Debug.WriteLine(inf);
        }
    }
}
#endif