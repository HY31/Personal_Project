using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampFire : MonoBehaviour
{
    public int damage;
    public float damageRate;
    
    private List<IDamagable> thingsToDamage = new List<IDamagable>();

    private void Start()
    {
        InvokeRepeating("DealDamage", 0, damageRate); // DealDamage�� �ٷ� �����ϴµ�(�Ű� ������ 0�̸� �������� �����Ѵ�.) damageRate��ŭ �ֱ������� �����Ѵ�.
    }

    void DealDamage() //����Ʈ�� �ִ� �͵鿡 ����� �ο�
    {
        for(int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage);  // ����Ʈ�� ���ִ� ��ҵ鿡 ���� ������� �ش�.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable)) // TryGetComponent�� ����� ������Ʈ�� ã�ƿԴٸ� true�� �Ǹ� ����Ǵ� if���̴�.
        {
            thingsToDamage.Add(damagable); // damagable�� ������ ���ϴ����� ������ IDamagable�� ��ӹ޾ұ� ������ TakePhysicalDamage�� ���� ���̴�. �������̽��� �װŸ� �����ϸ� �ȴ�.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable); // ����� �޴� ���¸� ����
        }
    }
}
