using System;
using System.Collections.Generic;
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
    private Slider _healthBar;
    private InventoryController _inventory;
    private static readonly int Forward = Animator.StringToHash("forward");
    private static readonly int Sideways = Animator.StringToHash("sideways");
    private int[] _previousMoveState = new []{0, 0, 0, 0};

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
        this._healthBar = GameObject.Find("PlayerHealthSlider").GetComponent<Slider>();
        this._inventory = GetComponent<InventoryController>();
    }
    
    /// <summary>
    /// Executes methods that need to be executed at Physics update.
    /// </summary>
    private void FixedUpdate()
    {
        if (this._animator.GetBool(Dead)) return;
        this._setState();
        this._rotate();
        this._move();
        this._chooseWeapon();
    }

    private void _chooseWeapon()
    {
        if (this.weapon.Equals(null)) return;
        bool change = Input.GetAxis("Jump") != 0;
        if (this.weapon.tag.Contains("WeaponMelee"))
        {
            this._inventory.SwitchMeleeWeapon();
        }
        else
        {
            this._inventory.SwitchRangeWeapon();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void UsePistol()
    {
        GameObject temp = this.weapon;
        this.weapon = this._inventory.GETRangeWeapon();
        if (this.weapon.Equals(null))
        {
            this.weapon = temp;
            return;
        }
        else
        {
            this._animator.SetBool(HasPistol, true);
            this.weapon.transform.SetParent(this._handContainer.transform);
            this._holdWeapon();
        }
    }

    private void _holdWeapon()
    {
        // Add newly grabbed item to the inventory slot.
        this.weapon.transform.SetParent(this._handContainer.transform, false);
        this.weapon.transform.SetPositionAndRotation(
            new Vector3(0, 0, -10),
            new Quaternion(0, 0, 0, 0)
            );
        // Reset the position and rotation of the weapon before use.
        this.weapon.transform.localPosition = new Vector3(0, 0, 0);
        this.weapon.transform.localRotation = new Quaternion(0, 0, 0, 0);
        this.weapon.transform.localScale = new Vector3(1, 1, 1);
    }

    /// <summary>
    /// Prepare the Player Character to use a melee weapon.
    ///
    /// If there is no melee weapon, it still deactivates the pistol. Use UsePistol method to reactivate pistol.
    /// It sets the proper parent of the melee weapon and sets the trigger for melee attack animation.
    /// </summary>
    public void UseMelee()
    {
        this.weapon = this._inventory.GETMeleeWeapon();
        this._animator.SetBool(HasPistol, false);
        this._holdWeapon();
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
        // Walk if any directional axis is not zero
        bool walk = Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0;
        // Only if going forwards and holding run button
        bool run = (
            Input.GetAxis("Fire3") != 0 &&
            Input.GetAxis("Vertical") > 0 &&
            Input.GetAxis("Horizontal") == 0
            );
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
        this._updateAnimator();
    }

    private void _updateAnimator()
    {
        // Collect values for forward, sideways, walk and run into integers
        // for forward and sideways negative values mean backwards and to the left, zero mean no movement.
        int forward = _floatToDirectionInt(Input.GetAxis("Vertical"));
        int sideways = _floatToDirectionInt(Input.GetAxis("Horizontal"));
        int isWalking = this._state.Contains(PlayerState.Walk) ? 1 : 0;
        int isRunning = this._state.Contains(PlayerState.Run) ? 1 : 0;
        // create an integer list that represent the current move state
        int[] currentState = new int[] { isWalking, isRunning, forward, sideways };
        // trigger stop animation if moving state has changed. This allows us to have transitions from stop to move
        // animations without the need to have transitions between each possible movement animation.
        bool resetMoveState = false;
        for (int i = 0; i < currentState.Length; i++)
        {
            if (currentState[i] != this._previousMoveState[i])
            {
                resetMoveState = true;
                break;
            }
        }
        if (resetMoveState)
        {
            this._animator.SetTrigger(Stop);
        }
        if (isWalking == 1 && isRunning == 0)
        {
            this._animator.SetTrigger(Walk);
        } else if (isRunning == 1)
        {
            this._animator.SetTrigger(Run);
        }
        else
        {
            this._animator.SetTrigger(Stop);
        }
        this._animator.SetInteger(Forward, forward);
        this._animator.SetInteger(Sideways, sideways);
        this._previousMoveState = currentState;
    }

    private static int _floatToDirectionInt(float n)
    {
        if (n == 0f) return 0;
        return (int) (n / Math.Abs(n));
    }

    private void _updateHealthBar( float health )
    {
        float healthNormalized = (health / 100);    // Normalize the value
        _healthBar.value = healthNormalized;
    }
}
