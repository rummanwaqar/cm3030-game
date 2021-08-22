using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

internal enum MeleeState { Idle, Bashing};

public class MeleeWeaponController : MonoBehaviour
{

    private Animator _animator;
    private PlayerCharacterBehaviour _characterBehaviour;
    private MeleeState _meleeState;

    private static readonly int Bash = Animator.StringToHash("bash");
    // Start is called before the first frame update
    void Start()
    {
        this._animator = GetComponent<Animator>();
        this._characterBehaviour = GetComponent<PlayerCharacterBehaviour>();
        this._meleeState = MeleeState.Idle;
    }

    /// <summary>
    /// Executes at Physics update
    ///
    /// Checks if the meleeWeapon has collided with an object 
    /// </summary>
    private void FixedUpdate()
    {
        if (Input.GetAxis("Fire2") != 0f)
        {
            this._setState();
            if (this._meleeState == MeleeState.Idle)
            {
                this._bash();
            }
        }
    }

    /// <summary>
    /// Handles collision between the MeleeWeapon and enemies.
    ///
    /// It checks if the melee weapon (with such tag) has collided with an enemy (with EnemyBehaviourScript) and if so
    /// deals damage to it passing the weapon as an argument.
    ///
    /// It uses the EnemyBehaviour script to check for enemies instead of tags because the absence of the script would
    /// imply we couldn't trigger the TakeDamage method we are expecting.
    /// This method is dependent on the existence of such method in the enemy behaviour script.
    /// </summary>
    /// <param name="other"></param> the collision object.
    private void OnCollisionEnter(Collision other)
    {
        if (this._meleeState != MeleeState.Bashing) return;
        if (other.gameObject.GetComponent<EnemyBehaviour>() == null) return;
        foreach (ContactPoint cp in other.contacts)
        {
            Collider c = cp.thisCollider;
            if (!c.CompareTag("")) continue;
            // Inflict the damage.
            // EnemyBehaviour is responsible for computing the damage. This is simply a trigger.
            c.GetComponent<EnemyBehaviour>().TakeDamage(c.gameObject);
        }
    }

    private void _bash()
    {
        AnimatorStateInfo info = this._animator.GetCurrentAnimatorStateInfo(0);
        this._characterBehaviour.UseMelee();
        this._animator.SetTrigger(Bash);
    }

    /// <summary>
    /// Sets the current state of the bashing
    /// </summary>
    private void _setState()
    {
        AnimatorStateInfo info = this._animator.GetCurrentAnimatorStateInfo(0);
        this._meleeState = info.IsName("m_melee_combat_attack_A") ? MeleeState.Bashing : MeleeState.Idle;
    }
}
