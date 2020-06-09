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
        FOLLOW_PATH
    }
    
    AIstate state = AIstate.SEARCH_BOX_PATH;
    
    private PathFinder pathFinder;
    private Transform aiPos;
    private Rigidbody2D aiBody;
    private Spawner spawner;
    [SerializeField] private float speed;
    private float distance;
    private bool haveBox = false;
    private Vector2 takenBoxPos;
    private Vector2 boxPos;
    

    private PathFinder.Node goalNode;
    private PathFinder.Node startNode;
    private PathFinder.Node currentNode;
    private PathFinder.Node truckNode;
    
    
    private void Awake()
    { 
        aiPos = GetComponent<Transform>();
        aiBody = GetComponent<Rigidbody2D>();
        pathFinder = FindObjectOfType<PathFinder>();
        spawner = FindObjectOfType<Spawner>();
        truckNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
    }
    
    private void Update()
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
            case AIstate.FOLLOW_PATH:
                
                CheckAi();
                FollowPath(goalNode);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FollowPath(PathFinder.Node goalNode)
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

    private void CheckAi()
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

    private void OnTriggerEnter2D(Collider2D other)
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

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("player") && !haveBox)
        {
            state = AIstate.SEARCH_BOX_PATH;
        }
    }
}