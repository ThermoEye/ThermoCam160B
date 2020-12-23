/*
 *  UVCCamera
 *  library and sample to access to UVC web camera on non-rooted Android device
 *
 * Copyright (c) 2014-2017 saki t_saki@serenegiant.com
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *   You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *   Unless required by applicable law or agreed to in writing, software
 *   distributed under the License is distributed on an "AS IS" BASIS,
 *   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *   See the License for the specific language governing permissions and
 *   limitations under the License.
 *
 *  All files in the folder are under this Apache License, Version 2.0.
 *  Files in the libjpeg-turbo, libusb, libuvc, rapidjson folder
 *  may have a different license, see the respective files.
 */

package com.serenegiant.ThermoCam160B;

import android.graphics.SurfaceTexture;
import android.hardware.usb.UsbDevice;
import android.os.Bundle;
import android.util.Log;
import android.view.Surface;
import android.view.View;
import android.view.View.OnClickListener;
import android.view.View.OnLongClickListener;
import android.view.WindowManager;
import android.widget.ImageButton;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;

import com.serenegiant.common.BaseActivity;
import com.serenegiant.usb.CameraDialog;
import com.serenegiant.usb.USBMonitor;
import com.serenegiant.usb.USBMonitor.OnDeviceConnectListener;
import com.serenegiant.usb.USBMonitor.UsbControlBlock;
import com.serenegiant.usb.UVCCamera;
import com.serenegiant.usbcameracommon.UVCCameraHandler;
import com.serenegiant.widget.CameraViewInterface;
import com.serenegiant.widget.UVCCameraTextureView;

public final class MainActivity extends BaseActivity implements CameraDialog.CameraDialogParent {
	private static final boolean DEBUG = true;    // TODO set false on release
	private static final String TAG = "MainActivity";


	/**
	 * for accessing USB
	 */
	private USBMonitor mUSBMonitor;
	/**
	 * Handler to execute camera related methods sequentially on private thread
	 */
	private UVCCameraHandler mCameraHandler;
	/**
	 * for camera preview display
	 */
	private CameraViewInterface mUVCCameraView;

	/**
	 * button for start/stop recording
	 */
	private ImageButton mVideoCaptureButton;
	private ImageButton mScreenshotCaptureButton;

	private ImageView mUnconnectedImageView;

	private TextView mTempMinTextView;
	private TextView mTempMaxTextView;

	//frame.c랑 연결하는 부분
	static {
		System.loadLibrary("uvc");
	}
	public native void connectNative();

    //frame.c 에서 매 프레임마다 해당 함수를 호출합니다. 프레임마다 최대 최소 온도값을 반환해줍니다.
	public void setMinMaxTempTextView(float frameTempMin, float frameTempMax){
		runOnUiThread(new Runnable() {
			@Override
			public void run() {
				mTempMinTextView.setText(String.valueOf(frameTempMin) + " ℃");
				mTempMaxTextView.setText(String.valueOf(frameTempMax) + " ℃");
			}
		});
	}

	@Override
	protected void onCreate(final Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		if (DEBUG) Log.v(TAG, "onCreate:");
		if (DEBUG) Log.v("DBG", "onCreate");

		getWindow().setFlags(WindowManager.LayoutParams.FLAG_FULLSCREEN, WindowManager.LayoutParams.FLAG_FULLSCREEN);

		setContentView(R.layout.activity_main);

		mScreenshotCaptureButton = findViewById(R.id.capture_button);
		mScreenshotCaptureButton.setOnClickListener(mOnClickListener);
		mScreenshotCaptureButton.setVisibility(View.INVISIBLE);

		mVideoCaptureButton = findViewById(R.id.record_button);
		mVideoCaptureButton.setOnClickListener(mOnClickListener);
		mVideoCaptureButton.setVisibility(View.INVISIBLE);


		mTempMinTextView = findViewById(R.id.temp_min_textView);
		mTempMaxTextView = findViewById(R.id.temp_max_textView);

		final View view = findViewById(R.id.thermalcamera_view);
		view.setOnLongClickListener(mOnLongClickListener);
		mUVCCameraView = (CameraViewInterface) view;
		((UVCCameraTextureView) mUVCCameraView).setOnClickListener(mOnClickListener);
		mUVCCameraView.setAspectRatio(UVCCamera.THERMAL_PREVIEW_WIDTH / (float) UVCCamera.THERMAL_PREVIEW_HEIGHT);

		mUnconnectedImageView = findViewById(R.id.frame_image_unconnected);

		mUSBMonitor = new USBMonitor(this, mOnDeviceConnectListener);

		//열화상 카메라 뷰 생성 (width:160, height:120, preview mode:12)
		mCameraHandler = UVCCameraHandler.createHandler(this, mUVCCameraView,
				1, UVCCamera.THERMAL_PREVIEW_WIDTH, UVCCamera.THERMAL_PREVIEW_HEIGHT, UVCCamera.THERMAL_PREVIEW_MODE);

		mUVCCameraView.setCallback(new CameraViewInterface.Callback() {
			@Override
			public void onSurfaceCreated(CameraViewInterface view, Surface surface) {
				Log.d("onSurfaceCreated", "");
			}

			@Override
			public void onSurfaceChanged(CameraViewInterface view, Surface surface, int width, int height) {
				Log.d("onSurfaceChanged", "");

			}

			@Override
			public void onSurfaceDestroy(CameraViewInterface view, Surface surface) {
				Log.d("onSurfaceDestroy", "");

			}
		});
	}

	@Override
	protected void onStart() {
		super.onStart();
		if (DEBUG) Log.v(TAG, "onStart:");
		if (DEBUG) Log.v("DBG", "onStart()");
		mUSBMonitor.register();
		if (mUVCCameraView != null)
			mUVCCameraView.onResume();
	}

	@Override
	protected void onStop() {
		if (DEBUG) Log.v(TAG, "onStop:");
		if (DEBUG) Log.v("DBG", "onStop()");
		mCameraHandler.close();
		if (mUVCCameraView != null)
			mUVCCameraView.onPause();
		setCameraButton(false);
		super.onStop();
	}

	@Override
	public void onDestroy() {
		if (DEBUG) Log.v(TAG, "onDestroy:");
		if (mCameraHandler != null) {
			mCameraHandler.release();
			mCameraHandler = null;
		}
		if (mUSBMonitor != null) {
			mUSBMonitor.destroy();
			mUSBMonitor = null;
		}
		mUVCCameraView = null;
		mVideoCaptureButton = null;
		super.onDestroy();
	}


	private final OnClickListener mOnClickListener = new OnClickListener() {
		@Override
		public void onClick(final View view) {
			switch (view.getId()) {
				case R.id.thermalcamera_view:
					if (mCameraHandler != null) {
						if (!mCameraHandler.isOpened()) {
							CameraDialog.showDialog(MainActivity.this);

	               // if camera is connected
	//					} else {
	//						mHandlerL.close();
	//						setCameraButton();
						}
					}
					break;


				case R.id.record_button:
					if (mCameraHandler.isOpened()) {
						if (checkPermissionWriteExternalStorage() && checkPermissionAudio()) {
							if (!mCameraHandler.isRecording()) {
								mVideoCaptureButton.setColorFilter(0xffff0000);    // turn red
								mCameraHandler.startRecording();
							} else {
								mVideoCaptureButton.setColorFilter(0);    // return to default color
								mCameraHandler.stopRecording();
							}
						}
					}
					break;


				case R.id.capture_button:
					if (mCameraHandler.isOpened()) {
						if (checkPermissionWriteExternalStorage()) {
							mCameraHandler.captureStill();
						}
					}
					break;

			}
		}
	};

	/**
	 * disconnect camera connection when you long click on preview image(not on buttons)
	 */
	private final OnLongClickListener mOnLongClickListener = new OnLongClickListener() {
		@Override
		public boolean onLongClick(final View view) {
			switch (view.getId()) {
				case R.id.thermalcamera_view:
					if (mCameraHandler != null) {
						mCameraHandler.close();
						setCameraButton(true);
					}
			}
			return false;
		}
	};



	private void setCameraButton(final boolean isOn) {
		runOnUiThread(new Runnable() {
			@Override
			public void run() {

				if (!isOn && (mVideoCaptureButton != null)) {

					getWindow().clearFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);

					mVideoCaptureButton.setVisibility(View.INVISIBLE);
					mScreenshotCaptureButton.setVisibility(View.INVISIBLE);
					mUnconnectedImageView.setVisibility(View.VISIBLE);
				}
			}
		}, 0);
	}

	private void startPreviewThermal() {
		final SurfaceTexture st = mUVCCameraView.getSurfaceTexture();
		final Surface surface = new Surface(st);
		mCameraHandler.startPreview(surface);
		runOnUiThread(new Runnable() {
			@Override
			public void run() {
				getWindow().addFlags(WindowManager.LayoutParams.FLAG_KEEP_SCREEN_ON);
				mVideoCaptureButton.setVisibility(View.VISIBLE);
				mScreenshotCaptureButton.setVisibility(View.VISIBLE);
				mUnconnectedImageView.setVisibility(View.INVISIBLE);
				final View view = findViewById(R.id.thermalcamera_view);
				Log.d("this.surface", surface.toString());
			}
		});
		connectNative();
	}

	private final OnDeviceConnectListener mOnDeviceConnectListener = new OnDeviceConnectListener() {
		@Override
		public void onAttach(final UsbDevice device) {
			Toast.makeText(MainActivity.this, "USB_DEVICE_ATTACHED", Toast.LENGTH_SHORT).show();
			if (DEBUG) Log.v("DBG", "onAttach");
		}

		@Override
		public void onConnect(final UsbDevice device, final UsbControlBlock ctrlBlock, final boolean createNew) {
			if (DEBUG) Log.d(TAG, "onConnect:");
			if (DEBUG) Log.v("DBG", "onConnect");
			mCameraHandler.open(ctrlBlock);
			startPreviewThermal();
			Toast.makeText(MainActivity.this, "CAMERA CONNECTED", Toast.LENGTH_LONG).show();
		}

		@Override
		public void onDisconnect(final UsbDevice device, final UsbControlBlock ctrlBlock) {
			if (DEBUG) Log.v(TAG, "onDisconnect:");
			if (DEBUG) Log.v("DBG", "onDisconnect");
			if (mCameraHandler != null) {
				queueEvent(new Runnable() {
					@Override
					public void run() {
						mCameraHandler.close();
					}
				}, 0);
				setCameraButton(false);
			}
		}

		@Override
		public void onDettach(final UsbDevice device) {
			Toast.makeText(MainActivity.this, "USB_DEVICE_DETACHED", Toast.LENGTH_SHORT).show();
		}

		@Override
		public void onCancel(final UsbDevice device) {
			setCameraButton(false);
		}
	};

	/**
	 * to access from CameraDialog
	 *
	 * @return
	 */
	@Override
	public USBMonitor getUSBMonitor() {
		return mUSBMonitor;
	}

	@Override
	public void onDialogResult(boolean canceled) {
		if (DEBUG) Log.v(TAG, "onDialogResult:canceled=" + canceled);
		if (canceled) {
			setCameraButton(false);
		}
	}

	private boolean isActive() {
		return mCameraHandler != null && mCameraHandler.isOpened();
	}

	private boolean checkSupportFlag(final int flag) {
		return mCameraHandler != null && mCameraHandler.checkSupportFlag(flag);
	}

	private int getValue(final int flag) {
		return mCameraHandler != null ? mCameraHandler.getValue(flag) : 0;
	}

	private int setValue(final int flag, final int value) {
		return mCameraHandler != null ? mCameraHandler.setValue(flag, value) : 0;
	}

	private int resetValue(final int flag) {
		return mCameraHandler != null ? mCameraHandler.resetValue(flag) : 0;
	}

}