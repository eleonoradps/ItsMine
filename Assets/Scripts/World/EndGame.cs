using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    private Spawner spawner;

    private void Start()
    {
        spawner = GetComponent<Spawner>();
    }

    private void Update()
    {
        int nbrBoxes = spawner.ReturnNbrBoxes();

        if (nbrBoxes <= 0)
        {
            Time.timeScale = 0;
        }
    }
}
