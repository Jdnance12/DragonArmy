using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class RangedEnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    //[SerializeField] Animator anim;

    [SerializeField] Transform shootPOS;
    [SerializeField] Transform headPOS;

    [SerializeField] int HP;
    [SerializeField] int faceTargetSpeed;
    [SerializeField] int FOV;
    [SerializeField] int roamDist;
    [SerializeField] int roamTimer;
    [SerializeField] int animSpeedTrans;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    bool playerInRange;
    bool isShooting;
    bool isRoaming;

    Vector3 playerDir;
    Vector3 startingPOS;

    Color colorOrig;

    float angleToPlayer;
    float stoppingDistOrig;
    float HPOrig;

    Coroutine co;

    // Start is called before the first frame update
    void Start()
    {
        colorOrig = model.material.color;
        // GameManager.instance.updateGameGoal(1);
        startingPOS = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        HPOrig = HP;
    }

    // Update is called once per frame
    void Update()
    {
        if (agent.isActiveAndEnabled)
        {

            float agentSpeed = agent.velocity.normalized.magnitude;
           // float animSpeed = anim.GetFloat("Speed");

            //anim.SetFloat("Speed", Mathf.MoveTowards(animSpeed, agentSpeed, Time.deltaTime * animSpeedTrans));

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

    bool canSeePlayer()
    {
        playerDir = GameManager.instance.player.transform.position - headPOS.position;
        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        Debug.DrawRay(headPOS.position, playerDir);

        RaycastHit hit;
        if (Physics.Raycast(headPOS.position, playerDir, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= FOV)
            {

                agent.SetDestination(GameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    faceTarget();
                }

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
                agent.stoppingDistance = stoppingDistOrig;
                return true;
            }
        }
        agent.stoppingDistance = 0;
        return false;
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
        if (co != null)
            StopCoroutine(co);
        isRoaming = false;
        StartCoroutine(flashRed());
        if (HP <= 0)
        {
            //I'm dead
            Destroy(gameObject);
        }
    }

    IEnumerator shoot()
    {
        isShooting = true;
       // anim.SetTrigger("Shoot");

        Instantiate(bullet, shootPOS.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOrig;
    }
}