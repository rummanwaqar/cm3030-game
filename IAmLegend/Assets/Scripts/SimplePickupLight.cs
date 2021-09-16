using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePickupLight : MonoBehaviour
{
    void OnTriggerEnter( Collider other )
    {   
        // Destroy the object when the player enters into it
        if(other.tag == "Player")
        {
            Destroy(gameObject);
        }
    }
}
