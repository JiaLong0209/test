using System.IO;
using System.Linq;
using UnityEngine;

namespace History
{
    public class JsonManager : MonoBehaviour
    {
        private string FilePath;
        public bool IsUsePersistentDataPath = false;
        public string DirectoryPath = "History";

        public void SaveToJson(HistoryData data)
        {
            string fileName = $"Round_{data.Round.Count()}.json";
            try
            {
                FilePath = Path.Combine(Application.dataPath, DirectoryPath);

                if (!Directory.Exists(FilePath))
                {
                    Directory.CreateDirectory(FilePath);
                }

                FilePath = Path.Combine(FilePath, fileName);

                string jsonData = JsonUtility.ToJson(data);

                File.WriteAllText(FilePath, jsonData);

                Debug.Log($"Json data saved: {FilePath}");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to save JSON data to directory {FilePath}: {e.Message}");

                // Retry saving to the Assets directory
                FilePath = Path.Combine(Application.dataPath, fileName);

                string jsonData = JsonUtility.ToJson(data);
                File.WriteAllText(FilePath, jsonData);

                Debug.Log($"Json data saved to Assets directory: {FilePath}");
            }
        }
    }
}
