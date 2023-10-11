using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIndicator : MonoBehaviour
{
    public Image image;
    public float flashSpeed;

    private Coroutine coroutine;

    public void Flash()
    {
        if(coroutine != null) // 코루틴이 null이 아니라는건 이전에 코루틴을 돌린 적이 있다라는 뜻이다.
        {
            StopCoroutine(coroutine);
        }

        image.enabled = true;
        image.color = Color.red;
        coroutine = StartCoroutine(FadeAway());
    }

    private IEnumerator FadeAway()
    {
        float startAlpha = 0.3f;
        float a = startAlpha;

        while(a > 0.0f)
        {
            a -= (startAlpha / flashSpeed) * Time.deltaTime;
            image.color = new Color(1.0f, 0.0f, 0.0f, a);  // 빨간색이라는 뜻이다. 순서대로 R, G, B 값
            yield return null;
        }

        image.enabled = false;
    }
}
