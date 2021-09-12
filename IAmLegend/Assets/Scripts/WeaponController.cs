using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public UiInventory uiInventory;
    public UiAction actionUi;

    private Gun _selectedWeapon = null;
    private int _selectedWeaponIndex = 0;
    private GameObject _pickUpObject = null;

    private void Start()
    {
        SelectWeapon();
        // set images for guns
        // todo: this is temp
        var index = 0;
        foreach (Transform weapon in transform)
        {
            // set weapon to active or inactive
            AddWeaponToUi(weapon, index);
            index++;
        }
    }

    private void Update()
    {
        // change weapons with scroll or number keys
        var previousWeapon = _selectedWeaponIndex;
        ProcessScrollInput();
        ProcessNumberKeys();
        if (_selectedWeaponIndex != previousWeapon) SelectWeapon();
        
        // shoot
        if (Input.GetButton("Fire1") && _selectedWeapon != null)
        {
            _selectedWeapon.Shoot();
        }
        
        // pick up objects
        if (Input.GetKeyDown(KeyCode.E) && _pickUpObject != null)
        {
            PickUpObject();
        }
    }

    // triggered by Player OnTriggerEnter
    public void PickUpInteraction(GameObject pickUpObject)
    {
        if (pickUpObject.layer == LayerMask.NameToLayer("Item"))
        {
            var gun = pickUpObject.GetComponent<Gun>();
            if (gun == null) return;

            var gunName = gun.gunName;
            
            // if object already exists do nothing
            var gunFromInventory = GetGunByName(gunName);
            if (gunFromInventory != null)
            {
                actionUi.ShowNoActionMessage(gunName + " already in inventory");
            } else if (IsInventoryFull())
            {
                actionUi.ShowNoActionMessage("Inventory full");
            }
            else // add to inventory and select it
            {
                actionUi.ShowActionMessage("Pick up " + gunName);
                _pickUpObject = pickUpObject;
            }
        }
    }
    
    // triggered by Player OnTriggerExit
    public void PickUpInteractionComplete(GameObject pickUpObject)
    {
        if (pickUpObject.layer == LayerMask.NameToLayer("Item"))
        {
            actionUi.Hide();
            _pickUpObject = null;
        }
    }

    private void PickUpObject()
    {
        _pickUpObject.transform.SetParent(transform);
        _pickUpObject.transform.localPosition = new Vector3(0, 0, 0.308f);
        _pickUpObject.transform.localRotation = Quaternion.identity;
        var index = transform.childCount - 1;
        _selectedWeaponIndex = index;
        AddWeaponToUi(_pickUpObject.transform, index);
        SelectWeapon();
        // since we pick up the object onTriggerExit is not called. We need to call it manually
        PickUpInteractionComplete(_pickUpObject);
    }

    private void AddWeaponToUi(Transform weapon, int index)
    {
        uiInventory.SetSlotImage(weapon.GetComponent<Gun>().uiIcon, index);
    }

    private Gun GetGunByName(string gunName)
    {
        foreach (Transform weapon in transform)
        {
            var gun = weapon.GetComponent<Gun>();
            if (gun != null && gun.gunName.Equals(gunName)) return gun;
        }

        return null;
    }

    private bool IsInventoryFull()
    {
        return uiInventory.numSlots == transform.childCount;
    }

    private void SelectWeapon()
    {
        var index = 0;
        _selectedWeapon = null; // reset selected weapon
        foreach (Transform weapon in transform)
        {
            weapon.gameObject.SetActive(index == _selectedWeaponIndex);
            if (index == _selectedWeaponIndex)
            {
                _selectedWeapon = weapon.GetComponent<Gun>();
            }
            index++;
        }
        uiInventory.SetSlotAsActive(_selectedWeaponIndex);
    }

    private void ProcessScrollInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            _selectedWeaponIndex++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            _selectedWeaponIndex--;
        }
        // wrap around
        if (_selectedWeaponIndex > transform.childCount - 1) _selectedWeaponIndex = 0;
        if (_selectedWeaponIndex < 0) _selectedWeaponIndex = transform.childCount - 1;
    }

    private void ProcessNumberKeys()
    {
        KeyCode[] keyCodes = {
            KeyCode.Alpha1,
            KeyCode.Alpha2,
            KeyCode.Alpha3,
            KeyCode.Alpha4,
            KeyCode.Alpha5,
            KeyCode.Alpha6,
            KeyCode.Alpha7,
            KeyCode.Alpha8,
            KeyCode.Alpha9,
        };
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]) && transform.childCount >= i)
            {
                _selectedWeaponIndex = i;
            }
        }
}
}
