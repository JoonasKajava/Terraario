using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private CharacterController c_controller;
    private SpriteRenderer s_renderer;
    private Animator animator;
    private Vector3 moveDirection = Vector3.zero;

    public float Gravity = 9.8f;
    public float Speed;
    public float JumpForce = 8.0f;
	// Use this for initialization
	void Start () {
        c_controller = GetComponent<CharacterController>();
        s_renderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        float h_axis = Input.GetAxis("Horizontal");

        if (h_axis < 0)
        {
            s_renderer.flipX = true;
        }else if(h_axis > 0)
        {
            s_renderer.flipX = false;
        }
        if(c_controller.isGrounded)
        {
            if(h_axis != 0)
            {
                animator.SetBool("Walking", true);
            }else
            {
                animator.SetBool("Walking", false);
            }
            moveDirection = new Vector3(h_axis, 0, 0);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= Speed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = JumpForce;
            }
        }else
        {
            Vector3 AirMove = new Vector3(h_axis, 0, 0);
            if(h_axis != 0)
            {
                moveDirection.x = AirMove.x;
            }
            AirMove *= Speed;
            c_controller.Move(AirMove * Time.deltaTime);
        }
        moveDirection.y -= Gravity * Time.deltaTime;

        c_controller.Move(moveDirection * Time.deltaTime);
	}
}
