using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;

    // Update is called once per frame
    void Update()
    {
        // If the player clicked Escape, call the menuLogic function
        if( Input.GetKeyDown(KeyCode.Escape) )
            menuLogic();
    }

    public void menuLogic()
    {
        // If time is ticking, pause the game by setting the time scale to 0
        if( Time.timeScale > 0 )
        {
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
        else
        {
            // Continue the game
            Time.timeScale = 1;
            pauseMenu.SetActive(false);
        }
    }
}
