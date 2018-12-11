using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class natsPlayerController : MonoBehaviour {

    public float speed = 6.0f;
    public float jumpSpeed = 8.0f;
    public float rabbitGravity = 20.0f;
    private float gravity;
    public float RotateSpeed = 3.0F;
    public float mouseSpeed = 3.0f;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController controller;
    public Transform playerCam;
    public Camera main, pericam;
    private bool periscoped;
    private int rabbitState = 0; // 0 grounded, 1 is periscoped *change to more formal state implementation later
    Vector3 temp;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        main.enabled = true;
        pericam.enabled = false;
        gravity = rabbitGravity;
        // let the gameObject fall down
        gameObject.transform.position = new Vector3(0, 5, 0);
    }

    void Update()
    {
        if (controller.isGrounded)
        {
            // We are grounded, so recalculate
            // move direction directly from axes


            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0.0f, Input.GetAxis("Vertical"));
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection = moveDirection * speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }


        }
        if (Input.GetMouseButton(1) && rabbitState == 0)
        {
            // periscope
            rabbitState = 1; //switch state
            periscope();

        }
        else if (!Input.GetMouseButton(1) && rabbitState == 1)
        {
            rabbitState = 0; //switch state
            unPeriscope();
        }

        // Apply gravity
        moveDirection.y = moveDirection.y - (gravity * Time.deltaTime);

        // Move the controller
        controller.Move(moveDirection * Time.deltaTime);
        // rotate with A and D
        //transform.Rotate(0, Input.GetAxis("Horizontal") * RotateSpeed, 0);
        //rotate with mouse 
        float X = Input.GetAxis("Mouse X") * mouseSpeed;
        if (rabbitState == 0)
        {
            transform.Rotate(0, X, 0);
        }
    }

    void periscope()
    {
        // Periscope
        gravity = 0; //Find a better way to do this, very prone to bugs
        transform.Rotate(-80, 0, 0);
        temp = new Vector3(transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
        transform.position = new Vector3(temp.x, temp.y + 0.3F, temp.z);
        moveDirection = Vector3.zero;
        main.enabled = false;
        pericam.enabled = true;
    }

    void unPeriscope()
    {
        //end periscope
        transform.position = temp;
        gravity = rabbitGravity;
        transform.Rotate(80, 0, 0);
        main.enabled = true;
        pericam.enabled = false;
    }
}

