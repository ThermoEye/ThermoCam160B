using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.InteropServices.ComTypes;
using DirectShowLib;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using Emgu.CV;

namespace ThermoCam160B
{
    static public class MediaSubFormat
    {
        static public readonly Guid Y16 = new Guid( 0x20363159, 0x0000, 0x0010, 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 );     // Y16
        static public readonly Guid BGR3 = new Guid(new byte[] { 0x7D, 0xEB, 0x36, 0xE4, 0x4F, 0x52, 0xCE, 0x11, 0x9F, 0x53, 0x00, 0x20, 0xAF, 0x0B, 0xA7, 0x70 });
    }

    public class DirectShowCam : IDisposable
    {
        private IVideoWindow videoWindow = null;
        private IMediaControl mediaControl = null;
        private IGraphBuilder graphBuilder = null;
        private ICaptureGraphBuilder2 captureGraphBuilder = null;
        private DsROTEntry rot = null;

        private IMediaSample mediaSample = null;
        private IMediaEvent mediaEvent = null;

        ISampleGrabber sampleGrabber = null;

        private int camWidth;
        private int camHeight;
        private int camStride;
        private int camFPS;
        private int camBpp;

        /// <summary>
        /// 
        /// </summary>
        public struct CameraProperty
        {
            public int Width;   // Width
            public int Height;  // Height
            public int FPS;     // Frame Per Seconds
            public int Bpp;     // Bits per Pixel
        };

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        static public DsDevice[] GetCameraDeviceList()
        {
            return DsDevice.GetDevicesOfCat( FilterCategory.VideoInputDevice );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo"></param>
        /// <returns></returns>
        static public CameraProperty ParseCameraProperty( string strInfo )
        {
            char[] delimiter = { 'x', '@', '-', ':', '\t' };

            string[] words = strInfo.Split( delimiter );

            CameraProperty camInfo = new CameraProperty();
            int.TryParse( words[0], out camInfo.Width );
            int.TryParse( words[1], out camInfo.Height );
            int.TryParse( words[2], out camInfo.FPS );
            int.TryParse( words[3], out camInfo.Bpp );

            return camInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="nWidth"></param>
        /// <param name="nHeight"></param>
        /// <param name="nFPS"></param>
        /// <param name="bpp"></param>
        /// <param name="grabberCallback"></param>
        /// <param name="subType"></param>
        /// <returns></returns>
        public int Open( DsDevice device, int nWidth, int nHeight, int nFPS, int bpp, ISampleGrabberCB grabberCallback = null, Guid subType = new Guid() )
        {
            camWidth = nWidth;
            camHeight = nHeight;
            camFPS = nFPS;
            camBpp = bpp;
            camStride = nWidth * bpp/8;

            return CaptureVideo(device, nFPS, nWidth, nHeight, grabberCallback, subType); ;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            Stop();
            CloseInterfaces();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <returns></returns>
        static public string[] GetCameraCapability( DsDevice dsDevice )
        {
            int hr;

            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;
            IPin pPin = null;

            string[] listVideoInfo;
            try
            {
                // add the video input device
                hr = filterGraph.AddSourceFilterForMoniker( dsDevice.Mon, null, "Source Filter", out capFilter );
                DsError.ThrowExceptionForHR( hr );
                pPin = DsFindPin.ByDirection( capFilter, PinDirection.Output, 0 );
                //listResolution = GetResolutionsAvailable( pPin ).ToList();
                listVideoInfo = GetResolutionsAvailable( pPin );
            }
            finally
            {
                Marshal.ReleaseComObject( pPin );
                pPin = null;
            }

            return listVideoInfo;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bit_count"></param>
        /// <returns></returns>
        private static bool IsBitCountAppropriate( short bit_count )
        {
            if (bit_count == 16 ||
                bit_count == 24 ||
                bit_count == 32)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Get bit count for mediatype
        /// </summary>
        /// <param name="media_type">Media type to analyze.</param>
        private static short GetBitCountForMediaType( AMMediaType media_type )
        {

            VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
            Marshal.PtrToStructure( media_type.formatPtr, videoInfoHeader );

            return videoInfoHeader.BmiHeader.BitCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="media_type"></param>
        /// <returns></returns>
        private static VideoInfoHeader GetResolutionForMediaType( AMMediaType media_type )
        {
            VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
            Marshal.PtrToStructure( media_type.formatPtr, videoInfoHeader );

            //return new Resolution( videoInfoHeader.BmiHeader.Width, videoInfoHeader.BmiHeader.Height );
            return videoInfoHeader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pinOutput"></param>
        /// <returns></returns>
        private static string[] GetResolutionsAvailable( IPin pinOutput )
        {
            int hr = 0;

            List<string> availResolutions = new List<string>();

            // Media type (shoudl be cleaned)
            AMMediaType media_type = null;

            //NOTE: pSCC is not used. All we need is media_type
            IntPtr pSCC = IntPtr.Zero;

            try
            {
                IAMStreamConfig videoStreamConfig = pinOutput as IAMStreamConfig;

                // -------------------------------------------------------------------------
                // We want the interface to expose all media types it supports and not only the last one set
                hr = videoStreamConfig.SetFormat( null );
                DsError.ThrowExceptionForHR( hr );

                int piCount = 0;
                int piSize = 0;

                hr = videoStreamConfig.GetNumberOfCapabilities( out piCount, out piSize );
                DsError.ThrowExceptionForHR( hr );

                for (int i = 0; i < piCount; i++)
                {
                    // ---------------------------------------------------
                    pSCC = Marshal.AllocCoTaskMem( piSize );
                    videoStreamConfig.GetStreamCaps( i, out media_type, pSCC );

                    // NOTE: we could use VideoStreamConfigCaps.InputSize or something like that to get resolution, but it's deprecated
                    //VideoStreamConfigCaps videoStreamConfigCaps = (VideoStreamConfigCaps)Marshal.PtrToStructure(pSCC, typeof(VideoStreamConfigCaps));
                    // ---------------------------------------------------
                    if (media_type.formatType == FormatType.VideoInfo)
                    {
                        if (IsBitCountAppropriate(GetBitCountForMediaType(media_type)))
                        {
                            VideoInfoHeader videoInfo = GetResolutionForMediaType(media_type);
                            availResolutions.Add($"{videoInfo.BmiHeader.Width}x{videoInfo.BmiHeader.Height}@{ (int)((10000 * 1000) / videoInfo.AvgTimePerFrame) }-{videoInfo.BmiHeader.BitCount}");
                        }
                    }

                    //FreeSCCMemory( ref pSCC );
                    if (pSCC != IntPtr.Zero)
                    {
                        Marshal.FreeCoTaskMem( pSCC );
                        pSCC = IntPtr.Zero;
                    }
                    //FreeMediaType( ref media_type );
                    if (media_type != null)
                    {
                        DsUtils.FreeAMMediaType( media_type );
                        media_type = null;
                    }
                }
            }
            catch
            {
                //EDDY throw;
            }
            finally
            {
                // clean up
                //FreeSCCMemory( ref pSCC );
                if (pSCC != IntPtr.Zero)
                {
                    Marshal.FreeCoTaskMem( pSCC );
                    pSCC = IntPtr.Zero;
                }
                //FreeMediaType( ref media_type );
                if (media_type != null)
                {
                    DsUtils.FreeAMMediaType( media_type );
                    media_type = null;
                }
            }

            availResolutions.Sort();
            return availResolutions.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int OpenInterfaces()
        {
            int hr = 0;

            // An exception is thrown if cast fail
            this.graphBuilder = (IGraphBuilder)new FilterGraph();
            this.captureGraphBuilder = (ICaptureGraphBuilder2)new CaptureGraphBuilder2();
            this.mediaControl = (IMediaControl)this.graphBuilder;
            this.videoWindow = (IVideoWindow)this.graphBuilder;

            DsError.ThrowExceptionForHR( hr );

            return hr;
        }

        /// <summary>
        /// 
        /// </summary>
        public void CloseInterfaces()
        {
            if (mediaControl != null)
            {
                // EDDY int hr = mediaControl.StopWhenReady();
                //int hr = mediaControl.Stop();
                //DsError.ThrowExceptionForHR( hr );
            }

            if (videoWindow != null)
            {
                videoWindow.put_Visible( OABool.False );
                videoWindow.put_Owner( IntPtr.Zero );
            }

            // Remove filter graph from the running object table.
            if (rot != null)
            {
                rot.Dispose();
                rot = null;
            }

            // EDDY_TEST
            if (sampleGrabber != null)
            {
                Marshal.ReleaseComObject( sampleGrabber );
                sampleGrabber = null;
            }
            if (mediaEvent != null)
            {
                Marshal.ReleaseComObject( mediaEvent );
                mediaEvent = null;
            }
            if (mediaSample != null)
            {
                Marshal.ReleaseComObject( mediaSample );
                mediaSample = null;
            }

            // Release DirectShow interfaces.
            if (this.mediaControl != null)
            {
                Marshal.ReleaseComObject( this.mediaControl );
                this.mediaControl = null;
            }
            if (this.videoWindow != null)
            {
                Marshal.ReleaseComObject( this.videoWindow );
                this.videoWindow = null;
            }
            if (this.graphBuilder != null)
            {
                Marshal.ReleaseComObject( this.graphBuilder );
                this.graphBuilder = null;
            }
            if (this.captureGraphBuilder != null)
            {
                Marshal.ReleaseComObject( this.captureGraphBuilder );
                this.captureGraphBuilder = null;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <param name="formatGUID"></param>
        public void SetVideoFormat( DsDevice dsDevice, Guid formatGUID )
        {
            int hr;

            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;
            IPin pPin = null;

            try
            {
                // add the video input device
                hr = filterGraph.AddSourceFilterForMoniker( dsDevice.Mon, null, "Source Filter", out capFilter );
                DsError.ThrowExceptionForHR( hr );
                pPin = DsFindPin.ByDirection( capFilter, PinDirection.Output, 0 );

                IAMStreamConfig videoStreamConfig = pPin as IAMStreamConfig;

                // Get the existing format block
                AMMediaType mediaType = null;
                hr = videoStreamConfig.GetFormat( out mediaType );
                DsError.ThrowExceptionForHR( hr );

                // copy out the videoinfoheader
                VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
                Marshal.PtrToStructure( mediaType.formatPtr, videoInfoHeader );

                // if overriding the framerate, set the frame rate
                if (camFPS > 0)
                {
                    videoInfoHeader.AvgTimePerFrame = 10000000 / camFPS;
                }

                // if overriding the width, set the width
                if (camWidth > 0)
                {
                    videoInfoHeader.BmiHeader.Width = camWidth;
                }

                // if overriding the Height, set the Height
                if (camHeight > 0)
                {
                    videoInfoHeader.BmiHeader.Height = camHeight;
                }

                // Copy the media structure back
                Marshal.StructureToPtr( videoInfoHeader, mediaType.formatPtr, false );

                mediaType.subType = formatGUID;

                // Set the new format
                hr = videoStreamConfig.SetFormat( mediaType );
                DsError.ThrowExceptionForHR( hr );

                DsUtils.FreeAMMediaType( mediaType );
                mediaType = null;

            }
            finally
            {
                Marshal.ReleaseComObject( pPin );
                pPin = null;
            }
        }
            
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IBaseFilter FindCaptureDevice()
        {
            int hr = 0;

            IEnumMoniker classEnum = null;
            IMoniker[] moniker = new IMoniker[1];
            object source = null;

            // Create the system device enumerator
            ICreateDevEnum devEnum = (ICreateDevEnum)new CreateDevEnum();

            // Create an enumerator for the video capture devices
            hr = devEnum.CreateClassEnumerator( FilterCategory.VideoInputDevice, out classEnum, 0 );
            DsError.ThrowExceptionForHR( hr );

            // The device enumerator is no more needed
            Marshal.ReleaseComObject( devEnum );

            // If there are no enumerators for the requested type, then 
            // CreateClassEnumerator will succeed, but classEnum will be NULL.
            if (classEnum == null)
            {
                throw new ApplicationException( "No video capture device was detected.\r\n\r\n" +
                                               "This sample requires a video capture device, such as a USB WebCam,\r\n" +
                                               "to be installed and working properly.  The sample will now close." );
            }

            // Use the first video capture device on the device list.
            // Note that if the Next() call succeeds but there are no monikers,
            // it will return 1 (S_FALSE) (which is not a failure).  Therefore, we
            // check that the return code is 0 (S_OK).

            if (classEnum.Next( moniker.Length, moniker, IntPtr.Zero ) == 0)
            {
                // Bind Moniker to a filter object
                Guid iid = typeof( IBaseFilter ).GUID;
                moniker[0].BindToObject( null, null, ref iid, out source );
            }
            else
            {
                throw new ApplicationException( "Unable to access video capture device!" );
            }

            // Release COM objects
            Marshal.ReleaseComObject( moniker[0] );
            Marshal.ReleaseComObject( classEnum );

            // An exception is thrown if cast fail
            return (IBaseFilter)source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public IBaseFilter SelectCaptureDevice( DsDevice device )
        {
            object source = null;
            Guid iid = typeof( IBaseFilter ).GUID;
            device.Mon.BindToObject( null, null, ref iid, out source );
            return (IBaseFilter)source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="capGraph"></param>
        /// <param name="capFilter"></param>
        /// <param name="iFrameRate"></param>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <param name="subType"></param>
        private void SetConfigParams( ICaptureGraphBuilder2 capGraph, IBaseFilter capFilter, int iFrameRate, int iWidth, int iHeight, Guid subType)
        {
            int hr;
            object config;
            AMMediaType mediaType;
            // Find the stream config interface
            hr = capGraph.FindInterface(
                PinCategory.Capture, MediaType.Video, capFilter, typeof( IAMStreamConfig ).GUID, out config );

            IAMStreamConfig videoStreamConfig = config as IAMStreamConfig;
            if (videoStreamConfig == null)
            {
                throw new Exception( "Failed to get IAMStreamConfig" );
            }

            // Get the existing format block
            hr = videoStreamConfig.GetFormat( out mediaType );
            DsError.ThrowExceptionForHR( hr );

            // copy out the videoinfoheader
            VideoInfoHeader videoInfoHeader = new VideoInfoHeader();
            Marshal.PtrToStructure( mediaType.formatPtr, videoInfoHeader );

            // if overriding the framerate, set the frame rate
            if (iFrameRate > 0)
            {
                videoInfoHeader.AvgTimePerFrame = 10000000 / iFrameRate;
            }

            // if overriding the width, set the width
            if (iWidth > 0)
            {
                videoInfoHeader.BmiHeader.Width = iWidth;
            }

            // if overriding the Height, set the Height
            if (iHeight > 0)
            {
                videoInfoHeader.BmiHeader.Height = iHeight;
            }

            // WORKAROUND If subtype is different from previous configured type, set new one.
            if (subType != Guid.Empty && subType != mediaType.subType)
            {
                // set designated subtype
                mediaType.subType = subType;
            }

            // Copy the media structure back
            Marshal.StructureToPtr( videoInfoHeader, mediaType.formatPtr, false );

            // Set the new format
            hr = videoStreamConfig.SetFormat( mediaType );
            DsError.ThrowExceptionForHR( hr );

            DsUtils.FreeAMMediaType( mediaType );
            mediaType = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleGrabber"></param>
        /// <param name="subType"></param>
        /// <param name="grabberCallback"></param>
        private void ConfigureSampleGrabber( ISampleGrabber sampleGrabber, Guid subType, ISampleGrabberCB grabberCallback )
        {
            AMMediaType media;
            int hr;

            // Set the media type to Video and format as subtype guid
            media = new AMMediaType();
            media.majorType = MediaType.Video;

            // WORKAROUND !!! In case of normal USB Web camera, it should be set RGB24 with parameter null.
            if (subType != MediaSubType.Null)
            {
                media.subType = subType;
            }else
            {
                media.subType = MediaSubType.RGB24;
            }
            media.formatType = FormatType.VideoInfo;

            hr = sampleGrabber.SetMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            DsUtils.FreeAMMediaType( media );
            media = null;

            // +EDDY_TEST
            ///sampleGrabber.SetBufferSamples( true );
            ///sampleGrabber.SetOneShot( true );
            // -EDDY_TEST

            hr = sampleGrabber.SetCallback( grabberCallback, 1 ); // BufferCallback
            //hr = sampleGrabber.SetCallback( grabberCallback, 0 );   // SampleCallback
            DsError.ThrowExceptionForHR( hr );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sampleGrabber"></param>
        private void SaveSizeInfo( ISampleGrabber sampleGrabber )
        {
            int hr;

            // Get the media type from the SampleGrabber
            AMMediaType media = new AMMediaType();
            hr = sampleGrabber.GetConnectedMediaType( media );
            DsError.ThrowExceptionForHR( hr );

            if ((media.formatType != FormatType.VideoInfo) || (media.formatPtr == IntPtr.Zero))
            {
                throw new NotSupportedException( "Unknown Grabber Media Format" );
            }

            // Grab the size info
            VideoInfoHeader videoInfoHeader = (VideoInfoHeader)Marshal.PtrToStructure( media.formatPtr, typeof( VideoInfoHeader ) );
            camStride = camWidth * (videoInfoHeader.BmiHeader.BitCount / 8);

            DsUtils.FreeAMMediaType( media );
            media = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device"></param>
        /// <param name="iFrameRate"></param>
        /// <param name="iWidth"></param>
        /// <param name="iHeight"></param>
        /// <param name="grabberCallback"></param>
        /// <param name="subType"></param>
        /// <returns></returns>
        public int CaptureVideo( DsDevice device, int iFrameRate, int iWidth, int iHeight, ISampleGrabberCB grabberCallback, Guid subType )
        {
            int hr = 0;
            IBaseFilter sourceFilter = null;
            IBaseFilter renderFilter = null;

            try
            {
                // Get DirectShow interfaces
                hr = OpenInterfaces();

                // Attach the filter graph to the capture graph
                hr = this.captureGraphBuilder.SetFiltergraph( this.graphBuilder );
                DsError.ThrowExceptionForHR( hr );

                // Use the system device enumerator and class enumerator to find
                // a video capture/preview device, such as a desktop USB video camera.
                sourceFilter = SelectCaptureDevice( device );
                // Add Capture filter to graph.
                hr = this.graphBuilder.AddFilter( sourceFilter, "DirectShowCam" );
                DsError.ThrowExceptionForHR( hr );

                // Configure preview settings.
                SetConfigParams( this.captureGraphBuilder, sourceFilter, iFrameRate, iWidth, iHeight, subType );

                // Initialize SampleGrabber.
                sampleGrabber = new SampleGrabber() as ISampleGrabber;
                // Configure SampleGrabber. Add preview callback.
                ConfigureSampleGrabber( sampleGrabber, subType, grabberCallback );
                // Add SampleGrabber to graph.
                hr = this.graphBuilder.AddFilter( sampleGrabber as IBaseFilter, "Frame Callback" );
                DsError.ThrowExceptionForHR( hr );

                // Add the Null Render to the filter graph
                renderFilter = new NullRenderer() as IBaseFilter;
                hr = this.graphBuilder.AddFilter( renderFilter, "NullRenderer" );

                // Render the preview
                hr = this.captureGraphBuilder.RenderStream( PinCategory.Preview, MediaType.Video, sourceFilter, (sampleGrabber as IBaseFilter), renderFilter );
                //hr = this.captureGraphBuilder.RenderStream( PinCategory.Preview, MediaType.Video, sourceFilter, (sampleGrabber as IBaseFilter), null );
                DsError.ThrowExceptionForHR( hr );

                SaveSizeInfo( sampleGrabber );

                // Add our graph to the running object table, which will allow
                // the GraphEdit application to "spy" on our graph
                rot = new DsROTEntry( this.graphBuilder );

                // Start previewing video data
                //hr = this.mediaControl.Run();
                //DsError.ThrowExceptionForHR( hr );
            }
            catch
            {
                ////MessageBox.Show( "An unrecoverable error has occurred." );
            }
            finally
            {
                if (sourceFilter != null)
                {
                    Marshal.ReleaseComObject( sourceFilter );
                    sourceFilter = null;
                }

                if (sampleGrabber != null)
                {
                    Marshal.ReleaseComObject( sampleGrabber );
                    sampleGrabber = null;
                }
            }

            return hr;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <param name="bEnable"></param>
        public void AutoFocus(DsDevice dsDevice, bool bEnable)
        {
            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;

            try
            {
                // add the video input device
                int hr = filterGraph.AddSourceFilterForMoniker( dsDevice.Mon, null, "Source Filter", out capFilter );
                DsError.ThrowExceptionForHR( hr );
                IAMCameraControl cameraControl = capFilter as IAMCameraControl;

                if(bEnable)
                {
                    cameraControl.Set( CameraControlProperty.Focus, 250, CameraControlFlags.Auto );
                }
                else
                {
                    cameraControl.Set( CameraControlProperty.Focus, 250, CameraControlFlags.Manual );
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine( ex.Message );
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public int GetCameraControl(DsDevice dsDevice, CameraControlProperty prop)
        {
            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;

            int retVal = 0;

            try
            {
                // add the video input device
                int hr = filterGraph.AddSourceFilterForMoniker(dsDevice.Mon, null, "Source Filter", out capFilter);
                DsError.ThrowExceptionForHR(hr);
                IAMCameraControl cameraControl = capFilter as IAMCameraControl;

                int min, max, step, default_val;
                CameraControlFlags flag = 0;
                cameraControl.GetRange(prop, out min, out max, out step, out default_val, out flag);

                cameraControl.Get(prop, out retVal, out flag);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retVal;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        public void SetCameraControl(DsDevice dsDevice, CameraControlProperty prop, int value, CameraControlFlags flag = CameraControlFlags.Auto)
        {
            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;

            try
            {
                // add the video input device
                int hr = filterGraph.AddSourceFilterForMoniker(dsDevice.Mon, null, "Source Filter", out capFilter);
                DsError.ThrowExceptionForHR(hr);
                IAMCameraControl cameraControl = capFilter as IAMCameraControl;

                cameraControl.Set(prop, value, flag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <param name="prop"></param>
        /// <returns></returns>
        public int GetVideoControl(DsDevice dsDevice, VideoProcAmpProperty prop)
        {
            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;

            int retVal = 0;

            try
            {
                // add the video input device
                int hr = filterGraph.AddSourceFilterForMoniker(dsDevice.Mon, null, "Source Filter", out capFilter);
                DsError.ThrowExceptionForHR(hr);
                IAMVideoProcAmp videoControl = capFilter as IAMVideoProcAmp;

                int min, max, step, default_val;
                VideoProcAmpFlags flag = 0;
                videoControl.GetRange(prop, out min, out max, out step, out default_val, out flag);

                videoControl.Get(prop, out retVal, out flag);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return retVal;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dsDevice"></param>
        /// <param name="prop"></param>
        /// <param name="value"></param>
        /// <param name="flag"></param>
        public void SetVideoControl(DsDevice dsDevice, VideoProcAmpProperty prop, int value = 0, VideoProcAmpFlags flag = VideoProcAmpFlags.Auto)
        {
            IFilterGraph2 filterGraph = new FilterGraph() as IFilterGraph2;
            IBaseFilter capFilter = null;

            try
            {
                // add the video input device
                int hr = filterGraph.AddSourceFilterForMoniker(dsDevice.Mon, null, "Source Filter", out capFilter);
                DsError.ThrowExceptionForHR(hr);
                IAMVideoProcAmp videoControl = capFilter as IAMVideoProcAmp;

                videoControl.Set(prop, value, flag);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Run()
        {
            int result = 0;
            if(mediaControl != null)
            {
                result = mediaControl.Run();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Stop()
        {
            int result = 0;
            if (mediaControl != null)
            {
                result = mediaControl.Stop();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Pause()
        {
            int result = 0;
            if (mediaControl != null)
            {
                result = mediaControl.Pause();
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size"></param>
        /// <param name="pBuffer"></param>
        public void GetCurrentBuffer(ref int size, IntPtr pBuffer)
        {
            IntPtr pBuff = IntPtr.Zero;
            if(sampleGrabber != null)
            {
                //sampleGrabber.GetCurrentBuffer( ref size, pBuffer );
                sampleGrabber.GetCurrentBuffer( ref size, pBuff );
                pBuff = Marshal.AllocCoTaskMem( size );
                sampleGrabber.GetCurrentBuffer( ref size, pBuff );
                Marshal.FreeCoTaskMem( pBuff );
                //sampleGrabber.GetCurrentSample( out mediaSample );
            }
        }
    }
}
