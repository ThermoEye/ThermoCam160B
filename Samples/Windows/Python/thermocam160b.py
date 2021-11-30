import cv2
import time
import threading

'''
    Open UVC device
'''
class camThread(threading.Thread):
    def __init__(self, previewName, camID, camFourCC):
        threading.Thread.__init__(self)
        self.previewName = previewName
        self.camID = camID
        self.camFourCC = camFourCC
    def run(self):
        print ("Starting " + self.previewName)
        camPreview(self.previewName, self.camID, self.camFourCC)

def camPreview(previewName, camID, camFourCC):
    cv2.namedWindow(previewName)
    cam = cv2.VideoCapture(camID, cv2.CAP_DSHOW)
    if cam.isOpened():  # try to get the first frame

        if(camFourCC == cv2.VideoWriter.fourcc('Y','1','6',' ')):
            cam.set(cv2.CAP_PROP_FOURCC, cv2.VideoWriter.fourcc('Y','1','6',' '))
            cam.set(cv2.CAP_PROP_CONVERT_RGB, 0)
        else:
            cam.set(cv2.CAP_PROP_FRAME_WIDTH, 800)
            cam.set(cv2.CAP_PROP_FRAME_HEIGHT, 600)
            cam.set(cv2.CAP_PROP_AUTOFOCUS, 0) # turn the autofocus off

        print('width: {}, height : {}, frame? : {}, ? : {}'.format(cam.get(3), cam.get(4), cam.get(5), cam.get(8)))

        rval, frame = cam.read()
    else:
        rval = False

    while rval:
        rval, frame = cam.read()
        if(camFourCC == cv2.VideoWriter.fourcc('Y','1','6',' ')):
            # In order to display image, should be scaled and normalize.
            #minVal = numpy.amin(gray16Frame)
            #maxVal = numpy.amax(gray16Frame)
            #cv2.normalize(gray16Frame, gray16Frame, minVal, maxVal, cv2.NORM_MINMAX)        
            #cv2.normalize(gray16Frame, gray16Frame, minVal, 65535, cv2.NORM_MINMAX)
            cv2.normalize(frame, frame, 20000, 65535, cv2.NORM_MINMAX) # Best Normalized   

        cv2.imshow(previewName, frame)

        key = cv2.waitKey(20)
        if key == 27:  # exit on ESC
            break

    if cam.isOpened():
        cam.release()
    cv2.destroyWindow(previewName)        

# Create two threads for both visible and thermal camera
#threadVisibleCam = camThread("visible camera", 0, None)
threadThermalCam = camThread("thermal camera", 0, cv2.VideoWriter.fourcc('Y','1','6',' '))

#threadVisibleCam.start()
threadThermalCam.start()


