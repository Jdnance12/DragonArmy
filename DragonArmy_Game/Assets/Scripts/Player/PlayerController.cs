using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gm;

    [Header("---- Bools ----")]
    public bool isGrounded;
    public bool isCrouched;
    public bool canClimb;
    public bool isGrabbing;

    [Header("---- Player Stats ----")]
    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedStep;
    private Vector3 movement;

    [Header("Jumping/Physics")]
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;
    private Vector3 velocity;

    [Header("Climbing")]
    [SerializeField] float climbSpeed;
    [SerializeField] float wallRunTime;
    private float wallRunTimer;

    [Header("---- Components ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animatorCtrlr;
    [SerializeField] LayerMask groundLayer;
    private Transform currentGrabPoint;
    private RaycastHit hit;

    [Header("Grab Points")]
    [SerializeField] float grabPointDistance;
    [SerializeField] float lerpSpeed;


    // Start is called before the first frame update
    void Start()
    {
        gm = GameManager.instance;
        controller = GetComponent<CharacterController>();
        animatorCtrlr = GetComponent<Animator>();
        wallRunTimer = wallRunTime;
    }

    // Update is called once per frame
    void Update()
    {
        CcMovement();
    }
    void CcMovement()
    {
        //Gravity
        if (!isGrounded || !canClimb)
        {
            controller.Move(velocity * Time.deltaTime);
            velocity.y -= gravity * Time.deltaTime;

        }
        else if (velocity.y < 0)
        {
            velocity.y = -2f; // Ensure player stays grounded
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

        // Crouching
        if (Input.GetKeyDown(KeyCode.C))
        {
            isCrouched = !isCrouched;
            animatorCtrlr.SetBool("IsCrouched", isCrouched);
        }
        if (isCrouched)
        {
            maxSpeed = 5;
            controller.height = 1.0f;
            controller.center = new Vector3(0, 0.55f, 0);
        }
        else
        {
            maxSpeed = 7;
            controller.height = 1.75f;
            controller.center = new Vector3(0, 0.9f, 0);
        }

        //Jumping
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded && !canClimb && !isCrouched)
            {
                velocity.y = jumpForce;
            }
        }

        WallPhysics();
    }
    void WallPhysics()
    {
        if (Input.GetKey(KeyCode.Space) && canClimb && !isGrabbing)
        {
            if (wallRunTimer > 0)
            {
                velocity.y = climbSpeed;
                wallRunTimer -= Time.deltaTime;
            }
        }
        if (isGrounded)
        {
            wallRunTimer = wallRunTime;
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Climbable") && wallRunTimer > 0)
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
}
