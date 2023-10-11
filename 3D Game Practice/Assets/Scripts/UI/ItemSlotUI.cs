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
        outline.enabled = equipped; // equipped�� ������ �ƿ������� ���ְڴٴ� ��
    }

    public void Set(ItemSlot slot)  // slot�̶�� ���� �������ָ� ������ ������ �����ϴ� �޼��� - ui ������Ʈ�� ����
    {
        curSlot = slot;
        icon.gameObject.SetActive(true); 
        icon.sprite = slot.item.icon;  // ������ ��������Ʈ ����
        quantityText.text = slot.quantity > 1 ? slot.quantity.ToString() : string.Empty; // ������ ���� ������Ʈ

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
