using System.Collections;
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
    private GameObject nearestSafeLoc;                  // The dog can run to the nearest safe location
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
    private bool dogSit;

    [Header("Health & Damage")]
    [SerializeField] private float deathAnimationTime;
    [SerializeField] private HealthSystem healthSystem;
    [SerializeField] private Slider healthBar;
    [SerializeField] private float recoverAmount;
    [SerializeField] private float recoveringCountdown;
    [SerializeField] private float recoverRate;
    private float currentDamage;

    // Animation
    private Animator animator;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        dogSit = false;
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
        Collider[] targets = Physics.OverlapSphere(transform.position, distanceSight, targetsLayers);

        detectedTarget = null;
        for( int i = 0; i < targets.Length; i++ )
        {
            Collider target = targets[i];

            // Calculate the distance to the target
            Vector3 directionToTarget = Vector3.Normalize(target.bounds.center - transform.position);

            // Calculate the angle view between the AI and the target
            float angleToTarget = Vector3.Angle(transform.forward, directionToTarget);

            // Check if the target is in the view angle of the AI
            if( angleToTarget < angleView )
            {
                // Check if there are any obstacles between the AI and the target
                if( !Physics.Linecast(transform.position, target.bounds.center, obstaclesLayers) )
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
        float distanceFromPlayer = Vector3.Distance(transform.position, followTarget.transform.position);
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
        else if( distanceFromPlayer >= radiusToStartFollow && dogSit == false )
        {
            // Walk to the player
            FollowPlayer(followTarget.transform.position);
        }
        else if( dogSit )
        {
            // Sit + health will regenerate
            Idle();
            RegenerateHealth();
        }
        else if( detectedTarget != null && dogSit == false )
        {
            // Calculate the distance to the target
            Vector3 posDog = transform.position;
            Vector3 postTarget = detectedTarget.transform.position;
            float distanceToTarget = Mathf.Abs(Vector3.Distance(posDog, postTarget));

            if( distanceToTarget < distanceToAttack && dogSit == false )
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
            // Health regenerate while Idle
            RegenerateHealth();
        }
    }

    private void Idle()
    {
        // Update animations
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", true);
    }

    private void FollowPlayer( Vector3 _posPLayer )
    {
        // Update animations
        agent.isStopped = false;
        animator.SetBool("Idle", false);
        animator.SetBool("Walking", true);

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

        // If the target is close, deal damange
        DealDamage();
    }

    private void DealDamage()
    {
        // Countdown between attacks
        if( attackingCountdown <= 0 )
        {
            detectedTarget.SetDamage(damagePower);
            detectedTarget.Hit();                                           // Process damage
            attackingCountdown = 1 / attackingRate;
        }
        attackingCountdown -= Time.deltaTime;
    }

    public void SetSitState( bool state )
    {
        // Set the dong's runaway state
        dogSit = state;
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

    private void RegenerateHealth()
    {
        healthSystem.SetRecover(recoverAmount);
        // Countdown between health recovering
        if( recoveringCountdown <= 0  && healthSystem.GetHealth() < 100)
        {
            healthSystem.Recover();
            recoveringCountdown = 1 / recoverRate;
        }
        recoveringCountdown -= Time.deltaTime;
    }

    private IEnumerator Dead()
    {
        agent.isStopped = true;

        // Play death animation manually
        animator.Play("death");

        // Remove 'Player' LayerMask so it won't be attacked while its dead
        gameObject.layer = LayerMask.NameToLayer("Default");

        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationTime);
    }
}
