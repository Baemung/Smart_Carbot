import numpy as np
import cv2
import time
from picamera.array import PiRGBArray
from picamera import PiCamera

min_confidence = 0.4
show_ratio = 1.0
title_name = 'Defect Inspection'

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

def detectAndDisplay(image):
    h, w = image.shape[:2]
    frame_height = int(h * frame_width / w)
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
    for i in range(len(boxes)):
        if i in indexes:
            x, y, w, h = boxes[i]
            label = '{} {:,.2%}'.format(names[i], confidences[i])
            color = colors[i]
            print(i, label, x, y, w, h)
            cv2.rectangle(img, (x, y), (x + w, y + h), color, 2)
            cv2.putText(img, label, (x, y - 10), font, 1, color, 2)
    cv2.imshow(title_name, img)

for frame in camera.capture_continuous(rawCapture, format="bgr", use_video_port=True):
    start_time = time.time()
    detectAndDisplay(frame.array)
    end_time = time.time()
    process_time = end_time - start_time
    print("=== A frame took {:.3f} seconds".format(process_time))
    rawCapture.truncate(0)
    key = cv2.waitKey(1) & 0xFF
    if key == ord("q"):
        break