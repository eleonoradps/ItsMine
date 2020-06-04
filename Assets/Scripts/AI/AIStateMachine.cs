using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIStateMachine : MonoBehaviour
{
    enum AIstate
    {
        IDLE,
        TAKE_BOX,
        PUT_BOX_IN_TRUCK,
        ATTACK_PLAYER
    }
    
    AIstate state = AIstate.TAKE_BOX;

    PathFinder pathFinder;
    Transform aiPos;
    Spawner spawner;
    
    void Awake()
    { 
        aiPos = GetComponent<Transform>(); 
        pathFinder = FindObjectOfType<PathFinder>();
        spawner = FindObjectOfType<Spawner>();
    }
    
    void Update()
    {
        switch (state)
        {
            case AIstate.IDLE:
                break;
            case AIstate.TAKE_BOX:

                PathFinder.Node startNode = pathFinder.GetClosestNode(new Vector3(aiPos.position.x, aiPos.position.y, 0));
                Vector2 boxPos = spawner.ReturnRandomBoxPos();
                PathFinder.Node goalNode = pathFinder.GetClosestNode(new Vector3(boxPos.x, boxPos.y, 0));
                pathFinder.FindPath(startNode, goalNode);
                //FollowPath();
                break;
            case AIstate.PUT_BOX_IN_TRUCK:
                
                break;
            case AIstate.ATTACK_PLAYER:
                
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    /*void FollowPath()
    {
        
    }*/
}
