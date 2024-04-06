using System;
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
 		public int index;

		public HistoryData(){
			Round = new List<int>();
			PlayersRoundWin = new List<int[]>();
			PlayersWinStreak = new List<int[]>();
			PlayersWinningRate = new List<float[]>();
			PlayersEpisodeReward = new List<float[]>();
		}

		public void UpdateData(int id, int round, int playerRoundWin, int playerWinStreak, float playerEpisodeReward){
			

		}

	}

}







