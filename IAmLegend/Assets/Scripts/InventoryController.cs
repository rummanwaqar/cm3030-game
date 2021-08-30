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
    private GameObject _rangeSlot1, _rangeSlot2, _meleeSlot1, _meleeSlot2;
    
    // Start is called before the first frame update
    void Start()
    {
        this._inventoryContainer = GameObject.Find("InventoryContainer");
        this._rangeSlot1 = GameObject.Find("RangeSlot1");
        this._rangeSlot2 = GameObject.Find("RangeSlot2");
        this._meleeSlot1 = GameObject.Find("MeleeSlot1");
        this._meleeSlot2 = GameObject.Find("MeleeSlot2");
        this._animator = GetComponent<Animator>();
        this._xAngle = .1f;
        this._yAngle = .2f;
        this._zAngle = .3f;
    }

    // Update is called once per frame
    void Update()
    {
        this._displayInventory();
    }

    private void _displayInventory()
    {
        GameObject[] inventory = new[]
        {
            this.rangeWeapon1,
            this.meleeWeapon1,
            this.meleeWeapon2,
            this.rangeWeapon2
        };
        foreach (var weapon in inventory)
        {
            if (weapon.Equals(null)) continue;
            weapon.transform.Rotate(_xAngle, _yAngle, _zAngle, Space.Self);
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
            if (other.gameObject.CompareTag("Weapon") || other.gameObject.CompareTag("WeaponMelee"))
            {
                this._grabWeapon(other.gameObject);
                Debug.Log("grabbed");
            }
        }
    }

    /// <summary>
    /// Gets the ready to use range weapon.
    ///
    /// If there is an item in the second slot but not in the first, items are switched before returning.
    /// </summary>
    /// <returns>The ready to use range weapon.</returns>
    public GameObject GETRangeWeapon()
    {
        if (this.rangeWeapon1.Equals(null)) this.SwitchRangeWeapon();
        return this.rangeWeapon1;
    }

    /// <summary>
    /// Gets the ready to use melee weapon.
    ///
    /// If there is an item in the second slot but not in the first, items are switched before returning.
    /// </summary>
    /// <returns>The ready to use melee weapon.</returns>
    public GameObject GETMeleeWeapon()
    {
        if (this.meleeWeapon1.Equals(null)) this.SwitchMeleeWeapon();
        return this.meleeWeapon1;
    }

    /// <summary>
    /// Switches main and secondary melee weapons.
    /// </summary>
    public void SwitchMeleeWeapon()
    {
        // Do not switch to a null weapon
        if (this.meleeWeapon2.Equals(null)) return;
        GameObject temp = this.meleeWeapon2;
        this.meleeWeapon2 = this.meleeWeapon1;
        this.meleeWeapon1 = temp;
        // If weapons are not null, add to the corresponding slot
        if (this.meleeWeapon1.Equals(null)) return;
        this._addToSlot(this.meleeWeapon1, this._meleeSlot1);
        if (this.meleeWeapon2.Equals(null)) return;
        this._addToSlot(this.meleeWeapon2, this._meleeSlot2);
    }
    
    /// <summary>
    /// Switches main and secondary range weapons.
    /// </summary>
    public void SwitchRangeWeapon()
    {
        if (this.rangeWeapon2.Equals(null)) return;
        GameObject temp = this.rangeWeapon2;
        this.rangeWeapon2 = this.rangeWeapon1;
        this.rangeWeapon1 = temp;
        // If weapons are not null, add to the corresponding slot
        if (this.rangeWeapon1.Equals(null)) return;
        this._addToSlot(this.rangeWeapon1, this._rangeSlot1);
        if (this.rangeWeapon2.Equals(null)) return;
        this._addToSlot(this.rangeWeapon2, this._rangeSlot2);
    }
    

    /// <summary>
    /// Grabs a new weapon.
    ///
    /// Replace the idle weapon of the specific type (melee or range) by the grabbed weapon.
    /// </summary>
    /// <param name="newWeapon">
    /// The weapon to grab. Should be a GameObject in the Item Layer with either Weapon or Weapon Melee tag.
    /// </param>
    private void _grabWeapon(GameObject newWeapon)
    {
        bool melee = newWeapon.tag.Contains("WeaponMelee");
        if (melee)
        {
            this._dropWeapon(this.meleeWeapon2);
            this.meleeWeapon2 = newWeapon;
        }
        else
        {
            this._dropWeapon(this.rangeWeapon2);
            this.rangeWeapon2 = newWeapon;
        }
        GameObject weapon = melee ? this.meleeWeapon2 : this.rangeWeapon2;
        GameObject slot = melee ? this._meleeSlot2 : this._rangeSlot2;
        this._addToSlot(weapon, slot);
    }

    private void _addToSlot(GameObject item, GameObject slot)
    {
        // Add newly grabbed item to the inventory slot.
        item.transform.SetParent(slot.transform, false);
        item.transform.SetPositionAndRotation(
            new Vector3(0, 0, -10),
            new Quaternion(0, 0, 0, 0)
            );
        // Reset the position and rotation of the weapon before use.
        item.transform.localPosition = new Vector3(0, 0, -10);
        item.transform.localRotation = new Quaternion(0, 0, 0, 0);
        item.transform.localScale = new Vector3(80, 80, 80);
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
