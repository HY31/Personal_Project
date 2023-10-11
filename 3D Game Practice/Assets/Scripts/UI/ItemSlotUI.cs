using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    public Button button;
    public Image icon;
    public TextMeshProUGUI quantityText;
    private ItemSlot curSlot;
    private Outline outline;

    public int index;
    public bool equipped;

    private void Awake()
    {
        outline = GetComponent<Outline>();
    }

    public void OnEnable()
    {
        outline.enabled = equipped; // equipped가 있으면 아웃라인을 켜주겠다는 뜻
    }

    public void Set(ItemSlot slot)  // slot이라는 값을 전달해주면 아이템 슬롯을 세팅하는 메서드 - ui 업데이트에 쓰임
    {
        curSlot = slot;
        icon.gameObject.SetActive(true); 
        icon.sprite = slot.item.icon;  // 아이콘 스프라이트 셋팅
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty; // 아이템 개수 업데이트

        if(outline != null)
        {
            outline.enabled = equipped;
        }
    }

    public void Clear()
    {
        curSlot = null;
        icon.gameObject.SetActive(false); 
        quantityText.text = string.Empty;
    }

    public void OnButtonClick()
    {
        Inventory.instance.SelectItem(index);
    }
}
