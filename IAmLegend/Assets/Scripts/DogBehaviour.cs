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
    
    private Animator _animator;
    private HashSet<DogState> _state;

    private Vector3 _posPlayer;
    private Vector3 _posDog;
    private NavMeshAgent _agent;

    // Start is called before the first frame update
    void Start()
    {
        this._animator = GetComponent<Animator>();
        this._state = new HashSet<DogState> { };
        this._agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        this._setState();
        this._move();
    }

    private void _setState()
    {
        // Get the current position of the player and the dog
        _posPlayer = followTarget.transform.position;
        _posDog = transform.position;

        // Using the distance formula to calculate each position
        float distanceX = Mathf.Pow(_posPlayer.x - _posDog.x, 2);
        float distanceZ = Mathf.Pow(_posPlayer.z - _posDog.z, 2);
        float distance = Mathf.Pow(distanceX + distanceZ, 0.5f);

        // If the distance radius is bigger than 5, the dog shall go to his master
        if( distance >= 5 )
            this._state.Add(DogState.Walk);

        // If the dog is close enough to the player, the dog shall sit
        if( distance <= 1.5f )
        {
            this._state.Add(DogState.nearPlayer);
            this._state.Remove(DogState.Walk);
        }
        else
            this._state.Remove(DogState.nearPlayer);
    }

    private void _move()
    {
        // Update the logic state based on _setState()
        bool isWalking = this._state.Contains(DogState.Walk);
        bool isNearPlayer = this._state.Contains(DogState.nearPlayer);

        if( isWalking )
        {
            // Pathfinding algorithm (the terrain must be baked first)
            Vector3 offset = new Vector3(1f, 0, 1f);  // to avoid collisions with the player
            _agent.SetDestination(_posPlayer - offset);
        }

        // Update animations
        this._animator.SetBool(IsWalking, isWalking);
        this._animator.SetBool(IsNearPlayer, isNearPlayer);
    }
}
