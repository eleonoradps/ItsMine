﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Video;
using Random = UnityEngine.Random;



public class PathFinder : MonoBehaviour
{
    
    public enum NodeType
    {
        FREE,
        NOT_FREE,
        PATH
    }
    
    public class Node
    {
        public float gCost;
        public float hCost;
        public List<Vector2Int> neighbors;
        public Node parent;
        public Vector2 pos;
        public NodeType type;

        public float fCost
        {
            get { return gCost + hCost; }// remplacer par une fonction
        }
    }

    Spawner spawner;

    Node currentNode;
    Node goalNode;
    Vector3 goalPos;
    
    [SerializeField] Tilemap groundTileMap;
    [SerializeField] Tilemap wallTileMap;

    Node[,] nodes;

    [SerializeField] Vector2Int sizeTilemap;
    
    void Start()
    {
        spawner = GetComponent<Spawner>();
        NodeGen();
    }

    void NodeGen()
    {
        //Node generation
        nodes = new Node[sizeTilemap.x, sizeTilemap.y];

        Vector2Int offset = (Vector2Int) wallTileMap.origin;

        for (int x = 0 + offset.x; x < sizeTilemap.x + offset.x; x++)
        {
            for (int y = 0 + offset.y; y < sizeTilemap.y + offset.y; y++)
            {
                Vector2Int nodeIndex = new Vector2Int(x - offset.x, y - offset.y);
                nodes[nodeIndex.x, nodeIndex.y] = new Node();
                nodes[nodeIndex.x, nodeIndex.y].pos = new Vector2(x, y) + (Vector2) groundTileMap.cellSize / 2.0f;

                nodes[nodeIndex.x, nodeIndex.y].type = NodeType.NOT_FREE;

                if (groundTileMap.HasTile(new Vector3Int(x, y, 0)))
                {
                    if (!wallTileMap.HasTile(new Vector3Int(x, y, 0)))
                    {
                        nodes[nodeIndex.x, nodeIndex.y].type = NodeType.FREE;
                    }
                }
            }
        }
        
        //Set neighbors
        BoundsInt bounds = new BoundsInt(-1, -1, 0, 3, 3, 1);

        for (int x = 0; x < sizeTilemap.x; x++)
        {
            for (int y = 0; y < sizeTilemap.y; y++)
            {
                if (nodes[x, y].type == NodeType.NOT_FREE) continue;

                nodes[x, y].neighbors = new List<Vector2Int>();

                foreach (Vector3Int index in bounds.allPositionsWithin)
                {
                    if (x + index.x < 0 || x + index.x >= sizeTilemap.x) continue;
                    if (y + index.y < 0 || y + index.y >= sizeTilemap.y) continue;

                    if (index.x == 0 && index.y == 0) continue;

                    if (nodes[x + index.x, y].type == NodeType.FREE &&
                        nodes[x, y + index.y].type == NodeType.FREE)
                    {
                        if (nodes[x + index.x, y + index.y].type == NodeType.NOT_FREE) continue;

                        Vector2 dir = nodes[x + index.x, y + index.y].pos - nodes[x, y].pos;

                        if (!Physics2D.Raycast(nodes[x, y].pos, dir.normalized,
                            Vector2.Distance(nodes[x + index.x, y + index.y].pos, nodes[x, y].pos)))
                        {
                            nodes[x, y].neighbors.Add(new Vector2Int(x + index.x, y + index.y));
                        }
                    }
                }
            }
        }
        spawner.SpawnEntities(sizeTilemap);
    }

    public float DistanceManhattan(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Abs(pos2.x - pos1.x) + Mathf.Abs(pos2.y - pos1.y);
    }

    float Distance(Vector3 pos1, Vector3 pos2)
    {
        return Mathf.Abs(Mathf.Sqrt((Mathf.Pow(pos2.x - pos1.x, 2)) + (Mathf.Pow(pos2.y - pos1.y, 2))));
    }

    public Node GetClosestNode(Vector3 position)
    {
        float minDistance = Mathf.Infinity;
        Node closestNode = null;

        foreach (Node node in nodes)
        {
            if (node == null) continue;
            if (node.type != NodeType.FREE) continue;

            float distance = DistanceManhattan(position, node.pos);

            if (distance > minDistance) continue;
            minDistance = distance;
            closestNode = node;
        }

        return closestNode;
    }


    public void FindPath(Node startNode, Node endNode)
    {
        //Find path with A* algorithm
        List<Node> openSet = new List<Node>();
        List<Node> closedSet = new List<Node>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Node node = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost <= node.fCost)
                {
                    if (openSet[i].hCost < node.hCost)
                        node = openSet[i];
                }
            }

            openSet.Remove(node);
            closedSet.Add(node);

            if (node == endNode)
            {
                RetracePath(startNode, endNode);
                return;
            }


            foreach (Vector2Int neighborPos in node.neighbors)
            {
                Node neighbor = nodes[neighborPos.x, neighborPos.y];
                if (neighbor == null) continue;
                if (neighbor.type != NodeType.FREE || closedSet.Contains(neighbor)) continue;

                float newCostToNeighbour = node.gCost + DistanceManhattan(node.pos, neighbor.pos);
                if (newCostToNeighbour < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbour;
                    neighbor.hCost = Distance(neighbor.pos, endNode.pos);
                    neighbor.parent = node;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
    }

    void RetracePath(Node startNode, Node endNode)
    {
        //Set path nodes
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            currentNode.type = NodeType.PATH;
            currentNode = currentNode.parent;
        }

        startNode.type = NodeType.PATH;
    }
    
    public void DeletePath(Node startNode, Node endNode)
    {
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            currentNode.type = NodeType.FREE;
            currentNode = currentNode.parent;
        }
        startNode.type = NodeType.FREE;
    }


    void OnDrawGizmos()
    {
        if (nodes == null) return;

        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                if (nodes == null) continue;
                if (nodes[x, y].type == NodeType.FREE)
                {
                    Gizmos.color = Color.green;
                }
                else
                {
                    Gizmos.color = Color.red;
                }
                
                if (nodes[x, y] == goalNode)
                {
                    Gizmos.color = Color.black;
                }
                else if (nodes[x, y].type == NodeType.PATH)
                {
                    Gizmos.color = Color.magenta;
                }


                Gizmos.DrawWireSphere(nodes[x, y].pos, 0.4f);


                if (nodes[x, y].neighbors == null) continue;

                foreach (Vector2Int index in nodes[x, y].neighbors)
                {
                    if (nodes[index.x, index.y].type == NodeType.PATH && nodes[x, y].type == NodeType.PATH)
                    {
                        Gizmos.color = Color.magenta;
                    }
                    else
                    {
                        Gizmos.color = Color.green;
                    }
                    Gizmos.DrawLine(nodes[x, y].pos, nodes[index.x, index.y].pos);
                }
            }
        }
    }
}