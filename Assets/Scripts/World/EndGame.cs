using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    Spawner spawner;

    void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    void Update()
    {
        int nbrBoxes = spawner.ReturnNbrBoxes();

        if (nbrBoxes <= 0)
        {
            Time.timeScale = 0;
        }
    }
}
