using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeBoxes : MonoBehaviour
{
    private bool haveBox = false;
    private bool canPick = false;
    private bool canPutInTruck = false;
    private int nbrBoxesInTruck = 0;
    
    private Collider2D boxCollider;
    private Spawner spawner;
    private AIStateMachine aiStateMachine;
    
    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        aiStateMachine = FindObjectOfType<AIStateMachine>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box") && !haveBox)
        {
            canPick = true;
            boxCollider = other;
        }
        
        if (other.CompareTag("playerTruck") && haveBox)
            canPutInTruck = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("box"))
            canPick = false;

        if (other.CompareTag("playerTruck"))
            canPutInTruck = false;
    }

    private void Update()
    {
        //Pick
        if (Input.GetKeyDown("e") && canPick)
        {
            spawner.DeleteBoxPos(boxCollider.gameObject.transform.position);
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
