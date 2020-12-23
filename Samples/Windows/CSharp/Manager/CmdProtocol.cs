using System;
using System.CodeDom;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermoCam160B
{
    /// <summary>
    /// 
    /// </summary>
    public enum PKT_INTERFACE : int
    {
        ERROR   = -1,
        UVC     = 0,
        RS485   = 1,
        RS232   = 2
    }

    /// <summary>
    /// 
    /// </summary>
    public enum BIN_OFFSET : ushort
    {
        VENDOR = 0x184,
        PRODUCT = 0x188,
        VERSION = 0x18C,
        BUILD_DATE = 0x190,
        BUILD_TIME = 0x19C
    }

    /// <summary>
    /// 
    /// </summary>
    public class Packet
    {
        public byte id;
        public byte command;
        public byte[] payload;
        public byte checksum;
    }

    /// <summary>
    /// 
    /// </summary>
    public enum PKT_ESC : byte
    {
        NUL = 0x00,      // NUL : NULL
        SOH = 0x01,      // SOH : Start of Header
        STX = 0x02,      // STX : Start of Text
        ETX = 0x03,      // ETX : End of Text
        EOT = 0x04,      // EOT : End of Transmission
        ACK = 0x06,
        NACK = 0x15,
        SYN = 0x16
    };

    /// <summary>
    /// 
    /// </summary>
    enum PKT_INDEX : int
    {
        SOH = 0,
        STX = 1,
        ID  = 2,
        CMD = 3,
        LEN = 4,
        PAYLOAD = 6
    };

    /// <summary>
    /// 
    /// </summary>
    public enum PKT_CMD : byte
    {
        CMD_IMAGE_FRAME = 0x10,

        CMD_TEMP_ROI = 0x21,

        CMD_CTRL_RESET = 0x31,
        CMD_CTRL_LED = 0x32,
        CMD_CTRL_BUZZER = 0x33,

        CMD_CAM_INFO = 0xC1,
        CMD_CAM_TEMP = 0xC2,
        CMD_CAM_VNC = 0xC3,

        CMD_CFG_SET_DEFAULT = 0xD0,
        CMD_CFG_READ_START = 0xD1,
        CMD_CFG_READ_ING = 0xD2,
        CMD_CFG_READ_END = 0xD3,
        CMD_CFG_WRITE_START = 0xD4,
        CMD_CFG_WRITE_ING = 0xD5,
        CMD_CFG_WRITE_END = 0xD6,

        CMD_SYS_GET_VERSION = 0xE1,
        CMD_SYS_GET_STATE = 0xE2,
        CMD_SYS_GET_WHERE = 0xE3,
        CMD_SYS_SET_WHERE = 0xE4,

        CMD_FLASH_START = 0xF1,
        CMD_FLASH_ING = 0xF2,
        CMD_FLASH_END = 0xF3,

        CMD_NACK    = 0xFF
    };

    /// <summary>
    /// 
    /// </summary>
    public class CmdProtocol
    {
        static public System.Windows.Forms.Form logForm;

        static public bool bShowLogMsg = false;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="pkt"></param>
        /// <returns></returns>
        static public bool WriteSerialPacket(SerialPort port, Packet pkt)
        {
            if (port == null || !port.IsOpen) return false;
            if (pkt == null || pkt.payload.Length < 0) return false;

            byte[] txData = new byte[pkt.payload.Length + 8];

            txData[(int)PKT_INDEX.SOH] = (byte)PKT_ESC.SOH;
            txData[(int)PKT_INDEX.STX] = (byte)PKT_ESC.STX;
            txData[(int)PKT_INDEX.ID] = pkt.id;
            txData[(int)PKT_INDEX.CMD] = pkt.command;
            txData[(int)PKT_INDEX.LEN] = (byte)(pkt.payload.Length & 0xFF);
            txData[(int)PKT_INDEX.LEN + 1] = (byte)(pkt.payload.Length >> 8 & 0xFF);

            Buffer.BlockCopy(pkt.payload, 0, txData, (int)PKT_INDEX.PAYLOAD, pkt.payload.Length);

            txData[(int)PKT_INDEX.PAYLOAD + pkt.payload.Length + 1] = (byte)PKT_ESC.ETX;

            byte checksum = 0;
            for (var index = (int)PKT_INDEX.ID; index < (int)(PKT_INDEX.PAYLOAD + pkt.payload.Length); index++)
            {
                checksum ^= txData[index];
            }
            txData[(int)PKT_INDEX.PAYLOAD + pkt.payload.Length] = pkt.checksum = checksum;

            port.DiscardOutBuffer();
            port.Write(txData, 0, txData.Length);

            if (bShowLogMsg)
            { 
                LogPackets(true, pkt);
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="port"></param>
        /// <param name="pkt"></param>
        /// <returns></returns>
        static public bool ReadSerialPacket(SerialPort port, ref Packet pkt)
        {
            byte checksum = 0;

            if (port.BytesToRead < 0) return false;
            if ((byte)port.ReadByte() != 0x01)
            {
                port.DiscardInBuffer();
                return false;
            }

            // sync the first byte
            if (port.BytesToRead < 0) return false;
            if ((byte)port.ReadByte() != 0x02)
            {
                port.DiscardInBuffer();
                return false;
            }

            // id
            if (port.BytesToRead < 0) return false;
            pkt.id = (byte)port.ReadByte();
            checksum ^= pkt.id;

            // cmd
            if (port.BytesToRead < 0) return false;
            pkt.command = (byte)port.ReadByte();
            checksum ^= pkt.command;

            // length
            if (port.BytesToRead < 0) return false;
            byte length_l = (byte)port.ReadByte();
            checksum ^= length_l;

            if (port.BytesToRead < 0) return false;
            byte length_h = (byte)port.ReadByte();
            checksum ^= length_h;

            ushort length = (ushort)((ushort)(length_h << 8) + (ushort)(length_l));

            //Array.Resize(ref pkt.payload, (int)length);
            pkt.payload = new byte[length];

            // payload
            ushort index = 0;
            while( index < length)
            {
                if(port.BytesToRead > 0)
                {
                    pkt.payload[index] = (byte)port.ReadByte();
                    checksum ^= pkt.payload[index];
                    index++;
                }
            }

            // checksum
            if (port.BytesToRead < 0) return false;
            //byte cs = (byte)port.ReadByte();
            pkt.checksum = (byte)port.ReadByte();
            //if (checksum != (byte)port.ReadByte())
            //{
            //    Console.WriteLine("Incorrect checksum");
            //    return false;
            //}

            // ETX
            if (port.BytesToRead < 0) return false;
            if ((byte)port.ReadByte() != 0x03)
            {
                Console.WriteLine("Incorrect ETX");
                return false;
            }

            port.DiscardInBuffer();

            if (bShowLogMsg)
            {
                LogPackets(false, pkt);
            }

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        static public bool CheckNACKPacket(Packet packet)
        {
            if(packet.command == (byte)PKT_CMD.CMD_NACK && packet.payload.Length == 1)
            {
                switch(packet.payload[0])
                {
                    case 0x01:
                        MessageBox.Show("Unknown Command", "Returned Error", MessageBoxButtons.OK);
                        break;

                    case 0x02:
                        MessageBox.Show("Wrong Packet", "Returned Error", MessageBoxButtons.OK);
                        break;

                    case 0x03:
                        MessageBox.Show("Incorrect Checksum", "Returned Error", MessageBoxButtons.OK);
                        break;

                    case 0x10:
                        MessageBox.Show("Invalid Argument", "Returned Error", MessageBoxButtons.OK);
                        break;

                    default:
                        break;
                }
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bTxRx"></param>
        /// <param name="packet"></param>
        static private void LogPackets(bool bTxRx, Packet packet)
        {
            if (packet == null || packet.payload.Length <= 0) return;

            string[] logStr = { DateTime.Now.ToString("HH:mm:ss.fff"),      // Log Time
                              "01",                                         // SOH
                              "02",                                         // STX
                              packet.id.ToString("X2"),                     // ID
                              packet.command.ToString("X2"),                // CMD
                              BitConverter.ToString(BitConverter.GetBytes((ushort)packet.payload.Length)).Replace('-', ' '),   // SIZE
                              BitConverter.ToString(packet.payload).Replace('-', ' '),    // PAYLOAD
                              packet.checksum.ToString("X2"),               // CHECKSUM
                              "03"                                          // ETX
                            };
            ListViewItem item = new ListViewItem(logStr);

            if (bTxRx == true)
            {
                // Transmission ( Tx )
                item.ForeColor = Color.Blue;
            }else
            {
                // Receive ( Rx )
                item.ForeColor = Color.Red;
            }

            ListView logView = logForm.Controls.Find("listView_protocol", true)[0] as ListView;

            logView.Update();

            if (logView.Items.Count > 10)
            {
                logView.Items.RemoveAt(0);
            }

            logView.Items.Add(item);
            logView.EnsureVisible(logView.Items.Count - 1);
            logView.Update();

            Application.DoEvents();
        }
        
    }
}
