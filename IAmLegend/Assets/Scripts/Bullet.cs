using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletForce = 20f;
    public float maxTimeAlive = 2f;

    private float _startTime;
    [SerializeField] private AudioSource audioSFX;

    private void Start()
    {
        _startTime = Time.fixedTime;
        var rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * bulletForce, ForceMode.Impulse);

        // Using a static function to keep playing SFX when the object is destroyed
        AudioSource.PlayClipAtPoint(audioSFX.clip, transform.position);
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Item") || other.gameObject.CompareTag("Bullet")) return;
        Destroy(this.gameObject);
    }
}
