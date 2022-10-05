import cv2
import os
import csv

show_video = False
save_results = False
save_video = True

video_name = "square"
objectname = "shapes"

if save_results:
    csvfile = open(os.path.dirname(os.path.realpath(__file__)) + '/results.csv', 'w', newline='')
    writer = csv.writer(csvfile)

current_directory = os.path.dirname(os.path.realpath(__file__))

file = f'{current_directory}/videos/{video_name}.mp4'
cap = cv2.VideoCapture(file)

width  = 416
height = 416
dim = (width, height)
objectfolder = f'{current_directory}/content/{objectname}/' 
classes = []

with open(f'{objectfolder}names.names', 'r') as f:
    classes = f.read().splitlines()

net = cv2.dnn.readNetFromDarknet(f'{objectfolder}config.config', f'{objectfolder}weights_old.weights')

r = width / float(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
dim = (width, int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT) * r))

if save_video: 
    out_video = cv2.VideoWriter('output.mp4', -1, 20.0, dim)
 
model = cv2.dnn_DetectionModel(net)
model.setInputParams(scale = 1 / 255, size=(width, height), swapRB=True)


while True:
    ret, frame = cap.read()
    
    if ret:
        frame = cv2.resize(frame, dim, interpolation = cv2.INTER_AREA)
        
        classIds, scores, boxes = model.detect(frame, confThreshold=0.6, nmsThreshold=0.04)
        
        data = []
        data.append(int(cap.get(cv2.CAP_PROP_POS_MSEC)))

        for (classId, score, box) in zip(classIds, scores, boxes):
            data.append(classes[classId])
            data.append(score)
            
            if show_video or save_video:
                cv2.rectangle(frame, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]), color=(0, 255, 0), thickness=2)
                text = '%s: %.4f' % (classes[classId], score)
                # print(text)
                cv2.putText(frame, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1, color=(0, 255, 0), thickness=2)
        
        if save_video:
            out_video.write(frame)

        if save_results:
            writer.writerow(data)
        if show_video:
            cv2.imshow('Image', frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    else:
        break
        
cv2.destroyAllWindows()

if save_video:
    out_video.release()

if save_results:
    csvfile.close()

