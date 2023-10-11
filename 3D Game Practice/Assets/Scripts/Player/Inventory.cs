using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using static UnityEditor.Progress;

public class ItemSlot
{
    public ItemData item;
    public int quantity;
}

public class Inventory : MonoBehaviour
{
    public ItemSlotUI[] uiSlots;
    public ItemSlot[] slots;

    public GameObject inventoryWindow;
    public Transform dropPosition;

    [Header("Selected Item")]
    private ItemSlot selectedItem;
    private int selectedItemIndex;
    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDescription;
    public TextMeshProUGUI selectedItemStatNames;
    public TextMeshProUGUI selectedItemStatValues;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unEquipButton;
    public GameObject dropButton;

    private int curEquipIndex;

    private PlayerController controller;
    private PlayerConditions condition;

    [Header("Events")]
    public UnityEvent onOpenInventory;
    public UnityEvent onCloseInventory;

    public static Inventory instance;

    void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerConditions>();
    }

    private void Start()
    {
        inventoryWindow.SetActive(false);
        slots = new ItemSlot[uiSlots.Length];

        for (int i = 0; i < slots.Length; i++)  // 초기화
        {
            slots[i] = new ItemSlot();
            uiSlots[i].index = i;
            uiSlots[i].Clear();
        }

        ClearSelectItemWindow();
    }

    public void OnInventoryButton(InputAction.CallbackContext callbackContext)  // 인벤토리 창을 여는 키를 누르면 동작하는 메서드
    {
        if(callbackContext.phase == InputActionPhase.Started) // 막 눌린 상태 Started
        {
            Toggle();
        }
    }

    public void Toggle()
    {
        if(inventoryWindow.activeInHierarchy) // 하이어라키 창에서 켜져있나(SetActive인가) 검색
        {
            inventoryWindow.SetActive(false);
            onCloseInventory?.Invoke();
            controller.ToggleCursor(false);
        }
        else
        {
            inventoryWindow.SetActive(true);
            onOpenInventory?.Invoke();
            controller.ToggleCursor(true);
        }
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void AddItem(ItemData item)  // 인벤토리에 아이템을 추가하는 메서드
    {
        if(item.canStack) // stack이 쌓일 수 있는 item이라면
        {
            ItemSlot slotToStackTo = GetItemStack(item);
            if(slotToStackTo != null) // 아이템이 인벤토리에 있다면 
            {
                slotToStackTo.quantity++; // 개수 추가
                UpdateUI();

                return;
            }
        }

        // 스택을 쌓을 수 없는 아이템이면 빈칸에다 그냥 넣음
        ItemSlot emptySlot = GetEmeptySlot();

        if(emptySlot != null)
        {
            emptySlot.item = item;
            emptySlot.quantity = 1;
            UpdateUI();

            return;
        }

        // 아이템이 꽉 차면
        ThrowItem(item);
    }

    void ThrowItem(ItemData item)
    {
        // 잡아놓은 드랍 지점(dropPosition.position)에 랜덤한 회전을 갖고(Quaternion.Euler) 다시 아이템(item.dropPrefab)을 생성하라(Instantiate)는 코드 - 여느 게임에 있는 아이템 버리기
        Instantiate(item.dropPrefab, dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value *  360f));
    }

    void UpdateUI()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
                uiSlots[i].Set(slots[i]);
            else
                uiSlots[i].Clear();
        }
    }

    ItemSlot GetItemStack(ItemData item) // 슬롯 찾기
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == item && slots[i].quantity < item.maxStackAmount)
                return slots[i];
        }
        return null;
    }

    ItemSlot GetEmeptySlot()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null)
                return slots[i];
        }
        return null;
    }
    
    public void SelectItem(int index)
    {
        if (slots[index].item == null)  // 아이템이 없으면 선택할 수 없으니 예외 처리
            return;

        // 초기화
        selectedItem = slots[index];
        selectedItemIndex = index;

        // 아이템 이름과 설명 텍스트를 ItemData와 연결한다
        selectedItemName.text = selectedItem.item.displayName;  
        selectedItemDescription.text = selectedItem.item.description;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        for(int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemStatNames.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValues.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        // 버튼 활성화
        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !uiSlots[index].equipped); // 장착이 안돼있으면 활성화
        unEquipButton.SetActive(selectedItem.item.type == ItemType.Equipable && uiSlots[index].equipped);
        dropButton.SetActive(true);
    }

    private void ClearSelectItemWindow()
    {
        // 클리어 해주는 작업
        selectedItem = null;
        selectedItemName.text = string.Empty;
        selectedItemDescription.text = string.Empty;

        selectedItemStatNames.text = string.Empty;
        selectedItemStatValues.text = string.Empty;

        useButton.SetActive(false);
        equipButton.SetActive(false);
        unEquipButton.SetActive(false);
        dropButton.SetActive(false);
    }

    public void OnUseButton()
    {
        if(selectedItem.item.type == ItemType.Consumable)
        {
            for (int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch(selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break;
                }
            }
        }
        RemoveSelectedItem();
    }

    public void OnEquiputton()
    {
        if (uiSlots[curEquipIndex].equipped) // 현재 equipped된 장비가 있다면
        {
            UnEquip(curEquipIndex);  // 장비 해제
        }

        uiSlots[selectedItemIndex].equipped = true;  // 장비 장착
        curEquipIndex = selectedItemIndex; // 선택한 아이템으로 curEquipIndex를 설정한다.
        EquipManager.instance.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedItemIndex);
    }

    void UnEquip(int index)  // 장비 해제 메서드
    {
        uiSlots[index].equipped = false;
        EquipManager.instance.UnEquip();
        UpdateUI();

        if (selectedItemIndex == index)
            SelectItem(index);

    }
    public void OnUnEquipButton()
    {
        UnEquip(selectedItemIndex);
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        RemoveSelectedItem();
    }

    private void RemoveSelectedItem()
    {
        selectedItem.quantity--;

        if(selectedItem.quantity <= 0)  // 아이템 개수가 0으로 떨어지면
        {
            if (uiSlots[selectedItemIndex].equipped)  // 아이템이 장착된 상태라면
            {
                UnEquip(selectedItemIndex);  // 장착 해제
            }

            selectedItem.item = null;
            ClearSelectItemWindow();
        }

        UpdateUI();
    }

    public void RemoveItem(ItemData item)
    {
        
    }

    public bool HasItems(ItemData item, int quantity)
    {
        return false;
    }
}
