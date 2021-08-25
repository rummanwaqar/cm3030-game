using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [Header("Zombie Waves Spawn Settings")]
    [SerializeField] private GameObject zombieWave;
    [SerializeField] private float spawnDistance;
    [SerializeField] private float spawnRate;

    Vector3 playerCurPosition;
    Vector3 playerLastPosition;

    // Start is called before the first frame update
    void Start()
    {
        // Spawn a new wave every time cycle to increase difficulty
        InvokeRepeating("SpawnZombies", 0f, spawnRate);
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
}
