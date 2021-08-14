using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    #region Variables
    [Header("Health & Damage")]
    [SerializeField] private float health;

    private float damagePower;
    #endregion

    public float GetHealth()
    {
        return health;
    }

    public void SetDamage( float _damagePower )
    {
        damagePower = _damagePower;
    }
    public void Hit()
    {
        health -= damagePower;       // Update health
    }
}
