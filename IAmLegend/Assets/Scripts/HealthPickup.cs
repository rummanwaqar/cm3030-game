using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    [SerializeField] private float maxHealth;
    private GameObject player;

    private HealthSystem playerHealthSystem;

    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        playerHealthSystem = player.GetComponents<HealthSystem>()[0];
    }

    void OnTriggerEnter( Collider other )
    {
        if(other.tag == "Player")
        {
            playerHealthSystem.SetRecover(maxHealth);
            playerHealthSystem.Recover();
            Destroy(gameObject);
        }
    }
}
