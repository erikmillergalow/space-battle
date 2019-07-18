using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ship : MonoBehaviour
{
	public float movementSpeed = 25f;

	// Rigidbody2D allows for easy physics-based gameplay
	private Rigidbody2D body;

    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();

        if (body == null) {
        	Debug.LogError("Player::Start can't find RigidBody2D");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // FixedUpdate() is used for physics calculations and processed less than Update()
    void FixedUpdate() {

    	// check for player inputs
    	if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) {
    		float horizontalMovement = Input.GetAxisRaw("Horizontal") * movementSpeed;
    		float verticalMovement = Input.GetAxisRaw("Vertical") * movementSpeed;

    		Vector2 heading = new Vector2(horizontalMovement, verticalMovement);

    		body.AddForce(heading);

    		// point ship in direction of movement input
    		transform.up = body.velocity;

    	}
    }
}
