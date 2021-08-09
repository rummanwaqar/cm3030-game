using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

enum PlayerState { Walk, Run, Die }

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
    public void TakeDamage(float damage)
    {
        this.healthPoints -= damage;
    }
    
    [SerializeField] private float baseSpeed = 1.5f;
    [SerializeField] private float runBoost = 2f;
    [SerializeField] private float healthPoints = 100;
    public GameObject weapon;
    
    private Animator _animator;
    private Camera _camera;
    private HashSet<PlayerState> _state;
    private Rigidbody _rigidbody;
    private Vector3 _lookPoint;
    private float _speed;
    private int _layerMaskFloor;
    private bool _hasPistol;
    private static readonly int IsDead = Animator.StringToHash("isDead");
    private static readonly int HasPistol = Animator.StringToHash("hasPistol");
    private GameObject _droppedWeapon;
    private static readonly int Die = Animator.StringToHash("die");
    private static readonly int Walk = Animator.StringToHash("walk");
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int Shoot = Animator.StringToHash(("shoot"));
    private static readonly int Stop = Animator.StringToHash("stop");


    void Start()
    {
        this._layerMaskFloor = LayerMask.GetMask("Floor");
        this._camera = Camera.main;
        this._rigidbody = GetComponent<Rigidbody>();
        this._speed = this.baseSpeed;
        this._animator = GetComponent<Animator>();
        this._state = new HashSet<PlayerState> { };
        this._animator.SetBool(HasPistol, false);
    }
    
    /// <summary>
    /// Executes methods that need to be executed at each frame.
    /// </summary>
    void Update()
    {
        if (!this._state.Contains(PlayerState.Die))
        {
            this._setState();
            this._rotate();
            this._move();
            this._debugBehaviour();
        }
        else
        {
            this._animator.SetTrigger(Die);
        }
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if (other.gameObject.CompareTag("Weapon"))
            {
                this._handleCollideWeapon(other.gameObject);
            }
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.gameObject == this._droppedWeapon)
        {
            this._droppedWeapon = null;
        }
    }

    
    /// <summary>
    /// Grab items dropped on the floor.
    /// </summary>
    /// <param name="droppedWeapon"></param>
    private void _grabWeapon(GameObject droppedWeapon)
    {
        if (this.weapon == null && droppedWeapon != this._droppedWeapon)
        {
            if (droppedWeapon.tag.Contains("Weapon"))
            {
                this.weapon = droppedWeapon;
                this.weapon.transform.SetPositionAndRotation(
                    new Vector3(0, 0, 0),
                    new Quaternion(0, 0, 0, 0)
                    );
                // Add newly grabbed item to the right hand container.
                GameObject handContainer = GameObject.Find(
                    "PlayerCharacter/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/R_hand_container"
                    );
                this.weapon.transform.SetParent(handContainer.transform, false);
                this._animator.SetBool(HasPistol, true);
            }
        }
    }

    private void _dropWeapon()
    {
        Transform tr = this.transform;
        Vector3 position = tr.position;
        if (this.weapon != null)
        {
            // Set the parent of the dropped weapon to be the same as the parent of the player character.
            this.weapon.transform.SetParent(tr.parent, false);
            // Move the dropped weapon to the floor
            this.weapon.transform.position = new Vector3( position.x, 0, position.z - 2);
        }
        this._droppedWeapon = this.weapon;
        this.weapon = null;
        this._animator.SetBool(HasPistol, false);
    }

    /// <summary>
    /// Computes the states the PlayerCharacter is in.
    ///
    /// method <c>_setState</c> handles conditions that allow the PlayerCharacter
    /// to be in one or more states.
    /// </summary>
    private void _setState()
    {
        if (this.healthPoints <= 0)
        {
            this._state.Remove(PlayerState.Run);
            this._state.Remove(PlayerState.Walk);
            this._state.Add(PlayerState.Die);
            this._animator.SetTrigger(Die);
            return;
        }
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
            if (isWalking) this._animator.SetTrigger(Walk);
            if (isRunning) this._animator.SetTrigger(Run);
        }
        else
        {
            this._animator.SetTrigger(Stop);
        }

    }

    private void _debugBehaviour()
    {
        Ray pointerRay = _camera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(pointerRay, out RaycastHit hit, 100f))
        {
            this._lookPoint = hit.point;
            // Avoid rapid rotation when the cursor is over the character
            if (Vector3.Distance(this._lookPoint, this.transform.position) > 0.2f)
            {
                if (hit.collider != null && hit.collider.gameObject == this.gameObject)
                {
                    if (Input.GetAxisRaw("Fire1") != 0f) this.TakeDamage(10);
                }
            }
        }
    }

    private void _handleCollideWeapon(GameObject droppedWeapon)
    {
        this._dropWeapon();
        this._grabWeapon(droppedWeapon.gameObject);
    }


}
