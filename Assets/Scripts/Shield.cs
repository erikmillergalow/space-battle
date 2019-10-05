﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Shield : NetworkBehaviour
{

	private float fadeRate = 0.01f;
    //public float shieldHealth = 100f;

	public SpriteRenderer sprite;

    // Start is called before the first frame update
    void Start() {
        sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update() {

    	// shield fades away to transparent constantly
        if (sprite.color.a > 0) {
        	sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - fadeRate);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile") {
            // call shield damage function on parent Ship
            print("shield collision registered");
            transform.parent.gameObject.GetComponent<Ship>().TakeShieldDamage(collision.gameObject.GetComponent<Projectile>().damageAmount);
            //CmdTakeDamage(collision.gameObject.GetComponent<Projectile>().damageAmount);
        }
    }

    // [Command]
    // void CmdTakeDamage(float damageAmount) 
    // {
    //     RpcTakeDamage(damageAmount);
    // }

    // [ClientRpc]
    // void RpcTakeDamage(float damageAmount)
    // {
    //     print("playerShield taking damage");
    //     shieldHealth -= damageAmount;
    //     print(shieldHealth);

    //     /*if (shieldHealth < 0) {
    //         NetworkIdentity.Destroy(this);
    //     }*/
    // }
}
