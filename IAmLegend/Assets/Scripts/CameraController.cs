using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 * 3rd person camera controller 
 */
public class CameraController : MonoBehaviour
{
    public Transform player;
    public float smoothTime = 0.3f;
    public float distance = 5f;
    public float height = 10f;
    public float cameraAngle = 65f;
    
    private Vector3 _velocity = Vector3.zero;

    private void Update()
    {
        var playerPos = player.position;
        var pos = new Vector3(playerPos.x, playerPos.y + height, playerPos.z - distance);
        // smoothly interpolate between current and target position (using smoothTime)
        transform.position = Vector3.SmoothDamp(transform.position, pos, ref _velocity, smoothTime);
        transform.rotation = Quaternion.Euler(cameraAngle, 0, 0);
    }
}
