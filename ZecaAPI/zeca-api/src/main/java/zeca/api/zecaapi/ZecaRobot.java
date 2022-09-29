package zeca.api.zecaapi;

import com.robosteps.api.core.Robosteps;
import com.robosteps.api.core.RsRobot;
import com.robosteps.api.core.UserSettings;
import org.robokind.api.animation.Animation;


/*

 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

/**
 *
 * @author david.alves
 */

public class ZecaRobot {
    
    private static RsRobot myRobot;
    
    public ZecaRobot(){
    }
    
    public boolean Connect(String IP){
        try {
            UserSettings.setRobotId("myRobot");
            UserSettings.setRobotAddress(IP);
            UserSettings.setSpeechAddress(IP);
            UserSettings.setAnimationAddress(IP);

            myRobot = Robosteps.connectRobot();
            if(myRobot == null) return false;
        }catch(Exception e){
            return false;
        }
        return true;
    }
    
    public boolean IsConnected(){
        return myRobot == null;
    }
    
    public boolean Disconnect(){
        try {
            Robosteps.disconnect();
        }catch(Exception e){
            return false;
        }
        return true;
    }
    
    
    public boolean SetSpeakVelocity(String velocity){
        try{
            myRobot.speak("\\spd=" + velocity + "\\");
        }catch(Exception e){
            return false;
        }
        return true;
    }
    
    public boolean Speak(String message){
        try{
            myRobot.speak(message);
        }catch(Exception e){
            return false;
        }
        return true;
    }
    
    //TODO: Fazer uma lista de animações predefinidas?
    public boolean PlayAnimation(String animation){
        try{
            Animation anm = Robosteps.loadAnimation("animations/" + animation);
            if(anm == null) return false;
            myRobot.playAnimation(anm);
        }catch(Exception e){
            return false;
        }
        return true;
    }
}
