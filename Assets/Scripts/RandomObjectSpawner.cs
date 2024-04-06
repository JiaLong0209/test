using UnityEngine;

public class RandomObjectSpawner : MonoBehaviour
{
    public GameObject objectToSpawn; 
    private GameObject[] spawnedObjects; 
    public Vector3 minPosition; 
    public Vector3 maxPosition;
    

    public void UpdateAndSpawnObjects()
    {
        DestroyOldObjects();

        int count = Random.Range(5, 11);
        spawnedObjects = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            Vector3 spawnPosition = new Vector3(Random.Range(minPosition.x, maxPosition.x),
                                                5,
                                                Random.Range(minPosition.z, maxPosition.z));
            spawnedObjects[i] = Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }

    private void DestroyOldObjects()
    {
        if (spawnedObjects != null)
        {
            for (int i = 0; i < spawnedObjects.Length; i++)
            {
                if (spawnedObjects[i] != null)
                {
                    Destroy(spawnedObjects[i]);
                }
            }
        }
    }
}
