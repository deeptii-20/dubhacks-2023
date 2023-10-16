using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomVillagerCreator : MonoBehaviour
{
    public GameObject villagerPrefab; // The prefab to instantiate
    public int numberOfObjectsToSpawn = 30; // Number of objects to spawn

    void Start()
    {
        for (int i = 0; i < numberOfObjectsToSpawn; i++)
        {
            // Instantiate the object with the random speed
            GameObject spawnedObject = Instantiate(villagerPrefab, transform.position, Quaternion.identity);
            spawnedObject.GetComponent<VillagerController>().InitRandomPathfindingVars();
        }
    }
}
