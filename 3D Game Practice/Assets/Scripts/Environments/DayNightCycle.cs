using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �ð��� ���� ���� ��ȯ�ϸ�, �� �� ��ȯ �ȿ��� �ؿ� ���� �߰� ���°ſ� ���� ���� ���� ������ ���ִ� Ŭ�����̴�.
public class DayNightCycle : MonoBehaviour
{
    [Range(0f, 1f)]
    public float time;
    public float fullDayLength;
    public float startTime = 0.4f;
    float timeRate;
    public Vector3 noon;  // ������ ����

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;


    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;  // ȯ�汤
    public AnimationCurve reflectionIntensityMultiplier;  // �ݻ籤

    private void Start()
    {
        timeRate = 1.0f / fullDayLength; // �ð��� �󸶾� �������� ���س���
        time = startTime;  // �غ��۾�
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f;  // 1.0f�� ���� �������� ���� ������ �ۼ������� ���� ���� �����̴�. 0���� 0.9999...�����ۿ� ���� ������ �� ����.

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);
        
        // UpdateLighting�� ���� �ٲ�°��� ������Ʈ�� ȯ���� �ٲ�� ���� �ƴϱ� ������ ũ�� �޶����� ���� ���⿡ �� �κ��� �������ش�.
        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.reflectionIntensity = reflectionIntensityMultiplier.Evaluate(time);

    }

    void UpdateLighting(Light lightSource, Gradient colorGradient, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // �ִϸ��̼� Ŀ�꿡 �ð� ���� �ָ� �� �ð��� �´� �׷����� �����´�.

        // lightSource�� ������ ���Ҽ��� ������ ��ȭ�� ���� ���� �ٲ�� ������ �޺��� ��ȭ�� �����ϴ�. ������ eulerAngles�� ��ȭ���� �޺��� �����Ѵ�.
        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f; // noon�� 90���̸�(12�� �������� �ذ� ��õ�̴�), noon�� �������� �Ϸ簡 �������� 4�� ������ 360���� ��ȯ�ؾ��Ѵ�.
                                                                                                         // �� �߿��� lightSource�� sun�̶�� 0�� �������� �����ؼ� 0.25�� �� �ذ� �߰��� ���??? �� ���� �Ȱ�
        lightSource.color = colorGradient.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if(lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if(lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }
}
