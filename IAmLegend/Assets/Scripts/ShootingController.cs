using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    public float waitTime = 0.4f;

    private float _lastShotTime = 0;

    private void Update()
    {
        if (Input.GetAxisRaw("Fire1") != 0f)
        {
            _shoot();
        }
    }

    private void _shoot()
    {
        if (Time.fixedTime > (_lastShotTime + waitTime))
        {
            var bullet = Instantiate(bulletPrefab, bulletSpawnPoint.transform.position,
                bulletSpawnPoint.transform.rotation);
            _lastShotTime = Time.fixedTime;
        }
    }
}
