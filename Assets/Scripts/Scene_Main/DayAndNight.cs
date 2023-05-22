using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayAndNight : MonoBehaviour
{
    Transform sunTr;

    public Light sunLight;      // 햇빛을 나타내는 Light
    public Image day;
    public Image night;

    float timer;                // 밤낮을 확인하는데 이용할 변수

    public bool isNight = false;

    float day_alph;
    float night_alph;

    public float fogDensityCalc;       // Fog밀도 계산량
    public float nightFogDensity;      // 밤의 Fog밀도
    public float dayFogDensity;        // 낮의 Fog밀도
    float currentFogDensity;    // 현재 Fog밀도

    private void Start()
    {
        sunTr = sunLight.transform;
    }

    private void Update()
    {
        sunTr.Rotate(Vector3.right, 3 * Time.deltaTime);
        
        TimeChk();

        // 변수 timer를 이용하여 밤낮 제어플래그 설정
        if (timer <= 180)
        {
            isNight = false;
        }
        else
        {
            isNight = true;
        }

        ChangeWithSunLight(timer);      // 밤낮에 따라 태양/달 이미지 알파값 변화

        ChangeFogDensity();
    }

    void TimeChk()      // 밤낮확인을 위한 변수 설정
    {
        timer += 3 * Time.deltaTime;

        if(timer >= 360)
        {
            timer = 0;
        }
    }

    void ChangeWithSunLight(float t)
    {
        if (!isNight)   // 낮인 동안에는 달의 알파값을 0으로(달은 안보이게), 태양 알파값을 천천히 올렸다가 내림
        {
            float timeToalpha = Mathf.Abs(t - 90);

            night_alph = 0;
            day_alph = 1 - timeToalpha / 90;
        }
        else            // 밤인 동안에는 태양의 알파값을 0으로(태양이 안보이게), 달 알파값을 천천히 올렸다가 내림
        {
            float timeToalpha = Mathf.Abs(t - 270);

            day_alph = 0;
            night_alph = 1 - timeToalpha / 90;
        }

        Color color_day = day.color;
        Color color_night = night.color;

        color_night.a = night_alph;
        color_day.a = day_alph;

        day.color = color_day;
        night.color = color_night;
    }

    void ChangeFogDensity()
    {
        if (isNight)        // 밤이 되면 안개가 nightFogDensity만큼 끼도록 만듦
        {
            if (currentFogDensity <= nightFogDensity)
            {
                currentFogDensity += 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
        else                // 낮이 되면 안개가 걷히도록 만듦
        {
            if (currentFogDensity >= dayFogDensity)
            {
                currentFogDensity -= 0.1f * fogDensityCalc * Time.deltaTime;
                RenderSettings.fogDensity = currentFogDensity;
            }
        }
    }
}
