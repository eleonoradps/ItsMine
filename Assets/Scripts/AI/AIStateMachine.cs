using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    enum AIstate
    {
        IDLE,
        SEARCH_PATH,
        TAKE_BOX,
        PUT_BOX_IN_TRUCK,
        ATTACK_PLAYER
    }
    
    AIstate state = AIstate.SEARCH_PATH;

    PathFinder pathFinder;
    Transform aiPos;
    Rigidbody2D aiBody;
    Spawner spawner;
    [SerializeField] float speed;
    float distance;
    bool haveBox = false;

    PathFinder.Node startNode;
    PathFinder.Node goalNode;
    PathFinder.Node currentNode;
    
    
    void Awake()
    { 
        aiPos = GetComponent<Transform>();
        aiBody = GetComponent<Rigidbody2D>();
        pathFinder = FindObjectOfType<PathFinder>();
        spawner = FindObjectOfType<Spawner>();
    }
    
    void Update()
    {
        switch (state)
        {
            case AIstate.IDLE:
                break;
            case AIstate.SEARCH_PATH:
                
                Vector2 boxPos = spawner.ReturnRandomBoxPos();
                startNode = pathFinder.GetClosestNode(new Vector3(boxPos.x, boxPos.y, 0));
                goalNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                pathFinder.FindPath(startNode, goalNode);
                currentNode = goalNode;
                state = AIstate.TAKE_BOX;
                break;
            case AIstate.TAKE_BOX:
                
                FollowPath(startNode);
                break;
            case AIstate.PUT_BOX_IN_TRUCK:
                
                
                break;
            case AIstate.ATTACK_PLAYER:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void FollowPath(PathFinder.Node startNode)
    {
        if (currentNode != startNode)
        {
            aiBody.velocity = (currentNode.parent.pos - (Vector2)aiPos.position).normalized * speed;
        }

        distance = pathFinder.DistanceManhattan(aiPos.position, currentNode.parent.pos);
        if (distance <= 0.1f)
        {
            currentNode = currentNode.parent;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
            Destroy(other.gameObject);
            haveBox = true;
        }
    }
}