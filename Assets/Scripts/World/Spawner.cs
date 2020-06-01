using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefabPlayer;
    [SerializeField] GameObject prefabBox;
    [SerializeField] GameObject prefabIa;

    public void SpawnPlayer(Vector2Int mapSize)
    {
        PathFinder pathFinder = GetComponent<PathFinder>();
        PathFinder.Node spawnPos = pathFinder.GetClosestNode(new Vector3(0, mapSize.y, 0));
        Instantiate(prefabPlayer, spawnPos.pos, Quaternion.identity);
    }

    public void BoxesSpawner()
    {
        
    }

    public void IaSpawner()
    {
        
    }

}
