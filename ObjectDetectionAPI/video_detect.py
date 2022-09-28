import cv2
import os
import csv

def image_resize(image, width = None, height = None, inter = cv2.INTER_AREA):
    # initialize the dimensions of the image to be resized and
    # grab the image size
    dim = None
    (h, w) = image.shape[:2]

    # if both the width and height are None, then return the
    # original image
    if width is None and height is None:
        return image

    # check to see if the width is None
    if width is None:
        # calculate the ratio of the height and construct the
        # dimensions
        r = height / float(h)
        dim = (int(w * r), height)

    # otherwise, the height is None
    else:
        # calculate the ratio of the width and construct the
        # dimensions
        r = width / float(w)
        dim = (width, int(h * r))

    # resize the image
    resized = cv2.resize(image, dim, interpolation = inter)

    # return the resized image
    return resized

csvfile = open(os.path.dirname(os.path.realpath(__file__)) + '/results.csv', 'w', newline='')
writer = csv.writer(csvfile)

file = os.path.dirname(os.path.realpath(__file__)) + '/videos/green.mp4'
cap = cv2.VideoCapture(file)

current_directory = os.path.dirname(os.path.realpath(__file__))
objectname = "colors"
objectfolder = f'{current_directory}/content/{objectname}/' 
classes = []

with open(f'{objectfolder}names.names', 'r') as f:
    classes = f.read().splitlines()

net = cv2.dnn.readNetFromDarknet(f'{objectfolder}config.config', f'{objectfolder}weights.weights')
 
model = cv2.dnn_DetectionModel(net)
model.setInputParams(scale = 1 / 255, size=(416, 416), swapRB=True)

show_video = False

while True:
    ret, frame = cap.read()
    
    if ret:
        if show_video:
            frame = image_resize(frame, height = 416)
        else:
            frame = cv2.resize(frame, (416, 416), interpolation = cv2.INTER_AREA)
        
        classIds, scores, boxes = model.detect(frame, confThreshold=0.6, nmsThreshold=0.04)
        
        data = []
        data.append(int(cap.get(cv2.CAP_PROP_POS_MSEC)))

        for (classId, score, box) in zip(classIds, scores, boxes):
            data.append(classes[classId])
            data.append(score)
            
            if show_video:
                cv2.rectangle(frame, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]), color=(0, 255, 0), thickness=2)
                text = '%s: %.4f' % (classes[classId], score)
                print(text)
                cv2.putText(frame, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1, color=(0, 255, 0), thickness=2)
        
        writer.writerow(data)
        if show_video:
            cv2.imshow('Image', frame)

        if cv2.waitKey(1) & 0xFF == ord('q'):
            break
    else:
        break
        
cv2.destroyAllWindows()
csvfile.close()


# import cv2
# import os
# import csv


# file = os.path.dirname(os.path.realpath(__file__)) + '/video.avi'
# cap = cv2.VideoCapture(file)

# # csvfile = open(os.path.dirname(os.path.realpath(__file__)) + '/results.csv', 'w', newline='')
# # writer = csv.writer(csvfile)

# # current_directory = os.path.dirname(os.path.realpath(__file__))
# # objectname = "shapes"
# # objectfolder = f'{current_directory}/content/{objectname}/' 
# # classes = []

# # with open(f'{objectfolder}names.names', 'r') as f:
# #     classes = f.read().splitlines()

# # net = cv2.dnn.readNetFromDarknet(f'{objectfolder}config.config', f'{objectfolder}weights.weights')
 
# # model = cv2.dnn_DetectionModel(net)
# # model.setInputParams(scale = 1 / 255, size=(416, 416), swapRB=False)

# while True:
    
#     ret, frame = cap.read()


#     if ret:
#         resize = cv2.resize(frame, (416,416))
#         #classIds, scores, boxes = model.detect(frame, confThreshold=0.6, nmsThreshold=0.04)
        
#         #data = []
#         #data.append(int(cap.get(cv2.CAP_PROP_POS_MSEC)))
#         # for (classId, score, box) in zip(classIds, scores, boxes):
            
#         #     #data.append(classes[classId])
#         #     #data.append(score)
            
#         #     cv2.rectangle(frame, (box[0], box[1]), (box[0] + box[2], box[1] + box[3]), color=(0, 255, 0), thickness=2)
#         #     text = '%s: %.4f' % (classes[classId], score)
#         #     cv2.putText(frame, text, (box[0], box[1] - 5), cv2.FONT_HERSHEY_SIMPLEX, 1, color=(0, 255, 0), thickness=2)
        
#         cv2.imshow('Image', frame)
#         #writer.writerow(data)
#     else:
#         break
    
# cv2.destroyAllWindows()

# # csvfile.close()