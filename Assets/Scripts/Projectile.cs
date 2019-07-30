using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	public int speed = 30;
	public Vector3 targetVector;
    public float shipVelocityFactor;
    public GameObject origin; // set to the GameObject that creates the projectile



    // start is called before the first frame update
    void Start()
    {
    	Rigidbody2D body = gameObject.GetComponentInChildren<Rigidbody2D>();
        
        // modify projectile velocity based on ship movement, only in the
        // direction of the ship's movement. speed divided by 4 may change as
        // development continues
        if (shipVelocityFactor > (speed / 4)) {
            body.velocity = targetVector.normalized * speed  * shipVelocityFactor;
        } else {
            body.velocity = targetVector.normalized * speed;
        }
        
    }

    // update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject != origin) {
            Destroy(gameObject);
        }
    }
}
