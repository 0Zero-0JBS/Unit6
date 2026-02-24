using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 5f;

    [Header("References")]
    public Transform cam;
    private Rigidbody rb;
    private Animator anim;

    private float turnSmoothVelocity;
    private float turnSmoothTime = 0.1f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // 1. Get Input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // 2. Determine States
        bool isMoving = direction.magnitude >= 0.1f;
        // Sprinting only works if moving and holding Left Control
        bool isSprinting = isMoving && Input.GetKey(KeyCode.LeftControl);

        // 3. Set Speed (Using walkSpeed because runningSpeed was missing)
        float currentSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // 4. Handle Movement & Rotation
        if (isMoving)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.linearVelocity = new Vector3(moveDir.x * currentSpeed, rb.linearVelocity.y, moveDir.z * currentSpeed);
        }
        else
        {
            rb.linearVelocity = new Vector3(0f, rb.linearVelocity.y, 0f);
        }

        // 5. Jump
        // Check if Space is pressed AND if the player is roughly on the ground
        if (Input.GetKeyDown(KeyCode.Space) && Mathf.Abs(rb.linearVelocity.y) < 0.05f)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            anim.SetTrigger("Jump");
        }

        // 6. Update Animator
        anim.SetBool("isRunning", isMoving);
        anim.SetBool("isSprinting", isSprinting);
    }
}
