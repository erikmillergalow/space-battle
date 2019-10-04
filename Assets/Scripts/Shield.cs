using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{

	private float fadeRate = 0.01f;
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
}
