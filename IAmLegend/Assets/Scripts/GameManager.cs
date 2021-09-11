using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject dog;
    [SerializeField] private GameObject directionalLight;

    [Header("Zombie Waves Spawn Settings")]
    [SerializeField] private GameObject zombieWave;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float spawnRateMin;
    [SerializeField] private float spawnRateMax;

    [Header("Scoring")]
    [SerializeField] private TMP_Text scoreTMP;
    [SerializeField] private TMP_Text scoreDeadScreenTMP;
    [SerializeField] private TMP_Text scoreWinScreenTMP;
    [SerializeField] private GameObject deadScreen;
    [SerializeField] private GameObject winScreen;
    private int score;
    private bool scoreCalculated;
    private float timer;
    private float timeMultipler;
    Vector3 playerCurPosition;
    Vector3 playerLastPosition;

    HealthSystem playerHealth;
    HealthSystem dogHealth;
    Light lightSource;
    // Start is called before the first frame update
    void Start()
    {
        score = 0;
        scoreCalculated = false;
        timeMultipler = 1;
        
        // Spawn a new wave every time cycle to increase difficulty
        float randomNum = Random.Range(spawnRateMin, spawnRateMax);
        InvokeRepeating("SpawnZombies", 0f, randomNum);

        playerHealth = player.GetComponents<HealthSystem>()[0];
        dogHealth = dog.GetComponents<HealthSystem>()[0];
        lightSource = directionalLight.GetComponents<Light>()[0];
    }

    void Update()
    {
        // Update the timer
        timer += Time.deltaTime;
        timer *= timeMultipler;
        // Update score in the UI
        scoreTMP.SetText(score.ToString());
        Sunrise();
        EndGame();
    }

    void SpawnZombies()
    {
        // If the player was Idle, don't spawn new waves
        playerCurPosition = player.transform.position;
        if( playerCurPosition != playerLastPosition )
        {
            // Spawn zombie wave in front of the player
            Vector3 playerPos = player.transform.position;
            Vector3 playerDirection = player.transform.forward;
            Quaternion playerRotation = player.transform.rotation;

            Vector3 spawnPos = playerPos + playerDirection * spawnDistance;
            Instantiate(zombieWave, spawnPos, playerRotation);
        }
        playerLastPosition = playerCurPosition;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    private void EndGame()
    {
        // If the player is dead (lose) 
        if(playerHealth.GetHealth() <= 0)
        {
            // If the dog is alive, triple score
            if(dogHealth.GetHealth() > 0 && !scoreCalculated)
            {
                score *= 3;
                scoreCalculated = true;
            }
            scoreDeadScreenTMP.SetText(score.ToString());
            deadScreen.SetActive(true);
            Time.timeScale = 0;
            return;
        }
        // If it's 6:00 AM (won)
        if(timer >= 360)
        {
            // If the dog is alive, triple score
            if(dogHealth.GetHealth() > 0 && !scoreCalculated)
            {
                score *= 3;
                scoreCalculated = true;
            }
            scoreWinScreenTMP.SetText(score.ToString());
            winScreen.SetActive(true);
            Time.timeScale = 0;
            return;
        }
    }

    private void Sunrise()
    {
        // Sunrise from 5:20 AM to 6:00 AM
        
        int timeMinutes = Mathf.FloorToInt(timer / 60F);
        int timeSeconds = Mathf.FloorToInt(timer - timeMinutes * 60);
        
        // If 5:30AM, increase sunlight and lerp to a white light source
        if(lightSource.intensity < 1  && timeMinutes >= 5 &&  timeSeconds > 20)
        {
            lightSource.intensity += 0.001f;
            lightSource.color =  Color.Lerp(lightSource.color, Color.white, Mathf.PingPong(Time.time, 1));
        }
    }
    public float GetTimer()
    {
        return timer;
    } 

    public void Replay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
