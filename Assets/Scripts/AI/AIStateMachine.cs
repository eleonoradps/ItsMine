using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    enum AIstate
    {
        IDLE,
        SEARCH_BOX_PATH,
        SEARCH_TRUCK_PATH,
        SEARCH_PLAYER_PATH,
        FOLLOW_PATH
    }
    
    AIstate state = AIstate.SEARCH_BOX_PATH;

    PathFinder pathFinder;
    Transform aiPos;
    Rigidbody2D aiBody;
    Spawner spawner;
    [SerializeField] float speed;
    float distance;
    bool haveBox = false;
    Vector2 takenBoxPos;
    

    PathFinder.Node goalNode;
    PathFinder.Node startNode;
    PathFinder.Node currentNode;
    PathFinder.Node truckNode;
    
    
    void Awake()
    { 
        aiPos = GetComponent<Transform>();
        aiBody = GetComponent<Rigidbody2D>();
        pathFinder = FindObjectOfType<PathFinder>();
        spawner = FindObjectOfType<Spawner>();
        truckNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
    }
    
    void Update()
    {
        switch (state)
        {
            case AIstate.IDLE:
                break;
            case AIstate.SEARCH_BOX_PATH:
                
                Vector2 boxPos = spawner.ReturnRandomBoxPos();
                goalNode = pathFinder.GetClosestNode(new Vector3(boxPos.x, boxPos.y, 0));
                startNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                pathFinder.FindPath(goalNode, startNode);
                currentNode = startNode;
                state = AIstate.FOLLOW_PATH;
                break;
            case AIstate.SEARCH_TRUCK_PATH:
                
                goalNode = truckNode;
                startNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                pathFinder.FindPath(goalNode, startNode);
                currentNode = startNode;
                state = AIstate.FOLLOW_PATH;
                break;
            case AIstate.SEARCH_PLAYER_PATH:
                
                /*Vector2 boxPos = spawner.ReturnRandomBoxPos();
                goalNode = pathFinder.GetClosestNode(new Vector3(boxPos.x, boxPos.y, 0));
                startNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                pathFinder.FindPath(goalNode, startNode);
                currentNode = startNode;
                state = AIstate.FOLLOW_PATH;*/
                break;
            case AIstate.FOLLOW_PATH:
                
                FollowPath(goalNode);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    void FollowPath(PathFinder.Node goalNode)
    {
        if (currentNode != goalNode)
        {
            aiBody.velocity = (currentNode.parent.pos - (Vector2)aiPos.position).normalized * speed;
        }

        distance = pathFinder.DistanceManhattan(aiPos.position, currentNode.parent.pos);
        if (distance <= 0.1f)
        {
            currentNode = currentNode.parent;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
            Destroy(other.gameObject);
            takenBoxPos = other.transform.position;
            haveBox = true;
            pathFinder.DeletePath(goalNode, startNode);
            state = AIstate.SEARCH_TRUCK_PATH;
        }
        else if (other.CompareTag("AITruck") && haveBox)
        {
            haveBox = false;
            pathFinder.DeletePath(goalNode, startNode);
            state = AIstate.SEARCH_BOX_PATH;
        }
    }

    public Vector2 returnTakenBoxPos()
    {
        return takenBoxPos;
    }
}