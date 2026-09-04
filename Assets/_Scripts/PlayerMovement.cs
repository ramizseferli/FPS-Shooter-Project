using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 2f;    // Yeriş sürəti (Blend Tree-də Walk eşik dəyəri)
    public float runSpeed = 6f;     // Qaçış sürəti (Blend Tree-də Run eşik dəyəri)
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

        Vector2 inputVector = new Vector2(horizontal, vertical);
        float inputMagnitude = Mathf.Clamp01(inputVector.magnitude);

        // Shift basılıdırsa runSpeed (6), deyilsə walkSpeed (2)
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        float activeSpeed = isRunning ? runSpeed : walkSpeed;

        // Animator üçün speed: 0 (Idle), ~2 (Walk), ~6 (Run)
        float currentSpeed = inputMagnitude * activeSpeed;

        if (animator != null)
        {
            // 1. Sürət parametrini göndəririk (Blend Tree özü animasiyanı rəvan keçirəcək)
            animator.SetFloat("Speed", currentSpeed);

            // 2. MotionSpeed parametri: Hərəkət olduqda 1f
            animator.SetFloat("MotionSpeed", inputMagnitude > 0 ? 1f : 0f);

            // 3. Grounded parametri
            animator.SetBool("Grounded", controller.isGrounded);
        }

        if (controller.isGrounded)
        {
            moveDirection = new Vector3(horizontal, 0, vertical);
            moveDirection = transform.TransformDirection(moveDirection);

            // Xarakterin fiziki hərəkət sürəti
            moveDirection *= activeSpeed;

            if (Input.GetButton("Jump"))
            {
                moveDirection.y = jumpSpeed;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }

    public void OnFootstep(AnimationEvent animationEvent)
    {

    }

    public void OnLand(AnimationEvent animationEvent)
    {

    }
}