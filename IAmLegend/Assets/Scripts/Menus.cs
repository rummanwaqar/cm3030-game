using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menus : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private void Awake()
    {
        // Pause the game on awake for the intro/tutorial screen
        PauseGame();
    }

    // Update is called once per frame
    void Update()
    {
        // Activate a pause menu when the player press the Escape button
        if( Input.GetKeyDown(KeyCode.Escape) )
        {
            ActivatePauseMenu(pauseMenu);
            PauseGame();
        }
    }

    // Overloading without a key interaction
    public void PauseGame()
    {
        // Pause the game by setting the time scale to 0
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        // Continue the game with the default time scale
        Time.timeScale = 1;
    }

    private void ActivatePauseMenu( GameObject obj )
    {
        obj.SetActive(true);
    }
}
