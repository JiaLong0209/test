using UnityEngine;


public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject ObjectToSpawn; 
    private GameObject[] SpawnedObjects; 
    public Vector3 MinPosition = new Vector3(-Global.StageSize.x/2, 0,-Global.StageSize.z/2); 
    public Vector3 MaxPosition = new Vector3(Global.StageSize.x/2, 0,Global.StageSize.z/2);
    public Transform Parent;
    

    public void UpdateAndSpawnObjects()
    {
        Debug.Log("Update Spawn Objects");
        DestroyOldObjects();

        int count = Random.Range(5, 11);
        SpawnedObjects = new GameObject[count];
        Debug.Log("Global.StageSize.z: " + Global.StageSize.z);
        Debug.Log($"{MinPosition.z}  {MaxPosition.z}");
        
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(MinPosition.x, MaxPosition.x),
                                                Random.Range(MinPosition.y, MaxPosition.y),
                                                Random.Range(-30, 30));
            GameObject obj = Instantiate(ObjectToSpawn, Parent.position + spawnPosition, Quaternion.identity);
            obj.transform.SetParent(Parent);
            SpawnedObjects[i] = obj;
        }
    }

    private void DestroyOldObjects()
    {
        if (SpawnedObjects != null)
        {
            for (int i = 0; i < SpawnedObjects.Length; i++)
            {
                if (SpawnedObjects[i] != null)
                {
                    Destroy(SpawnedObjects[i]);
                }
            }
        }
    }
}
