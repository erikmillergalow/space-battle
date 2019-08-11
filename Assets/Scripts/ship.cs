using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Ship : NetworkBehaviour
{
	public float movementSpeed = 75f;

	// Rigidbody2D allows for easy physics-based gameplay
	private Rigidbody2D body;
    public GameObject projectilePrefab;

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

        if (!isLocalPlayer) {
            return;
        }

        // check for mouse clicks
        if (Input.GetMouseButton(0)) {
            Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseVector.z = 0; // working in 2D...
            float shipVelocityFactor = Vector3.Dot(body.velocity.normalized, 
                                                        transform.up.normalized);
            CmdShoot(mouseVector, shipVelocityFactor);
        }
    }

    [Command]
    void CmdShoot(Vector3 mouseVector, float shipVelocityFactor) {

        //Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mouseVector.z = 0; // working in 2D...

        // create projectile and instance of script?
        GameObject projectileObject = Instantiate(projectilePrefab, 
                                                  gameObject.transform.position, 
                                                  Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // position of mouse click - position of player = direction of projectile
        projectile.targetVector = mouseVector - gameObject.transform.position;

        // alter velocity of projectile based on ship movement
        //projectile.shipVelocityFactor = Vector3.Dot(body.velocity.normalized, 
        //                                                transform.up.normalized);

        projectile.shipVelocityFactor = shipVelocityFactor;

        // so clients can ignore the collision
        projectile.spawnedBy = netId; 
        
        // ignore collisions between shooter and projectile locally
        Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), 
                                  GetComponent<Collider2D>());
        NetworkServer.Spawn(projectileObject);
    }

    // FixedUpdate() is used for physics calculations and processed less than Update()
    void FixedUpdate() {

        if (!isLocalPlayer) {
            return;
        }

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
