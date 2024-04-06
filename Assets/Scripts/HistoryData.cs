using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace History
{
	public class HistoryData{
 		public List<int> Round;
 		public List<int[]> PlayersRoundWin;
 		public List<int[]> PlayersWinStreak;
 		public List<float[]> PlayersWinningRate;
 		public List<float[]> PlayersEpisodeReward;

		public HistoryData(){
			Round = new List<int>();
			PlayersRoundWin = new List<int[]>();
			PlayersWinStreak = new List<int[]>();
			PlayersWinningRate = new List<float[]>();
			PlayersEpisodeReward = new List<float[]>();
		}


		public void UpdateData(int round, int[] roundWin, int[] winStreak, float[] winningRate,  float[] episodeReward){
			Round.Add(round);
			PlayersRoundWin.Add(roundWin);
			PlayersWinStreak.Add(winStreak);
			PlayersWinningRate.Add(winningRate);
			PlayersEpisodeReward.Add(episodeReward);


		}

        public void PrintData()
        {

            for (int i = 0; i < Round.Count(); i++)
            {
                Debug.Log($"Round: {Round[i]}");
                Debug.Log("Player Round Win:");
                PrintArray(PlayersRoundWin[i]);
                Debug.Log("Player Win Streak:");
                PrintArray(PlayersWinStreak[i]);
                Debug.Log("Player Winning Rate:");
                PrintArray(PlayersWinningRate[i]);
                Debug.Log("Player Episode Reward:");
                PrintArray(PlayersEpisodeReward[i]);
            }


        }

        private void PrintArray<T>(T[] array)
        {
        	string str = "";
            foreach (var item in array)
            {
            	str += $"{item} ";
            }
            Debug.Log(str);
        }
    }

}







