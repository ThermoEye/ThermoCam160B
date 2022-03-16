using DirectShowLib;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;

using Emgu.CV;
using Emgu.CV.Structure;

namespace ThermoCam160B
{
    public partial class Main : Form
    {
        private Mat matPalette = Palette.GetPresetColorMat("Ironbow");
        private CameraBase tCameraUVC;      // Thermal Camera
        private CameraBase vCameraUVC;      // Visible Camera

        private System.Windows.Forms.Timer monitorTimer = new System.Windows.Forms.Timer()
        {
            Interval = 1000,        // on 1 sec timer
        };

        private void MonitorTimer_Tick(object sender, EventArgs e)
        {
            if(tCameraUVC.IsConnect)
            {
                int camStatus = GetSystemStatus();
                if (camStatus != -1)
                {
                    label_status_connection.ForeColor = (camStatus & 0x01) == 0x01 ? Color.LightGreen : Color.OrangeRed;
                    label_status_stablization.ForeColor = (camStatus & 0x02) == 0x02 ? Color.LightGreen : Color.OrangeRed;
                }else
                {
                    label_status_connection.ForeColor = Color.DarkGray;
                    label_status_stablization.ForeColor = Color.DarkGray;
                }
            }
        }

        private void button_GetCameraInfo_Click(object sender, EventArgs e)
        {
            label_SerialNumber.Text = GetSensorSerial();
            label_BootloaderVersion.Text = GetVerBootloader();
            label_MainAppVersion.Text = GetVerMainApp();
        }

        private void button_buzzer_set_Click(object sender, EventArgs e)
        {
            byte buzCtrl = (byte)(Convert.ToByte(numericUpDown_buz_octave.Value) << 4);
            buzCtrl += (byte)(comboBox_buz_note.SelectedIndex);

            SendPacket(0, PKT_CMD.CMD_CTRL_BUZZER, new byte[] { buzCtrl, Convert.ToByte(numericUpDown_buz_duration.Value) });
        }

        private void comboBox_ColorMap_SelectedIndexChanged(object sender, EventArgs e)
        {
            string strPalette = (sender as ComboBox).SelectedItem.ToString();
            matPalette = Palette.GetPresetColorMat(strPalette);
        }

        /// <summary>
        /// Connect selected camera from listbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_Connect_Click(object sender, EventArgs e)
        {
            if (button_Connect.Text == "Connect")
            {
                if (comboBox_camera_list.SelectedIndex != -1)
                {
                    // Do Connection
                    button_Connect.Text = "Disconnect";

                    var itemObj = (comboBox_camera_list.SelectedItem as dynamic);
                    if(itemObj != null)
                    {
                        string[] listCap = DirectShowCam.GetCameraCapability(itemObj.Value);

                        DirectShowCam.CameraProperty camProperty;

                        //foreach (var cap in listCap)
                        {
                            camProperty = DirectShowCam.ParseCameraProperty(listCap[0]);    // select just first property
                        }

                        groupBox_status.Enabled = true;
                        tabControl_Control.Enabled = true;

                        tCameraUVC.Connect(itemObj.Text, 
                                          new DsDevice(itemObj.Value.Mon),
                                          camProperty.Width, camProperty.Height, camProperty.FPS, camProperty.Bpp);

                        tCameraUVC.frameEventHandler += tFrameEventHandler;
                        monitorTimer.Tick += MonitorTimer_Tick;
                        monitorTimer.Start();
                    }
                }
                else
                {
                    MessageBox.Show("Select one of cameras in uvc camera list", "Information", MessageBoxButtons.OK);
                }
            }
            else
            {
                // Do Disconnection
                monitorTimer.Stop();
                tCameraUVC.Disconnect();

                monitorTimer.Tick -= MonitorTimer_Tick;
                tCameraUVC.frameEventHandler -= tFrameEventHandler;

                tabControl_Control.Enabled = false;
                groupBox_status.Enabled = false;

                button_Connect.Text = "Connect";

                if (imageBox_ThermalView.Image != null)
                {
                    imageBox_ThermalView.Image.Dispose();
                    imageBox_ThermalView.Image = null;
                }

                comboBox_camera_list.SelectedIndex = -1;    // unselect any item
            }
        }

        private void button_vConnect_Click(object sender, EventArgs e)
        {
            if (button_vConnect.Text == "Connect")
            {
                if (comboBox_vcamera_list.SelectedIndex != -1)
                {
                    // Do Connection
                    button_vConnect.Text = "Disconnect";

                    var itemObj = (comboBox_vcamera_list.SelectedItem as dynamic);
                    if (itemObj != null)
                    {
                        string[] listCap = DirectShowCam.GetCameraCapability(itemObj.Value);

                        DirectShowCam.CameraProperty camProperty = new DirectShowCam.CameraProperty();

                        //foreach (var cap in listCap)
                        {
                            camProperty = DirectShowCam.ParseCameraProperty(listCap[0]);    // select just first property
                        }

                        //tCameraUVC.Connect(itemObj.Text,
                        //                  new DsDevice(itemObj.Value.Mon),
                        //                  camProperty.Width, camProperty.Height, camProperty.FPS, camProperty.Bpp);

                        vCameraUVC.Connect(itemObj.Text,
                                          new DsDevice(itemObj.Value.Mon),
                                          800, 600, 21, 24);    // FIXED

                        vCameraUVC.frameEventHandler += vFrameEventHandler;
                    }
                }
                else
                {
                    MessageBox.Show("Select one of cameras in uvc camera list", "Information", MessageBoxButtons.OK);
                }
            }
            else
            {
                // Do Disconnection
                vCameraUVC.Disconnect();
                vCameraUVC.frameEventHandler -= vFrameEventHandler;

                button_vConnect.Text = "Connect";

                if (imageBox_VisiblelView.Image != null)
                {
                    imageBox_VisiblelView.Image.Dispose();
                    imageBox_VisiblelView.Image = null;
                }

                comboBox_vcamera_list.SelectedIndex = -1;    // unselect any item
            }
        }

        /// <summary>
        /// event handler for received frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tFrameEventHandler(object sender, EventArgs e)
        {
            if (!tCameraUVC.IsConnect) return;

            using(Mat matImage = tCameraUVC.QueryFrame())
            {
                if(matImage != null)
                {
                    using (Image<Gray, ushort> grayImage = matImage.ToImage<Gray, ushort>())
                    {
                        Image<Bgr, byte> rgbImage = grayImage.Convert<Bgr, byte>();

                            // Preview Image
                        if (imageBox_ThermalView.Image != null)
                        {
                            imageBox_ThermalView.Image.Dispose();
                        }

                        // Apply Color Map with selected palette
                        Palette.ApplyColorMap(ref rgbImage, matPalette);

                        imageBox_ThermalView.Image = Utils.ResizeImage(rgbImage,
                                                                    imageBox_ThermalView.Size.Width, imageBox_ThermalView.Size.Height,
                                                                    Emgu.CV.CvEnum.Inter.Linear);

                        

                        // Get Temperatures
                        double[] minValue, maxValue;
                        Point[] minPoint, maxPoint;
                        matImage.MinMax(out minValue, out maxValue, out minPoint, out maxPoint);

                        tCameraUVC.tempCenterKelvin = BitConverter.ToUInt16(matImage.GetData(matImage.Height / 2, matImage.Width / 2), 0);

                        double minTemp = Utils.ConvertKelvinToCelsius(minValue[0] / 100);
                        double avgTemp = Utils.ConvertKelvinToCelsius(CvInvoke.Mean(matImage).V0 / 100);
                        double maxTemp = Utils.ConvertKelvinToCelsius(maxValue[0] / 100);
                        double roiTemp = Utils.ConvertKelvinToCelsius((double)tCameraUVC.tempCenterKelvin / 100);

                        minTemp = Utils.CompensateHumanTemperature(minTemp);
                        avgTemp = Utils.CompensateHumanTemperature(avgTemp);
                        maxTemp = Utils.CompensateHumanTemperature(maxTemp);
                        roiTemp = Utils.CompensateHumanTemperature(roiTemp);

                        // Update values
                        Invoke((Action) delegate
                        {
                            label_MinTemp.Text = string.Format("{0:0.0} ℃", minTemp);
                            label_AvgTemp.Text = string.Format("{0:0.0} ℃", avgTemp);
                            label_MaxTemp.Text = string.Format("{0:0.0} ℃", maxTemp);
                        });

                    }
                }
            }
        }

        /// <summary>
        /// event handler for received frame
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void vFrameEventHandler(object sender, EventArgs e)
        {
            if (!vCameraUVC.IsConnect) return;

            using (Mat matImage = vCameraUVC.QueryFrame())
            {
                if (matImage != null)
                {
                    using (Image<Bgr, byte> bgrImage = matImage.ToImage<Bgr, byte>())
                    {
                        // Preview Image
                        if (imageBox_VisiblelView.Image != null)
                        {
                            imageBox_VisiblelView.Image.Dispose();
                        }
                        imageBox_VisiblelView.Image = Utils.ResizeImage(bgrImage,
                                                                    imageBox_VisiblelView.Size.Width, imageBox_VisiblelView.Size.Height,
                                                                    Emgu.CV.CvEnum.Inter.Linear);
                    }
                }
            }
        }
    }
}
