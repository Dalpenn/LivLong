using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BloodEffect : MonoBehaviour
{
    //[HideInInspector]             // 이렇게 하면 inspector창에서는 숨겨져 있음
    public static Image image;      // static을 사용하면, HideInInspector + public 사용한것과 같은 효과가 남
    public Sprite blood_1;
    public Sprite blood_2;

    public Color startColor = new Color(1, 1, 1, 0);    // 처음 색깔(알파값 0)
    public Color endColor = new Color(1, 1, 1, 1);      // 마지막 색깔(알파값 1)
    public Color endColors = new Vector4(1, 1, 1, 1);   // 마지막 색깔(알파값 1)

    private void Start()
    {
        image = GetComponent<Image>();
    }

    private void Update()
    {
        if(PlayerState.instance.playerState == PlayerState.PLAYERSTATE.DAMAGE)
        {
            if (GameManager.instance.playerHp >= GameManager.instance.playerMaxHp * 0.2)
            {
                if (image.color.a > 0)
                {
                    return;
                }

                StartCoroutine(FadeEffect());
            }
        }

        if (GameManager.instance.playerHp < GameManager.instance.playerMaxHp * 0.2)
        {
            image.sprite = blood_2;
            image.color = Color.Lerp(endColor, startColor, Mathf.PingPong(Time.time, 1.0f));
        }
    }

    // for문을 사용한 FadeEffect
    IEnumerator FadeEffect()
    {
        image.sprite = blood_1;
        image.color = startColor;

        for (float i = 1f; i >= -0.1f; i -= 0.02f)
        {
            Color color = new Vector4(1, 1, 1, i);
            image.color = color;

            yield return new WaitForEndOfFrame();
        }
    }
}
