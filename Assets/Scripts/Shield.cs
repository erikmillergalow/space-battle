﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class Shield : NetworkBehaviour
{

    private float fadeRate = 0.01f;
    public SpriteRenderer sprite;
    public float shieldHealth;

    // Start is called before the first frame update
    void Start() 
    {
        sprite = GetComponent<SpriteRenderer>();
        shieldHealth = 100f;
    }

    // Update is called once per frame
    void Update() 
    {
    	// shield fades away to transparent constantly
        if (sprite.color.a > 0) 
        {
        	sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, sprite.color.a - fadeRate);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Projectile") 
        {
            // call shield damage function on parent Ship
            Ship parent = transform.parent.gameObject.GetComponent<Ship>();
            float damageAmount = collision.gameObject.GetComponent<Projectile>().damageAmount;
            parent.TakeShieldDamage(netId, damageAmount);

            /*shieldHealth -= damageAmount;
            
            if (parent.isLocalPlayer)
            {
                parent.shieldHealthBar.value = shieldHealth;

                if (shieldHealth < 0) 
                {
                    NetworkIdentity.Destroy(this.gameObject);
                    parent.shieldActive = false;
                }

            }*/


        }
    }
}
