using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelPause : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;

    void Update()
    {
        if(isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            ActivatePauseMenu();
        }
    }

    public void ActivatePauseMenu()
    {
        pausePanel.SetActive(true);
        isPaused = true;
    }

    public void DesactivatePauseMenu()
    {
        pausePanel.SetActive(false);
        isPaused = false;
    }
}
