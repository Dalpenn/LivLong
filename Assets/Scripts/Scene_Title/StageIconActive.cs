using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageIconActive : MonoBehaviour
{
    public GameObject iconActive;
    public GameObject iconLock;

    private void Start()
    {
        string tmpStr = gameObject.name;    //선택된 스테이지 아이콘 이름

        #region string.Length에 대한 설명
        // "Stage xy"
        // x(7번째 글자) : Stage 0~9의 숫자
        // y(8번째 글자) : Stage 10~99의 숫자
        #endregion

        #region string.Length == 7에 대한 설명
        // 스테이지 아이콘 이름이 7글자
        // 7번째 숫자보다 clearStage + 1이 크거나 같으면 스테이지 잠금 해제.
        #endregion
        if (tmpStr.Length == 7 && (CryptoPlayerPrefs.GetInt("clearStage") + 1 >= int.Parse(tmpStr.Substring(6, 1))))
        {
            iconActive.SetActive(true);
            iconLock.SetActive(false);
        }
        #region string.Length == 8에 대한 설명
        // 스테이지 아이콘 이름이 8글자
        // 7 & 8번째 2자리 숫자보다 clearStage + 1이 크거나 같으면 스테이지 잠금 해제.
        #endregion
        else if (tmpStr.Length == 8 && (CryptoPlayerPrefs.GetInt("clearStage") + 1 >= int.Parse(tmpStr.Substring(6, 2))))
        {
            iconActive.SetActive(true);
            iconLock.SetActive(false);
        }
        else   // 이외의 경우, 스테이지 잠금상태.
        {
            iconActive.SetActive(false);
            iconLock.SetActive(true);
        }
        
        #region 예시
        /****************************
        clearStage = 8인 경우,
        stage는 0~9까지 다 잠금해제 / 10부터는 잠금상태
        ****************************/
        #endregion
    }

    public void OnClick()       // 마우스 클릭했을 시의 이벤트 만들기 "public void OnClick"
    {
        string tmpStr = gameObject.name;

        if (iconActive.activeSelf)      // 그냥 (iconActive) 를 사용해도 됨. 똑같은 bool변수임
        {
            CryptoPlayerPrefs.SetString("titleOnlyStage", "On");

            // titlePlayStage를 클릭한 아이콘의 스테이지 숫자로 설정. 이러면 StageManager에서 Stage를 불러올 때, 해당하는 스테이지를 불러온다.
            if(tmpStr.Length == 7)
            {
                CryptoPlayerPrefs.SetInt("titlePlayStage", int.Parse(tmpStr.Substring(6, 1)));
            }
            if (tmpStr.Length == 8)
            {
                CryptoPlayerPrefs.SetInt("titlePlayStage", int.Parse(tmpStr.Substring(6, 2)));
            }

            TitleManager.instance.stageSelect.text = "Stage " + CryptoPlayerPrefs.GetInt("titlePlayStage");
        }
        else
        {
            TitleManager.instance.stageSelect.text = "Locked";
        }

        CryptoPlayerPrefs.SetInt("currentStage", CryptoPlayerPrefs.GetInt("titlePlayStage"));
    }
}
