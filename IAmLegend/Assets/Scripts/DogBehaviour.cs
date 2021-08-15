using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class DogBehaviour : MonoBehaviour
{
    #region Variables
    [Header("AI & Navigation")]
    [SerializeField] GameObject followTarget;
    [SerializeField] private float radiusToStartFollow; // The dog will start follow the player
    [SerializeField] private float radiusToSit;         // Distance to the player
    [SerializeField] private float delayTimeFSM;
    private NavMeshAgent agent;

    [Header("Attacking Targets")]
    [SerializeField] private float distanceSight;
    [SerializeField] private float distanceToAttack;
    [SerializeField] private float angleView;
    [SerializeField] private LayerMask targetsLayers;
    [SerializeField] private LayerMask obstaclesLayers;
    [SerializeField] private HealthSystem detectedTarget;
    [SerializeField] private float damagePower;
    [SerializeField] private float attackingCountdown;
    [SerializeField] private float attackingRate;
    private bool runAway;

    [Header("Health & Damage")]
    [SerializeField] private float deathAnimationTime;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Slider healthBar;
    private float currentDamage;

    // Animation
    private Animator animator;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        runAway = false;
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine(ScanArea());        // Look for targets
        StartCoroutine(UpdateState());     // FSM
    }

    private IEnumerator ScanArea()
    {
        // Wait some time to prevent overloading
        yield return new WaitForSeconds(delayTimeFSM);

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
                    detectedTarget = target.GetComponentInParent<HealthSystem>();
                    break;
                }
            }
        }
    }

    private IEnumerator UpdateState()
    {
        // Wait some time to prevent overloading
        yield return new WaitForSeconds(delayTimeFSM);

        // Update variables for the FSM's decisions
        float distanceFromPlayer = GetDistanceFromPlayer();
        //float distanceToTarget = GetDistanceToTarget(detectedTarget);
        UpdateHealthBar(healthSystem.GetHealth());                         // Update health bar

        if( healthSystem.GetHealth() <= 0 )
        {
            // No health, die
            StartCoroutine(Dead());
        }
        else if( currentDamage != 0 )
        {
            // If a damage is needed to be processed
            healthSystem.SetDamage(currentDamage);
            healthSystem.Hit();                                            // Process damage
            currentDamage = 0;                                             // The damage was processed
        }
        else if( distanceFromPlayer >= radiusToStartFollow && runAway == false )
        {
            // Walk to the player
            FollowPlayer(followTarget.transform.position);
        }
        else if( runAway )
        {
            // running away on player's command
        }
        else if( detectedTarget != null )
        {
            // Calculate the distance to the target
            Vector3 posDog = transform.position;
            Vector3 postTarget = detectedTarget.transform.position;
            float distanceToTarget = Mathf.Abs(Vector3.Distance(posDog, postTarget));

            if( runAway == false && distanceToTarget < distanceToAttack )
            {
                // If a target was detected and near, attack
                AttackTarget();
            }
            else
            {
                // If a target was detectet but far, chase
                ChaseTarget();
            }

        }
        else if( distanceFromPlayer <= radiusToSit )
        {
            Idle();
        }
    }

    private float GetDistanceFromPlayer()
    {
        // Get the current position of the player and the dog
        Vector3 posPlayer = followTarget.transform.position;
        Vector3 posDog = transform.position;

        // Return the result from the distance
        return Mathf.Abs(Vector3.Distance(posPlayer, posDog));
    }

    private void Idle()
    {
        // Update animations
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", true);

        // Remove from the enemies' targeted LayerMask
        gameObject.layer = LayerMask.NameToLayer("Default");
    }

    private void FollowPlayer( Vector3 _posPLayer )
    {
        // Update animations
        agent.isStopped = false;
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);

        // Remove from the enemies' targeted LayerMask
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Pathfinding algorithm
        Vector3 offset = new Vector3(1f, 0, 1f);  // Avoid collisions with the player
        agent.SetDestination(_posPLayer - offset);
    }

    private void ChaseTarget()
    {
        agent.isStopped = false;
        agent.SetDestination(detectedTarget.transform.position);
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);
    }

    private void AttackTarget()
    {
        // Update animations
        agent.isStopped = true;
        animator.SetTrigger("Attacking");

        // Add to the enemies' targeted LayerMask
        gameObject.layer = LayerMask.NameToLayer("Player");

        // If the target is close, deal damange
        DealDamage();
    }

    private void DealDamage()
    {
        // Countdown between attacks
        if( attackingCountdown <= 0 )
        {
            detectedTarget.SetDamage(damagePower);
            detectedTarget.Hit();                    // Process damage
            attackingCountdown = 1 / attackingRate;
        }

        attackingCountdown -= Time.deltaTime;
    }

    private void UpdateHealthBar( float _health )
    {
        float healthNormalized = (_health / 100);    // Normalize the value
        healthBar.value = healthNormalized;
    }

    public void SetDamage( float damage )
    {
        // The damange which will be received
        currentDamage = damage;
    }

    private IEnumerator Dead()
    {
        // Play death animation manually
        animator.Play("death");

        // Remove 'Player' LayerMask so it won't be attacked while its dead
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationTime);
    }
}
