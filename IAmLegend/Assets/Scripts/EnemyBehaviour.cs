using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    private enum EnemyState { Idle, ChaseTarget, AttackTarget }
   
    [SerializeField] private float distanceSight;
    [SerializeField] private float angleView; // both left & right
    [SerializeField] private LayerMask targetsLayers;
    [SerializeField] private LayerMask obstaclesLayers;

    private EnemyState enemyCurrentState;
    private Collider detectedTarget;
    private NavMeshAgent agent;
    private Animator animator;

    public void TakeDamage(GameObject weapon)
    {
        Debug.Log(weapon.name);
    }

    // Start is called before the first frame update
    void Awake()
    {
        this.agent = GetComponentInParent<NavMeshAgent>();
        this.animator = GetComponentInParent<Animator>();
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

    private void UpdateState()
    {
        if( enemyCurrentState == EnemyState.Idle )
            Idle();
        else if( enemyCurrentState == EnemyState.ChaseTarget )
            ChaseTarget();
        else if( enemyCurrentState == EnemyState.AttackTarget )
            AttackTarget();
    }

    private void Idle()
    {
        // If target was detected, change state
        if( this.detectedTarget != null )
        {
            this.enemyCurrentState = EnemyState.ChaseTarget;
            return;
        }

        this.agent.isStopped = true;
        this.animator.SetTrigger("Idle");
    }

    private void ChaseTarget()
    {
        // If target wasn't detected, change state
        if( this.detectedTarget == null )
        {
            this.enemyCurrentState = EnemyState.Idle;
            return;
        }

        this.agent.isStopped = false;
        this.animator.SetTrigger("Walking");

        // Calculate the distance to the target
        float distanceToTarget = Vector3.Distance(this.transform.position, this.detectedTarget.transform.position);
        this.agent.SetDestination(this.detectedTarget.transform.position);
        
        // TODO: if near target -> attack.
    }

    private void AttackTarget()
    {
        // If target wasn't detected, change state
        if( this.detectedTarget == null )
        {
            this.enemyCurrentState = EnemyState.Idle;
            return;
        }
        // else if(target runs) -> chase him

        this.agent.isStopped = true;
        this.animator.SetTrigger("Attacking");
    }

    //// Debugging variables with visual gizmos
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(transform.position, distanceSight);
    //}
}
