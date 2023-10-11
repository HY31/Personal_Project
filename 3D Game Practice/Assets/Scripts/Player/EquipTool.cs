using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipTool : Equip
{
    public float attackRate;
    public float attackDistance;

    public float useStamina;

    private bool attacking;

    [Header("Resource Gathering")]
    public bool doesGatherResources;

    [Header("Combat")]
    public bool doesDealDamage;
    public int damage;

    private Animator animator;
    private Camera camera;

    private void Awake()
    {
        camera = Camera.main;
        animator = GetComponent<Animator>();
    }
    public override void OnAttackInput(PlayerConditions conditions)  // 실제 공격 구동부
    {
        if(!attacking) 
        {
            if(conditions.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate); // attackRate만큼 지연 실행
            }
            
        }
    }

    void OnCanAttack()
    {
        attacking = false;
    }

    public void OnHit()
    {
        Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, attackDistance)) // attackDistance만큼 ray를 쏴서 ray에 맞은 정보는 hit에 들어온다
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))  // 리소스 채취 가능한 상태(doesGatherResources)이면서 레이에 맞은 콜라이더가 리소스일 경우 Gather 호출
            {
                resource.Gather(hit.point, hit.normal);  // point는 레이가 충돌한 지점의 3D 공간 상의 위치, normal은 레이와 충돌한 지점에서의 표면 노말 벡터를 나타냄
                                                         // 표면 노말 벡터란 방향을 가리키며, 레이와 충돌한 표면이 어떤 각도로 놓여있는지를 나타낸다.

            }

            if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damagable))  // TryGetComponent는 컴포넌트가 존재한다면 out 후의 변수에 컴포넌트를 할당하고 true를 반환한다.
            {
                damagable.TakePhysicalDamage(damage);
            }
        }
    }

}
