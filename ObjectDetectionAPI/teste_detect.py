import sys
sys.path.append("c:\\program files\\python\\lib\\site-packages")

import cv2
import time
 

with open('colors.names', 'r') as f:
    classes = f.read().splitlines()

net = cv2.dnn.readNetFromDarknet('yolov4-tiny-custom.cfg', 'yolov4-tiny-custom_best-colors.weights')
 
model = cv2.dnn_DetectionModel(net)
model.setInputParams(scale=1 / 255, size=(416, 416), swapRB=True)

start = time.time()

img = cv2.imread('2.jpg')
classIds, scores, boxes = model.detect(img, confThreshold=0.6, nmsThreshold=0.04)
for (classId, score, box) in zip(classIds, scores, boxes):
    cv2.rectangle(img, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]),
                color=(0, 255, 0), thickness=2)
    text = '%s: %.4f' % (classes[classIds.item(0)], score)
    cv2.putText(img, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1,
                color=(0, 255, 0), thickness=2)

end = time.time()
print(end - start)

while True:
    #img = cv2.imread('1.jpg')
    ret, img = cap.read()
    
    classIds, scores, boxes = model.detect(img, confThreshold=0.6, nmsThreshold=0.04)
    
    for (classId, score, box) in zip(classIds, scores, boxes):
        cv2.rectangle(img, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]),
                    color=(0, 255, 0), thickness=2)
        text = '%s: %.4f' % (classes[classIds.item(0)], score)
        cv2.putText(img, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1,
                    color=(0, 255, 0), thickness=2)
 
    cv2.imshow('Image', img)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    
# cv2.waitKey(0)
cv2.destroyAllWindows()
