using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermoCam160B
{
    public partial class Main : Form
    {
        #region Get Version
        private string GetVerBootloader()
        {
            if (!tCameraUVC.IsConnect) return null;

            if(SendPacket(0, PKT_CMD.CMD_SYS_GET_VERSION, new byte[] { 0x00 }) != PKT_INTERFACE.ERROR)
            {
                return string.Format("{0}.{1}.{2}", tCameraUVC.cmdRxPacket.payload[2], tCameraUVC.cmdRxPacket.payload[1], tCameraUVC.cmdRxPacket.payload[0]);
            }

            return null;
        }

        private string GetVerMainApp()
        {
            if (!tCameraUVC.IsConnect) return null;

            if(SendPacket(0, PKT_CMD.CMD_SYS_GET_VERSION, new byte[] { 0x01 }) != PKT_INTERFACE.ERROR)
            {
                return string.Format("{0}.{1}.{2}", tCameraUVC.cmdRxPacket.payload[2], tCameraUVC.cmdRxPacket.payload[1], tCameraUVC.cmdRxPacket.payload[0]);
            }

            return null;
        }
        #endregion

        #region Information of Camera Sensor Module 
        private string GetSensorSerial()
        {
            if (!tCameraUVC.IsConnect) return null;

            if (SendPacket(0, PKT_CMD.CMD_CAM_INFO, new byte[] { 0x01 }) != PKT_INTERFACE.ERROR)
            {
                return tCameraUVC.cmdRxPacket.payload[7].ToString("X") + tCameraUVC.cmdRxPacket.payload[6].ToString("X") + tCameraUVC.cmdRxPacket.payload[5].ToString("X") + tCameraUVC.cmdRxPacket.payload[4].ToString("X")
                                                + tCameraUVC.cmdRxPacket.payload[3].ToString("X") + tCameraUVC.cmdRxPacket.payload[2].ToString("X") + tCameraUVC.cmdRxPacket.payload[1].ToString("X") + tCameraUVC.cmdRxPacket.payload[0].ToString("X");
            }

            return null;
        }
        #endregion

        #region System Status
        private int GetSystemStatus()
        {
            if (!tCameraUVC.IsConnect) return -1;

            int camStatus = -1;
            if (SendPacket(0, PKT_CMD.CMD_SYS_GET_STATE, new byte[] { 0x01 }) != PKT_INTERFACE.ERROR)
            {
                camStatus = tCameraUVC.cmdRxPacket.payload[0];
            }
            return camStatus;
        }
        #endregion

        private PKT_INTERFACE SendPacket(byte id, PKT_CMD cmd, byte[] payload)
        {
            Packet packet = new Packet()
            {
                id = id,
                command = (byte)cmd,
                payload = payload,
            };

            return tCameraUVC.Send(packet);
        }
    }
}
