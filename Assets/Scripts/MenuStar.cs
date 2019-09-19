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
        //animator.enabled = false;
        StartCoroutine("Flicker");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Flicker() {
    	while (true) {
	
			//animator.enabled = false;
			float delay = Random.Range(2.0f, 4.0f);
			yield return new WaitForSeconds(delay);
    		animator.Play("Twinkle", 0);
    		//animator.enabled = true;
    	}
    }
}
