
from flask import Flask, request
from flask_restful import Resource, Api
import cv2
import json
import numpy as np
import os

app = Flask(__name__)
api = Api(app)
exerciseslist = {}
current_dir = os.path.dirname(os.path.realpath(__file__))

def Detect(request, exercise):    
    data = []
    try:
        nparr = np.frombuffer(request.data, np.uint8)
        img = cv2.imdecode(nparr, cv2.IMREAD_COLOR)
        classIds, scores, points = exercise.model.detect(img, confThreshold=0.6, nmsThreshold=0.04)
        for (classId, score, point) in zip(classIds, scores, points):
            detection = {}
            detection['x1'] = int(point[0])
            detection['y1'] = int(point[1])
            detection['x2'] = int(point[0]) + int(point[2])
            detection['y2'] = int(point[1]) + int(point[3])
            detection['score'] = float('{:.3f}'.format(score*100))
            detection['objectname'] = exercise.objects[classId]
            data.append(detection)
            # print('detected:' + exercise.objects[classId])
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

class Exercises(Resource):
    def get(self):
        return list(exerciseslist.keys()), 200

class Objects(Resource):
    def get(self, exercise):
        if exercise.lower() in exerciseslist:
            return exerciseslist[exercise.lower()].objects, 200
        return {'error': 'Exercise was not found.'}, 404

class Detection(Resource):
    def put(self, exercise):
        if exercise.lower() in exerciseslist:
            return Detect(request, exerciseslist[exercise.lower()]), 200
        return {'error': 'Exercise was not found.'}, 404

class Probe(Resource):
    def get(self):
        return {'message': 'ok'}, 200

api.add_resource(Probe, '/probe')
api.add_resource(Exercises, '/exercises')
api.add_resource(Objects, '/objects/<string:exercise>')
api.add_resource(Detection, '/detect/<string:exercise>')

if __name__ == '__main__':

    database = json.load(open(current_dir + '/database.json'))
    for exercise in database['exercises']:
        exerciseslist[exercise['name']] = ObjectDetection(exercise['name'], exercise['objects'])
    app.run()