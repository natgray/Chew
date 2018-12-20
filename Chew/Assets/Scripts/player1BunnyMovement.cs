using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player1BunnyMovement : MonoBehaviour {

    Animator anim;
    
    //int jumpHash = Animator.StringToHash("Jump");
    //int runStateHash = Animator.StringToHash("Base Layer.Run");

    // Use this for initialization
    void Start () {
        anim = GetComponent<Animator>();
    }
	
	// Update is called once per frame
	void Update () {
        //anim.Play("Rab_Idle01", 0);
        //|| Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D)
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            anim.SetBool("isRunning", true);
        }
        /*if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            anim.SetBool("isRunning", true);
            anim.SetBool("isChewing", true);
        }*/
        else
        {
            anim.SetBool("isRunning", false);
            //anim.SetBool("isChewing", false);
        }

    }
}
