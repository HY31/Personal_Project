using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public interface IDamagable
{
    void TakePhysicalDamage(int damageAmount);
}
[System.Serializable]
public class Condition
{
    [HideInInspector]  // ����ǵ��� �������ϴ� ������ �غ���
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount)  // ȸ��(����) �޼���
    {
        curValue = Mathf.Min(curValue + amount, maxValue);  // maxValue�� ���� �ʰ� �ϱ� ���� �� �߿��� ���� ���� ����Ѵ�.
    }

    public void Subtract(float amount) // ���(����) �޼���
    { 
        curValue = Mathf.Max(curValue - amount, 0.0f); // 0���� �۾����� �ʰ� max�� �������ش�.
    }

    public float GetPercentage() // UI���� ����Ϸ��� �ۼ�Ʈ�� ��ߵȴ�.
    {
        return curValue / maxValue;
    }
}
public class PlayerConditions : MonoBehaviour, IDamagable
{
    public Condition health;
    public Condition hunger;
    public Condition stamina;

    public float noHungerHealthDecay;

    public UnityEvent onTakeDamage;

    private void Start()
    {
        health.curValue = health.startValue;
        hunger.curValue = hunger.startValue;
        stamina.curValue = stamina.startValue;
    }

    private void Update()
    {
        hunger.Subtract(hunger.decayRate * Time.deltaTime);  // ������� �ֱ�������(* Time.deltaTime) hunger.decayRate��ŭ ���ҵȴ�.
        stamina.Add(stamina.regenRate * Time.deltaTime);  // ���¹̳��� �ֱ������� stamina.regenRate��ŭ ȸ���ȴ�.

        if (hunger.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);  // ������� 0�̶�� ü���� �ֱ������� noHungerHealthDecay��ŭ ���ҵȴ�.

        if (health.curValue == 0.0f)  // ü���� 0�̸� �״´�.
            Die();

        // ������ ���۵��� �� �������� ���� �����ֱ⸦ �ؾߵȴ�. ó���ο� �����ΰ� �ٸ��ٴ� ��������
        health.uiBar.fillAmount = health.GetPercentage();
        hunger.uiBar.fillAmount = hunger.GetPercentage();
        stamina.uiBar.fillAmount = stamina.GetPercentage();
    }

    public void Heal(float amount)
    {
        health.Add(amount);
    }

    public void Eat(float amount)
    { 
        hunger.Add(amount); 
    }

    public bool UseStamina(float amount)
    {
        if (stamina.curValue - amount < 0)
            return false;

        stamina.Subtract(amount);
        return true;
    }

    public void Die()
    {
        Time.timeScale = 0f;
    }

    public void TakePhysicalDamage(int damageAmount)
    {
        health.Subtract(damageAmount);
        onTakeDamage?.Invoke();
    }
}
