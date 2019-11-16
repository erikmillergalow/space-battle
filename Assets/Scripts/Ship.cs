using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class Ship : NetworkBehaviour
{
	public float movementSpeed = 75f;
	
    public float playerHealthMax = 100f;
    public float shieldHealthMax = 100f;

    [SyncVar]
	public float playerHealth = 100f;
    [SyncVar]
	public float shieldHealth = 100f;
	public Slider shipHealthBar;
	public Slider shieldHealthBar;
    [SyncVar]
    public bool shieldActive;

    // time to wait before respawning a shield (seconds)
    public float shieldRechargeTime = 100f;

	// Rigidbody2D allows for easy physics-based gameplay
	private Rigidbody2D body;
    public GameObject projectilePrefab;
    public GameObject shieldPrefab;
    private Shield playerShield;

    private Coroutine rechargeRoutine;

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

            playerHealth = playerHealthMax;
        	shieldHealth = shieldHealthMax;

        	shipHealthBar.value = playerHealth;
        	shieldHealthBar.value = shieldHealth;
        }

        // create shield object that will follow player around
        var shield = Instantiate(shieldPrefab, 
                             gameObject.transform.position, 
                             Quaternion.identity, 
                             this.transform);
        //NetworkServer.Spawn(shield);

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

        shieldHealthBar.value = shieldHealth;
        shipHealthBar.value = playerHealth;

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

        // allow projectiles out of the shooting player's shield
        if (shieldActive)
        {
            Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), 
                                      playerShield.GetComponent<Collider2D>());
        }

        // position of mouse click - position of player = direction of projectile
        projectile.targetVector = mouseVector - gameObject.transform.position;

        projectile.shipVelocityFactor = shipVelocityFactor;

        // so clients can ignore the collision
        projectile.spawnedBy = netId; 
        
        // ignore collisions between shooter and projectile locally
        Physics2D.IgnoreCollision(projectileObject.GetComponent<Collider2D>(), 
                                  GetComponent<Collider2D>());
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
    	//RpcTakeDamage(netId, damageAmount);
        if (this.netId == netId) 
        {
            playerHealth -= damageAmount;

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
        if (shieldActive)
        {
            if (playerShield.netId == netId) 
            {
                shieldHealth -= damageAmount;
                
                if (isLocalPlayer)
                {
                    shieldHealthBar.value = shieldHealth;
                }

                if (shieldHealth <= 0) 
                {
                    RpcDestroytShield();
                    //NetworkIdentity.Destroy(this.playerShield.gameObject);
                    shieldActive = false;
                }

                // start countdown to when shield wil recharge
                if (rechargeRoutine != null)
                {
                    StopCoroutine(rechargeRoutine);
                }

                rechargeRoutine = StartCoroutine("ShieldRechargeTimer");
            }
        }
    }

    [ClientRpc]
    void RpcDestroytShield()
    {
        Destroy(this.playerShield.gameObject);
    }

    [Command]
    void CmdRespawnShield(uint netId)
    {
        RpcRespawnShield();
        shieldActive = true;
    }

    [ClientRpc]
    void RpcRespawnShield()
    {
         // create shield object that will follow player around
        var shield = Instantiate(shieldPrefab, 
                             gameObject.transform.position, 
                             Quaternion.identity, 
                             this.transform);

        playerShield = shield.GetComponent<Shield>();

        // this needs to be fixed because only the server can assign client authority, maybe put into a command?
        playerShield.GetComponent<NetworkIdentity>().AssignClientAuthority(this.GetComponent<NetworkIdentity>().connectionToClient);
    }

    IEnumerator ShieldRechargeTimer()
    {
        print("waiting to recharge shield");
        yield return new WaitForSeconds(2f);

        if (!shieldActive) {
            CmdRespawnShield(netId); 
        }


        while(shieldHealth < shieldHealthMax)
        {
            CmdAddShieldHealth(netId);
            yield return new WaitForSeconds(0.01f);
        }
    }

    [Command]
    void CmdAddShieldHealth(uint netId) 
    {
        if (this.netId == netId)
        {
            shieldHealth += 1f;
        }
    }
}

