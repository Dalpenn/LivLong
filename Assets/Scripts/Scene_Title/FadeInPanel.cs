using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeInPanel : MonoBehaviour
{
    public GameObject FIpanel;
    Image panelIMG;

    private void Awake()
    {
        panelIMG = FIpanel.GetComponent<Image>();
    }

    private void Start()
    {
        StartCoroutine(FadeIn());
    }

    IEnumerator FadeIn()
    {
        float fadeCount = 1.0f;

        while (fadeCount > 0)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.05f);
            panelIMG.color = new Color(0, 0, 0, fadeCount);
        }

        FIpanel.SetActive(false);
    }
}
