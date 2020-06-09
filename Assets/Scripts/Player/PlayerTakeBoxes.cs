using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeBoxes : MonoBehaviour
{
    bool haveBox = false;
    bool canPick = false;
<<<<<<< HEAD
    bool canPutInTruck = false;
    int nbrBoxesInTruck = 0;
    
    Collider2D boxCollider;
    Spawner spawner;
    EndGame endGame;
    //AIStateMachine aiStateMachine;
=======
    
    Collider2D boxCollider;
    Spawner spawner;
    AIStateMachine aiStateMachine;
>>>>>>> parent of 36a6768... player can put boxes in truck

    void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
<<<<<<< HEAD
        endGame = FindObjectOfType<EndGame>();
        //aiStateMachine = FindObjectOfType<AIStateMachine>();
=======
        aiStateMachine = FindObjectOfType<AIStateMachine>();
>>>>>>> parent of 36a6768... player can put boxes in truck
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box") && !haveBox)
        {
            canPick = true;
            boxCollider = other;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("box"))
            canPick = false;
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
            endGame.SetTruePlayerHaveBox();
        }
<<<<<<< HEAD
        
        if (Input.GetKeyDown("e") && canPutInTruck)
        {
            haveBox = false;
            canPutInTruck = false;
            endGame.SetFalsePlayerHaveBox();
        }
=======
>>>>>>> parent of 36a6768... player can put boxes in truck
    }
}
