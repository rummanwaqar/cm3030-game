using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    private enum EnemyState { Idle, ChaseTarget, AttackTarget, GotHit, Dead }

    // AI & navigation
    [SerializeField] private float distanceSight;
    [SerializeField] private float distanceAttack;
    [SerializeField] private float angleView;
    [SerializeField] private LayerMask targetsLayers;
    [SerializeField] private LayerMask obstaclesLayers;
    private Collider detectedTarget;
    private NavMeshAgent agent;

    // Health & damage
    [SerializeField] private float bulletDamage;
    [SerializeField] private float deathAnimationTime;
    [SerializeField] private float health;
    [SerializeField] private Slider healthbar;
    
    // Animation
    private EnemyState enemyCurrentState;
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();
        this.animator = GetComponent<Animator>();
        this.enemyCurrentState = EnemyState.Idle;    // Enemy is idle on initial spawn
    }

    // Update is called once per frame
    void Update()
    {
        this.ScanArea();     // Look for targets
        this.UpdateState();  // FSM
    }

    private void ScanArea()
    {
        // Get all the targets which the AI should attack
        Collider[] targets = Physics.OverlapSphere(this.transform.position, this.distanceSight, this.targetsLayers);

        detectedTarget = null;
        for( int i = 0; i < targets.Length; i++ )
        {
            Collider target = targets[i];

            // Calculate the distance to the target
            Vector3 directionToTarget = Vector3.Normalize(target.bounds.center - this.transform.position);

            // Calculate the angle view between the AI and the target
            float angleToTarget = Vector3.Angle(this.transform.forward, directionToTarget);

            // Check if the target is in the view angle of the AI
            if( angleToTarget < angleView )
            {
                // Check if there are any obstacles between the AI and the target
                if( !Physics.Linecast(this.transform.position, target.bounds.center, this.obstaclesLayers) )
                {
                    // The target was detected
                    detectedTarget = target;
                    break;
                }
            }
        }
    }

    // Update the enemy's state
    private void UpdateState()
    {
        if( enemyCurrentState == EnemyState.Dead || health <= 0 )
            StartCoroutine(Dead());
        else if( enemyCurrentState == EnemyState.Idle )
            Idle();
        else if( enemyCurrentState == EnemyState.ChaseTarget )
            ChaseTarget();
        else if( enemyCurrentState == EnemyState.AttackTarget )
            AttackTarget();
        else if( enemyCurrentState == EnemyState.GotHit )
            ReceiveDamage(50);
    }

    private void Idle()
    {
        // If target was detected, chase him
        if( this.detectedTarget != null )
        {
            // Target detected, chase him
            this.enemyCurrentState = EnemyState.ChaseTarget;
            return;
        }
        else if( this.health <= 0 )
        {
            // No health, die
            this.enemyCurrentState = EnemyState.Dead;
            return;
        }

        this.agent.isStopped = true;
        this.animator.SetTrigger("Idle");
    }

    private void ChaseTarget()
    {
        // If target wasn't detected, be idle
        if( this.detectedTarget == null )
        {
            this.enemyCurrentState = EnemyState.Idle;
            return;
        }
        else if( this.health <= 0 )
        {
            // No health, die
            this.enemyCurrentState = EnemyState.Dead;
            return;
        }

        // Calculate the distance to the target
        float distanceToTarget = Vector3.Distance(this.transform.position, this.detectedTarget.transform.position);
        this.agent.SetDestination(this.detectedTarget.transform.position);

        // Attack the near targets
        if( Mathf.Abs(distanceToTarget) < distanceAttack )
            this.AttackTarget();
        else
        {
            // Chase the target
            this.agent.isStopped = false;
            this.animator.SetTrigger("Walking");
        }
    }

    private void AttackTarget()
    {
        // If target wasn't detected, be idle
        if( this.detectedTarget == null )
        {
            this.enemyCurrentState = EnemyState.Idle;
            return;
        }
        else if( this.health <= 0 )
        {
            // No health, die
            this.enemyCurrentState = EnemyState.Dead;
            return;
        }

        this.agent.isStopped = true;
        this.animator.SetTrigger("Attacking");

        // If the target is close, deal damange
        DealDamage();
    }

    private IEnumerator Dead()
    {
        this.agent.isStopped = true;

        // Play death animation manually
        animator.Play("zombie_death_standing");

        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationTime);

        // Delete the zombie from the scene
        Destroy(gameObject);
    }

    // The zombie was attacked successfully
    private void ReceiveDamage( float damage )
    {
        
        health -= damage;                           // Update health
        float healthNormalized = ( health / 100 );  // Normalize the value
        healthbar.value = healthNormalized;
    }

    private void DealDamage()
    {
        // Call external functions:
        // Target.damage()
    }

    // Collision with tag bullets/ammo receive damage
    void OnTriggerEnter( Collider other )
    {
        switch( other.tag )
        {
            case "Bullet":
                ReceiveDamage(bulletDamage);
                break;
            default:
                ReceiveDamage(0);
                break;
        }
    }

    //#if UNITY_EDITOR
    //    // visual gizmos
    //    private void OnDrawGizmos()
    //    {
    //        {
    //            // Display how far the zombie can see
    //            Gizmos.color = Color.red;
    //            Gizmos.DrawSphere(transform.position, distanceSight);
    //        }
    //    }
    //#endif
}
