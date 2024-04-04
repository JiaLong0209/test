using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Global {
    public static int mode = 0;
    public static int round = 0;
    public static int round_win = 0;
    public static int winStreak = 0;

    public static  void resetRound(){
        Global.round = 0;
        Global.round_win = 0;
        Global.winStreak = 0;
    }

    public static void update(){
        if (Input.GetKeyDown(KeyCode.R)){
            Debug.Log("Key Down R");
            Global.resetRound();
        }
        if (Input.GetKeyDown(KeyCode.Z)){
            Debug.Log("Key Down Z");
            Global.mode = 0;
        }
        if (Input.GetKeyDown(KeyCode.X)){
            Debug.Log("Key Down X");
            Global.mode = 1;
        }
    }
}

public class GlobalClass : MonoBehaviour{


}
