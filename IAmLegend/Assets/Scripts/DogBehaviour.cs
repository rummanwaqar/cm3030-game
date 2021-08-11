using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum DogState { Walk, Attack, nearPlayer, Hit, Dead }

public class DogBehaviour : MonoBehaviour
{
    [SerializeField] GameObject followTarget;

    private static readonly int IsWalking = Animator.StringToHash(("isWalking"));
    private static readonly int IsNearPlayer = Animator.StringToHash(("isNearPlayer"));
    private static readonly int IsAttacking = Animator.StringToHash(("isAttacking"));
    private static readonly int IsHit = Animator.StringToHash(("isHit"));
    private static readonly int IsDead = Animator.StringToHash(("isDead"));
    
    private Animator animator;
    private NavMeshAgent agent;
    private HashSet<DogState> state;

    private Vector3 posPlayer;
    private Vector3 posDog;

    // Distance between the player and the dog which the dog will go up and follow him
    [SerializeField] private float radiusToFollow;
    // Distance between the player and the dog which the dog will sit
    [SerializeField] private float radiusToSit;
    // Distance between the player and the enemies to attack
    [SerializeField] private float radiusToAttack;

    // Start is called before the first frame update
    void Start()
    {
        this.animator = GetComponent<Animator>();
        this.state = new HashSet<DogState> { };
        this.agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        this.SetState();
        this.Move();
    }

    private void SetState()
    {
        // Get the current position of the player and the dog
        posPlayer = followTarget.transform.position;
        posDog = transform.position;

        // Using the distance formula to calculate each axis
        float distanceX = Mathf.Pow(posPlayer.x - posDog.x, 2);
        float distanceZ = Mathf.Pow(posPlayer.z - posDog.z, 2);
        float distance = Mathf.Pow(distanceX + distanceZ, 0.5f);

        // If the distance radius is bigger than 'radiusToIdle', the dog shall go to his master
        if( distance >= radiusToFollow )
            this.state.Add(DogState.Walk);

        // If the dog is close enough to the player, the dog shall sit
        if( distance <= radiusToSit )
        {
            this.state.Add(DogState.nearPlayer);
            this.state.Remove(DogState.Walk);
        }
        else
            this.state.Remove(DogState.nearPlayer);
    }

    private void Move()
    {
        // Update the logic state based on _setState()
        bool isWalking = this.state.Contains(DogState.Walk);
        bool isNearPlayer = this.state.Contains(DogState.nearPlayer);

        if( isWalking )
        {
            // Pathfinding algorithm (the terrain must be baked first)
            Vector3 offset = new Vector3(1f, 0, 1f);  // to avoid collisions with the player
            agent.SetDestination(posPlayer - offset);
        }

        // Update animations
        this.animator.SetBool(IsWalking, isWalking);
        this.animator.SetBool(IsNearPlayer, isNearPlayer);
    }
}
