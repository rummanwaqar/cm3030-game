#undef DEBUG
//#define DEBUG

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyBehaviour : MonoBehaviour
{
    #region Variables

    [Header("AI, Navigation & Attacking")]
    [SerializeField] private float distanceSight;
    [SerializeField] private float distanceToAttack;
    [SerializeField] private float angleView;
    [SerializeField] private LayerMask targetsLayers;
    [SerializeField] private LayerMask obstaclesLayers;
    [SerializeField] float delayTimeFSM;
    private Collider detectedTarget;
    private NavMeshAgent agent;

    [Header("Health & Damage")]
    [SerializeField] private float bulletDamage;
    [SerializeField] private float deathAnimationTime;
    [SerializeField] private float health;
    [SerializeField] private Slider healthbar;

    // Animation
    private Animator animator;

    // Start is called before the first frame update
    void Awake()
    {
        this.agent = GetComponent<NavMeshAgent>();
        this.animator = GetComponent<Animator>();
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(this.ScanArea());        // Look for targets
        StartCoroutine(this.UpdateState());     // FSM
    }

    private IEnumerator ScanArea()
    {
        // Wait some time to prevent overloading
        yield return new WaitForSeconds(delayTimeFSM);

        // Get all the targets which the AI should attack
        Collider[] targets = Physics.OverlapSphere(this.transform.position, this.distanceSight, this.targetsLayers);
        detectedTarget = null;

        // Iterate through all of them to see which target is close
        for( int i = 0; i < targets.Length; i++ )
        {
            Collider target = targets[i];

            // Calculate the distance to the target
            Vector3 directionToTarget = Vector3.Normalize(target.bounds.center - this.transform.position);

            // Calculate the angle view between the AI and the target
            float angleToTarget = Vector3.Angle(this.transform.forward, directionToTarget);

            // Check if the target is in the view angle of the AI
            if( angleToTarget < this.angleView )
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

    private IEnumerator UpdateState()
    {
        // Wait some time to prevent overloading
        yield return new WaitForSeconds(delayTimeFSM);

        float distanceToTarget = GetDistanceToTarget(detectedTarget);

        if( this.health <= 0 )
        {
            StartCoroutine(Dead());
        }
        // Attack near targets
        else if( this.detectedTarget != null && distanceToTarget < distanceToAttack )
        {
            AttackTarget();
        }
        // If a target was detected, chase him
        else if( this.detectedTarget != null )
        {
            ChaseTarget();
        }
        // else if --> ReceiveDamage(bulletDamage);
        else
        {
            Idle(); // or patrol?
        }
    }

    private float GetDistanceToTarget( Collider _detectedTarget )
    {
        if( _detectedTarget != null )
        {
            // Calculate the distance to the target
            Vector3 posEnemy = this.transform.position;
            Vector3 postTarget = _detectedTarget.transform.position;

            return Mathf.Abs(Vector3.Distance(posEnemy, postTarget));
        }

        return -1;  // unusable value
    }

    private void Idle()
    {
        this.agent.isStopped = true;
        this.animator.SetBool("Walking", false);
        this.animator.SetBool("Idle", true);
    }

    private void ChaseTarget()
    {
        this.agent.isStopped = false;
        this.agent.SetDestination(detectedTarget.transform.position);
        this.animator.SetBool("Idle", false);
        this.animator.SetBool("Walking", true);
    }

    private void AttackTarget()
    {
        this.agent.isStopped = true;
        this.animator.SetTrigger("Attacking");

        // If the target is close, deal damange
        DealDamage();
    }
    private void DealDamage()
    {
        // Call external functions: Target.damage()
    }

    private void ReceiveDamage( float damage )
    {
        health -= damage;                           // Update health
        float healthNormalized = (health / 100);    // Normalize the value
        healthbar.value = healthNormalized;
    }

    private IEnumerator Dead()
    {
        // Play death animation manually
        animator.Play("zombie_death_standing");

        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationTime);

        // Delete the zombie from the scene
        Destroy(gameObject);
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

#if DEBUG
    // visual gizmos
    private void OnDrawGizmos()
    {
        {
            // Display how far the zombie can see
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, distanceSight);
        }
    }
#endif
}
