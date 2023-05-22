using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    #region 변수들
    public Camera CamWhenDie;

    public int playerHp;
    public int playerMaxHp;
    public GameObject playerEnergyBar;

    //레벨 및 경험치 관련
    public int playerLevel;
    public float[] playerLimitExp;      //목표 경험치
    public float playerCurrentExp;      //현재 경험치
    public float playerMaxExp;          //현재 최대 경험치

    public GameObject playerLevelView;
    public GameObject playerExpBar;
    public GameObject playerDmgView;
    public GameObject playerFSView;
    public GameObject currentStageView;

    int currentStage = 1;
    int clearStage = 0;

    public Transform gate;     // 게이트 prefab

    //골드 정보
    public int coin = 0;
    public Text coinTxt;

    #region 플레이어 능력치(상점 품목 연관)
    public Text dmgTxt;
    public int playerDmg = 1;
    public int[] playerDmgTable;
    int playerDmgLevel = 1;

    public int[] playerMaxHPTable;
    int playerMaxHPLevel = 1;

    public Text FSTxt;
    public float playerFS = 0.35f;
    public float[] playerFSTable;
    int playerFSLevel = 1;
    #endregion

    public bool isUIDetection = false;

    public static GameManager instance;

    //HUD 정보
    public GameObject disableWhenShopOpened;
    public Canvas hud;
    bool isHUD = false;

    public Canvas controller;
    public Canvas QuitMsgBox;
    public Canvas MenuBox;
    public Canvas TouchPad;
    #endregion

    private void Awake()
    {
        instance = this;

        StartCoroutine(HUDStart());

        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        #region 보안코드/정보저장 및 호출
        CryptoPlayerPrefs_HasKeyIntFind("playerLevel", playerLevel = 1);
        CryptoPlayerPrefs_HasKeyFloatFind("playerCurrentExp", playerCurrentExp = 0);
        CryptoPlayerPrefs_HasKeyFloatFind("playerMaxExp", playerMaxExp = playerLimitExp[0]);

        CryptoPlayerPrefs_HasKeyIntFind("currentStage", currentStage = 1);
        CryptoPlayerPrefs_HasKeyIntFind("currentStage", clearStage = 0);

        CryptoPlayerPrefs_HasKeyIntFind("coin", coin = 50);
        CryptoPlayerPrefs_HasKeyIntFind("playerDmg", playerDmg = 1);
        CryptoPlayerPrefs_HasKeyIntFind("playerDmgLevel", playerDmgLevel = 1);

        CryptoPlayerPrefs_HasKeyIntFind("playerMaxHp", playerMaxHp = 100);
        CryptoPlayerPrefs_HasKeyIntFind("playerMaxHPLevel", playerMaxHPLevel = 1);

        //데이터 로드하기
        playerLevel = CryptoPlayerPrefs.GetInt("playerLevel");
        playerCurrentExp = CryptoPlayerPrefs.GetFloat("playerCurrentExp");
        playerMaxExp = CryptoPlayerPrefs.GetFloat("playerMaxExp");

        currentStage = CryptoPlayerPrefs.GetInt("currentStage");
        clearStage = CryptoPlayerPrefs.GetInt("clearStage");

        coin = CryptoPlayerPrefs.GetInt("coin");
        playerDmg = CryptoPlayerPrefs.GetInt("playerDmg");
        playerDmgLevel = CryptoPlayerPrefs.GetInt("playerDmgLevel");
        playerHp = CryptoPlayerPrefs.GetInt("playerMaxHp");
        playerMaxHp = CryptoPlayerPrefs.GetInt("playerMaxHp");
        playerMaxHPLevel = CryptoPlayerPrefs.GetInt("playerMaxHPLevel");
        playerFS = CryptoPlayerPrefs.GetFloat("playerFireSpeed");
        #endregion
    }

    void Start()
    {
        #region 게임 시작 시, 플레이어 체력, 경험치, 데미지, 공격속도, 레벨, 소지 금액 출력
        playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(playerMaxHp);
        playerEnergyBar.GetComponent<EnergyBar>().SetValueMin(0);
        playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(playerMaxHp);
        playerExpBar.GetComponent<EnergyBar>().SetValueMax((int)playerMaxExp);
        playerExpBar.GetComponent<EnergyBar>().SetValueCurrent((int)playerCurrentExp);
        playerDmgView.GetComponentInChildren<Text>().text = playerDmg.ToString();
        playerFSView.GetComponentInChildren<Text>().text = playerFS.ToString();
        playerLevelView.GetComponentInChildren<Text>().text = "Lv " + playerLevel.ToString();
        currentStageView.GetComponentInChildren<Text>().text = "Stage " + currentStage.ToString();
        coinTxt.text = coin.ToString();
        #endregion

        QuitMsgBox.enabled = false;
        MenuBox.enabled = false;
    }

    private void Update()
    {
        if(playerHp <= 0)
        {
            PlayerState.instance.playerState = PlayerState.PLAYERSTATE.DEAD;

            hud.enabled = false;
            CamWhenDie.gameObject.SetActive(true);
        }
        else
        {
            CamWhenDie.gameObject.SetActive(false);
        }

        // 모바일용
        if (Application.platform == RuntimePlatform.Android)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (QuitMsgBox.enabled)
                {
                    QuitMsgBox.enabled = false;

                    Time.timeScale = 1;
                }
                else if (!QuitMsgBox.enabled)
                {
                    QuitMsgBox.enabled = true;

                    Time.timeScale = 0;
                }
            }
        }
        
        //// PC체크용
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    QuitMsgBox.enabled = true;
        //}
    }

    #region 모바일용 터치패드 발사 버튼
    public void FireStart()
    {
        GameController.instance.isShoot = true;
    }

    public void FireStop()
    {
        GameController.instance.isShoot = false;
    }
    #endregion

    #region QuitMsgBox 버튼
    public void QuitGame_YES()
    {
        Application.Quit();
    }

    public void QuitGame_NO()
    {
        QuitMsgBox.enabled = false;
    }
    #endregion

    #region MenuBox 버튼
    public void SoundControl()
    {

    }

    public void OpenHelp()
    {

    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Title");

        Time.timeScale = 1;
    }

    public void ClickMenu()
    {
        if(MenuBox.enabled)
        {
            MenuBox.enabled = false;
            TouchPad.enabled = true;

            Time.timeScale = 1;
        }
        else if(!MenuBox.enabled)
        {
            MenuBox.enabled = true;
            TouchPad.enabled = false;

            Time.timeScale = 0;
        }
    }
    #endregion

    #region 기타 함수들
    // 스테이지 클리어 시, 포탈 생성 함수
    public void GateGen()
    {
        // Gate가 생성될 시, titleOnlyStage를 Off로 설정하여 clearStage와 currentStage를 StageManager에서 수정할 수 있게 함.
        CryptoPlayerPrefs.SetString("titleOnlyStage", "Off");

        Instantiate(gate, GameObject.FindGameObjectWithTag("Player").transform.position, Quaternion.identity);

        SoundManager.instance.PlaySfx(GameObject.FindGameObjectWithTag("Player").transform.position, SoundManager.instance.sound_OpenPortal, 0.0f, 3);
    }

    // 코인 획득 함수
    public int AddCoin(int amount)
    {
        int tCoin = CryptoPlayerPrefs.GetInt("coin");

        tCoin += amount;

        coinTxt.text = tCoin.ToString();
        CryptoPlayerPrefs.SetInt("coin", tCoin);

        return tCoin;
    }

    public float ExpCal(float exp)
    {
        playerCurrentExp += exp;

        if (playerLevel < playerLimitExp.Length)
        {
            if (playerCurrentExp >= playerLimitExp[playerLevel - 1])
            {
                playerLevel += 1;

                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_LvUP, 0.0f, SoundManager.instance.volume_Sfx);

                #region 레벨 업 효과
                #region 체력 회복
                playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(CryptoPlayerPrefs.GetInt("playerMaxHp"));        // 체력바의 최대체력 출력 갱신
                playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(CryptoPlayerPrefs.GetInt("playerMaxHp"));    // 체력바의 현재체력 출력 갱신
                #endregion
                #endregion

                //playerCurrentExp = 0;
                playerCurrentExp = Mathf.Abs(playerLimitExp[playerLevel - 2] - playerCurrentExp);

                playerMaxExp = playerLimitExp[playerLevel - 1];
            }
        }
        else if( playerCurrentExp >= playerLimitExp[playerLimitExp.Length-1])
        {
            playerLevel += 1;

            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_LvUP, 0.0f, SoundManager.instance.volume_Sfx);

            playerCurrentExp = Mathf.Abs(playerLimitExp[playerLimitExp.Length - 1] - playerCurrentExp);

            playerMaxExp = playerLimitExp[playerLimitExp.Length - 1];
        }

        //경험치 바 텍스트 출력
        playerLevelView.GetComponentInChildren<Text>().text = playerLevel.ToString();
        playerExpBar.GetComponent<EnergyBar>().SetValueMax((int)playerMaxExp);
        playerExpBar.GetComponent<EnergyBar>().SetValueCurrent((int)playerCurrentExp);

        /***************************************************************************************/
        //데이터 저장
        CryptoPlayerPrefs.SetInt("playerLevel", playerLevel);
        CryptoPlayerPrefs.SetFloat("playerCurrentExp", playerCurrentExp);
        CryptoPlayerPrefs.SetFloat("playerMaxExp", playerMaxExp);
        /***************************************************************************************/

        return exp;
    }

    // Main 씬이 로드되면 HUD가 출력되도록 설정. hud변수는 Main씬의 Canvas.
    IEnumerator HUDStart()
    {
        hud.enabled = false;
        controller.enabled = false;

        while(!isHUD)
        {
            isHUD = true;

            if(SceneManager.GetSceneByName("Stage").isLoaded)   // Stage이름을 가진 씬이 로드되었는지 확인 후, 로드되었다면 hud출력
            {
                hud.enabled = true;
                controller.enabled = true;
                break;
            }

            yield return new WaitForSeconds(0.2f);
            isHUD = false;
        }
    }
    #endregion

    // 건물 비활성 버튼
    //public void BD_DeActive()
    //{
    //    GameObject.Find("Stage_01(Clone)").SetActive(false);
    //}

    #region 보안코드
    //CryptoPlayerPrefs의 해시키를 찾는 함수(정수)
    public void CryptoPlayerPrefs_HasKeyIntFind(string key, int value)
    {
        if(!CryptoPlayerPrefs.HasKey(key))
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
