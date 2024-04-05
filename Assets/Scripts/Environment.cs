using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment : MonoBehaviour
{
    public bool IsPlayer1Win = false;
    public bool IsPlayer2Win = false;


    public void Update()
    {
 
        if (Input.GetKeyDown(KeyCode.R)){
            Global.ResetRound();
            Debug.Log($"Reset round!");
        }
        if (Input.GetKeyDown(KeyCode.Z)){
            Global.Mode = 0;
            Debug.Log($"Change game mode: {Global.Mode}");
        }
        if (Input.GetKeyDown(KeyCode.X)){
            Global.Mode = 1;
            Debug.Log($"Change game mode: {Global.Mode}");
        }
        if (Input.GetKeyDown(KeyCode.A)){
            Global.BaseSpeed += -1;
            Debug.Log($"Decrease base speed: {Global.BaseSpeed}");
        }
        if (Input.GetKeyDown(KeyCode.S)){
            Global.BaseSpeed += 1;
            Debug.Log($"Increase base speed: {Global.BaseSpeed}");
        }
        if (Input.GetKeyDown(KeyCode.Q)){
            Global.ResetPlayersSpeed();
            Debug.Log($"Reset players speed!");
        }

    } 
    
}
