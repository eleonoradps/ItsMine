using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    bool playerWin = false;
    bool aiWin = false;
    int nbrBoxPlayer = 0;
    int nbrBoxAi = 0;
    int nbrBoxes = 0;
    bool playerHaveBox = false;
    bool aiHaveBox = false;
    Spawner spawner;

    void Awake()
    {
        spawner = GetComponent<Spawner>();
    }

    void Update()
    {
        if (nbrBoxPlayer > nbrBoxAi)
        {
            playerWin = true;
        }
        else if (nbrBoxAi > nbrBoxPlayer)
        {
            aiWin = true;
        }

        nbrBoxes = spawner.ReturnNbrBoxes();

        if (nbrBoxes <= 0 && !playerHaveBox && !aiHaveBox)
        {
            ShowWinner();
        }
    }

    void ShowWinner()
    {
        Time.timeScale = 0;
        
        /*if (playerWin)
            
        else if (aiWin)
            
        else  */
    }

    public void SetTruePlayerHaveBox()
    {
        playerHaveBox = true;
    }
    
    public void SetFalsePlayerHaveBox()
    {
        playerHaveBox = false;
    }
    
    public void SetTrueAiHaveBox()
    {
        aiHaveBox = true;
    }
    
    public void SetFalseAiHaveBox()
    {
        aiHaveBox = false;
    }
}
