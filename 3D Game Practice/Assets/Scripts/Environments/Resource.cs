using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit = 1;  // 타격 당 얻는 자원의 수
    public int capacity;  //  갖고있는 자원의 총 개수

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)  // 플레이어가 리소스를 공격했을 때 아이템 떨어지게 하는 메서드
    {
        for(int i = 0; i < quantityPerHit; i++) // 한 번 타격 시 얻을 수 있는 자원의 수만큼 반복
        {
            if(capacity <= 0) { break; }
            capacity -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.forward, Quaternion.LookRotation(hitNormal, Vector3.up)); // 바라보는 방향의 위에서 dropPrefab을 생성한다.
        }

        if(capacity <= 0)  // 총 용량보다 적어지면 오브젝트 파괴
            Destroy(gameObject);
    }
}
