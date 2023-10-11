using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EquipManager : MonoBehaviour
{
    public Equip curEquip;
    public Transform equipParent;

    private PlayerController controller;
    private PlayerConditions conditions;

    public static EquipManager instance;

    private void Awake()
    {
        instance = this;
        controller = GetComponent<PlayerController>();
        conditions = GetComponent<PlayerConditions>();
    }

    public void OnAttackInput(InputAction.CallbackContext context) // 공격 이벤트를 받아옴 -> Equip의 virtual을 거쳐 EquipTool의 override로
    {
        if(context.phase == InputActionPhase.Performed && curEquip != null && controller.canLook) 
        {
            curEquip.OnAttackInput(conditions);
        }
    }

    public void EquipNew(ItemData item)
    {
        UnEquip();
        curEquip = Instantiate(item.equipPrefab, equipParent).GetComponent<Equip>();
    }

    public void UnEquip()
    {
        if(curEquip != null)
        {
            Destroy(curEquip.gameObject);
            curEquip = null;
        }
    }
}
