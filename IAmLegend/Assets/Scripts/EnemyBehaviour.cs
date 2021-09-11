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

    [Header("Health & Damage")]
    [SerializeField] private float deathAnimationTime;
    private HealthSystem _healthSystem;
    [SerializeField] private Slider healthBar;
    [SerializeField] private float distanceToShowHealthBar;
    [SerializeField] private GameObject player;
    [SerializeField] private int scoreWhenDead;
    private float currentDamage;

    [Header("AI")]
    [SerializeField] float delayTimeFSM;
    private NavMeshAgent agent;

    [Header("Audio")]
    [SerializeField] private AudioSource attackingSound;

    // Game manager
    private GameManager gameManager;

    // Animation
    private Animator animator;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        gameManager = (GameManager)FindObjectOfType(typeof(GameManager));
        _healthSystem = GetComponent<HealthSystem>();
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
        UpdateHealthBar(_healthSystem.GetHealth());                         // Update health bar

        // If the player is close, display health bar
        ShowHealthBar();

        if( _healthSystem.GetHealth() <= 0 )
        {
            // No health, die
            StartCoroutine(Dead());
        }
        else if( currentDamage != 0 )
        {
            // If a damage is needed to be processed
            _healthSystem.SetDamage(currentDamage);
            _healthSystem.Hit();                                            // Process damage
            currentDamage = 0;                                             // The damage was processed
        }
        else if( detectedTarget != null )
        {
            // Calculate the distance to the target
            float distanceToTarget = Mathf.Abs(Vector3.Distance(transform.position, detectedTarget.transform.position));

            if( distanceToTarget < distanceToAttack )
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
        else
        {
            Idle(); // or patrol?
        }
    }
    private void Idle()
    {
        agent.isStopped = true;
        animator.SetBool("Walking", false);
        animator.SetBool("Idle", true);
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
        agent.isStopped = true;
        animator.SetTrigger("Attacking");
        attackingSound.Play();

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

    // Collision with tag weapons to receive damage
    void OnTriggerEnter( Collider other )
    {
        switch( other.tag )
        {
            case "Bullet":
                _healthSystem.SetDamage(20);
                _healthSystem.Hit();
                break;
            default:
                _healthSystem.SetDamage(0);
                _healthSystem.Hit();
                break;
        }
    }
    private IEnumerator Dead()
    {
        // Play death animation manually
        animator.Play("zombie_death_standing");

        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationTime);

        // Delete the zombie from the scene & update score
        gameManager.AddScore(scoreWhenDead);
        Destroy(gameObject);
    }

    // If the player is close show health bar
    private void ShowHealthBar()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        if( distance < distanceToShowHealthBar )
            healthBar.gameObject.SetActive(true);
        else
            healthBar.gameObject.SetActive(false);
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
