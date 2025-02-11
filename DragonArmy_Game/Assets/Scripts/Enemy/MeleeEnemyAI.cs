using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] Animator anim;
    [SerializeField] int animSpeedTrans;
    [SerializeField] int HP;
    [SerializeField] int roamDist;
    [SerializeField] int roamTimer;

    [SerializeField] Transform headPos;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int damage;

    [SerializeField] float attackCooldown;
    [SerializeField] float attackRange;

    bool isAttacking = false;
    bool playerInRange;
    bool isRoaming;

    Color colorOrig;
    Vector3 playerDir;
    Vector3 startingPOS;
    float angleToPlayer;
    float stoppingDistOrig;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        startingPOS = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
    }
    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        
        if (playerInRange && canSeePlayer())
        {
            if (playerInRange && !canSeePlayer())
            {
                if (!isRoaming && agent.remainingDistance < 0.01f)
                {
                    co = StartCoroutine(roam());
                }
            }
            else if (!playerInRange)
            {
                if (!isRoaming && agent.remainingDistance < 0.01f)
                {
                    co = StartCoroutine(roam());
                }
            }
        }
    }
    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPos.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPos.position, playerDir);

        agent.SetDestination(GameManager.instance.player.transform.position);

        if (agent.remainingDistance < agent.stoppingDistance)
        {
            faceTarget();
        }

        if (playerDir.magnitude <= attackRange)
        {
            Debug.Log("In Range!");

            if (!isAttacking)
            {
                StartCoroutine(attacking());
            }

        }
        agent.stoppingDistance = 2;

        return true;
    }
    IEnumerator attacking()
    {
        isAttacking = true;
        // AttackAnimation
        //anim.SetTrigger("Attack1");
        // Attack Player
        //GameManager.instance.playerScript.takeDamage(damage);
        Debug.Log("ATTACK!!!!");

        yield return new WaitForSeconds(attackCooldown);
        isAttacking = false;
    }

    void faceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(new Vector3(playerDir.x, 0, playerDir.z));
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            agent.stoppingDistance = 0;
        }
    }
    public void takeDamage(int amount)
    {
        HP -= amount;
        agent.SetDestination(GameManager.instance.player.transform.position);
        isRoaming = false;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            // Dead
            Destroy(gameObject);
        }

    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
    IEnumerator roam()
    {
        isRoaming = true;
        yield return new WaitForSeconds(roamTimer);

        agent.stoppingDistance = 0;

        Vector3 randomPOS = Random.insideUnitSphere * roamDist;
        randomPOS += startingPOS;

        NavMeshHit hit;
        NavMesh.SamplePosition(randomPOS, out hit, roamDist, 1);
        agent.SetDestination(hit.position);

        isRoaming = false;
    }
}