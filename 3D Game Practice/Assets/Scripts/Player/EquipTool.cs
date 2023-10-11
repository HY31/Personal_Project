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
    public override void OnAttackInput(PlayerConditions conditions)  // ���� ���� ������
    {
        if(!attacking) 
        {
            if(conditions.UseStamina(useStamina))
            {
                attacking = true;
                animator.SetTrigger("Attack");
                Invoke("OnCanAttack", attackRate); // attackRate��ŭ ���� ����
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

        if (Physics.Raycast(ray, out hit, attackDistance)) // attackDistance��ŭ ray�� ���� ray�� ���� ������ hit�� ���´�
        {
            if (doesGatherResources && hit.collider.TryGetComponent(out Resource resource))  // ���ҽ� ä�� ������ ����(doesGatherResources)�̸鼭 ���̿� ���� �ݶ��̴��� ���ҽ��� ��� Gather ȣ��
            {
                resource.Gather(hit.point, hit.normal);  // point�� ���̰� �浹�� ������ 3D ���� ���� ��ġ, normal�� ���̿� �浹�� ���������� ǥ�� �븻 ���͸� ��Ÿ��
                                                         // ǥ�� �븻 ���Ͷ� ������ ����Ű��, ���̿� �浹�� ǥ���� � ������ �����ִ����� ��Ÿ����.

            }

            if (doesDealDamage && hit.collider.TryGetComponent(out IDamagable damagable))  // TryGetComponent�� ������Ʈ�� �����Ѵٸ� out ���� ������ ������Ʈ�� �Ҵ��ϰ� true�� ��ȯ�Ѵ�.
            {
                damagable.TakePhysicalDamage(damage);
            }
        }
    }

}
