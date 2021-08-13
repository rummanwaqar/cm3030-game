using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

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
    [SerializeField] private float distanceAttack;
    [SerializeField] private float angleView;
    [SerializeField] private LayerMask targetsLayers;
    [SerializeField] private LayerMask obstaclesLayers;
    private Collider detectedTarget;
    private bool attackTarget;

    [Header("Health & Damage")]
    [SerializeField] private float zombieDamage;
    [SerializeField] private float deathAnimationTime;
    [SerializeField] private float health;

    // Animation
    private Animator animator;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        this.animator = GetComponent<Animator>();
        this.agent = GetComponent<NavMeshAgent>();
        attackTarget = false;
    }

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

    private IEnumerator UpdateState()
    {
        // Wait some time to prevent overloading
        yield return new WaitForSeconds(delayTimeFSM);

        float distanceFromPlayer = GetDistanceFromPlayer();

        if( this.health <= 0 )
        {
            // No health, die
            StartCoroutine(Dead());
        }
        else if( distanceFromPlayer >= radiusToStartFollow )
        {
            // Walk to the player
            Walk(followTarget.transform.position);
        }
        else if( this.detectedTarget != null && attackTarget == true )
        {
            // If a target was detected, attack
            Attack();
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
        this.agent.isStopped = true;
        this.animator.SetBool("Walking", false);
        this.animator.SetBool("Idle", true);
    }

    private void Walk(Vector3 _posPLayer)
    {
        // Update animations
        this.agent.isStopped = false;
        this.animator.SetBool("Idle", false);
        this.animator.SetBool("Walking", true);

        // Pathfinding algorithm
        Vector3 offset = new Vector3(1f, 0, 1f);  // Avoid collisions with the player
        agent.SetDestination(_posPLayer - offset);
    }

    private void Attack()
    {
        Debug.Log("Attacking");

        // If the target is close, deal damange
        DealDamage();
    }

    private void DealDamage()
    {
        // Call external functions: Target.damage()
    }

    private void ReceiveDamage(float damage)
    {
        Debug.Log("Damage");
    }

    private IEnumerator Dead()
    {
        // Play death animation manually
        animator.Play("death");

        // Wait for the death animation to finish
        yield return new WaitForSeconds(deathAnimationTime);
    }
}
