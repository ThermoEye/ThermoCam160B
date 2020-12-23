using DirectShowLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ThermoCam160B
{
    public partial class Main : Form
    {
        /// <summary>
        /// Main Form
        /// </summary>
        public Main()
        {
            InitializeComponent();

            // register USB device event handler
            UsbNotification.RegisterUsbDeviceNotification(this.Handle);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_Load(object sender, EventArgs e)
        {
            // initial listup cameras
            enumerateCameras();

            cameraUVC = new CameraUVC(this);
        }

        /// <summary>
        /// Enumerate UVC Cameras and enrolled
        /// </summary>
        private void enumerateCameras()
        {
            DsDevice[] systemCameras = DirectShowCam.GetCameraDeviceList();

            comboBox_camera_list.Items.Clear();
            foreach (var camera in systemCameras)
            {
                comboBox_camera_list.Items.Add(new { Text = camera.Name, Value = camera });
            }
        }

        /// <summary>
        /// Overrided Window Procedure
        /// </summary>
        /// <param name="m"></param>
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == UsbNotification.WmDevicechange)
            {
                switch ((int)m.WParam)
                {
                    // any device is attached.
                    case UsbNotification.DbtDevicearrival:
                        Console.WriteLine("USB device is attached");
                        Thread.Sleep(500);
                        enumerateCameras();
                        break;

                    // any device is detached.
                    case UsbNotification.DbtDeviceremovecomplete:
                        Console.WriteLine("USB device is removed");
                        Thread.Sleep(100);
                        enumerateCameras();
                        break;
                }
            }
        }
        #region USB Notification
        internal static class UsbNotification
        {
            public const int DbtDevicearrival = 0x8000;         // system detected a new device        
            public const int DbtDeviceremovecomplete = 0x8004;  // device is gone      
            public const int WmDevicechange = 0x0219;           // device change event      
            private const int DbtDevtypDeviceinterface = 5;
            private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
            private static IntPtr notificationHandle;

            /// <summary>
            /// Registers a window to receive notifications when USB devices are plugged or unplugged.
            /// </summary>
            /// <param name="windowHandle">Handle to the window receiving notifications.</param>
            ///
            public static void RegisterUsbDeviceNotification(IntPtr windowHandle)
            {
                DevBroadcastDeviceinterface dbi = new DevBroadcastDeviceinterface
                {
                    DeviceType = DbtDevtypDeviceinterface,
                    Reserved = 0,
                    ClassGuid = GuidDevinterfaceUSBDevice,
                    Name = 0
                };

                dbi.Size = Marshal.SizeOf(dbi);
                IntPtr buffer = Marshal.AllocHGlobal(dbi.Size);
                Marshal.StructureToPtr(dbi, buffer, true);

                notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
            }

            /// <summary>
            /// Unregisters the window for USB device notifications
            /// </summary>
            public static void UnregisterUsbDeviceNotification()
            {
                UnregisterDeviceNotification(notificationHandle);
            }

            [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
            private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

            [DllImport("user32.dll")]
            private static extern bool UnregisterDeviceNotification(IntPtr handle);

            [StructLayout(LayoutKind.Sequential)]
            private struct DevBroadcastDeviceinterface
            {
                internal int Size;
                internal int DeviceType;
                internal int Reserved;
                internal Guid ClassGuid;
                internal short Name;
            }
        }

        #endregion


    }
}
