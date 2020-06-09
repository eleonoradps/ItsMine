using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeBoxes : MonoBehaviour
{
    bool haveBox = false;
    bool canPick = false;
    bool canPutInTruck = false;
    
    Collider2D boxCollider;
    Spawner spawner;
    //AIStateMachine aiStateMachine;

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        //aiStateMachine = FindObjectOfType<AIStateMachine>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box") && !haveBox)
        {
            canPick = true;
            boxCollider = other;
        }

        if (other.CompareTag("playerTruck") && haveBox)
            canPutInTruck = true;
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("box"))
            canPick = false;
        
        if (other.CompareTag("playerTruck"))
        {
            canPutInTruck = false;
        }
    }

    void Update()
    {
        //Pick
        if (Input.GetKeyDown("e") && canPick)
        {
            spawner.DeleteBoxPos(boxCollider.gameObject.transform.position);
            /*Vector2 boxPos = aiStateMachine.ReturnBoxPos();
            if (boxPos == (Vector2)boxCollider.gameObject.transform.position) ;
            {
                aiStateMachine.PutSearchBoxState();
            }*/
            
            Destroy(boxCollider.gameObject);
            haveBox = true;
            canPick = false;
        }
        
        if (Input.GetKeyDown("e") && canPutInTruck)
        {
            haveBox = false;
            canPutInTruck = false;
        }
    }
}
