using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{

	public float interpolationVelocity;
	public float minimumDistance;
	public float followDistance;
	public GameObject player;
	public Vector3 offset;
	Vector3 playerPosition;

    // Start is called before the first frame update
    void Start()
    {
        playerPosition = transform.position;
    }

	// switch this to Update() if we move away from physics based movement
    void FixedUpdate()
    {
        if (player) {
        	Vector3 positionNoZ = transform.position;
        	positionNoZ.z = player.transform.position.z;

        	Vector3 playerDirection = (player.transform.position - positionNoZ);

        	interpolationVelocity = playerDirection.magnitude * 10f;

        	playerPosition = transform.position + (playerDirection.normalized * 
        										   interpolationVelocity * 
        										   Time.deltaTime);

        	transform.position = Vector3.Lerp(transform.position, playerPosition + offset, 0.3f);
        }
    }
}
