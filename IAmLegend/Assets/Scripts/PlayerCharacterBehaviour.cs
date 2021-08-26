using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
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
    private static readonly int HasPistol = Animator.StringToHash("hasPistol");
    private static readonly int Run = Animator.StringToHash("run");
    private static readonly int Shoot = Animator.StringToHash(("shoot"));
    private static readonly int Stop = Animator.StringToHash("stop");
    private static readonly int Walk = Animator.StringToHash("walk");
    
    public GameObject weapon;
    public GameObject meleeWeapon;
    
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
    private GameObject _handContainer;
    private HealthSystem _healthSystem;
    private static readonly int Dead = Animator.StringToHash("dead");
    private static readonly int Die = Animator.StringToHash("die");
    [SerializeField] private Slider healthBar;

    private void Start()
    {
        this._layerMaskFloor = LayerMask.GetMask("Floor");
        this._camera = Camera.main;
        this._rigidbody = GetComponent<Rigidbody>();
        this._speed = this.baseSpeed;
        this._animator = GetComponent<Animator>();
        this._state = new HashSet<PlayerState> { };
        this._animator.SetBool(HasPistol, false);
        this._handContainer =  GameObject.Find("/PlayerCharacter/Bip001/Bip001 Pelvis/Bip001 Spine/Bip001 R Clavicle/Bip001 R UpperArm/Bip001 R Forearm/Bip001 R Hand/R_hand_container").gameObject;
        this._healthSystem = GetComponent<HealthSystem>();
    }
    
    /// <summary>
    /// Executes methods that need to be executed at Physics update.
    /// </summary>
    private void FixedUpdate()
    {
        if (!this._animator.GetBool(Dead))
        {
            this._setState();
            this._rotate();
            this._move();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UsePistol()
    {
        if (this.meleeWeapon)
        {
            this.meleeWeapon.SetActive(false);
        }
        if (this.weapon)
        {
            this.weapon.SetActive(true);
            this._animator.SetBool(HasPistol, true);
        }
    }

    /// <summary>
    /// Prepare the Player Character to use a melee weapon.
    ///
    /// If there is no melee weapon, it still deactivates the pistol. Use UsePistol method to reactivate pistol.
    /// It sets the proper parent of the melee weapon and sets the trigger for melee attack animation.
    /// </summary>
    public void UseMelee()
    {
        if (this.meleeWeapon)
        {
            this.meleeWeapon.SetActive(true);
            this._animator.SetBool(HasPistol, false);
        }
        if (this.weapon)
        {
            this.weapon.SetActive(false);
        }
    }

    /// <summary>
    /// Computes the states the PlayerCharacter is in.
    ///
    /// method <c>_setState</c> handles conditions that allow the PlayerCharacter
    /// to be in one or more states.
    /// </summary>
    private void _setState()
    {
        _updateHealthBar(_healthSystem.GetHealth());      // Update health bar

        if (this._healthSystem.GetHealth() <= 0)
        {
            this._state.Remove(PlayerState.Run);
            this._state.Remove(PlayerState.Walk);
            this._state.Add(PlayerState.Die);
            this._animator.SetTrigger(Die);
            this._animator.SetBool(Dead, true);
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

    private void _updateHealthBar( float health )
    {
        float healthNormalized = (health / 100);    // Normalize the value
        healthBar.value = healthNormalized;
    }
}
