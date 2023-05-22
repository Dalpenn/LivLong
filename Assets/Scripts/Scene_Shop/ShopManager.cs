using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopManager : MonoBehaviour
{
    #region 변수들
    public GameObject shopOpenBtn;
    public GameObject shopUI;
    public GameObject msgBox;

    public UILabel coinLabel;

    #region 상점에 추가할 품목 변수
    bool isAtkChange = true;
    public int[] attackPrice;
    public UILabel atkPriceLabel;
    public UILabel cur_atkLabel;
    public UILabel atkLabel;
    public UILabel atkLevel;
    private int _atkPreview;
    public int atkPreview
    {
        get { return _atkPreview; }
        set { _atkPreview = Mathf.Clamp(value, 0, GameManager.instance.playerDmgTable.Length); }
    }

    bool isHPMaxChange = true;
    public int[] hpMaxPrice;
    public UILabel hpMaxPriceLabel;
    public UILabel cur_hpMaxLabel;
    public UILabel hpMaxLabel;
    public UILabel hpMaxLevel;
    private int _hpMaxPreview;
    public int hpMaxPreview
    {
        get { return _hpMaxPreview; }
        set { _hpMaxPreview = Mathf.Clamp(value, 0, GameManager.instance.playerMaxHPTable.Length); }
    }

    bool isAtkSChange = true;
    public int[] attackSPrice;
    public UILabel atkSPriceLabel;
    public UILabel cur_atkSLabel;
    public UILabel atkSLabel;
    public UILabel atkSLevel;
    private int _atkSPreview;
    public int atkSPreview
    {
        get { return _atkSPreview; }
        set { _atkSPreview = Mathf.Clamp(value, 0, GameManager.instance.playerFSTable.Length); }
    }
    #endregion

    public UILabel pText;

    const int MAXVALUE = 6;
    #endregion

    private void Awake()
    {
        #region 보안코드/정보저장 및 호출
        CryptoPlayerPrefs_HasKeyIntFind("_atkPreview", _atkPreview = 1);
        CryptoPlayerPrefs_HasKeyIntFind("_hpMaxPreview", _hpMaxPreview = 1);
        CryptoPlayerPrefs_HasKeyIntFind("_atkSPreview", _atkSPreview = 1);

        _atkPreview = CryptoPlayerPrefs.GetInt("_atkPreview");
        _hpMaxPreview = CryptoPlayerPrefs.GetInt("_hpMaxPreview");
        _atkSPreview = CryptoPlayerPrefs.GetInt("_atkSPreview");
        #endregion
    }

    private void Start()
    {
        shopOpenBtn.SetActive(true);
        shopUI.SetActive(false);
    }

    public void OpenShop()
    {
        SoundManager.instance.PlaySfx(GameObject.FindGameObjectWithTag("Player").transform.position, SoundManager.instance.sound_ShopOpenClose, 0.0f, SoundManager.instance.volume_Sfx);
        shopOpenBtn.SetActive(false);
        shopUI.SetActive(true);
        GameManager.instance.disableWhenShopOpened.SetActive(false);

        GameManager.instance.isUIDetection = true;

        #region 상점 열었을 때, 실시간 플레이어 정보 로드
        coinLabel.text = CryptoPlayerPrefs.GetInt("coin").ToString();

        #region 상점 품목 정보들
        #region 공격력 정보
        CryptoPlayerPrefs.SetInt("playerDmgLevel", atkPreview);
        atkLevel.text = CryptoPlayerPrefs.GetInt("playerDmgLevel").ToString();
        cur_atkLabel.text = "Atk " + CryptoPlayerPrefs.GetInt("playerDmg").ToString();
        if (CryptoPlayerPrefs.GetInt("playerDmgLevel") <= MAXVALUE)
        {
            atkLabel.text = GameManager.instance.playerDmgTable[CryptoPlayerPrefs.GetInt("playerDmgLevel") - 1].ToString();
            atkPriceLabel.text = attackPrice[CryptoPlayerPrefs.GetInt("playerDmgLevel") - 1].ToString();
        }
        #endregion

        #region 체력 정보
        CryptoPlayerPrefs.SetInt("playerMaxHPLevel", hpMaxPreview);
        hpMaxLevel.text = CryptoPlayerPrefs.GetInt("playerMaxHPLevel").ToString();
        cur_hpMaxLabel.text = "HP " + CryptoPlayerPrefs.GetInt("playerMaxHp").ToString();
        if (CryptoPlayerPrefs.GetInt("playerMaxHPLevel") <= MAXVALUE)
        {
            hpMaxLabel.text = GameManager.instance.playerMaxHPTable[CryptoPlayerPrefs.GetInt("playerMaxHPLevel") - 1].ToString();
            hpMaxPriceLabel.text = hpMaxPrice[CryptoPlayerPrefs.GetInt("playerMaxHPLevel") - 1].ToString();
        }
        #endregion

        #region 공격속도 정보
        CryptoPlayerPrefs.SetInt("playerFSLevel", _atkSPreview);
        atkSLevel.text = CryptoPlayerPrefs.GetInt("playerFSLevel").ToString();
        cur_atkSLabel.text = "FireSpeed " + CryptoPlayerPrefs.GetFloat("playerFireSpeed").ToString();
        if (CryptoPlayerPrefs.GetInt("playerFSLevel") <= MAXVALUE)
        {
            atkSLabel.text = GameManager.instance.playerFSTable[CryptoPlayerPrefs.GetInt("playerFSLevel") - 1].ToString();
            atkSPriceLabel.text = attackSPrice[CryptoPlayerPrefs.GetInt("playerFSLevel") - 1].ToString();
        }
        #endregion
        #endregion
        #endregion

        // 상점 열었을 때, 게임 일시정지 기능
        Time.timeScale = 0;
    }

    public void CloseShop()
    {
        SoundManager.instance.PlaySfx(GameObject.FindGameObjectWithTag("Player").transform.position, SoundManager.instance.sound_ShopOpenClose, 0.0f, SoundManager.instance.volume_Sfx);
        shopOpenBtn.SetActive(true);
        shopUI.SetActive(false);
        GameManager.instance.disableWhenShopOpened.SetActive(true);

        GameManager.instance.isUIDetection = false;

        // 상점 닫았을 때, 게임 재개
        Time.timeScale = 1;
    }

    #region 공격력 업그레이드 구매
    public void AtkPricePreview()
    {
        isAtkChange = true;

        cur_atkLabel.text = "Atk " + CryptoPlayerPrefs.GetInt("playerDmg").ToString();
        atkPreview += 1;
        atkLevel.text = atkPreview.ToString();

        atkLabel.text = GameManager.instance.playerDmgTable[atkPreview - 1].ToString();
        atkPriceLabel.text = attackPrice[atkPreview - 1].ToString();
    }

    public void AtkPurchase()
    {
        if(isAtkChange && int.Parse(atkLevel.text) <= MAXVALUE && CryptoPlayerPrefs.GetInt("playerDmg") < GameManager.instance.playerDmgTable[MAXVALUE-1])
        {
            if(CryptoPlayerPrefs.GetInt("coin") >= int.Parse(atkPriceLabel.text))
            {
                CryptoPlayerPrefs.SetInt("playerDmgLevel", int.Parse(atkLevel.text));

                #region 코인 출력 갱신
                int currCoin = CryptoPlayerPrefs.GetInt("coin") - int.Parse(atkPriceLabel.text);
                CryptoPlayerPrefs.SetInt("coin", currCoin);
                coinLabel.text = currCoin.ToString();       // 상점 코인 출력
                #endregion

                isAtkChange = false;

                CryptoPlayerPrefs.SetInt("playerDmg", GameManager.instance.playerDmgTable[CryptoPlayerPrefs.GetInt("playerDmgLevel") - 1]);

                GameManager.instance.coinTxt.text = currCoin.ToString();        // 인게임 코인 출력

                // 업그레이드 한 공격력을 상태창에 출력
                GameManager.instance.playerDmgView.GetComponentInChildren<Text>().text = CryptoPlayerPrefs.GetInt("playerDmg").ToString();

                StartCoroutine(ActiveMsgBox());
                pText.text = "Purchased";

                AtkPricePreview();
                CryptoPlayerPrefs.SetInt("_atkPreview", atkPreview);
            }
            else
            {
                StartCoroutine(ActiveMsgBox());
                pText.text = "Need Coin!";
            }
        }
        else
        {
            StartCoroutine(ActiveMsgBox());
            pText.text = "Full Upgraded";
        }
    }
    #endregion

    #region 체력 업그레이드 구매
    public void HPMaxPricePreview()
    {
        isHPMaxChange = true;

        cur_hpMaxLabel.text = "HP " + CryptoPlayerPrefs.GetInt("playerMaxHp").ToString();
        hpMaxPreview += 1;
        hpMaxLevel.text = hpMaxPreview.ToString();

        hpMaxLabel.text = GameManager.instance.playerMaxHPTable[hpMaxPreview - 1].ToString();
        hpMaxPriceLabel.text = hpMaxPrice[hpMaxPreview - 1].ToString();
    }

    public void HPMaxPurchase()
    {
        if (isHPMaxChange && int.Parse(hpMaxLevel.text) <= MAXVALUE && CryptoPlayerPrefs.GetInt("playerMaxHp") < GameManager.instance.playerMaxHPTable[MAXVALUE - 1])
        {
            if (CryptoPlayerPrefs.GetInt("coin") >= int.Parse(hpMaxPriceLabel.text))
            {
                CryptoPlayerPrefs.SetInt("playerMaxHPLevel", int.Parse(hpMaxLevel.text));

                #region 코인 출력 갱신
                int currCoin = CryptoPlayerPrefs.GetInt("coin") - int.Parse(hpMaxPriceLabel.text);
                CryptoPlayerPrefs.SetInt("coin", currCoin);
                coinLabel.text = currCoin.ToString();       // 상점 코인 출력
                #endregion

                isHPMaxChange = false;

                CryptoPlayerPrefs.SetInt("playerMaxHp", GameManager.instance.playerMaxHPTable[CryptoPlayerPrefs.GetInt("playerMaxHPLevel") - 1]);

                GameManager.instance.coinTxt.text = currCoin.ToString();        // 인게임 코인 출력

                GameManager.instance.playerMaxHp = CryptoPlayerPrefs.GetInt("playerMaxHp");     // 플레이어 최대 체력을 상점에서 구입한 만큼 늘림
                GameManager.instance.playerHp = CryptoPlayerPrefs.GetInt("playerMaxHp");        // 플레이어 현재 체력을 늘어난 최대체력으로 Full로 채움
                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(CryptoPlayerPrefs.GetInt("playerMaxHp"));        // 체력바의 최대체력 출력 갱신
                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(CryptoPlayerPrefs.GetInt("playerMaxHp"));    // 체력바의 현재체력 출력 갱신

                StartCoroutine(ActiveMsgBox());
                pText.text = "Purchased";

                HPMaxPricePreview();
                CryptoPlayerPrefs.SetInt("_hpMaxPreview", hpMaxPreview);
            }
            else
            {
                StartCoroutine(ActiveMsgBox());
                pText.text = "Need Coin!";
            }
        }
        else
        {
            StartCoroutine(ActiveMsgBox());
            pText.text = "Full Upgraded";
        }
    }
    #endregion

    #region 공격속도 업그레이드 구매
    public void AtkSPricePreview()
    {
        isAtkSChange = true;

        cur_atkSLabel.text = "FireSpeed " + CryptoPlayerPrefs.GetFloat("playerFireSpeed").ToString();
        atkSPreview += 1;
        atkSLevel.text = atkSPreview.ToString();

        atkSLabel.text = GameManager.instance.playerFSTable[atkSPreview - 1].ToString();
        atkSPriceLabel.text = attackSPrice[atkSPreview - 1].ToString();
    }

    public void AtkSPurchase()
    {
        if (isAtkSChange && int.Parse(atkSLevel.text) <= MAXVALUE && CryptoPlayerPrefs.GetFloat("playerFireSpeed") > GameManager.instance.playerFSTable[MAXVALUE - 1])
        {
            if (CryptoPlayerPrefs.GetInt("coin") >= int.Parse(atkPriceLabel.text))
            {
                CryptoPlayerPrefs.SetInt("playerFSLevel", int.Parse(atkSLevel.text));

                #region 코인 출력 갱신
                int currCoin = CryptoPlayerPrefs.GetInt("coin") - int.Parse(atkSPriceLabel.text);
                CryptoPlayerPrefs.SetInt("coin", currCoin);
                coinLabel.text = currCoin.ToString();       // 상점 코인 출력
                #endregion

                isAtkSChange = false;

                CryptoPlayerPrefs.SetFloat("playerFireSpeed", GameManager.instance.playerFSTable[CryptoPlayerPrefs.GetInt("playerFSLevel") - 1]);

                GameManager.instance.coinTxt.text = currCoin.ToString();        // 인게임 코인 출력

                // 업그레이드 한 공격속도를 상태창에 출력
                GameManager.instance.playerFSView.GetComponentInChildren<Text>().text = CryptoPlayerPrefs.GetFloat("playerFireSpeed").ToString();
                GameController.instance.playerFireSpeed = CryptoPlayerPrefs.GetFloat("playerFireSpeed");

                StartCoroutine(ActiveMsgBox());
                pText.text = "Purchased";

                AtkSPricePreview();
                CryptoPlayerPrefs.SetInt("_atkSPreview", atkSPreview);
            }
            else
            {
                StartCoroutine(ActiveMsgBox());
                pText.text = "Need Coin!";
            }
        }
        else
        {
            StartCoroutine(ActiveMsgBox());
            pText.text = "Full Upgraded";
        }
    }
    #endregion

    IEnumerator ActiveMsgBox()
    {
        msgBox.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);
        msgBox.SetActive(false);
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