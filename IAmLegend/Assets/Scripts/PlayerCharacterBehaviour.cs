using System.Collections.Generic;
using UnityEngine;

enum PlayerState { Walk, Run }

/// <summary>
/// Class <c>PlayerCharacterBehaviour</c> implements player character actions.
/// </summary>
///
/// <remarks>
/// It relies on PlayerState enum to set possible character states. There can be any number of states active
/// simultaneously. The <c>_setState</c> method is responsible for determine all current states and other methods use
/// <c>_state</c>.
/// </remarks>
public class PlayerCharacterBehaviour : MonoBehaviour
{
    private static readonly int IsRunning = Animator.StringToHash(("isRunning"));
    private static readonly int IsWalking = Animator.StringToHash(("isWalking"));
    private static readonly int Shoot = Animator.StringToHash(("shoot"));
    private static readonly int Bash = Animator.StringToHash(("bash"));
    
    [SerializeField] private float baseSpeed = 1.5f;
    [SerializeField] private float runBoost = 2f;
    
    private Animator _animator;
    private Camera _camera;
    private HashSet<PlayerState> _state;
    private Rigidbody _rigidbody;
    private Vector3 _lookPoint;
    private float _speed;
    private int _layerMaskFloor;
    private bool _hasPistol;


    void Start()
    {
        this._layerMaskFloor = LayerMask.GetMask("Floor");
        this._camera = Camera.main;
        this._rigidbody = GetComponent<Rigidbody>();
        this._speed = this.baseSpeed;
        this._animator = GetComponent<Animator>();
        this._state = new HashSet<PlayerState> { };
    }

    /// <summary>
    /// Executes methods that need to be executed at each frame.
    /// </summary>
    void Update()
    {
        this._setState();
        this._rotate();
        this._move();
    }

    /// <summary>
    /// Computes the states the PlayerCharacter is in.
    ///
    /// method <c>_setState</c> handles conditions that allow the PlayerCharacter
    /// to be in one or more states.
    /// </summary>
    private void _setState()
    {
        bool walk = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        bool run = Input.GetAxis("Fire3") != 0;
        if (walk && !run)
            this._state.Add(PlayerState.Walk);
        else
            this._state.Remove(PlayerState.Walk);
        if (walk && run)
            this._state.Add(PlayerState.Run);
        else
            this._state.Remove(PlayerState.Run);
    }

    /// <summary>
    /// method <c>_rotate</c> rotates the <c>PlayerCharacter</c> character so that
    /// it looks where the mouse is pointing to.
    /// </summary>
    private void _rotate()
    {
        Ray pointerRay = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(pointerRay, out RaycastHit hit, 100f, _layerMaskFloor))
        {
            this._lookPoint = hit.point;
            // Avoid rapid rotation when the cursor is over the character
            if (Vector3.Distance(this._lookPoint, this.transform.position) > 0.2f)
            {
                transform.LookAt(
                    new Vector3(
                        _lookPoint.x,
                        transform.position.y,
                        _lookPoint.z
                        )
                    );
            }
        }
    }

    /// <summary>
    /// Moves the <c>PlayerCharacter</c> forward
    /// </summary>
    private void _move()
    {
        bool isWalking = this._state.Contains(PlayerState.Walk);
        bool isRunning = this._state.Contains(PlayerState.Run);
        if (isWalking || isRunning)
        {
            float forward = Input.GetAxis("Vertical");
            float sideways = Input.GetAxis("Horizontal");
            this._speed = this.baseSpeed * (isRunning ? this.runBoost: 1);
            Vector3 movement = new Vector3(sideways, 0, forward);
            // Normalize movement vector to avoid walking faster in diagonals
            transform.Translate(
                movement.normalized *
                    (this._speed * Time.deltaTime)
                );
        } 
        this._animator.SetBool(IsWalking, isWalking);
        this._animator.SetBool(IsRunning, isRunning);
    }

}
