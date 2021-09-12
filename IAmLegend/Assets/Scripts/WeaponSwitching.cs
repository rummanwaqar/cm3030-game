using UnityEngine;

public class WeaponSwitching : MonoBehaviour
{
    public int selectedWeapon = 0;
    public UiInventory uiInventory;
    
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
        var previousWeapon = selectedWeapon;
        ProcessScrollInput();
        ProcessNumberKeys();
        if (selectedWeapon != previousWeapon) SelectWeapon();
    }

    private void AddWeaponToUi(Transform weapon, int index)
    {
        uiInventory.SetSlotImage(weapon.GetComponent<Gun>().uiIcon, index);
    }

    private void SelectWeapon()
    {
        var index = 0;
        foreach (Transform weapon in transform)
        {
            // set weapon to active or inactive
            weapon.gameObject.SetActive(index == selectedWeapon);
            index++;
        }
        uiInventory.SetSlotAsActive(selectedWeapon);
    }

    private void ProcessScrollInput()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            selectedWeapon++;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            selectedWeapon--;
        }
        // wrap around
        if (selectedWeapon > transform.childCount - 1) selectedWeapon = 0;
        if (selectedWeapon < 0) selectedWeapon = transform.childCount - 1;
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
                selectedWeapon = i;
            }
        }
}
}
