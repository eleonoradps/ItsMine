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
        CHASE_PLAYER,
        FOLLOW_PATH
    }
    
    AIstate state = AIstate.SEARCH_BOX_PATH;

    PlayerController playerController;
    PathFinder pathFinder;
    Transform aiPos;
    Rigidbody2D aiBody;
    Spawner spawner;
    [SerializeField] float speed;
    float distance;
    [SerializeField] float minDistance = 0f;
    bool haveBox = false;
    Vector2 takenBoxPos;
    Vector2 boxPos;
    

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
        playerController = FindObjectOfType<PlayerController>();
        truckNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
    }
    
    void Update()
    {
        switch (state)
        {
            case AIstate.IDLE:
                break;
            case AIstate.SEARCH_BOX_PATH:
                
                CheckAi();
                boxPos = spawner.ReturnRandomBoxPos();
                goalNode = pathFinder.GetClosestNode(new Vector3(boxPos.x, boxPos.y, 0));
                startNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                pathFinder.FindPath(goalNode, startNode);
                currentNode = startNode;
                state = AIstate.FOLLOW_PATH;
                break;
            case AIstate.SEARCH_TRUCK_PATH:
                
                CheckAi();
                goalNode = truckNode;
                startNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                pathFinder.FindPath(goalNode, startNode);
                currentNode = startNode;
                state = AIstate.FOLLOW_PATH;
                break;
            case AIstate.CHASE_PLAYER:
                
                CheckAi();
                CheckDistance();
                break;
            case AIstate.FOLLOW_PATH:
                
                CheckAi();
                CheckDistance();
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

    void CheckDistance()
    {
        
        Vector2 playerPos = playerController.ReturnPlayerPos();
        float distance = pathFinder.DistanceManhattan(aiPos.position, playerPos);
        if (distance <= minDistance && !haveBox)
        {
            pathFinder.DeletePath(goalNode, startNode);
            ChasePlayer();
            state = AIstate.CHASE_PLAYER;
        }
        else
        {
            state = AIstate.FOLLOW_PATH;
        }
    }

    void ChasePlayer()
    {
        Vector2 playerPos = playerController.ReturnPlayerPos();
        aiBody.velocity = (playerPos - (Vector2)aiPos.position).normalized * speed;
    }

    void CheckAi()
    {
        int nbrBoxes = spawner.ReturnNbrBoxes();
        if (nbrBoxes <= 0)
        {
            state = AIstate.IDLE;
        }
    }

    public void PutSearchBoxState()
    {
        state = AIstate.SEARCH_BOX_PATH;
    }

    public Vector2 ReturnBoxPos()
    {
        return boxPos;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box"))
        {
            Destroy(other.gameObject);
            takenBoxPos = other.transform.position;
            spawner.DeleteBoxPos(takenBoxPos);
            haveBox = true;
            pathFinder.DeletePath(goalNode, startNode);
            state = AIstate.SEARCH_TRUCK_PATH;
        }
        
        if (other.CompareTag("AITruck") && haveBox)
        {
            haveBox = false;
            pathFinder.DeletePath(goalNode, startNode);
            state = AIstate.SEARCH_BOX_PATH;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("player") && !haveBox)
        {
            state = AIstate.SEARCH_BOX_PATH;
        }
    }
}