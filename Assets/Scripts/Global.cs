using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;
using History;

public static class Global {
    public static int TotalPlayers = 2;
    public static int Mode = 0;
    public static int Round = 0;

    public static float BaseSpeed = 30.0f;
    public static float MinSpeed = 8.0f;
    public static float MaxSpeed = 200.0f;

    public static float SpeedBonus = 1f;

    public static bool IsActiveSpeedObservation = true;
    public static bool IsActiveDecreaseSpeed = true;
    public static bool CanResetPlayersSpeed = true;

    public static float[] PlayersSpeed = {15.0f, 15.0f};
    public static int[] PlayersRoundWin = {0,0};
    public static int[] PlayersWinStreak = {0,0};
    public static float[] PlayersWinningRate = {0.0f, 0.0f};
    public static float[] PlayersEpisodeReward = {0.0f, 0.0f};

    public static HistoryData History = new HistoryData();


    public static void ResetPlayersSpeed(){
        Array.Fill<float>(PlayersSpeed, BaseSpeed);
    }

    public static  void ResetRound(){
        Round = 0;
        Array.Fill<int>(PlayersWinStreak, 0);
        Array.Fill<int>(PlayersRoundWin, 0);
    }

    public static float AddSpeed(float speed, float num){
        return num > 0 ? (speed+num >= MaxSpeed ? speed : speed+num) : (speed+num <= MinSpeed ? speed : speed+num);
    }

    public static float GetPlayerSpeed(int id){
        return PlayersSpeed[id];
    }

    public static void AddPlayerSpeed(int id, float num){
        if(num < 0 && !IsActiveDecreaseSpeed) return;
        if(PlayersSpeed[id]+num > MaxSpeed && CanResetPlayersSpeed){
            ResetPlayersSpeed();
            return;
        }
        PlayersSpeed[id] = AddSpeed(PlayersSpeed[id], num);
    }

    public static void UpdateRound(){
        Round += 1;
        for(int id = 0; id < TotalPlayers; id++){
            UpdatePlayerWinningRate(id);
        }
        UpdateHistory();
        PrintInfo();
    }

    public static void UpdateHistory(){
        History.UpdateData(round: Round,
                           roundWin: (int[])PlayersRoundWin.Clone(),
                           winStreak: (int[])PlayersWinStreak.Clone(),
                           winningRate: (float[])PlayersWinningRate.Clone(),
                           episodeReward: (float[])PlayersEpisodeReward.Clone() );
    }

    public static void UpdatePlayerWinningRate(int id){
        if(Round != 0){
            PlayersWinningRate[id] = (float)Math.Round(((float)PlayersRoundWin[id] / Round) * 100, 2);
        }
    }

    public static void PrintInfo(){
        string str = $"-- Round {Round} --";
        for(int id = 0; id < TotalPlayers; id++){
            str += $"Player{id+1}: Episode reward: {PlayersEpisodeReward[id]} |  Win: {PlayersRoundWin[id]} | Win rate: {PlayersWinningRate[id]}% | Win streak: {PlayersWinStreak[id]} | Speed: {GetPlayerSpeed(id)}\n";
        }
        Debug.Log(str);

        if(Round % 10 == 0){
            Global.History.PrintData();
        }
    }

    public static void PrintRoundInfo(int id, float cumulativeReward){
        Debug.Log($"Player{id+1}: Current episode reward: {cumulativeReward}\nRound: {Round}  Win: {PlayersRoundWin[id]}  Win rate: {PlayersWinningRate[id]}% Win streak: {PlayersWinStreak[id]}");
    }

    public static void PrintPlayerSpeed(int id, float speedBonus) {
        Debug.Log($"Player{id + 1} Speed({(speedBonus >= 0 ? "+" : "")}{speedBonus}): {GetPlayerSpeed(id)}");
    }

}



