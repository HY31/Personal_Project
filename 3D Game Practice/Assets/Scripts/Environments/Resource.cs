using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit = 1;  // Ÿ�� �� ��� �ڿ��� ��
    public int capacity;  //  �����ִ� �ڿ��� �� ����

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)  // �÷��̾ ���ҽ��� �������� �� ������ �������� �ϴ� �޼���
    {
        for(int i = 0; i < quantityPerHit; i++) // �� �� Ÿ�� �� ���� �� �ִ� �ڿ��� ����ŭ �ݺ�
        {
            if(capacity <= 0) { break; }
            capacity -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.forward, Quaternion.LookRotation(hitNormal, Vector3.up)); // �ٶ󺸴� ������ ������ dropPrefab�� �����Ѵ�.
        }

        if(capacity <= 0)  // �� �뷮���� �������� ������Ʈ �ı�
            Destroy(gameObject);
    }
}
