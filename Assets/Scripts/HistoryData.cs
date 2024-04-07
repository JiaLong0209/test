using System.Collections.Generic;
using UnityEngine;

namespace History
{
    [System.Serializable]
    public class HistoryData
    {
        public List<int> Round;
        public List<SerializedArray<int>> PlayersRoundWin;
        public List<SerializedArray<int>> PlayersWinStreak;
        public List<SerializedArray<float>> PlayersWinningRate;
        public List<SerializedArray<float>> PlayersEpisodeReward;

        public HistoryData()
        {
            Round = new List<int>();
            PlayersRoundWin = new List<SerializedArray<int>>();
            PlayersWinStreak = new List<SerializedArray<int>>();
            PlayersWinningRate = new List<SerializedArray<float>>();
            PlayersEpisodeReward = new List<SerializedArray<float>>();
        }

        public void UpdateData(int round, int[] roundWin, int[] winStreak, float[] winningRate, float[] episodeReward)
        {
            Round.Add(round);
            PlayersRoundWin.Add(new SerializedArray<int>(roundWin));
            PlayersWinStreak.Add(new SerializedArray<int>(winStreak));
            PlayersWinningRate.Add(new SerializedArray<float>(winningRate));
            PlayersEpisodeReward.Add(new SerializedArray<float>(episodeReward));
        }

        public void PrintData()
        {
            for (int i = 0; i < Round.Count; i++)
            {
                Debug.Log($"Round: {Round[i]}");
                Debug.Log("Player Round Win:");
                PrintArray(PlayersRoundWin[i].array);
                Debug.Log("Player Win Streak:");
                PrintArray(PlayersWinStreak[i].array);
                Debug.Log("Player Winning Rate:");
                PrintArray(PlayersWinningRate[i].array);
                Debug.Log("Player Episode Reward:");
                PrintArray(PlayersEpisodeReward[i].array);
            }
        }

        public void PrintJson()
        {
            Debug.Log(GetJsonData());
        }

        public void SaveToJson(){
            JsonManager jsonManager = new JsonManager(); // Instantiate JsonManager
            jsonManager.SaveToJson(this);
        }

        public string GetJsonData()
        {
            return JsonUtility.ToJson(this);
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

    [System.Serializable]
    public class SerializedArray<T>
    {
        public T[] array;
    	public T player1;
    	public T player2;

        public SerializedArray(T[] array)
        {
        	this.array = array;
            this.player1 = array[0];
            this.player2 = array[1];
        }
    }
}
