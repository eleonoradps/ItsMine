using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTakeAndDropObjects : MonoBehaviour
{
    bool haveBox = false;
    bool canPick = false;
    
    
    [SerializeField] Transform firePosition;
    [SerializeField] GameObject boxPrefab;
    
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("box") && !haveBox)
            canPick = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("box"))
            canPick = false;
    }

    void Update()
    {
        
        //drop
        if (haveBox && Input.GetKeyDown("e"))
        {
            haveBox = false;
            GameObject box = Instantiate(boxPrefab, firePosition.position, boxPrefab.transform.rotation);
        }

        //Pick
        if (Input.GetKeyDown("e") && canPick)
        {
            GameObject one = GameObject.FindGameObjectWithTag("box");
            Destroy(one);
            haveBox = true;
            canPick = false;
        }
    }
}
