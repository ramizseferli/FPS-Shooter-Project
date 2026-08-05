using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header ("Movement Settings")]
    public float speed = 6f;
    public float jumpSpeed = 5f;
    public float gravity = 20.0f;

    private CharacterController controller;
    private Vector3 moveDirection = Vector3.zero;
    private Animator animator;

    
    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponentInChildren<Animator>();
    }


    
    void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        float currentSpeed = new Vector2(horizontal, vertical).magnitude;
        if (animator != null)
        {
            animator.SetFloat("Speed", currentSpeed);
        }
        
        if (controller.isGrounded)
        {
            moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            moveDirection=transform.TransformDirection(moveDirection);
            moveDirection *= speed;
            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
                
        }
        moveDirection.y-= gravity*Time.deltaTime;
        controller.Move(moveDirection*Time.deltaTime);
    }
}
