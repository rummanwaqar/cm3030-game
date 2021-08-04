using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [SerializeField] GameObject prefab;
    [SerializeField] float startTime;   // time to start the wave
    [SerializeField] float spawnRate;   // time between waves
    [SerializeField] float endTime;     // time to end the wave
    
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
        Instantiate(this.prefab, this.transform.position, this.transform.rotation);
    }
}
