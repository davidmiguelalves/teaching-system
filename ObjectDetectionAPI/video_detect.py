
import cv2
import os
 
#cap = cv2.VideoCapture('D:\\Tese\\videos\\square_rotating.mp4')
cap = cv2.VideoCapture(1)

current_directory = os.path.dirname(os.path.realpath(__file__))
objectname = current_directory + '/content/test/' 
classes = []

with open(objectname + 'names.names', 'r') as f:
    classes = f.read().splitlines()

net = cv2.dnn.readNetFromDarknet(objectname + 'config.config', objectname+ 'new_weights.weights')
 
model = cv2.dnn_DetectionModel(net)
model.setInputParams(scale=1 / 255, size=(416, 416), swapRB=False)

while True:
    ret, frame = cap.read()
    frame = cv2.resize(frame, (1080,720))

    classIds, scores, boxes = model.detect(frame, confThreshold=0.6, nmsThreshold=0.04)
    
    for (classId, score, box) in zip(classIds, scores, boxes):
        cv2.rectangle(frame, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]), color=(0, 255, 0), thickness=2)
        text = '%s: %.4f' % (classes[classId], score)
        print(text)
        cv2.putText(frame, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1, color=(0, 255, 0), thickness=2)
 
    cv2.imshow('Image', frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break
    
cv2.destroyAllWindows()
