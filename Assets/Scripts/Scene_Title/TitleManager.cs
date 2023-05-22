using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    #region 변수들
    int currentStage;
    int clearStage;
    int titlePlayStage;

    public UILabel stageSelect;

    public Canvas WarningMsgBox;

    public static TitleManager instance;
    #endregion

    private void Awake()
    {
        instance = this;

        #region 현재 스테이지 진행도 저장 및 불러오기
        CryptoPlayerPrefs_HasKeyIntFind("titlePlayStage", titlePlayStage = 1);
        CryptoPlayerPrefs_HasKeyIntFind("clearStage", clearStage = 0);
        CryptoPlayerPrefs_HasKeyIntFind("currentStage", currentStage = 1);
        CryptoPlayerPrefs_HasKeyStringFind("titleOnlyStage", "On");                 // On/Off를 string으로 지정

        titlePlayStage = CryptoPlayerPrefs.GetInt("titlePlayStage");
        clearStage = CryptoPlayerPrefs.GetInt("clearStage");
        currentStage = CryptoPlayerPrefs.GetInt("currentStage");
        #endregion
    }

    private void Start()
    {
        WarningMsgBox.enabled = false;
    }

    public void StartGame()     // StartGame 버튼
    {
        if(stageSelect.text == "Locked")    // 잠겨있는 스테이지는 실행안되도록 설정
        {
            return;
        }

        SceneManager.LoadScene("Loading");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    //플레이어 저장 데이터 초기화 버튼
    public void ClearData()
    {
        WarningMsgBox.enabled = true;
    }

    public void ClearData_Yes()
    {
        CryptoPlayerPrefs.DeleteAll();

        SceneManager.LoadScene("Title");
    }

    public void ClearData_No()
    {
        WarningMsgBox.enabled = false;
    }

    #region 보안코드
    //CryptoPlayerPrefs의 해시키를 찾는 함수(정수)
    public void CryptoPlayerPrefs_HasKeyIntFind(string key, int value)
    {
        if (!CryptoPlayerPrefs.HasKey(key))
        {
            CryptoPlayerPrefs.SetInt(key, value);
        }
        else
        {
            CryptoPlayerPrefs.GetInt(key, 0);
        }
    }

    //CryptoPlayerPrefs의 해시키를 찾는 함수(실수)
    public void CryptoPlayerPrefs_HasKeyFloatFind(string key, float value)
    {
        if (!CryptoPlayerPrefs.HasKey(key))
        {
            CryptoPlayerPrefs.SetFloat(key, value);
        }
        else
        {
            CryptoPlayerPrefs.GetFloat(key, 0);
        }
    }

    //CryptoPlayerPrefs의 해시키를 찾는 함수(문자열)
    public void CryptoPlayerPrefs_HasKeyStringFind(string key, string value)
    {
        if (!CryptoPlayerPrefs.HasKey(key))
        {
            CryptoPlayerPrefs.SetString(key, value);
        }
        else
        {
            CryptoPlayerPrefs.GetString(key, "0");
        }
    }
    #endregion
}