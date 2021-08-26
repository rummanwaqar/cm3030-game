using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [SerializeField] private GameObject meleeWeapon1;
    [SerializeField] private GameObject meleeWeapon2;
    [SerializeField] private GameObject rangeWeapon1;
    [SerializeField] private GameObject rangeWeapon2;
    private float _xAngle, _yAngle, _zAngle;

    [SerializeField] private GameObject inUse;
    
    private Animator _animator;
    private GameObject _droppedWeapon;
    private bool _hasPistol;
    
    // Container to hold items in the inventory
    private GameObject _inventoryContainer;
    private GameObject _slot1;
    private GameObject _slot2;
    
    // Start is called before the first frame update
    void Start()
    {
        this._inventoryContainer = GameObject.Find("InventoryContainer");
        this._slot1 = GameObject.Find("Slot1");
        this._slot2 = GameObject.Find("Slot2");
        this._animator = GetComponent<Animator>();
        this._xAngle = 1f;
        this._yAngle = 1f;
        this._zAngle = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.meleeWeapon2)
        {
            this.meleeWeapon2.transform.Rotate(_xAngle, _yAngle, _zAngle, Space.Self);
        }

    }
    
    /// <summary>
    /// Executes when entering triggers colliders.
    /// </summary>
    /// <param name="other"></param> the game object the player has collided with.
    private void OnTriggerEnter(Collider other)
    {
        // Only grab items
        if (other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            if (other.gameObject.CompareTag("Weapon"))
            {
                this._grabRangeWeapon(other.gameObject);
            } else if (other.gameObject.CompareTag("WeaponMelee"))
            {
                this._grabMeleeWeapon(other.gameObject);
            }
        }
    }

    private void _storeWeapon(GameObject weapon, int slot)
    {
        
    }
    
    private void _grabMeleeWeapon(GameObject newMeleeWeapon)
    {
        this._dropWeapon(this.meleeWeapon2);
        if (newMeleeWeapon.tag.Contains("WeaponMelee"))
        {
            this.meleeWeapon2 = newMeleeWeapon;
            // Add newly grabbed item to the inventory slot.
            this.meleeWeapon2.transform.SetParent(this._slot2.transform, false);
            // Reset the position and rotation of the weapon before use.
            this.meleeWeapon2.transform.SetPositionAndRotation(
                new Vector3(0, 0, 0),
                new Quaternion(0, 0, 0, 0)
                );
            this.meleeWeapon2.transform.localPosition = new Vector3(0, 0, 0);
            this.meleeWeapon2.transform.localRotation = new Quaternion(0, 0, 0, 0);
            this.meleeWeapon2.transform.localScale = new Vector3(80, 80, 80);
            // add weapon to the UI layer
            this.meleeWeapon2.layer = 5;
        }
    }
    
    /// <summary>
    /// Handles the consequences of colliding with weapon items.
    ///
    /// This method is responsible for deciding whether to grab an item and orchestrate the consequences.
    /// </summary>
    /// <param name="newWeapon"></param>
    private void _grabRangeWeapon(GameObject newWeapon)
    {
        this._dropWeapon(this.rangeWeapon2);
        if (this.rangeWeapon2 == null && newWeapon != this._droppedWeapon)
        {
            if (newWeapon.tag.Contains("Weapon"))
            {
                this.rangeWeapon2 = newWeapon;
                this.rangeWeapon2.transform.SetPositionAndRotation(
                    new Vector3(0, 0, 0),
                    new Quaternion(0, 0, 0, 0)
                    );
                this.rangeWeapon2.transform.localScale = new Vector3(80, 80, 80);
                // Add newly grabbed item to the inventory slot.
                this.rangeWeapon2.transform.SetParent(
                    this._slot1.transform
                    );
            }
        }
    }

    /// <summary>
    /// Drops the current weapon.
    /// </summary>
    private void _dropWeapon(GameObject weapon)
    {
        if (weapon == null) return;
        Transform tr = this.transform;
        Vector3 position = tr.position;
        // Set the parent of the dropped weapon to be the same as the parent of the player character.
        weapon.transform.SetParent(tr.parent, false);
        // Move the dropped weapon to the floor
        weapon.transform.position = new Vector3( position.x, 0, position.z - 2);
        this._droppedWeapon = weapon;
    }
    
    /// <summary>
    /// Executes when leaving a trigger collider.
    /// </summary>
    /// <param name="other"></param> the game object the player is leaving.
    private void OnTriggerExit(Collider other)
    {
        // Avoid grabbing the same weapon we just dropped.
        if (other.gameObject.gameObject == this._droppedWeapon)
        {
            this._droppedWeapon = null;
        }
    }
}
