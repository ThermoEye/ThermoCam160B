using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
//using System.Linq;
using Microsoft.Win32;
using System.Windows.Forms;

namespace ThermoCam160B
{
    public class UsbSerialPort
    {
        public readonly string PortName;
        public readonly string DeviceId;
        public readonly string FriendlyName;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="id"></param>
        /// <param name="friendly"></param>
        private UsbSerialPort( string name, string id, string friendly )
        {
            PortName = name;
            DeviceId = id;
            FriendlyName = friendly;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static IEnumerable<RegistryKey> GetSubKeys( RegistryKey key )
        {
            foreach (string keyName in key.GetSubKeyNames())
                using (var subKey = key.OpenSubKey( keyName ))
                    yield return subKey;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strVID"></param>
        /// <param name="strPID"></param>
        /// <returns></returns>
        public static List<UsbSerialPort> FindUsbSerialPort(string strVID, string strPID)
        {
            List<UsbSerialPort> usbPort = new List<UsbSerialPort>();
            //int timeoutCount = 20000;

            Application.DoEvents();

            foreach (var port in GetPorts())
            {
                string usbVID = GetUSB_VID(port.DeviceId);
                string usbPID = GetUSB_PID(port.DeviceId);

                if (usbVID == strVID && usbPID == strPID)
                {
                    usbPort.Add(port);
                }
            }
            
            return usbPort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="friendly"></param>
        /// <returns></returns>
        static public UsbSerialPort FindUsbSerialPort(string friendly)
        {
            UsbSerialPort usbPort = null;
            int timeoutCount = 20000;

            while (usbPort == null && --timeoutCount != 0)
            {
                //Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                Application.DoEvents();

                foreach (var port in GetPorts())
                {
                    if (port.FriendlyName == friendly)
                    {
                        usbPort = port;
                        break;
                    }
                }
                //Application.Current.Dispatcher.Invoke(System.Windows.Threading.DispatcherPriority.Background, new Action(delegate { }));
                //Thread.Sleep(500);
            }
            return usbPort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<UsbSerialPort> GetPorts()
        {
            var existingPorts = SerialPort.GetPortNames();
            using (var enumUsbKey = Registry.LocalMachine.OpenSubKey( @"SYSTEM\CurrentControlSet\Enum\USB" ))
            {
                if (enumUsbKey == null)
                    throw new ArgumentNullException( "USB", "No enumerable USB devices found in registry" );
                foreach (var devBaseKey in GetSubKeys( enumUsbKey ))
                {
                    foreach (var devFnKey in GetSubKeys( devBaseKey ))
                    {
                        string friendlyName =
                            (string)devFnKey.GetValue( "FriendlyName" ) ??
                            (string)devFnKey.GetValue( "DeviceDesc" );
                        using (var devParamsKey = devFnKey.OpenSubKey( "Device Parameters" ))
                        {
                            string portName = (string)devParamsKey?.GetValue( "PortName" );
                            if (!string.IsNullOrEmpty( portName ) &&
                                existingPorts.Contains( portName ))
                                yield return new UsbSerialPort( portName, GetName( devBaseKey ) + @"\" + GetName( devFnKey ), friendlyName );
                        }
                    }

                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetName(RegistryKey key)
        {
            string name = key.Name;
            int idx;
            return (idx = name.LastIndexOf('\\')) == -1 ?
                name : name.Substring(idx + 1);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public static string GetUSB_VID(string deviceID)
        {
            string id = deviceID.ToUpper();
            return id.Substring(id.IndexOf("VID") + 4, 4);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceID"></param>
        /// <returns></returns>
        public static string GetUSB_PID(string deviceID)
        {
            string id = deviceID.ToUpper();
            return id.Substring(id.IndexOf("PID") + 4, 4);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format( "{0} Friendly: {1} DeviceId: {2}", PortName, FriendlyName, DeviceId );
        }
    }
}
