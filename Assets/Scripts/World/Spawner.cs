using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefabPlayer;
    [SerializeField] GameObject prefabBox;
    [SerializeField] GameObject prefabIa;
    PathFinder pathFinder;

    private void Start()
    {
        pathFinder = GetComponent<PathFinder>();
    }

    public void SpawnEntities(Vector2Int mapSize)
    {
        PathFinder.Node spawnNode = pathFinder.GetClosestNode(new Vector3(0, mapSize.y, 0));
        Instantiate(prefabPlayer, spawnNode.pos, Quaternion.identity);

        spawnNode = pathFinder.GetClosestNode(new Vector3(mapSize.x, 0, 0));
        Instantiate(prefabIa, spawnNode.pos, Quaternion.identity);
    }

}
