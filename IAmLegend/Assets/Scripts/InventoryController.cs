using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    private struct MeleeRangeCollections<T>
    {
        public List<T> Melee;
        public List<T> Range;
    }

    private int _inventoryCapacity;
    private MeleeRangeCollections<GameObject> _inventoryWeapons = new MeleeRangeCollections<GameObject>();
    private MeleeRangeCollections<GameObject> _inventorySlots = new MeleeRangeCollections<GameObject>();
    private float _xAngle, _yAngle, _zAngle;
    // the active weapon is a weapon that was checked out of the inventory. Player is using it.
    private GameObject _checkedOut = null;

    [SerializeField] private GameObject inUse;
    
    private Animator _animator;
    // the dropped weapon is a weapon recently dropped. We don't want to grab it again right away.
    private GameObject _droppedWeapon;
    
    // Container to hold items in the inventory
    private Dictionary<string, GameObject[]> _weapons = new Dictionary<string, GameObject[]>();
    
    // Start is called before the first frame update
    void Start()
    {
        this._inventoryCapacity = 2;
        this._inventoryWeapons.Melee = new List<GameObject>(this._inventoryCapacity);
        this._inventoryWeapons.Range = new List<GameObject>(this._inventoryCapacity);
        this._inventorySlots.Melee = new List<GameObject>();
        this._inventorySlots.Range = new List<GameObject>();
        this._inventorySlots.Melee.Add(GameObject.Find("MeleeSlot1"));
        this._inventorySlots.Melee.Add(GameObject.Find("MeleeSlot2"));
        this._inventorySlots.Range.Add(GameObject.Find("RangeSlot1"));
        this._inventorySlots.Range.Add(GameObject.Find("RangeSlot2"));
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
        foreach (var weapon in this._inventoryWeapons.Melee)
        {
            if (weapon is null) continue;
            if (weapon == this._checkedOut) continue;
            weapon.transform.Rotate(_xAngle, _yAngle, _zAngle, Space.Self);
        }
        foreach (var weapon in this._inventoryWeapons.Range)
        {
            if (weapon is null) continue;
            if (weapon == this._checkedOut) continue;
            weapon.transform.Rotate(_xAngle, _yAngle, _zAngle, Space.Self);
        }
    }

    /// <summary>
    /// Executes when entering triggers colliders.
    /// </summary>
    /// <param name="other"></param> the game object the player has collided with.
    private void OnTriggerEnter(Collider other)
    {
        GameObject go = other.gameObject;
        // Only grab items
        if (go.layer == LayerMask.NameToLayer("Item"))
        {
            List<GameObject> container = _appropriateList(this._inventoryWeapons, go);
            if (container.Contains(go)) return;
            this._grabWeapon(go);
        }
    }

    /// <summary>
    /// Gets the ready to use range weapon.
    ///
    /// If there is an item in the second slot but not in the first, items are switched before returning.
    /// </summary>
    /// <returns>The ready to use range weapon.</returns>
    public GameObject CheckoutRangeWeapon()
    {
        if (this._inventoryWeapons.Range.Count == 0) return null;
        if (this._inventoryWeapons.Range[0] is null) this.SwitchWeapon("Range");
        this._checkedOut = this._inventoryWeapons.Range[0];
        this._updateSlots();
        return this._checkedOut;
    }

    /// <summary>
    /// Gets the ready to use melee weapon.
    ///
    /// If there is an item in the second slot but not in the first, items are switched before returning.
    /// </summary>
    /// <returns>The ready to use melee weapon.</returns>
    public GameObject CheckoutMeleeWeapon()
    {
        if (this._inventoryWeapons.Melee.Count == 0) return null;
        if (this._inventoryWeapons.Melee[0] is null) this.SwitchWeapon("Melee");
        this._checkedOut = this._inventoryWeapons.Melee[0];
        this._updateSlots();
        return this._checkedOut;
    }

    /// <summary>
    /// Stores the given weapon in the primary slot.
    ///
    /// A weapon put on hold is readly available.
    /// Putting it on hold simply means the player is using the other type of weapon.
    ///
    /// This method places the weapon in the proper place in the UI, scales and rotates it.
    /// </summary>
    public void CheckinWeapon(GameObject weapon)
    {
        if (weapon is null) return;
        List<GameObject> weaponTypeList = _appropriateList(this._inventoryWeapons, weapon);
        if (this._checkedOut == weapon)
        {
            this._checkedOut = null;
        }
        this._updateSlots();
    }

    /// <summary>
    /// Updates the UI to reflect the inventory weapons.
    /// </summary>
    private void _updateSlots()
    {
        for (int i = 0; i < 2; i++)
        {
            if (this._inventoryWeapons.Melee.Count > i && this._inventorySlots.Melee.Count > i)
            {
                if (this._inventoryWeapons.Melee[i] != this._checkedOut)
                {
                    this._addToSlot(this._inventoryWeapons.Melee[i], this._inventorySlots.Melee[i]);
                }
            }
            if (this._inventoryWeapons.Range.Count > i && this._inventorySlots.Range.Count > i)
            {
                if (this._inventoryWeapons.Range[i] != this._checkedOut)
                {
                    this._addToSlot(this._inventoryWeapons.Range[i], this._inventorySlots.Range[i]);
                }
            }
        }
    }
    
    public void SwitchWeapon(string weaponType)
    {
        List<GameObject> container;
        if (weaponType == "Melee")
        {
            container = this._inventoryWeapons.Melee;
        } else if (weaponType == "Range")
        {
            container = this._inventoryWeapons.Range;
        }
        else
        {
            return;
        }
        this._swapList(container);
        this._updateSlots();
    }

    /// <summary>
    /// Switches the first and second elements in a inventory list, given the current game object.
    ///
    /// The current game object is used to find the correct list.
    /// </summary>
    /// <param name="currentWeapon"></param>
    public void SwitchWeapon(GameObject currentWeapon)
    {
        List<GameObject> container = _appropriateList(this._inventoryWeapons, currentWeapon);
        this._swapList(container);
        this._updateSlots();
    }

    /// <summary>
    /// Swaps the first and second items in a list.
    ///
    /// If there are not more than 1 item in the list, do nothing.
    /// </summary>
    /// <param name="l"></param>
    /// <typeparam name="T"></typeparam>
    private void _swapList<T>(List<T> l)
    {
        if (l.Count < 2) return;
        T tmp = l[0];
        l[0] = l[1];
        l[1] = tmp;
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
        List<GameObject> weaponList = _appropriateList(this._inventoryWeapons, newWeapon);
        if (weaponList.Count < this._inventoryCapacity)
        {
            weaponList.Add(newWeapon);
        }
        else
        {
            Debug.Log("Inventory full.");
        }
        this._updateSlots();
    }

    /// <summary>
    /// Returns the appropriate collection for a particular game object.
    /// </summary>
    /// <param name="collection">the MeleeRangeCollections object where to look for the correct collection.</param>
    /// <param name="item">the GameObject instance where to check for the tag.</param>
    /// <returns></returns>
    private static List<GameObject> _appropriateList(MeleeRangeCollections<GameObject> collection, GameObject item)
    {
        if (item is null) return new List<GameObject>();
        if (item.tag.Contains("Melee"))
        {
            return collection.Melee;
        }
        else if (item.tag.Contains("Range"))
        {
            return collection.Range;
        }
        return new List<GameObject>();
    }

    /// <summary>
    /// Adds an item to a slot in the Inventory UI.
    ///
    /// This method does not handle removing an item from the slot.
    /// This method should not be executed every frame, but only when it is necessary to add new item to the slot.
    /// Re-adding an item will not add a new item. Adding an item to the slot means:
    ///
    /// - setting the parent of the transform object so that it shows in the correct place in the UI
    /// - transforming the object (scale, position and rotation) so that it fits in the UI
    /// </summary>
    /// <param name="item">the item to be added to the slot</param>
    /// <param name="slot">the slot to be used</param>
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
    /// Drops the given weapon.
    ///
    /// Dropping a weapon means:
    /// - moving it to a position close to the Player (changing the parent of the transform object).
    /// - transform the object to set it's scale to 1 and local rotation and position to zeros.
    /// - removing it from the inventory.
    /// - setting the dropped weapon to this._droppedWeapon so that we can avoid grabbing a recently dropped weapon.
    /// </summary>
    public void DropWeapon(GameObject weapon)
    {
        if (weapon == null) return;
        Transform tr = this.transform;
        Vector3 position = tr.position;
        // Set the parent of the dropped weapon to be the same as the parent of the player character.
        weapon.transform.SetParent(tr.parent, false);
        // Move the dropped weapon to the floor
        weapon.transform.position = new Vector3( position.x, 0, position.z - 2);
        // Remove the Dropped weapon from the inventory.
        List<GameObject> container = _appropriateList(this._inventoryWeapons, weapon);
        container.Remove(weapon);
        // Set the _droppedWeapon attribute.
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
