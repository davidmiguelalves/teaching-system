import cv2
import os
   
video = cv2.VideoCapture(1)
   
frame_width = int(video.get(3))
frame_height = int(video.get(4))
   
size = (frame_width, frame_height)
   
file = os.path.dirname(os.path.realpath(__file__)) + '/output.avi'
result = cv2.VideoWriter(file,  cv2.VideoWriter_fourcc(*'MJPG'), 10, size)
    
while(True):
    ret, frame = video.read()
  
    if ret == True: 
  
        result.write(frame)
        cv2.imshow('Frame', frame)
        if cv2.waitKey(1) & 0xFF == ord('s'):
            break
  
    # Break the loop
    else:
        break
  
video.release()
result.release()
    
cv2.destroyAllWindows()
   