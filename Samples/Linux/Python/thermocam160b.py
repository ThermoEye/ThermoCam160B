import cv2
import numpy as np
import threading

'''
    Open UVC device
'''
class camThread(threading.Thread):
    def __init__(self, previewName, camID):
        threading.Thread.__init__(self)
        self.previewName = previewName
        self.camID = camID
    def run(self):
        print("Starting " + self.previewName)
        camPreview(self.previewName, self.camID)

def camPreview(previewName, camID):
    cv2.namedWindow(previewName, cv2.WINDOW_NORMAL)
    cam = cv2.VideoCapture(camID)
    # cam = cv2.VideoCapture(camID, cv2.CAP_DSHOW) # For windows
    if cam.isOpened():  # try to get the first frame

        cam.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter.fourcc('Y', '1', '6', ' '))
        cam.set(cv2.CAP_PROP_CONVERT_RGB, 0)
        cam.set(cv2.CAP_PROP_FRAME_WIDTH, 256)
        cam.set(cv2.CAP_PROP_FRAME_HEIGHT, 192)

        print('width: {}, height: {}, format: {}'.format(cam.get(3), cam.get(4), cam.get(cv2.CAP_PROP_FORMAT)))

        rval, frame = cam.read()
    else:
        rval = False

    while rval:
        rval, frame = cam.read()
        if rval:
            # Normalize the 16-bit image
            frame = cv2.normalize(frame, frame, 20000, 65535, cv2.NORM_MINMAX)
            frame = np.uint8(frame / 256)  # Convert to 8-bit image
            # Apply a colormap
            frame = cv2.applyColorMap(frame, cv2.COLORMAP_JET)
        
        cv2.imshow(previewName, frame)

        key = cv2.waitKey(20)
        if key == 27:  # exit on ESC
            break

    if cam.isOpened():
        cam.release()
    cv2.destroyWindow(previewName)

# Create a thread for the thermal camera
threadThermalCam = camThread("thermal camera", "/dev/video8")

# Start the thread
threadThermalCam.start()

