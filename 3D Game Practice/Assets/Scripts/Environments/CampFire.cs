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
        InvokeRepeating("DealDamage", 0, damageRate); // DealDamage를 바로 실행하는데(매개 변수가 0이면 지연없이 실행한다.) damageRate만큼 주기적으로 실행한다.
    }

    void DealDamage() //리스트에 있는 것들에 대미지 부여
    {
        for(int i = 0; i < thingsToDamage.Count; i++)
        {
            thingsToDamage[i].TakePhysicalDamage(damage);  // 리스트에 들어가있는 요소들에 전부 대미지를 준다.
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.TryGetComponent(out IDamagable damagable)) // TryGetComponent가 제대로 컴포넌트를 찾아왔다면 true가 되며 실행되는 if문이다.
        {
            thingsToDamage.Add(damagable); // damagable이 누구를 말하는지는 모르지만 IDamagable을 상속받았기 때문에 TakePhysicalDamage가 있을 것이다. 인터페이스는 그거만 생각하면 된다.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out IDamagable damagable))
        {
            thingsToDamage.Remove(damagable); // 대미지 받는 상태를 해제
        }
    }
}
