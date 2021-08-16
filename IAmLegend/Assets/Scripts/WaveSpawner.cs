using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float startTime;   // time to start the wave
    [SerializeField] float spawnRate;   // time between waves
    [SerializeField] float endTime;     // time to end the wave

    int zombiesNum = 0;                 // track how many zombies spawned

    // Start is called before the first frame update
    void Start()
    {
        // Repeat 'Spawn' periodically
        InvokeRepeating("Spawn", startTime, spawnRate);
        // Cancel the spawn wave with 'endTime' variable
        Invoke("CancelInvoke", endTime);
    }

    
    private void Spawn()
    {
        // Don't spawn them on each other (spawn diagonally)
        Vector3 spawnPos = new Vector3(transform.position.x + zombiesNum, transform.position.y, transform.position.z + zombiesNum);
        
        Instantiate(prefab, spawnPos, transform.rotation);
        zombiesNum++;
    }
}
