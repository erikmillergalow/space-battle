using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuStar : MonoBehaviour
{

	private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine("Flicker");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // stars flicker at random intervals
    IEnumerator Flicker() {
    	while (true) {
			float delay = Random.Range(2.0f, 5.0f);
			yield return new WaitForSeconds(delay);
    		animator.Play("Twinkle", 0);
    	}
    }
}
