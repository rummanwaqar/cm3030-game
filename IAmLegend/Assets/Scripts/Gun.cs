using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;

public class Gun : MonoBehaviour
{
    public float fireRate = 2.5f;
    public int numBullets = 1;
    public float coneAngle = 50;
    
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    public Sprite uiIcon;
    public String gunName;

    private float _nextTimeToFire = 0f;

    public void Shoot()
    {
        if (Time.time >= _nextTimeToFire)
        {
            _nextTimeToFire = Time.time + 1f / fireRate;
            var position = bulletSpawnPoint.transform.position;
            var rotation = bulletSpawnPoint.transform.rotation;

            if (numBullets > 1)
            {
                var distanceBetweenBullets = coneAngle / (numBullets - 1);
                var currentAngle = -coneAngle / 2;
                for (int i = 0; i < numBullets; i++)
                {
                    Instantiate(bulletPrefab, position, rotation * Quaternion.Euler(Vector3.up * currentAngle));
                    currentAngle += distanceBetweenBullets;
                }
            }
            else // handle one bullet
            {
                Instantiate(bulletPrefab, position, rotation);
            }
        }
    }
}
