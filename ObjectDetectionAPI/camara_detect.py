
net = cv2.dnn.readNetFromDarknet('config.config', 'weights.weights')
 
model = cv2.dnn_DetectionModel(net)
model.setInputParams(scale = 1 / 255, size = (416, 416), swapRB = True)

ret, frame = cap.read()
frame = cv2.resize(frame, (416, 416), interpolation = cv2.INTER_AREA)
classIds, scores, boxes = model.detect(frame, confThreshold=0.6, nmsThreshold=0.04)



import cv2
import os

cap = cv2.VideoCapture(2)

current_directory = os.path.dirname(os.path.realpath(__file__))
objectname = "shapes"
objectfolder = f'{current_directory}/content/{objectname}/' 
classes = []

with open(f'{objectfolder}names.names', 'r') as f:
    classes = f.read().splitlines()

net = cv2.dnn.readNetFromDarknet(f'{objectfolder}config.config', f'{objectfolder}weights_old.weights')
 
model = cv2.dnn_DetectionModel(net)
model.setInputParams(scale=1 / 255, size=(416, 416), swapRB=True)

while True:
    ret, frame = cap.read()
    
    if ret:
        frame = cv2.resize(frame, (416, 416), interpolation = cv2.INTER_AREA)
        classIds, scores, boxes = model.detect(frame, confThreshold=0.6, nmsThreshold=0.04)
        
        for (classId, score, box) in zip(classIds, scores, boxes):
            cv2.rectangle(frame, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]), color=(0, 255, 0), thickness=2)
            text = '%s: %.4f' % (classes[classId], score)
            cv2.putText(frame, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1, color=(0, 255, 0), thickness=2)
    
        cv2.imshow('Image', frame)
        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    else:
        break
        
cv2.destroyAllWindows()
