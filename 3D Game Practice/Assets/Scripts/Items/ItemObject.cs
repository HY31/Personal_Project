using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData item;

    public string GetInteractPrompt()  // ��ȣ�ۿ� �� �� �ִ� �������� �ٶ󺸸� �ߴ� ���ڸ� �ٲ���
    {
        return string.Format("Pickup {0}", item.displayName);
    }

    public void OnInteract() // ������ ��ȣ�ۿ�
    {
        Inventory.instance.AddItem(item);  // �������� �԰� �κ��丮�� �߰�
        Destroy(gameObject);    // �ٴڿ� �ִ� �������� ����
    }
}
