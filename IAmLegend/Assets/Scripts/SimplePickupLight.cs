using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimplePickupLight : MonoBehaviour
{
    //[SerializeField] private GameObject weapon;
    [SerializeField] private Transform _transform;
    [SerializeField] private Vector3 _pickupPosition;

    private void Start()
    {
        _transform = GetComponents<Transform>()[0];
        _pickupPosition = _transform.position;
    }

    private void Update()
    {
        // If the pickup's position changed, destroy its highlight
        if(_transform.position != _pickupPosition)
        {
            Destroy(gameObject);
        }
    }
}
