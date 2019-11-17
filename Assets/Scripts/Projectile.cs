using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Projectile : NetworkBehaviour
{

	public int speed = 30;
    public float damageAmount = 1f;
	public Vector3 targetVector;
    public float shipVelocityFactor;

     // set to the GameObject that creates the projectile
    public GameObject origin;

    [SyncVar]
    public uint spawnedBy;


    // start is called before the first frame update
    void Start()
    {
    	Rigidbody2D body = gameObject.GetComponentInChildren<Rigidbody2D>();

    	GameObject spawner = NetworkServer.FindLocalObject(spawnedBy);
        
        // modify projectile velocity based on ship movement, only in the
        // direction of the ship's movement. speed divided by 4 may change as
        // development continues
        if (shipVelocityFactor > (speed / 4)) 
        {
            body.velocity = targetVector.normalized * speed  * shipVelocityFactor;
        } 
        else 
        {
            body.velocity = targetVector.normalized * speed;
        }
        
    }

    // update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (collision.gameObject != origin && collision.gameObject.tag == "Shield") 
        {
            if (spawnedBy != collision.gameObject.GetComponent<Shield>().netId)
            {
            	this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
           		triggerShield(collision.gameObject.GetComponent<Shield>());     
            	Destroy(this.gameObject);
            }
        }
        
        if (collision.gameObject != origin && collision.gameObject.tag == "Wall") 
        {
 			Destroy(this.gameObject);
 		}

 		if (collision.gameObject != origin && collision.gameObject.tag == "Ship") 
 		{
 			this.gameObject.GetComponent<Rigidbody2D>().isKinematic = true;
 			Destroy(this.gameObject);
 		}
    }

    void triggerShield(Shield shield)
    {
        var currentShieldColor = shield.sprite.color;
        shield.sprite.color = new Color(currentShieldColor.r, currentShieldColor.g, currentShieldColor.b, 1f);
    }

}
