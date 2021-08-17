using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingController : MonoBehaviour
{
    public GameObject bulletPrefab;
    public GameObject bulletSpawnPoint;
    public float waitTime = 0.4f;

    private float _lastShotTime = 0;
    private Animator _animator;
    private static readonly int Shoot = Animator.StringToHash("shoot");
    private PlayerCharacterBehaviour _characterBehaviour;

    void Start()
    {
        this._animator = GetComponent<Animator>();
        this._characterBehaviour = GetComponent<PlayerCharacterBehaviour>();
    }

    private void Update()
    {
        if (!this._characterBehaviour.weapon) return;
        if (Input.GetAxisRaw("Fire1") == 0f) return;
        this._characterBehaviour.UsePistol();
        this._shoot();
        this._animator.SetTrigger(Shoot);
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
