using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RefactorEnemyWithFlee : MonoBehaviour
{
    [System.Serializable]
    public struct Stats
    {
        [Header("Enemy Settings")]

        [Tooltip("How fast the enemy walks when idle.")]
        public float walkSpeed;

        [Tooltip("How fast the enemy turns while walking.")]
        public float rotateSpeed;

        [Tooltip("How fast the enemy runs while chasing.")]
        public float chaseSpeed;
    }

    public Stats enemyStats;

    [Tooltip("The transform the enemy uses to keep track of the player.")]
    public Transform sight;

    [Tooltip("The speed at which the enemy flees when health is at maximum.")]
    public float fleeSpeed = 5f;

    [Tooltip("The maximum health of the enemy.")]
    public float maxHealth = 10f;

    [Tooltip("Current health of the enemy.")]
    public float currentHealth;

    private bool idle = true;
    private bool slipping = false;
    private Rigidbody rb;
    private GameObject player;

    [Tooltip("The patrol points for the enemy to walk between while idle.")]
    public Transform[] patrolPoints;

    private int currentPatrolPoint = 0;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;  // Start at maximum health
    }

    private void Update()
    {
        // Flee if at maximum health, else follow regular behavior
        if (currentHealth == maxHealth)
        {
            Flee();
        }
        else if (idle)
        {
            Patrol();
        }
        else
        {
            Chase();
        }

        CheckIfSlipping();
    }

    private void Patrol()
    {
        Vector3 moveToPoint = patrolPoints[currentPatrolPoint].position;
        transform.position = Vector3.MoveTowards(transform.position, moveToPoint, enemyStats.walkSpeed * Time.deltaTime);

        // Rotate slowly while patrolling
        transform.Rotate(Vector3.up, enemyStats.rotateSpeed * Time.deltaTime);

        // Move to the next patrol point once close enough
        if (Vector3.Distance(transform.position, moveToPoint) < 0.01f)
        {
            currentPatrolPoint++;
            if (currentPatrolPoint >= patrolPoints.Length)
            {
                currentPatrolPoint = 0;
            }
        }
    }

    private void Flee()
    {
        if (player != null)
        {
            Vector3 directionAwayFromPlayer = (transform.position - player.transform.position).normalized;
            transform.position += directionAwayFromPlayer * fleeSpeed * Time.deltaTime;

            // Rotate to face away from the player while fleeing
            Quaternion targetRotation = Quaternion.LookRotation(-directionAwayFromPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * enemyStats.rotateSpeed);
        }
    }

    private void Chase()
    {
        if (player != null)
        {
            sight.position = new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z);
            transform.LookAt(sight);
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, enemyStats.chaseSpeed * Time.deltaTime);
        }
    }

    private void CheckIfSlipping()
    {
        if (slipping)
        {
            transform.Translate(Vector3.back * 20 * Time.deltaTime, Space.World);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Set slipping to true if on specific layers (example: slippery surface layer)
        if (other.gameObject.layer == LayerMask.NameToLayer("Slippery"))
        {
            slipping = true;
        }
        else
        {
            slipping = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            idle = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            idle = true;
        }
    }
}
