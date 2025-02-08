using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("---- Bools ----")]
    public bool isSneaking;
    public bool isGrounded;
    public bool canClimb;
    public bool canGrabLedge;
    public bool isGrabbingLedge;

    [Header("---- Player Stats ----")]
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedStep;
    private Vector3 movement;

    [Header("Jumping/Physics")]
    [SerializeField] float gravity;
    [SerializeField] float groundCheckDistance = 0.1f;
    [SerializeField] float jumpForce;
    [SerializeField] int jumpCount;
    [SerializeField] int jumpMax;
    private Vector3 velocity;

    [Header("Climbing")]
    [SerializeField] float climbSpeed;
    [SerializeField] float ledgeGrabDistance = 1.0f;
    [SerializeField] float ledgeClimbUpDistance = 0.5f;

    [Header("---- Components ----")]
    //[SerializeField] Rigidbody rb;
    [SerializeField] CharacterController controller;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask wallLayer;
    [SerializeField] LayerMask ledgeLayer;
    private RaycastHit hit;


    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //RBMovement();
        CcMovement();
        CheckForLedges();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            canClimb = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Climbable"))
        {
            canClimb = false;
        }
    }
    //void RBMovement()
    //{
    //    isGrounded = CheckGrounded();
    //    // Movement Input
    //    movement.x = Input.GetAxisRaw("Horizontal");
    //    movement.z = Input.GetAxisRaw("Vertical");

    //    movement = movement.normalized;

    //    // Player Movement
    //    if (Input.GetKeyDown(KeyCode.LeftShift)) // Toggle Sprint and Speed
    //    {
    //        isSneaking = !isSneaking;
    //    }
    //    if (isSneaking)
    //    {
    //        currentSpeed = moveSpeed / speedMultiplier;
    //    }
    //    else
    //    {
    //        currentSpeed = moveSpeed;

    //    }

    //    Vector3 move = new Vector3(movement.x, 0, movement.z);
    //    rb.MovePosition(rb.position + move * currentSpeed * Time.deltaTime);

    //    // Rotate to face direction of movement
    //    if (movement != Vector3.zero)
    //    {
    //        Quaternion rot = Quaternion.LookRotation(movement, Vector3.up);
    //        rb.MoveRotation(rot);
    //    }
    //}
    bool CheckGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);
    }
    void CheckForLedges()
    {
        //RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 0.5f, transform.forward, out hit, ledgeGrabDistance))
        {
            if (hit.collider.CompareTag("Ledge"))
            {
                canGrabLedge = true;
            }
            else
            {
                canGrabLedge = false;
            }
            
        }
        else
        {
            canGrabLedge = false;
        }
    }
    void CcMovement()
    {
        //Gravity
        if (!isGrabbingLedge || !isGrounded || !canClimb)
        {
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= gravity * Time.deltaTime;

        }
        isGrounded = controller.isGrounded;
        // Movement Input
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.z = Input.GetAxisRaw("Vertical");

        movement = movement.normalized;

        // Player Movement
        if (mouseScroll != 0)
        {
            moveSpeed += mouseScroll * speedStep;
            moveSpeed = Mathf.Clamp(moveSpeed, minSpeed, maxSpeed);
        }

        Vector3 move = new Vector3(movement.x, 0, movement.z);
        controller.Move(move * moveSpeed * Time.deltaTime);

        // Rotate to face direction of movement
        if (movement != Vector3.zero)
        {
            Quaternion rot = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = rot;
        }

        //Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                velocity.y = jumpForce;
            }
            else if (canGrabLedge && !isGrabbingLedge)
            {
                GrabLedge();
            }
        }

        WallRun();
    }
    void GrabLedge()
    {
        isGrabbingLedge = true;
        transform.position = new Vector3(hit.point.x, hit.point.y + ledgeClimbUpDistance, transform.position.z); // Move to ledge position

        velocity = Vector3.zero; // Stops gravity
    }
    void WallRun()
    {
        if(Input.GetKey(KeyCode.Space) && canClimb)
        {
            velocity.y = climbSpeed;
        }
        else
        {
            velocity.y -= gravity * Time.deltaTime;
        }
    }
}
