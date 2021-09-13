using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    #region Variables
    [Header("Health & Damage")]
    [SerializeField] private float health;

    public GameObject bloodSplat;

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
        if (bloodSplat)
        {
            bloodSplat.SetActive(true);
            StartCoroutine(wait());
        }

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

    IEnumerator wait()
    {
        yield return new WaitForSeconds(0.5f);
        bloodSplat.SetActive(false);
    }
}
