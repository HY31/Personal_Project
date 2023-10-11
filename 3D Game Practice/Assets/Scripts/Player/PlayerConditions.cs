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
    [HideInInspector]  // 컨디션들이 가져야하는 값들을 준비함
    public float curValue;
    public float maxValue;
    public float startValue;
    public float regenRate;
    public float decayRate;
    public Image uiBar;

    public void Add(float amount)  // 회복(증가) 메서드
    {
        curValue = Mathf.Min(curValue + amount, maxValue);  // maxValue를 넘지 않게 하기 위해 둘 중에서 작은 값을 사용한다.
    }

    public void Subtract(float amount) // 사용(감소) 메서드
    { 
        curValue = Mathf.Max(curValue - amount, 0.0f); // 0보다 작아지지 않게 max로 조정해준다.
    }

    public float GetPercentage() // UI에서 사용하려면 퍼센트를 써야된다.
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
        hunger.Subtract(hunger.decayRate * Time.deltaTime);  // 배고픔이 주기적으로(* Time.deltaTime) hunger.decayRate만큼 감소된다.
        stamina.Add(stamina.regenRate * Time.deltaTime);  // 스태미나가 주기적으로 stamina.regenRate만큼 회복된다.

        if (hunger.curValue == 0.0f)
            health.Subtract(noHungerHealthDecay * Time.deltaTime);  // 배고픔이 0이라면 체력이 주기적으로 noHungerHealthDecay만큼 감소된다.

        if (health.curValue == 0.0f)  // 체력이 0이면 죽는다.
            Die();

        // 위에서 조작들을 다 해줬으니 이제 보여주기를 해야된다. 처리부와 구동부가 다르다는 느낌으로
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
