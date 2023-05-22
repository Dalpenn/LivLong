using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    #region 변수들
    public Transform bullet;        //총알 프리팹
    public Transform spawnPoint;    //총알 생성 위치
    public Transform muzzlePoint;   // 총알 발사이펙트 생성 위치

    bool isFire = false;            //코루틴 제어 플래그
    public bool isAttack = false;   //플레이어 공격 상태 체크 플래그
    public bool isShoot = false;    //모바일 공격 제어 플래그

    public static GameController instance;

    public float playerFireSpeed;
    int playerFSLevel = 1;
    #endregion

    private void Awake()
    {
        instance = this;

        #region 보안코드/정보저장 및 호출
        CryptoPlayerPrefs_HasKeyFloatFind("playerFireSpeed", playerFireSpeed = 0.4f);
        CryptoPlayerPrefs_HasKeyIntFind("playerFSLevel", playerFSLevel = 1);

        playerFireSpeed = CryptoPlayerPrefs.GetFloat("playerFireSpeed");
        playerFSLevel = CryptoPlayerPrefs.GetInt("playerFSLevel");
        #endregion
    }

    void Update()
    {
        SearchEnemy();

        #region PC용 발사버튼 제어
        //if (Input.GetMouseButton(0) && !GameManager.instance.isUIDetection)
        //{
        //    isAttack = true;
        //    StartCoroutine(FireController(playerFireSpeed));
        //}

        //else if (Input.GetMouseButtonUp(0))
        //{
        //    isAttack = false;
        //}
        #endregion

        #region 모바일용 발사버튼 제어
        if (isShoot && !GameManager.instance.isUIDetection)
        {
            isAttack = true;
            StartCoroutine(FireController(playerFireSpeed));
        }

        else
        {
            isAttack = false;
        }
        #endregion
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Gate")
        {
            // 게이트 입장 시, 현재 스테이지가 클리어 스테이지보다 높으면
            if (CryptoPlayerPrefs.GetInt("currentStage") > CryptoPlayerPrefs.GetInt("clearStage"))
            {
                CryptoPlayerPrefs.SetInt("clearStage", CryptoPlayerPrefs.GetInt("clearStage") + 1);
            }

            CryptoPlayerPrefs.SetInt("currentStage", CryptoPlayerPrefs.GetInt("currentStage") + 1);

            Destroy(other.gameObject);
            
            Debug.Log("게이트 입장 :" + "curstage :" + CryptoPlayerPrefs.GetInt("currentStage") + "/clearstage" + CryptoPlayerPrefs.GetInt("clearStage"));

            SceneManager.LoadScene("Loading", LoadSceneMode.Single);
        }
    }

    #region 총알, 총 발사 이펙트 생성
    IEnumerator FireController(float fSpeed)
    {
        if (!isFire)
        {
            isFire = true;
            FireStart();
            MuzzleEffect();
            yield return new WaitForSeconds(fSpeed);
            isFire = false;

            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_Fire, 0.0f, SoundManager.instance.volume_Gun);
        }
    }

    void FireStart()        // 총알 생성
    {
        Instantiate(bullet, spawnPoint.position, spawnPoint.rotation );
        EZCameraShake.CameraShaker.Instance.ShakeOnce(3f, 3f, 0f, 0.3f);
    }

    void MuzzleEffect()
    {
        GameObject muzzleEffect = Instantiate(ParticleManager.instance.muzzleImpact, muzzlePoint.position, muzzlePoint.rotation);
        Destroy(muzzleEffect, 0.15f);
    }
    #endregion

    #region 조준 시 적 체력바 출력
    void SearchEnemy()
    {
        Debug.DrawRay(spawnPoint.position, spawnPoint.forward * 45, Color.red);

        RaycastHit hit;
        
        if(Physics.Raycast(spawnPoint.position, spawnPoint.forward, out hit, 45))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                if (hit.transform.gameObject.tag == "EnemyBoss_1")
                {
                    EnemyManager.instance.BossHPBar.SetActive(true);
                    hit.transform.SendMessage("EnemyInfo", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    EnemyManager.instance.enemyHpBar.SetActive(true);
                    EnemyManager.instance.enemyHpBar.GetComponent<EnergyBarToolkit.EnergyBarFollowObject>().followObject = hit.transform.gameObject;
                    hit.transform.SendMessage("EnemyInfo", SendMessageOptions.DontRequireReceiver);
                    //hit.transform.GetComponent<ZombieProcess>().EnemyInfo();
                }
            }
            else
            {
                EnemyManager.instance.BossHPBar.SetActive(false);
                EnemyManager.instance.enemyHpBar.SetActive(false); 
            }
        }
    }
    #endregion

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