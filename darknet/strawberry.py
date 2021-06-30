import numpy as np
import cv2
import time
from picamera.array import PiRGBArray
from picamera import PiCamera

min_confidence = 0.4
show_ratio = 1.0
title_name = 'Strawberry Inspection'

# Load Yolo
net = cv2.dnn.readNet("./model/strawberry_yolotiny_2000.weights", "./model/strawberry_yolotiny.cfg")
classes = []
with open("./model/classes.names", "r") as f:
    classes = [line.strip() for line in f.readlines()]
color_lists = np.random.uniform(0, 255, size=(len(classes), 3))

layer_names = net.getLayerNames()
output_layers = [layer_names[i[0] - 1] for i in net.getUnconnectedOutLayers()]

frame_width = 416
frame_height = 416
frame_resolution = [frame_width, frame_height]
frame_rate = 16

# initialize the camera and grab a reference to the raw camera capture
camera = PiCamera()
camera.resolution = frame_resolution
camera.framerate = frame_rate
rawCapture = PiRGBArray(camera, size=(frame_resolution))

# allow the camera to warmup
time.sleep(0.1)
        
class STRAWBERRY():
    def __init__(self):
        self.camera = camera
        self.rawCapture = rawCapture
        
    def detectAndDisplay(self, image):
        start_time = time.time()
        h, w = image.shape[:2]
        img = cv2.resize(image, (frame_width, frame_height))

        blob = cv2.dnn.blobFromImage(img, 0.00392, (416, 416), swapRB=True, crop=False)

        net.setInput(blob)
        outs = net.forward(output_layers)

        confidences = []
        names = []
        boxes = []
        colors = []

        for out in outs:
            for detection in out:
                scores = detection[5:]
                class_id = np.argmax(scores)
                confidence = scores[class_id]
                if confidence > min_confidence:
                    center_x = int(detection[0] * frame_width)
                    center_y = int(detection[1] * frame_height)
                    w = int(detection[2] * frame_width)
                    h = int(detection[3] * frame_height)

                    x = int(center_x - w / 2)
                    y = int(center_y - h / 2)

                    boxes.append([x, y, w, h])
                    confidences.append(float(confidence))
                    names.append(classes[class_id])
                    colors.append(color_lists[class_id])

        indexes = cv2.dnn.NMSBoxes(boxes, confidences, min_confidence, 0.4)
        font = cv2.FONT_HERSHEY_PLAIN
        
        ripe_cnt = 0
        unripe_cnt = 0;
        for i in range(len(boxes)):
            if i in indexes:
                x, y, w, h = boxes[i]
                label = '{} {:,.2%}'.format(names[i], confidences[i])
                color = colors[i]
                if label.split()[0] == "Unripe_Strawberry":
                    unripe_cnt += 1
                elif label.split()[0] == "Strawberry":
                    ripe_cnt += 1
                
                print(i, label, x, y, w, h)
                cv2.rectangle(img, (x, y), (x + w, y + h), color, 2)
                cv2.putText(img, label, (x, y - 10), font, 1, color, 2)
                
        cv2.imshow(title_name, img)
        if ripe_cnt != 0 or unripe_cnt != 0:
            print("=== A frame took {:.3f} seconds".format(time.time() - start_time))
            return ripe_cnt, unripe_cnt
        else :
            return 0, 0
        
if __name__=="__main__":
    strawberry = STRAWBERRY()
    for frame in strawberry.camera.capture_continuous(strawberry.rawCapture, format="bgr", use_video_port=True):
        Ripe, unRipe = strawberry.detectAndDisplay(frame.array)
        print("Ripe : ",Ripe," - unRipe : ",unRipe)
        strawberry.rawCapture.truncate(0)
