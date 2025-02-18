using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    GameManager gm;

    [Header("---- Bools ----")]
    [Header("Grounded Bools")]
    public bool isGrounded;
    public bool isCrouched;

    [Header("Attacking")]
    public bool isAttackingWithSword;

    [Header("Wall Physics Bools")]
    public bool wallInRange;
    public bool canWallRun;
    public bool canClimb;
    public bool isGrabbing;

    [Header("---- Player Stats ----")]
    [Header("Health")]
    [SerializeField] public float maxHP = 100;
    [SerializeField] float currentHP;

    [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float speedStep;
    [SerializeField] float sprintSpeed;
    private Vector3 movement;

    [Header("Jumping/Physics")]
    [SerializeField] float gravity;
    [SerializeField] float jumpForce;
    private Vector3 velocity;

    [Header("Climbing")]
    [SerializeField] float wallDetectDistonce;
    [SerializeField] float climbSpeed;
    [SerializeField] float wallRunTime;
    private Collider ledgeCollider;
    private float wallRunTimer;

    [Header("---- Components ----")]
    [SerializeField] CharacterController controller;
    [SerializeField] Animator animatorCtrlr;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] SwordWeapon swordWeapon;
    private Transform currentGrabPoint;
    private RaycastHit hit;
    public GameObject targetLocation;

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

        //Initializing Stats
        currentHP = maxHP;
    }

    // Update is called once per frame
    void Update()
    {
        CcMovement();
        Attack();
    }
    void CcMovement()
    {
        //Gravity
        if (!isGrounded || !canClimb || !isGrabbing)
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

        if (isGrabbing)
        {
            // Turn off Gravity
            // Climbing Movement
            // Press Spacebar to deacive isGrabbing
        }
        else
        {
            Vector3 move = new Vector3(movement.x, 0, movement.z);
            if (Input.GetButton("Jump"))
            {
                isCrouched = false;
                controller.Move(move * sprintSpeed * Time.deltaTime);
            }
            else
            {
                controller.Move(move * moveSpeed * Time.deltaTime);
            }

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
        }

        WallPhysics();
        AnimationStates();
    }
    void WallPhysics()
    {

        RaycastHit hit;
        Vector3 direction = transform.forward;

        if(Physics.Raycast(transform.position + Vector3.up * 1f, direction, out hit, wallDetectDistonce))
        {
            if (hit.collider.CompareTag("Wall") || hit.collider.CompareTag("Climbable"))
            {
                canWallRun = true;
            }
            else
            {
                canWallRun = false;
            }
        }
        else
        {
            canWallRun = false;
        }

        if (Input.GetKey(KeyCode.Space) && canWallRun && !isGrabbing)
        {
            canWallRun = false;
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
    void AnimationStates()
    {
        //Crouching
        animatorCtrlr.SetBool("IsCrouched", isCrouched);
    }

    void Attack()
    {
        if (Input.GetButtonDown("Fire1") && !isAttackingWithSword)
        {
            //animatorCtrlr.SetBool("SwordAttack", true);
            StartCoroutine(SwordSwing());
            //Player Sword Attack Coroutine
        }
    }
    IEnumerator SwordSwing()
    {
        isAttackingWithSword = true;
        swordWeapon.EnableCollider();
        animatorCtrlr.SetBool("SwordAttack", true);
        int layerIndex = animatorCtrlr.GetLayerIndex("UpperBody");
        float animationDuration = animatorCtrlr.GetCurrentAnimatorStateInfo(layerIndex).length;

        yield return new WaitForSeconds(0.5f);
        animatorCtrlr.SetBool("SwordAttack", false);
        isAttackingWithSword = false;
        swordWeapon.DisableCollider();
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == targetLocation)
        {
            GameManager.instance.youWin();
        }
    }

}
