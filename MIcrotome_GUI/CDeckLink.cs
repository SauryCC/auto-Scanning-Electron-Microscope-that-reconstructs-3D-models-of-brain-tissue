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
using DeckLinkAPI;

//using CLRWrapper;
//using System.Windows.Media.Media3D;
using System.IO;
namespace MIcrotome_GUI
{
    public delegate void DeckLinkInputSignalHandler(bool inputSignal);
    public delegate void DeckLinkFormatChangedHandler(IDeckLinkDisplayMode newDisplayMode);
    public delegate void DeckLinkArrivedFrameHandler(byte[] arrivedFramePtr);
    public delegate void DeckLinkMessageHandler(string printedStr);

    
    public class CDeckLink : IDeckLinkInputCallback
    {

        private IDeckLink mDeckLink;
        private IDeckLinkInput mDeckLinkInput;

        public IDeckLinkIterator _deckLinkIterator;
        public List<IDeckLink> _deckLinkList = new List<IDeckLink>();

        private bool mIsCapturing = false;
        private bool mValidInput = false;
        public bool mImageArrived = false;

        private string mDeviceName;

        public event DeckLinkInputSignalHandler EventInputSignalChanged;
        public event DeckLinkFormatChangedHandler EventInputFormatChanged;
        public event DeckLinkArrivedFrameHandler EventArrivedFrame;
        public event DeckLinkMessageHandler EventPrintedStr;

        const _BMDDisplayMode displayMode = _BMDDisplayMode.bmdModeHD1080i6000;
        const _BMDPixelFormat pixelFormat = _BMDPixelFormat.bmdFormat10BitRGB;
        const _BMDVideoInputFlags inputFlags = _BMDVideoInputFlags.bmdVideoInputEnableFormatDetection;

        public byte[] RGB_ImageSEM;

        uint ImageCount = 0;

        public CDeckLink()
        {
            // SEM computer output only support bmdModeHD1080i6000 with bmdFormat10BitRGB format
            _deckLinkIterator = new CDeckLinkIterator();
            IDeckLink temp = null;
            while (true)
            {
                _deckLinkIterator.Next(out temp);

                if (temp == null)
                    break;
                else
                    _deckLinkList.Add(temp);
            }

            try
            {
                mDeckLink = _deckLinkList[_deckLinkList.Count - 1];
                mDeckLinkInput = (IDeckLinkInput)mDeckLink;
                mDeckLink.GetDisplayName(out mDeviceName);
            }
            catch
            {
                mDeckLink = null;
                return;
            }
            RGB_ImageSEM = new byte[1920 * 1080 * 4];
        }

        public IDeckLink deckLink
        {
            get { return mDeckLink; }
        }

        public IDeckLinkInput deckLinkInput
        {
            get { return mDeckLinkInput; }
        }

        public string deviceName
        {
            get { return mDeviceName; }
        }

        public bool IsCapturing
        {
            get { return mIsCapturing; }
        }

        public void VideoInputFrameArrived(IDeckLinkVideoInputFrame video, IDeckLinkAudioInputPacket audio)
        {
            IntPtr pData_ImageSEM;
            video.GetBytes(out pData_ImageSEM);
            System.Runtime.InteropServices.Marshal.Copy(pData_ImageSEM, RGB_ImageSEM, 0, 1920 * 1080 * 4);
            if (ImageCount == 2)
            {
                if (EventArrivedFrame != null)
                {
                    EventArrivedFrame(RGB_ImageSEM);
                }
                ImageCount = 0;
            }
            else
                ImageCount++;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(video);
        }

        public void VideoInputFormatChanged(_BMDVideoInputFormatChangedEvents events,
            IDeckLinkDisplayMode displayMode,
            _BMDDetectedVideoInputFormatFlags flags)
        {

        }

        public bool StartCapture()
        {
            if (mIsCapturing)
            {
                MY_DEBUG("SEM Image: The SEM image streaming has been started.");
                return false;
            }
            MY_DEBUG("SEM Image: The streming device " + mDeviceName);
            // Set capture callback
            mDeckLinkInput.SetCallback(this);
            // Set the video input mode
            mDeckLinkInput.EnableVideoInput(displayMode, pixelFormat, inputFlags);
            // Start the capture
            mDeckLinkInput.StartStreams();
            mIsCapturing = true;
            return true;
        }

        public bool StopCapture()
        {
            if (!mIsCapturing)
            {
                MY_DEBUG("SEM Image: The SEM image streaming has been stopped.");
                return false;
            }
            RemoveAllListeners();
            // Stop the capture
            deckLinkInput.StopStreams();
            // disable callbacks
            deckLinkInput.SetCallback(null);
            mIsCapturing = false;
            return true;
        }

        void RemoveAllListeners()
        {
            EventInputSignalChanged = null;
            EventInputFormatChanged = null;
        }

        void MY_DEBUG(string meassage)
        {
            if (EventPrintedStr != null)
            {
                EventPrintedStr(meassage);
            }
        }
    }
}