package zeca.api.zecaapi;


import org.robokind.api.animation.Animation;
import org.robokind.api.animation.messaging.RemoteAnimationPlayerClient;
import org.robokind.api.animation.player.AnimationJob;
import org.robokind.api.motion.messaging.RemoteRobot;
import org.robokind.api.speech.SpeechJob;
import org.robokind.api.speech.messaging.RemoteSpeechServiceClient;
import org.robokind.client.basic.Robokind;

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
    
    private static RemoteRobot myRobot;
    private static RemoteAnimationPlayerClient myPlayer;
    private static RemoteSpeechServiceClient mySpeaker;
    
    public ZecaRobot(){
    }
    
    public boolean Connect(String IP){
        /*
        try {
            
            return true;
            UserSettings.setRobotId("myRobot");
            UserSettings.setRobotAddress(IP);
            UserSettings.setSpeechAddress(IP);
            UserSettings.setAnimationAddress(IP);

            myRobot = Robokind.connectRobot();
            myPlayer = Robokind.connectAnimationPlayer();
            mySpeaker = Robokind.connectSpeechService();
            if(myRobot == null) return false;
        }catch(Exception e){
            return false;
        }
        */
        return true;
    }
    
    public boolean Disconnect(){
        try {
            Robokind.disconnect();
        }catch(Exception e){
            return false;
        }
        return true;
    }
    public boolean SetSpeakVelocity(String velocity){
        try{
            SpeechJob job = mySpeaker.speak("\\spd=" + velocity + "\\");
        }catch(Exception e){
            return false;
        }
        return true;
    }
    public boolean Speak(String message){
        try{
            SpeechJob job = mySpeaker.speak(message);
        }catch(Exception e){
            return false;
        }
        return true;
    }
    
    //TODO: Fazer uma lista de animações predefinidas?
    public boolean PlayAnimation(String animation){
        try{
            Animation anim = Robokind.loadAnimation("C:\\Users\\David.Alves\\Downloads\\TESE\\ZECA2\\DemoZECA\\DEMO\\V11\\01");
            //AnimationJob animJob = myPlayer.playAnimation(anim);
            AnimationJob job = myPlayer.playAnimation(anim);
            Robokind.sleep(500 + anim.getLength());
            
        }catch(Exception e){
            return false;
        }
        return true;
    }
}
