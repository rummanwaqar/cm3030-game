using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletForce = 20f;
    public float maxTimeAlive = 2f;

    private float _startTime;
    
    private void Start()
    {
        _startTime = Time.fixedTime;
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);
    }

    private void Update()
    {
        if ((_startTime + maxTimeAlive) < Time.fixedTime)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item")) return;
        Destroy(this.gameObject);
    }
}
