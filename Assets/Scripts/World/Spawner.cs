using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [SerializeField] GameObject prefabPlayer;
    [SerializeField] GameObject prefabBox;
    [SerializeField] GameObject prefabIa;
    [SerializeField] int maxBoxes;
    [SerializeField] Tile groundTile;
    PathFinder pathFinder;
    Vector2 playerStartpos;
    Vector2 aiStartPos;
    AIStateMachine aiStateMachine;

    List<Vector2> boxesPos;

    private void Awake()
    {
        pathFinder = GetComponent<PathFinder>();
        boxesPos = new List<Vector2>();
        aiStateMachine = FindObjectOfType<AIStateMachine>();
    }

    public void SpawnEntities(Vector2Int mapSize)
    {
        //spawn player on the most top left node
        PathFinder.Node spawnNode = pathFinder.GetClosestNode(new Vector3Int(0, mapSize.y, 0));
        playerStartpos = spawnNode.pos;
        Instantiate(prefabPlayer, playerStartpos, Quaternion.identity);

        //spawn AI on the most down right node
        spawnNode = pathFinder.GetClosestNode(new Vector3Int(mapSize.x, 0, 0));
        aiStartPos = spawnNode.pos;
        Instantiate(prefabIa, aiStartPos, Quaternion.identity);

        //spawn boxes
        for (int i = 0; i < maxBoxes; i++)
        {
            int spawnX = Random.Range(0, mapSize.x);
            int spawnY = Random.Range(0, mapSize.y);
            Vector3Int spawnPos = new Vector3Int(spawnX, spawnY, 0);
            bool canSpawn = true;


            //block spawn in player and AI position
            spawnNode = pathFinder.GetClosestNode(spawnPos);
            if (spawnNode.pos == playerStartpos
                || spawnNode.pos == aiStartPos)
            {
                canSpawn = false;
            }

            //block spawn in other boxes position
            foreach (Vector2 boxPos in boxesPos)
            {
                if (spawnNode.pos == boxPos)
                {
                    canSpawn = false;
                    break;
                }
            }

            if (canSpawn)
            {
                boxesPos.Add(spawnNode.pos);
                Instantiate(prefabBox, boxesPos[i], Quaternion.identity);
            }
            else
            {
                i--;
            }
        }
    }

    public Vector2 ReturnRandomBoxPos()
    {
        int boxIndex = Random.Range(0, maxBoxes - 1);
        return boxesPos[boxIndex];
    }

    public void DeleteBoxPos(Vector2 takenBoxPos)
    {
        Vector2 indexBox = new Vector2(0, 0);
        for (int i = 0; i < boxesPos.Count; i++)
        {
            if (takenBoxPos == boxesPos[i])
            {
                indexBox = boxesPos[i];
            }
        }
        boxesPos.Remove(indexBox);
    }
}