using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 시간을 정해 놓고 순환하며, 그 한 순환 안에서 해와 달이 뜨고 지는거에 맞춰 빛에 대한 조절을 해주는 클래스이다.
public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    float timeRate;
    public Vector3 noon;  // 자정의 각도

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;


    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;  // 환경광
    public AnimationCurve reflectionIntensityMultiplier;  // 반사광

    private void Start()
    {
        timeRate = 1.0f / fullDayLength; // 시간이 얼마씩 지나는지 정해놓기
        time = startTime;  // 준비작업
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;  // 1.0f로 나눈 나머지를 쓰는 이유는 퍼센테이지 값을 쓰기 위함이다. 0부터 0.9999...까지밖에 값이 설정될 수 없다.

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);
        
        // UpdateLighting은 빛이 바뀌는거지 프로젝트의 환경이 바뀌는 것이 아니기 때문에 크게 달라지는 점이 없기에 그 부분을 설정해준다.
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);

    }

    void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // 애니메이션 커브에 시간 값을 주면 그 시간에 맞는 그래프를 가져온다.

        // lightSource가 각도가 변할수록 각도의 변화에 맞춰 빛이 바뀌기 때문에 햇빛의 변화와 유사하다. 때문에 eulerAngles를 변화시켜 햇빛을 구현한다.
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f; // noon은 90도이며(12시 정각에는 해가 중천이다), noon을 기준으로 하루가 지나려면 4가 곱해져 360도를 순환해야한다.
                                                                                                         // 그 중에서 lightSource가 sun이라면 0을 기준으로 시작해서 0.25일 때 해가 중간에 뜬다??? 잘 이해 안감
        lightSource.color = colorGradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
