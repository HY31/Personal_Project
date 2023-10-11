using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData item;

    public string GetInteractPrompt()  // 상호작용 할 수 있는 아이템을 바라보면 뜨는 문자를 바꿔줌
    {
        return string.Format("Pickup {0}", item.displayName);
    }

    public void OnInteract() // 아이템 상호작용
    {
        Inventory.instance.AddItem(item);  // 아이템을 먹고 인벤토리에 추가
        Destroy(gameObject);    // 바닥에 있는 아이템을 없앰
    }
}
