using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using History;

public class Environment : MonoBehaviour
{
    public bool IsPlayer1Win = false;
    public bool IsPlayer2Win = false;
    public bool[] PlayersIsEndEpisode = {false, false};

    public void Update()
    {

        bool isAllPlayerEnEpisode = PlayersIsEndEpisode.All(value => value == true);


        if(isAllPlayerEnEpisode){
            Global.UpdateRound();
            Array.Fill<bool>(PlayersIsEndEpisode, false);
        }

        InputListener();
    } 

    public void EndPlayerEpisode(int id, float episodeReward){
        PlayersIsEndEpisode[id] = true;
        Global.PlayersEpisodeReward[id] = episodeReward;
    }

    public void InputListener(){

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
        if (Input.GetKeyDown(KeyCode.P)){
            Global.History.PrintData();
        }

    }
    
}
