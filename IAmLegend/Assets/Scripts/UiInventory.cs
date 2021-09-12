using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

struct ImageWithTransform
{
    public Image image;
    public RectTransform transform;

    public ImageWithTransform(Image i, RectTransform t)
    {
        image = i;
        transform = t;
    }
}

public class UiInventory : MonoBehaviour
{
    public Transform weaponSlotTemplate;

    private const int Padding = 3;
    private const int NumSlots = 4;
    
    private Vector2 _weaponSlotSize;
    private List<Image> _backgroundImages;
    private List<ImageWithTransform> _weaponImages;

    public void SetSlotAsActive(int slotIndex)
    {
        const float activeOpacity = 200f / 255f;
        const float inactiveOpacity = 50f / 255f;

        var index = 0;
        foreach (var image in _backgroundImages)
        {
            image.color = new Color(image.color.r, image.color.g, image.color.b,
                index == slotIndex ? activeOpacity : inactiveOpacity);
            index++;
        }
    }

    public void SetSlotImage(Sprite sprite, int slotIndex)
    {
        if (slotIndex >= _weaponImages.Count) return;
        var imageWithTransform = _weaponImages[slotIndex];
        imageWithTransform.image.enabled = true;
        imageWithTransform.image.sprite = sprite;
        var width = _weaponSlotSize.x - 10;
        var height = width * sprite.rect.height / sprite.rect.width;
        imageWithTransform.transform.sizeDelta = new Vector2(width, height);
    }

    private void UnsetSlotImage(int slotIndex)
    {
        _weaponImages[slotIndex].image.enabled = false;
    }

    private void Awake()
    {
        _weaponSlotSize = weaponSlotTemplate.GetComponent<RectTransform>().sizeDelta;
        _backgroundImages = new List<Image>();
        _weaponImages = new List<ImageWithTransform>();
        
        ResizeAndPositionSelf();
        CreateWeaponSlots();
    }

    private void ResizeAndPositionSelf()
    {
        // calculate size UiInventory object
        var width = _weaponSlotSize.x * NumSlots + Padding * (NumSlots + 1);
        var height = _weaponSlotSize.y + Padding * 2;
        // calculate position of UiInventory object (anchored at bottom right)
        var x = -width - Padding;
        var y = Padding;
        // set size and position
        var selfRectTransform = this.GetComponent<RectTransform>();
        selfRectTransform.sizeDelta = new Vector2(width, height);
        selfRectTransform.anchoredPosition = new Vector2(x, y);
    }
    
    private void CreateWeaponSlots()
    {
        for (var i = 0; i < NumSlots; i++)
        {
            var weaponSlotTransform = Instantiate(weaponSlotTemplate, transform);
            // position slot
            var weaponSlotRectTransform = weaponSlotTransform.GetComponent<RectTransform>();
            var x = (_weaponSlotSize.x / 2 + Padding) + (_weaponSlotSize.x + Padding) * i;
            weaponSlotRectTransform.anchoredPosition = new Vector2(x, 0f);
            // set text
            weaponSlotTransform.Find("slotText").GetComponent<TMPro.TextMeshProUGUI>().SetText("" + (i + 1));
            // add background image to a list (used to show which slot is active)
            _backgroundImages.Add(weaponSlotTransform.Find("background").GetComponent<Image>());
            // add weapon image to a list
            var weaponImage = weaponSlotTransform.Find("weaponImage");
            _weaponImages.Add(new ImageWithTransform(weaponImage.GetComponent<Image>(), 
                weaponImage.GetComponent<RectTransform>()));
        }
    }
}
