using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{
    #region 변수들
    AsyncOperation async_1;       // 비동기화 방식의 로딩 변수
    AsyncOperation async_2;
    AsyncOperation async_3;

    public GameObject loadingBar;

    float progress;
    #endregion

    private void Awake()
    {
        loadingBar.GetComponent<EnergyBar>().SetValueMax(100);
        loadingBar.GetComponent<EnergyBar>().SetValueMin(0);
    }

    private void Start()
    {
        async_1 = SceneManager.LoadSceneAsync("Main", LoadSceneMode.Single);
        async_2 = SceneManager.LoadSceneAsync("Shop", LoadSceneMode.Additive);
        async_3 = SceneManager.LoadSceneAsync("Stage", LoadSceneMode.Additive);   // Stage씬이 가장 무거움

        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        // 비동기화가 안끝났는지 확인(async 내에 확인가능한 bool함수가 존재한다)
        // 스테이지 씬이 가장 무거우므로, 얘가 다 끝나지 않았으면 로딩바를 출력하도록 설정
        if (!async_3.isDone)
        {
            progress = async_3.progress * 100f;
            loadingBar.GetComponent<EnergyBar>().valueCurrent = (int)progress;

            //Debug.Log(progress);
        }

        // 비동기화가 모두 끝난 후에는 Destroy하도록 설정
        if(async_3.isDone)
        {
            FadePanel.isFadeIn = true;

            Destroy(this.gameObject);
        }
    }
}