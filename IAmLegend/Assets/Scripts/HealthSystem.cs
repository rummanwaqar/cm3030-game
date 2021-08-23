using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    #region Variables
    [Header("Health & Damage")]
    [SerializeField] private float health;

    private float damagePower;
    private float recoverAmounth;
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
    public void SetRecover( float _recoverAmount )
    {
        recoverAmounth = _recoverAmount;
    }
    public void Recover()
    {
        health += recoverAmounth;       // Update health
    }
}
