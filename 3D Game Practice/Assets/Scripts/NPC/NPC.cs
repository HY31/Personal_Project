using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,       // ���
    Wandering,  // ��Ȳ
    Attacking,  // ����
    Fleeing     // ����
}

// FSM - ���� ���� �ӽ�

public class NPC : MonoBehaviour, IDamagable
{
    [Header("Stats")]
    public int health;
    public float walkSpeed;
    public float runSpeed;
    public ItemData[] dropOnDeath;

    [Header("AI")]
    private AIState aiState;
    public float detectDistance;
    public float safeDistance;

    [Header("Wandering")]
    public float minWanderDistance;
    public float maxWanderDistance;
    public float minWanderWaitTime;
    public float maxWanderWaitTime;

    [Header("Combat")]
    public int damage;
    public float attackRate;
    private float lastAttackTime;
    public float attackDistance;

    private float playerDistance;

    public float fieldOfView = 120f;

    private NavMeshAgent agent;
    private Animator animator;
    private SkinnedMeshRenderer[] meshRenderers;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(); // ������ GetComponents -> ��� �ڽ��� �������� ��
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position); // npc�� �÷��̾� ���� �Ÿ�

        animator.SetBool("Moving", aiState != AIState.Idle);

        switch(aiState)
        {
            case AIState.Idle: PassiveUpdate(); break;
            case AIState.Wandering: PassiveUpdate(); break;
            case AIState.Attacking: AttackingUpdate(); break;
            case AIState.Fleeing: FleeingUpdate(); break;
        }
    }

    private void FleeingUpdate()
    {
        if(agent.remainingDistance < 0.1f) // ��ǥ �������� �Ÿ��� 0.1 ���϶��
        {
            agent.SetDestination(GetFleeLocation());
        }
        else
        {
            SetState(AIState.Wandering);
        }
    }

    private void AttackingUpdate()
    {
        if(playerDistance > attackDistance || !IsPlayerInFieldOfView()) // �÷��̾���� �Ÿ��� ���� ��Ÿ��� ��� ���ų�, �÷��̾ �Ⱥ��� ���
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();
            if(agent.CalculatePath(PlayerController.instance.transform.position, path))  // ���ο� ��� �˻��� �ƴٸ�  -> CalculatePath �޼��尡 true���� ��ȯ�ߴٸ�
                                                                                         // ���⼱ �÷��̾��� ��ġ�� �̵������� �������� üũ�ϴ� ��
            {
                agent.SetDestination(PlayerController.instance.transform.position); // ��ġ ���� ���� - �������� �������ָ� ���������� ��θ� �˾Ƽ� ã�� �̵���. (agent�� ����� �� ���� ���� ���̴� �ڵ�)
                                                                                    // �÷��̾��� ��ġ�� �Ѿư� �� �ִ� ���̶�� �÷��̾ �Ѿư���.
            }
            else
            {
                SetState(AIState.Fleeing);  // �÷��̾ ���Ѿư��� ����ģ��
            }
        }
        else
        {
            agent.isStopped = true;  // ���缭
            if(Time.time - lastAttackTime > attackRate)  // �����Ѵ�.
            {
                lastAttackTime = Time.time;
                PlayerController.instance.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
    }

    private void PassiveUpdate()  // �нú� ����
    {
        if(aiState == AIState.Wandering && agent.remainingDistance < 0.1f)  // ��ǥ �������� �Ÿ��� 0.1 ���ϸ�
        {
            SetState(AIState.Idle); // Idle�� ���� ����
            Invoke("WanderToNewLocation", UnityEngine.Random.Range(minWanderWaitTime, maxWanderWaitTime)); // ���� �ð� ��� �� �ٽ� �̵�
        }

        if(playerDistance < detectDistance) // �÷��̾���� �Ÿ��� detectDistance���� ������ �������� ���� ����
        {
            SetState(AIState.Attacking);
        }
    }
    private bool IsPlayerInFieldOfView() // �þ߰�
    {
        Vector3 directionToPlayer = PlayerController.instance.transform.position - transform.position; // �÷��̾ �ٶ󺸴� ���� ���ϱ�
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f;

    }

    private void SetState(AIState newState) // � ���¸� ���� �� �� ���¸� �����ϱ� ���ؼ� ó�� �������ִ� �޼���
    {
        aiState = newState;
        switch(aiState)
        {
            case AIState.Idle:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = true;
                }
                break;
            case AIState.Wandering:
                {
                    agent.speed = walkSpeed;
                    agent.isStopped = false;
                }
                break;
            case AIState.Attacking:
                {
                    agent.speed = runSpeed;
                    agent.isStopped = false;
                }
                break;
            case AIState.Fleeing:
                {
                    agent.speed = runSpeed;
                    agent.isStopped = false;
                }
                break;
        }

        animator.speed = agent.speed / walkSpeed; // �ִϸ��̼� �ӵ� ���� - ������
    }

    void WanderToNewLocation()
    {
        if(aiState != AIState.Idle)
        {
            return;
        }
        SetState(AIState.Wandering);
        agent.SetDestination(GetWanderLocation());
    }

    private Vector3 GetWanderLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere) * UnityEngine.Random.Range(minWanderDistance, maxWanderDistance), out hit, maxWanderDistance, NavMesh.AllAreas); // ��� ���� ���� ����� ���� ������.
        int i = 0;
        while(Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere) * UnityEngine.Random.Range(minWanderDistance, maxWanderDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }
        
        return hit.position;                                                                                                                                                                               // (UnityEngine.Random.onUnitSphere) -> 1�̶�� �Ÿ��� ������ �ִ� ��ü�� �̷� �� �ִ� ��� ������ �ش�.
    }

    private Vector3 GetFleeLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas); // ��� ���� ���� ����� ���� ������.
        int i = 0;
        while (GetDestinationAngle(hit.position) > 90 ||  playerDistance < safeDistance)
        {
            NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }

        return hit.position;
    }

    private float GetDestinationAngle(Vector3 targetPos)
    {
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPos); // �� ���� ������ ���� ���ϴ� �޼��� Angle
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health -= damageAmount;
        if (health <= 0)
            Die();

        StartCoroutine(DamageFlash());
    }

    void Die()
    {
        for(int x = 0; x < dropOnDeath.Length; x++)
        {
            Instantiate(dropOnDeath[x].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);  // �׾��� �� ������ ���
        }

        Destroy(gameObject);  // �׾����� �����
    }

    IEnumerator DamageFlash()
    {
        for(int x = 0; x < meshRenderers.Length; x++)  // �ݺ������� ������ ������? -> �迭�� ���ԵǴ� ��� �Ϳ� �����ϱ� ����
        {
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);  // ����� �Ծ��� �� ���� ����(���� ������ �迭)
        }

        yield return new WaitForSeconds(0.1f);  // return�̶�� �����°� �ƴϰ� �ð� ��ٸ� ������ �������� �Ѿ(���� ���� ����?)

        for(int x = 0; x < meshRenderers.Length; x++)
        {
            meshRenderers[x].material.color = Color.white;
        }
    }
}
