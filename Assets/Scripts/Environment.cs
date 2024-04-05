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

        if (Input.GetButtonDown("Jump1")){

            Debug.Log("Jump1");
            Transform player2Transform = transform.Find("player2");

            // Check if the player1 node exists
            if (player2Transform != null)
            {
                // Access the script attached to player1 (assuming it's called PlayerScript)
                Player2Movement player2Script = player2Transform.GetComponent<Player2Movement>();

                // Check if the script exists
                if (player2Script != null)
                {
                    player2Script.controller.enabled = !player2Script.controller.enabled;
                }
                else
                {
                    Debug.LogWarning("PlayerScript component not found on player2");
                }
            }
            else
            {
                Debug.LogWarning("Player2 not found under Environment node");
            }
        }

    } 
    
}
