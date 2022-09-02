/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package zeca.api.zecaapi;

import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RestController;

/**
 *
 * @author david.alves
 */
@RestController
public class ZecaRobotController {
    
    private final ZecaRobot robot = new ZecaRobot();
    @PostMapping("Connect")
    public ResponseEntity<String> Connect(@RequestBody String IP){
        if (!robot.Connect(IP))
            return new ResponseEntity<>("Error Connect", HttpStatus.BAD_REQUEST);
        return new ResponseEntity<>(HttpStatus.OK);
    }
    
    @PostMapping("Disconnect")
    public ResponseEntity<String> Disconnect(){
        if (!robot.Disconnect())
            return new ResponseEntity<>("Error Disconnect", HttpStatus.BAD_REQUEST);
        return new ResponseEntity<>(HttpStatus.OK);
    }
    
    @PostMapping("SetSpeakVelocity")
    public ResponseEntity<String> SetSpeakVelocity(@RequestBody String velocity){
        if (!robot.SetSpeakVelocity(velocity))
            return new ResponseEntity<>("Error SetSpeakVelocity", HttpStatus.BAD_REQUEST);
        return new ResponseEntity<>(HttpStatus.OK);
    }
    
    @PostMapping("Speak")
    public ResponseEntity<String> Speak(@RequestBody String message){
        if (!robot.Speak(message))
            return new ResponseEntity<>("Error Speak", HttpStatus.BAD_REQUEST);
        return new ResponseEntity<>(HttpStatus.OK);
    }
    
    @PostMapping("PlayAnimation")
    public ResponseEntity<String> PlayAnimation(@RequestBody String animation){
        if (!robot.PlayAnimation(animation))
            return new ResponseEntity<>("Error PlayAnimation", HttpStatus.BAD_REQUEST);
        return new ResponseEntity<>(HttpStatus.OK);
    }
}