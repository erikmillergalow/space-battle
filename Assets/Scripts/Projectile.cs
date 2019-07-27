using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{

	public int speed = 30;
	public Vector3 targetVector;
    public float shipVelocity;


    // Start is called before the first frame update
    void Start()
    {
    	Rigidbody2D body = gameObject.GetComponentInChildren<Rigidbody2D>();
        body.velocity = targetVector.normalized * speed * shipVelocity;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
