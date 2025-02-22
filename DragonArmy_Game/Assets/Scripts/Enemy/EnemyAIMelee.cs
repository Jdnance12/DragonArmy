using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAIMelee : MonoBehaviour, IDamage
{
    [Header("---- Bools ----")]
    public bool isAttacking;
    public bool playerInRange;
    public bool isSearching;

    [Header("---- Stats ----")]
    [SerializeField] float maxHP;
    [SerializeField] float currentHP;
    [SerializeField] float detectionRadius;
    [SerializeField] float searchDuartion;

    [Header("---- Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Transform[] waypoints;
    [SerializeField] Transform playerTrans;

    [Header("----- Enemy Sounds -----")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audDamage;
    [SerializeField] [Range(0, 1)] float audDamageVol;
    [SerializeField] AudioClip[] audDead;
    [SerializeField] [Range(0, 1)] float audDeadVol;
    [SerializeField] AudioClip[] audAttack;
    [SerializeField] [Range(0, 1)] float audAttackVol;

    int currentWaypointIndex;
    Color colorOrig;
    Vector3 playerDir;

    // Start is called before the first frame update
    void Start()
    {
        playerTrans = GameManager.instance.player.transform;
        currentHP = maxHP;
        colorOrig = model.material.color;
        currentWaypointIndex = 0;
        agent.SetDestination(waypoints[currentWaypointIndex].position);
    }

    // Update is called once per frame
    void Update()
    {
        if(playerInRange)
        {
            agent.SetDestination(playerTrans.position);
        }
        else if(isSearching && !playerInRange)
        {
            // Roaming Code
        }
        else
        {
            if(!agent.pathPending && agent.remainingDistance < 0.5f)
            {
                Patroling();
            }
        }
    }
    void Patroling()
    {
        if(waypoints.Length == 0)
        {
            return;
        }
        else
        {
            agent.destination = waypoints[currentWaypointIndex].position;
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
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
        }
    }

    public void takeDamage(float amount)
    {
        currentHP -= amount;
        
        StartCoroutine(flashRed());

        if (currentHP <= 0)
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
}
