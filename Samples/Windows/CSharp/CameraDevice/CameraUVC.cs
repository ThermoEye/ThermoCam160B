using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Emgu.CV;
using Emgu.CV.Structure;

using DirectShowLib;
using System.IO.Ports;
using System.Drawing;

namespace ThermoCam160B
{
    public class CameraUVC : CameraBase
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="form"></param>
        public CameraUVC(System.Windows.Forms.Form form)
            : base(form)
        {
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
        public override bool Connect(string portName, DsDevice camDevice, int width, int height, int fps, int bpp = 16)
        {

            if(base.Connect(portName, camDevice, width, height, fps, bpp))
            {
                if (portName == "ThermoCam160")
                {
                    foreach (var usb in FindUsbPort("1209", "0160"))
                    {
                        if (vcpPort == null && !string.IsNullOrWhiteSpace(usb.PortName))
                        {
                            if (base.ConnectPort(usb.PortName)) return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool Disconnect()
        {
            base.DisconnectPort();
            return base.Disconnect();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vid"></param>
        /// <param name="pid"></param>
        /// <returns></returns>
        private List<UsbSerialPort> FindUsbPort(string vid, string pid)
        {
            return UsbSerialPort.FindUsbSerialPort(vid, pid);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pkt"></param>
        /// <returns></returns>
        public override PKT_INTERFACE Send(Packet pkt)
        {
            if (vcpPort != null)
            {
                if (CmdProtocol.WriteSerialPacket(vcpPort, pkt))
                {
                    if (rxPacketEvent.WaitOne(3000))
                    {
                        if (CmdProtocol.CheckNACKPacket(cmdRxPacket))
                        {
                            return PKT_INTERFACE.UVC;
                        }
                        return PKT_INTERFACE.ERROR;
                    }
                    else
                    {
                        MessageBox.Show("Timeout to receive packet", "Warning", MessageBoxButtons.OK);
                        return PKT_INTERFACE.ERROR;
                    }
                }
                MessageBox.Show("Error in sending packet", "Error", MessageBoxButtons.OK);
                return PKT_INTERFACE.ERROR;
            }
            MessageBox.Show("VCP port is not opened", "Warning", MessageBoxButtons.OK);
            return PKT_INTERFACE.ERROR;
        }
    }

}
