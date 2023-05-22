using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    public GameObject[] stage;

    int currentStage;
    int clearStage;

    private void Awake()
    {
        #region 보안코드에 저장된 정보로부터 플레이어 스테이지 진행도 불러오기
        clearStage = CryptoPlayerPrefs.GetInt("clearStage");
        currentStage = CryptoPlayerPrefs.GetInt("currentStage");
        #endregion

        // 게이트 생성 시, titleOnlyStage가 Off가 됨.
        // 그렇게 Off가 되면 currentStage와 clearStage를 비교해서, 아래와 같이 설정해줌.
        if (CryptoPlayerPrefs.GetString("titleOnlyStage") == "Off")
        {
            // 현재 층이 최대로 올라간 층보다 낮은 층이면, 현재층에서 바로 다음층으로 가도록 설정.
            if (currentStage < clearStage)       
            {
                // 3층까지 열어놔도 1층 클리어 시 2층으로 가도록 설정.
                CryptoPlayerPrefs.SetInt("currentStage", currentStage + 1);
            }
            else
            {
                // 만약 현재 층이 최대기록 층이면, 클리어 시 다음층 새롭게 열도록 설정
                CryptoPlayerPrefs.SetInt("currentStage", clearStage + 1);
            }
        }
        //else
        //{
        //    currentStage = CryptoPlayerPrefs.GetInt("titlePlayStage");      // 타이틀에서 적용되는 코드
        //    Debug.Log("타이틀 스테이지 커런트");
        //}

        Debug.Log("씬 로드 :" + "curstage :" + CryptoPlayerPrefs.GetInt("currentStage") + "/clearstage" + CryptoPlayerPrefs.GetInt("clearStage"));
        Debug.Log(CryptoPlayerPrefs.GetString("titleOnlyStage"));
    }

    private void Start()
    {
        if (currentStage > stage.Length)     
        {
            // 현재 층이 스테이지 총 개수보다 높으면, 최고층이 열리도록 설정.
            // 최고 스테이지 : 스테이지 배열 개수 -1
            // ex) 배열 개수가 5개면 스테이지는 0~4까지 들어가있는 것이므로, 최고 스테이지가 4이다.
            Instantiate(stage[stage.Length - 1], Vector3.zero, Quaternion.identity);
        }
        else
        {
            // 현재 층이 스테이지 총 개수보다 적거나 같으면, 현재 층이 열림.
            // ex) 배열 개수 5, 현재 층 3이면, 스테이지3은 stage[2]이므로.
            Instantiate(stage[currentStage - 1], Vector3.zero, Quaternion.identity);
            CryptoPlayerPrefs.SetInt("currentStage", currentStage);
        }
    }
}
