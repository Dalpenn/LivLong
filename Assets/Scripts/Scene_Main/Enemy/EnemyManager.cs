using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyManager : MonoBehaviour
{
    #region 변수들
    public GameObject[] enemy;
    public float spawnTime = 1.0f;
    int spawnCnt = 0; 
    public int maxSpawnCnt = 6;  // 생성가능한 적 총 개체 수
    public int initMaxSpawnCnt;   //
    bool isSpawn = false;         // 코루틴 제어 플래그

    public GameObject enemyKillBar;
    public int killCnt = 0;

    public GameObject enemyHpBar;
    public GameObject BossHPBar;

    public float[] enemyExp;  // 적 경험치 관리자
    public int[] enemyCoinAmount;

    bool isBossSpawned = false;

    public static EnemyManager instance;
    #endregion

    #region Awake, Start, Update
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        ESpawnCntCtrl();
    }

    void Update()
    {
        if (spawnCnt >= maxSpawnCnt)
        {
            if (killCnt >= maxSpawnCnt && !isBossSpawned && CryptoPlayerPrefs.GetInt("currentStage") % 5 == 0)
            {
                SpawnBoss();

                return;     // 보스 소환 후에 여기서 끊기
            }
            else
            {
                return;     // 맵에 최대 개체수가 소환된 상태면 아래로 진행하지 않고 여기서 끊기
            }
        }
        StartCoroutine(EnemySpawn());  // 코루틴 함수 호출
    }
    #endregion

    public void ESpawnCntCtrl()
    {
        // stage가 올라갈수록 zombie가 소환되는 숫자가 많아지도록 설정
        maxSpawnCnt += (CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1);
    }

   IEnumerator EnemySpawn()
    {
        #region maxSpawnCnt < 15 일 때 Enemy[0]소환
        if (!isSpawn && maxSpawnCnt < 15)
        {
            isSpawn = true;
            GameObject enemyObj = Instantiate(enemy[0]);

            SpawnEnemyHere(enemyObj);
            
            // 생성된 적들을 서로 구분하기 위해 생성된 적들에게 각각 이름을 붙여줌
            enemyObj.name = "Enemy_" + spawnCnt;

            yield return new WaitForSeconds(spawnTime);

            isSpawn = false;
            ++spawnCnt;     // 생성된 적 개체수 체크

            #region 적이 체력바 갖고 생성되도록 하는 코드 예시
            //GameObject enemyHP = Instantiate(enemyHPBar);
            //enemyHP.name = "EnemyHP_" + spawnCnt;             // 생성된 적들을 서로 구분하기 위해 생성된 적들에게 각각 이름을 붙여줌
            //enemyHP.transform.SetParent(GameObject.Find("Canvas").transform);       // SetParent 로 생성된 enemyHP의 부모를 설정해줌
            //enemyHP.GetComponent<EnergyBarToolkit.EnergyBarFollowObject>().followObject = enemyObj;    // EnergyBarToolkit에 들어있는 followObject 컴포넌트를 사용하여 생성된 적군에 체력바를 붙여줌
            #endregion
        }
        #endregion
        #region maxSpawnCnt >= 15 일 때 Enemy[1]소환
        else if (!isSpawn && maxSpawnCnt >= 15)
        {
            isSpawn = true;

            GameObject enemyObj = Instantiate(enemy[1]);

            int ranSpawn = Random.Range(0, 2);

            if (ranSpawn == 0)
            {
                float x = Random.Range(-40.0f, 75.0f);
                float z = Random.Range(2f, 11.0f);
                enemyObj.transform.position = new Vector3(x, 1.0f, z);
            }
            else
            {
                float x = Random.Range(15f, 28f);
                float z = Random.Range(-5.5f, -46.0f);
                enemyObj.transform.position = new Vector3(x, 1.0f, z);
            }

            // 생성된 적들을 서로 구분하기 위해 생성된 적들에게 각각 이름을 붙여줌
            enemyObj.name = "Enemy_" + spawnCnt + "_female";

            yield return new WaitForSeconds(spawnTime);

            isSpawn = false;
            ++spawnCnt;     // 생성된 적 개체수 체크
            maxSpawnCnt -= 4;
        }
        #endregion

        enemyKillBar.GetComponent<EnergyBar>().SetValueMax(maxSpawnCnt);
    }

    #region 적이 무작위 구역에서 생성되도록 하는 함수
    void SpawnEnemyHere(GameObject obj)
    {
        // 1구역 x(-40 ~ 80) / z(-2 ~ 13)
        // 2구역 x(13.5 ~ 29.5) / z(-5.5 ~ -46)
        int ranSpawn = Random.Range(0, 2);

        float x = Random.Range(-60f, 60f);
        float z = Random.Range(-60f, 60f);
        obj.transform.position = new Vector3(x, 1.0f, z);      // 항상 적은 1m 상공에서 생성되도록 설정
    }
    #endregion

    void SpawnBoss()
    {
        isSpawn = false;

        GameObject enemyObj = Instantiate(enemy[2]);

        float x = Random.Range(20f, 60f);
        float z = Random.Range(-60f, -30f);
        enemyObj.transform.position = new Vector3(x, 1.0f, z);

        enemyObj.name = "Enemy_" + spawnCnt + "_Boss1";

        isBossSpawned = true;
    }

    public void PauseToResume(GameObject obj)
    {
        obj.GetComponent<SWS.splineMove>().Pause();  // 일시정지

        obj.GetComponent<Animator>().SetBool("isIdle", true);
        obj.GetComponent<Animator>().SetBool("isWalk", false);

        StartCoroutine(ResumeToPause(2.0f, obj));
    }


    IEnumerator ResumeToPause(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time);

        try
        {
            obj.GetComponent<SWS.splineMove>().Resume();  // 일시정지 해지

            obj.GetComponent<Animator>().SetBool("isIdle", false);
            obj.GetComponent<Animator>().SetBool("isWalk", true);
        }
        catch { };
    }
}
