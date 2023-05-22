using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement ;
using UnityEngine.UI;

public class FadePanel : MonoBehaviour
{
    public GameObject FadePanel_Main;
    Image panelIMG;

    public static bool isFadeOut = false;
    public static bool isFadeIn = false;

    private void Awake()
    {
        panelIMG = FadePanel_Main.GetComponent<Image>();
    }

    private void Update()
    {
        if(isFadeOut)
        {
            StartCoroutine(FadeOut());
        }
        else if(isFadeIn)
        {
            StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeOut()
    {
        FadePanel_Main.SetActive(true);

        isFadeOut = false;

        float fadeCount = 0;

        while(fadeCount < 1.0f)
        {
            fadeCount += 0.01f;
            yield return new WaitForSeconds(0.04f);
            panelIMG.color = new Color(0, 0, 0, fadeCount);
        }

        yield return new WaitForSeconds(1);

        SceneManager.LoadSceneAsync("Title", LoadSceneMode.Single);
    }

    IEnumerator FadeIn()
    {
        FadePanel_Main.SetActive(true);

        isFadeIn = false;

        float fadeCount = 1.0f;

        while (fadeCount > 0)
        {
            fadeCount -= 0.01f;
            yield return new WaitForSeconds(0.04f);
            panelIMG.color = new Color(0, 0, 0, fadeCount);
        }

        FadePanel_Main.SetActive(false);
    }
}
