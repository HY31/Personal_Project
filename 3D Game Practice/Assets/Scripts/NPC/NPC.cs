using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public enum AIState
{
    Idle,       // 대기
    Wandering,  // 방황
    Attacking,  // 공격
    Fleeing     // 도망
}

// FSM - 유한 상태 머신

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
        meshRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(); // 복수형 GetComponents -> 모든 자식을 데려오는 것
    }

    private void Start()
    {
        SetState(AIState.Wandering);
    }

    private void Update()
    {
        playerDistance = Vector3.Distance(transform.position, PlayerController.instance.transform.position); // npc와 플레이어 간의 거리

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
        if(agent.remainingDistance < 0.1f) // 목표 지점과의 거리가 0.1 이하라면
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
        if(playerDistance > attackDistance || !IsPlayerInFieldOfView()) // 플레이어와의 거리가 공격 사거리를 벗어난 경우거나, 플레이어가 안보일 경우
        {
            agent.isStopped = false;
            NavMeshPath path = new NavMeshPath();
            if(agent.CalculatePath(PlayerController.instance.transform.position, path))  // 새로운 경로 검색이 됐다면  -> CalculatePath 메서드가 true값을 반환했다면
                                                                                         // 여기선 플레이어의 위치가 이동가능한 곳인지를 체크하는 중
            {
                agent.SetDestination(PlayerController.instance.transform.position); // 위치 정보 전달 - 목적지를 설정해주면 목적지로의 경로를 알아서 찾아 이동함. (agent를 사용할 때 가장 많이 쓰이는 코드)
                                                                                    // 플레이어의 위치가 쫓아갈 수 있는 곳이라면 플레이어를 쫓아간다.
            }
            else
            {
                SetState(AIState.Fleeing);  // 플레이어를 못쫓아가면 도망친다
            }
        }
        else
        {
            agent.isStopped = true;  // 멈춰서
            if(Time.time - lastAttackTime > attackRate)  // 공격한다.
            {
                lastAttackTime = Time.time;
                PlayerController.instance.GetComponent<IDamagable>().TakePhysicalDamage(damage);
                animator.speed = 1;
                animator.SetTrigger("Attack");
            }
        }
    }

    private void PassiveUpdate()  // 패시브 상태
    {
        if(aiState == AIState.Wandering && agent.remainingDistance < 0.1f)  // 목표 지점과의 거리가 0.1 이하면
        {
            SetState(AIState.Idle); // Idle로 상태 변경
            Invoke("WanderToNewLocation", UnityEngine.Random.Range(minWanderWaitTime, maxWanderWaitTime)); // 일정 시간 대기 후 다시 이동
        }

        if(playerDistance < detectDistance) // 플레이어와의 거리가 detectDistance보다 낮으면 공격으로 상태 변경
        {
            SetState(AIState.Attacking);
        }
    }
    private bool IsPlayerInFieldOfView() // 시야각
    {
        Vector3 directionToPlayer = PlayerController.instance.transform.position - transform.position; // 플레이어를 바라보는 방향 구하기
        float angle = Vector3.Angle(transform.forward, directionToPlayer);
        return angle < fieldOfView * 0.5f;

    }

    private void SetState(AIState newState) // 어떤 상태를 줬을 때 그 상태를 적용하기 위해서 처음 세팅해주는 메서드
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

        animator.speed = agent.speed / walkSpeed; // 애니메이션 속도 조절 - 빠르게
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
        NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere) * UnityEngine.Random.Range(minWanderDistance, maxWanderDistance), out hit, maxWanderDistance, NavMesh.AllAreas); // 경로 상의 가장 가까운 곳을 가져옴.
        int i = 0;
        while(Vector3.Distance(transform.position, hit.position) < detectDistance)
        {
            NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere) * UnityEngine.Random.Range(minWanderDistance, maxWanderDistance), out hit, maxWanderDistance, NavMesh.AllAreas);
            i++;
            if (i == 30)
                break;
        }
        
        return hit.position;                                                                                                                                                                               // (UnityEngine.Random.onUnitSphere) -> 1이라는 거리를 가지고 있는 구체를 이룰 수 있는 모든 방향을 준다.
    }

    private Vector3 GetFleeLocation()
    {
        NavMeshHit hit;
        NavMesh.SamplePosition(transform.position + (UnityEngine.Random.onUnitSphere * safeDistance), out hit, maxWanderDistance, NavMesh.AllAreas); // 경로 상의 가장 가까운 곳을 가져옴.
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
        return Vector3.Angle(transform.position - PlayerController.instance.transform.position, transform.position + targetPos); // 두 벡터 사이의 각을 구하는 메서드 Angle
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
            Instantiate(dropOnDeath[x].dropPrefab, transform.position + Vector3.up * 2, Quaternion.identity);  // 죽었을 때 아이템 드랍
        }

        Destroy(gameObject);  // 죽었으니 사라짐
    }

    IEnumerator DamageFlash()
    {
        for(int x = 0; x < meshRenderers.Length; x++)  // 반복문으로 돌리는 이유는? -> 배열에 포함되는 모든 것에 적용하기 위함
        {
            meshRenderers[x].material.color = new Color(1.0f, 0.6f, 0.6f);  // 대미지 입었을 때 색깔 변경(현재 빨간색 계열)
        }

        yield return new WaitForSeconds(0.1f);  // return이라고 끝나는게 아니고 시간 기다린 다음에 다음으로 넘어감(지연 실행 느낌?)

        for(int x = 0; x < meshRenderers.Length; x++)
        {
            meshRenderers[x].material.color = Color.white;
        }
    }
}
