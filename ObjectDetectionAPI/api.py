
from flask import Flask, request
from flask_restful import Resource, Api
import cv2
import json
import numpy as np
import os

app = Flask(__name__)
api = Api(app)
activitylist = {}
current_dir = os.path.dirname(os.path.realpath(__file__))

def Detect(request, activity):    
    data = []
    try:
        nparr = np.frombuffer(request.data, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        classIds, scores, points = activity.model.detect(img, confThreshold=0.6, nmsThreshold=0.04)
        for (classId, score, point) in zip(classIds, scores, points):
            detection = {}
            detection['x1'] = int(point[0])
            detection['y1'] = int(point[1])
            detection['x2'] = int(point[0]) + int(point[2])
            detection['y2'] = int(point[1]) + int(point[3])
            detection['score'] = float('{:.3f}'.format(score*100))
            detection['objectname'] = activity.objects[classId]
            data.append(detection)
            print('detected:' + activity.objects[classId])
    except Exception as e:
        print('________________ERROR________________')
        print(e)
        print('_____________________________________')
    
    return data

class ObjectDetection: 
    def __init__(self, objectname, objects): 
        self.objectname = objectname 
        self.objects = objects 
        self.net = cv2.dnn.readNetFromDarknet(current_dir + '/content/' + objectname + '/config.config', current_dir + '/content/' + objectname+ '/weights.weights')
        self.model = cv2.dnn_DetectionModel(self.net)
        self.model.setInputParams(scale=1 / 255, size=(416, 416), swapRB=True)

class Activities(Resource):
    def get(self):
        return list(activitylist.keys()), 200

class Objects(Resource):
    def get(self, activity):
        if activity.lower() in activitylist:
            return activitylist[activity.lower()].objects, 200
        return {'error': 'Activity was not found.'}, 404

class Detection(Resource):
    def put(self, activity):
        if activity.lower() in activitylist:
            return Detect(request, activitylist[activity.lower()]), 200
        return {'error': 'Activity was not found.'}, 404

class Probe(Resource):
    def get(self):
        return {'message': 'ok'}, 200

api.add_resource(Probe, '/probe')
api.add_resource(Activities, '/activities')
api.add_resource(Objects, '/objects/<string:activity>')
api.add_resource(Detection, '/detect/<string:activity>')

if __name__ == '__main__':

    database = json.load(open(current_dir + '/database.json'))
    for act in database['activities']:
        activitylist[act['name']] = ObjectDetection(act['name'], act['objects'])
    app.run()