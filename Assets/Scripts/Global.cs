using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public static void ResetPlayersSpeed(){
        for(int i = 0; i < TotalPlayers; i++){
            PlayersSpeed[i] = BaseSpeed;
        }
    }

    public static  void ResetRound(){
        Round = 0;
        for(int i = 0; i < TotalPlayers; i++){
            PlayersWinStreak[i] = 0;
            PlayersRoundWin[i] = 0;
        }
    }

    public static float AddSpeed(float speed, float num){
        return num > 0 ? (speed+num > MaxSpeed ? speed : speed+num) : (speed+num < MinSpeed ? speed : speed+num);
    }

    public static float GetPlayerSpeed(int playerId){
        return PlayersSpeed[playerId];
    }

    public static void AddPlayerSpeed(int playerId, float num){
        if(num < 0 && !IsActiveDecreaseSpeed) return;
        if(PlayersSpeed[playerId]+num > MaxSpeed && CanResetPlayersSpeed){
            ResetPlayersSpeed();
            return;
        }
        PlayersSpeed[playerId] = AddSpeed(PlayersSpeed[playerId], num);
    }

}



