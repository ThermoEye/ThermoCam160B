using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using DirectShowLib;
using Emgu.CV;

namespace ThermoCam160B
{
    public class CameraBase : IDisposable, ISampleGrabberCB
    {
        public System.Windows.Forms.Form form;

        /// <summary>
        /// USB Camera Device Handles
        /// </summary>
        public DsDevice dsDevice = null;
        public DirectShowCam dsCamera = null;
        public SerialPort vcpPort = null;

        /// <summary>
        /// Camera Proeprty
        /// </summary>
        public Size frameSize;
        public int frameFPS;
        public int frameBPP;

        /// <summary>
        /// Image FrameBuffer
        /// </summary>
        public event EventHandler frameEventHandler;
        public Mat matFrame = null;
        public IntPtr pFrameBuffer = IntPtr.Zero;

        /// <summary>
        /// VCP Command Protocols
        /// </summary>
        public Packet cmdRxPacket = new Packet();
        public AutoResetEvent rxPacketEvent = new AutoResetEvent(false);

        public ushort tempCenterKelvin;
        public ushort tempFPaKelvin;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        public CameraBase(System.Windows.Forms.Form form)
        {
            this.form = form;
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public bool IsConnect
        {
            get
            {
                //if (dsCamera != null && vcpPort != null) return true;
                if (dsCamera != null ) return true;
                else return false;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        virtual public void Dispose()
        {
            this.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="camDevice"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="fps"></param>
        /// <param name="bpp"></param>
        /// <returns></returns>
        public virtual bool Connect(string portName, DsDevice camDevice, int width, int height, int fps, int bpp)
        {
            if (pFrameBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pFrameBuffer);
                pFrameBuffer = IntPtr.Zero;
            }
            this.pFrameBuffer = Marshal.AllocHGlobal(width * height * bpp / 8);

            if (bpp == 16)
            {
                this.matFrame = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv16U, 1);
            }else
            {
                this.matFrame = new Mat(height, width, Emgu.CV.CvEnum.DepthType.Cv8U, bpp / 8);
            }

            this.frameSize.Width = width;
            this.frameSize.Height = height;
            this.frameFPS = fps;
            this.frameBPP = bpp;

            if (dsCamera == null && !string.IsNullOrWhiteSpace(portName))
            {
                this.dsDevice = camDevice;
                this.dsCamera = new DirectShowCam();
                if (this.dsCamera != null)
                {
                    if (bpp == 16)
                    {
                        this.dsCamera.Open(camDevice, width, height, fps, bpp, this, MediaSubFormat.Y16);
                    }else
                    {
                        this.dsCamera.Open(camDevice, width, height, fps, bpp, this, MediaSubType.Null);
                    }
                    this.dsCamera.Run();

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual public bool Disconnect()
        {
            if (this.dsCamera != null)
            {
                this.dsCamera.Stop();
                this.dsCamera.Close();
                this.dsCamera.Dispose();
                this.dsCamera = null;
            }

            if (pFrameBuffer != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(pFrameBuffer);
                pFrameBuffer = IntPtr.Zero;
            }

            if (this.matFrame != null)
            {
                this.matFrame.Dispose();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SampleTime"></param>
        /// <param name="pBuffer"></param>
        /// <param name="BufferLen"></param>
        /// <returns></returns>
        unsafe public int BufferCB(double SampleTime, IntPtr pBuffer, int BufferLen)
        {
            if (pBuffer != IntPtr.Zero && BufferLen > 0)
            {
                Buffer.MemoryCopy((void*)pBuffer, (void*)pFrameBuffer, BufferLen, BufferLen);
                if(this.frameEventHandler != null)
                {
                    frameEventHandler(this, EventArgs.Empty);
                }
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SampleTime"></param>
        /// <param name="pSample"></param>
        /// <returns></returns>
        public int SampleCB(double SampleTime, IMediaSample pSample)
        {
            IntPtr pBuffer;
            pSample.GetPointer(out pBuffer);
            int BufferLen = pSample.GetActualDataLength();
            Marshal.ReleaseComObject(pSample);
            return 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual public unsafe Mat QueryFrame()
        {
            if (!IsConnect) return null;

            if (pFrameBuffer != IntPtr.Zero)
            {
                this.matFrame.Dispose();

                if (frameBPP == 16)
                {
                    this.matFrame = new Mat(frameSize.Height, frameSize.Width, Emgu.CV.CvEnum.DepthType.Cv16U, 1, (IntPtr)pFrameBuffer, frameSize.Width * frameBPP / 8);
                    if (this.matFrame != null)
                    {
                        CvInvoke.Flip(this.matFrame, this.matFrame, Emgu.CV.CvEnum.FlipType.Horizontal);
                    }
                }
                else
                {
                    matFrame = new Mat(frameSize.Height, frameSize.Width, Emgu.CV.CvEnum.DepthType.Cv8U, frameBPP / 8, (IntPtr)pFrameBuffer, frameSize.Width * frameBPP / 8);
                    if (this.matFrame != null)
                    {
                        CvInvoke.Flip(this.matFrame, this.matFrame, Emgu.CV.CvEnum.FlipType.Horizontal);
                        CvInvoke.Flip(this.matFrame, this.matFrame, Emgu.CV.CvEnum.FlipType.Vertical);
                    }
                }
            }

            return this.matFrame;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        virtual public bool ConnectPort(string portName)
        {
            try
            {
                vcpPort = new SerialPort(portName)
                {
                    BaudRate = 115200,
                    DataBits = 8,
                    Parity = Parity.None,
                    StopBits = StopBits.One
                };

                if(vcpPort != null)
                {
                    vcpPort.Open();

                    if (vcpPort.IsOpen)
                    {
                        vcpPort.DiscardInBuffer();
                        vcpPort.DiscardOutBuffer();

                        vcpPort.DataReceived += DataReceived;
                        return true;
                    }
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message, "Warning", MessageBoxButtons.OK);
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        virtual public void DisconnectPort()
        {
            if (vcpPort != null)
            {
                vcpPort.DataReceived -= DataReceived;
                vcpPort.Close();
                vcpPort.Dispose();
                vcpPort = null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        virtual public unsafe void DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            if (CmdProtocol.ReadSerialPacket(sender as SerialPort, ref cmdRxPacket))
            {
                if (cmdRxPacket.payload != null && cmdRxPacket.payload.Length > 0)
                {
                    fixed (byte* pBuffer = cmdRxPacket.payload)
                    {
                        Buffer.MemoryCopy((void*)pBuffer, (void*)pFrameBuffer, cmdRxPacket.payload.Length, cmdRxPacket.payload.Length);
                    }
                    rxPacketEvent.Set();    // send signal to wait object for rx packet event
                }
            }
            else
            {
                Console.WriteLine("NG");
            }
        }



        virtual public PKT_INTERFACE Send(Packet pkt)
        {
            return PKT_INTERFACE.ERROR;
        }
    }
}
