using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Ship : NetworkBehaviour
{
	public float movementSpeed = 75f;
	
	public float playerHealth = 100f;
	public float shieldHealth = 100f;
	public Slider shipHealthBar;
	public Slider shieldHealthBar;
    public bool shieldActive;

	// Rigidbody2D allows for easy physics-based gameplay
	private Rigidbody2D body;
    public GameObject projectilePrefab;
    public GameObject shieldPrefab;
    private Shield playerShield;

    // Start is called before the first frame update
    void Start()
    {
        body = gameObject.GetComponent<Rigidbody2D>();

        // set target of main camera's follow script
        if (isLocalPlayer) 
        {
            PlayerCamera playerCamera = Camera.main.gameObject.GetComponent<PlayerCamera>();
            playerCamera.player = this.gameObject;

            shipHealthBar = GameObject.FindGameObjectWithTag("ShipHealthBar").GetComponent<Slider>();
            shieldHealthBar = GameObject.FindGameObjectWithTag("ShieldHealthBar").GetComponent<Slider>();

            playerHealth = 100;
        	shieldHealth = 100;

        	shipHealthBar.value = playerHealth;
        	shieldHealthBar.value = shieldHealth;

            //playerShield.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
        }

        // create shield object that will follow player around
        var shield = Instantiate(shieldPrefab, 
                             gameObject.transform.position, 
                             Quaternion.identity, 
                             this.transform);

        playerShield = shield.GetComponent<Shield>();
        // this needs to be fixed because only the server can assign client authority, maybe put into a command?
        playerShield.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
        shieldActive = true;
    }

    // Update is called once per frame
    void Update() 
    {

        if (!isLocalPlayer) 
        {
            return;
        }

        // check for mouse clicks
        if (Input.GetMouseButton(0)) 
        {
            Vector3 mouseVector = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseVector.z = 0; // working in 2D...
            float shipVelocityFactor = Vector3.Dot(body.velocity.normalized, 
                                                        transform.up.normalized);
            CmdShoot(mouseVector, shipVelocityFactor);
        }
    }

    // FixedUpdate() is used for physics calculations and processed less than Update()
    void FixedUpdate() 
    {

        if (!isLocalPlayer) {
            return;
        }

        // check for player inputs
        if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0) 
        {
    		float horizontalMovement = Input.GetAxisRaw("Horizontal") * movementSpeed;
    		float verticalMovement = Input.GetAxisRaw("Vertical") * movementSpeed;

    		Vector2 heading = new Vector2(horizontalMovement, verticalMovement);

    		body.AddForce(heading);

    		// point ship in direction of movement input
    		transform.up = body.velocity;
    	}
	}

    [Command]
    void CmdShoot(Vector3 mouseVector, float shipVelocityFactor) 
    {
        // instantiate projectiles on clients        
        RpcShoot(mouseVector, shipVelocityFactor);
    }

    [ClientRpc]
    private void RpcShoot(Vector3 mouseVector, float shipVelocityFactor) 
    {
        // create projectile instance
        GameObject projectileObject = Instantiate(projectilePrefab, 
                                                  gameObject.transform.position, 
                                                  Quaternion.identity);
        Projectile projectile = projectileObject.GetComponent<Projectile>();

        // position of mouse click - position of player = direction of projectile
        projectile.targetVector = mouseVector - gameObject.transform.position;

        projectile.shipVelocityFactor = shipVelocityFactor;

        // so clients can ignore the collision
        projectile.spawnedBy = netId; 
        
        // ignore collisions between shooter and projectile locally
        Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), 
                                  GetComponent<Collider2D>());

        // allow projectiles out of the shooting player's shield
        if (shieldActive)
        {
            Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), 
                                      playerShield.GetComponent<Collider2D>());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    	if (collision.gameObject.tag == "Projectile" && !shieldActive) 
        {
    		CmdTakeDamage(this.netId, collision.gameObject.GetComponent<Projectile>().damageAmount);
    	}
    }

    // player ship health and damage
    [Command]
    void CmdTakeDamage(uint netId, float damageAmount) 
    {
    	RpcTakeDamage(netId, damageAmount);
    }

    [ClientRpc]
    void RpcTakeDamage(uint netId, float damageAmount)
    {
        if (this.netId == netId) 
        {
            playerHealth -= damageAmount;

            if (isLocalPlayer) 
            {
            	shipHealthBar.value = playerHealth;
            }

            if (playerHealth < 0) 
            {
                NetworkIdentity.Destroy(this.gameObject);
            }
        }
    }

    // player shield health and damage
    public void TakeShieldDamage(uint netId, float damageAmount)
    {
    	CmdTakeShieldDamage(netId, damageAmount);
    }

    [Command]
    void CmdTakeShieldDamage(uint netId, float damageAmount) 
    {
        RpcTakeShieldDamage(netId, damageAmount);
    }

    [ClientRpc]
    void RpcTakeShieldDamage(uint netId, float damageAmount)
    {
        if (shieldActive)
        {
            if (playerShield.netId == netId) 
            {
                shieldHealth -= damageAmount;
                
                if (isLocalPlayer)
                {
    				shieldHealthBar.value = shieldHealth;
                }

                if (shieldHealth < 0) 
                {
                    NetworkIdentity.Destroy(this.playerShield.gameObject);
                    shieldActive = false;
                }
            }
        }
    }

}

