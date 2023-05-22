using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieProcess : MonoBehaviour
{
    #region 변수들
    enum ZOMBIESTATE { IDLE = 0, MOVE, ATTACK, DAMAGE, DEAD }
    ZOMBIESTATE zombieState = ZOMBIESTATE.IDLE;

    Vector3 currentVelocty;
    float gravity = 10.0f;
    [HideInInspector]public int hp = 5;
    [HideInInspector]public int maxHp = 5;
    [HideInInspector]public float speed = 5.0f;
    [HideInInspector]public float rotSpeed = 10.0f;
    [HideInInspector]public float attakbleRange = 2.5f;
    [HideInInspector]public float traceRange = 13.0f;
    CharacterController cc;

    Transform target;

    Animator _ani;

    SWS.PathManager path;

    Vector3 enemyOrigin;

    bool isAtkSound = false;
    bool isBossKilled = false;

    public static ZombieProcess instance;
    #endregion

    private void Awake()
    {
        instance = this;

        cc = GetComponent<CharacterController>();
        _ani = GetComponent<Animator>();
        
        if(PlayerState.instance.playerState != PlayerState.PLAYERSTATE.DEAD)
        {
            target = GameObject.FindGameObjectWithTag("Player").transform;
        }

        #region 웨이포인트 설정
        enemyOrigin = transform.position;  // 자신이 생성된 위치 기억
        path = Instantiate(EnemyPathManager.instance.enemyPath[0]);
        this.GetComponent<SWS.splineMove>().pathContainer = path;
        #endregion

        #region 스테이지에 따라 좀비 체력/이동속도 업그레이드
        if (this.gameObject.tag == "Enemy_1")
        {
            maxHp = 5 + (1 * (CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1));
            hp = maxHp;
            speed = 5.0f + (0.35f * (CryptoPlayerPrefs.GetInt("currentStage") - 1));
        }
        else if (this.gameObject.tag == "Enemy_2")
        {
            maxHp = 15 + (2 * (CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1));
            hp = maxHp;
            speed = 5.5f + (0.5f * (CryptoPlayerPrefs.GetInt("currentStage") - 1));
        }
        else if (this.gameObject.tag == "EnemyBoss_1")
        {
            maxHp = 70 + (3 * (CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1));
            hp = maxHp;
            speed = Random.Range(5f, 10f);
            traceRange = 20f;
            attakbleRange = 5f;
        }
        #endregion
    }

    void Start()
    {
        #region 웨이포인트 설정
        // y값만 웨이포인트 것으로 잡고, x & z축은 좀비 위치로 잡음
        path.transform.position = new Vector3(this.transform.position.x, path.transform.position.y, this.transform.position.z);

        this.GetComponent<SWS.splineMove>().StartMove();  // 웨이포인트 시작

        this.GetComponent<SWS.splineMove>().events[0].AddListener(delegate { EnemyManager.instance.PauseToResume(this.gameObject); });
        this.GetComponent<SWS.splineMove>().events[1].AddListener(delegate { EnemyManager.instance.PauseToResume(this.gameObject); });
        this.GetComponent<SWS.splineMove>().events[2].AddListener(delegate { EnemyManager.instance.PauseToResume(this.gameObject); });
        this.GetComponent<SWS.splineMove>().events[3].AddListener(delegate { EnemyManager.instance.PauseToResume(this.gameObject); });
        #endregion
    }

    void Update()
    {
        // 이렇게 해놓으면 DEAD상태에서는 return으로 인해 아래의 함수들로 진행이 더 이상 되지 않는다
        if (zombieState == ZOMBIESTATE.DEAD || PlayerState.instance.playerState == PlayerState.PLAYERSTATE.DEAD)    
        {
            return;
        }

        if (hp <= 0)
        {
            zombieState = ZOMBIESTATE.DEAD;
        }

        switch (zombieState)
        {
            case ZOMBIESTATE.IDLE:
                {
                    _ani.SetBool("isIdle", true);  // 모션
                    _ani.SetBool("isAttack", false);
                    _ani.SetBool("isRun", false);

                    float distance = Vector3.Distance(target.position, transform.position);

                    //Debug.Log(distance);

                    // 좀비의 플레이어 추적 유효 범위
                    if (distance < traceRange && PlayerState.instance.playerState != PlayerState.PLAYERSTATE.DEAD)
                    {
                        zombieState = ZOMBIESTATE.MOVE;
                        if (distance <= attakbleRange)          // 좀비의 공격 유효 범위
                        {
                            zombieState = ZOMBIESTATE.ATTACK;
                        }
                    }

                    break;
                }
            case ZOMBIESTATE.MOVE:
                {
                    this.GetComponent<SWS.splineMove>().Stop();     //웨이포인트 중지

                    _ani.SetBool("isRun", true);
                    _ani.SetBool("isAttack", false);
                    _ani.SetBool("isWalk", false);

                    float distance = Vector3.Distance(target.position, transform.position);

                    Vector3 dir = target.position - transform.position;
                    dir.y = 0;
                    dir.Normalize();    // 거리 평준화 함수
                    cc.SimpleMove(dir * speed);

                    //Lerp - 선형보간함수
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

                    // 플레이어가 공격 유효범위에 들어왔을 때
                    if (distance <= attakbleRange && PlayerState.instance.playerState != PlayerState.PLAYERSTATE.DEAD)
                    {
                        zombieState = ZOMBIESTATE.ATTACK;
                    }
                    // 플레이어가 추적 유효범위에서 벗어났을 때
                    if (distance > traceRange || PlayerState.instance.playerState == PlayerState.PLAYERSTATE.DEAD)
                    {
                        zombieState = ZOMBIESTATE.IDLE;
                        MonsterAwake();                  //웨이포인트 재개
                    }

                    break;
                }
            case ZOMBIESTATE.ATTACK:
                {
                    StartCoroutine(ZombieAtkSfx());

                    this.GetComponent<SWS.splineMove>().Stop();     //웨이포인트 중지

                    _ani.SetBool("isRun", false);
                    _ani.SetBool("isAttack", true);
                    _ani.SetBool("isIdle", false);
                    _ani.SetBool("isWalk", false);

                    float distance = Vector3.Distance(target.position, transform.position);
                    Vector3 dir = target.position - transform.position;
                    dir.y = 0;
                    dir.Normalize(); // 거리 평준화 
                    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), rotSpeed * Time.deltaTime);

                    IsAtkToRun();

                    break;
                }
            case ZOMBIESTATE.DAMAGE:
                {
                    break;
                }
            case ZOMBIESTATE.DEAD:
                {
                    Destroy(gameObject);
                    Instantiate(ParticleManager.instance.deadEffect, transform.position, Quaternion.identity);      // transform.rotation 과 Quaternion.identity는 동일한 기능

                    #region 적 죽을 때, 체력바 지우기 참고용
                    /***
                    // 적이 죽을 때, 적 체력바 지우도록 설정
                    if (name.Length == 7)
                    {
                        Destroy(GameObject.Find("EnemyHP_" + name.Substring(6, 1)));
                    }
                    else 
                    {
                        Destroy(GameObject.Find("EnemyHP_" + name.Substring(6, 2)));
                    }
                    ***/
                    #endregion

                    #region 킬 카운트 증가
                    // ■■■■■■■■■■정보: 보스 킬■■■■■■■■■■
                    if (gameObject.tag == "EnemyBoss_1")
                    {
                        isBossKilled = true;
                    }
                    else
                    {
                        // ■■■■■■■■■■정보: 좀비 킬■■■■■■■■■■
                        ++EnemyManager.instance.killCnt;
                        //킬수 체크
                        EnemyManager.instance.enemyKillBar.GetComponent<EnergyBar>().SetValueCurrent(EnemyManager.instance.maxSpawnCnt - EnemyManager.instance.killCnt);
                    }
                    #endregion

                    #region 경험치 획득
                    // ■■■■■■■■■■정보: 좀비 죽일 시 경험치■■■■■■■■■■
                    //GameManager.instance.ExpCal(EnemyManager.instance.enemyExp[0]);
                    float enemyExp = EnemyManager.instance.enemyExp[Random.Range(0, EnemyManager.instance.enemyExp.Length)];
                    float enemyExpCalc_byStageLv = 1 + (0.3f * CryptoPlayerPrefs.GetInt("currentStage"));       // 현재 스테이지 레벨에 따른 추가량
                    GameManager.instance.ExpCal(enemyExpCalc_byStageLv * enemyExp);
                    #endregion

                    #region 골드 획득
                    // ■■■■■■■■■■정보: 좀비 죽일 시 골드■■■■■■■■■■
                    int enemyCoin = EnemyManager.instance.enemyCoinAmount[Random.Range(0, EnemyManager.instance.enemyCoinAmount.Length)];
                    int enemyCoinCalc_byStageLv = (int)(1 + (0.5f * CryptoPlayerPrefs.GetInt("currentStage")));       // 현재 스테이지 레벨에 따른 추가량
                    GameManager.instance.AddCoin(enemyCoinCalc_byStageLv * enemyCoin);
                    #endregion

                    #region 다음 스테이지 포탈 생성
                    if (EnemyManager.instance.killCnt >= EnemyManager.instance.maxSpawnCnt)
                    {
                        if (CryptoPlayerPrefs.GetInt("currentStage") % 5 == 0)   // 보스는 5스테이지마다 나옴
                        {
                            if (isBossKilled)    // 보스 죽일 시, 게이트오픈 & 골드획득
                            {
                                GameManager.instance.AddCoin(500);

                                Debug.Log("스테이지 클리어!");
                                GameManager.instance.GateGen();
                            }
                        }
                        else
                        {
                            Debug.Log("스테이지 클리어!");
                            GameManager.instance.GateGen();
                        }
                    }
                    #endregion

                    SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_BloodDeath, 0.0f, SoundManager.instance.volume_Sfx);

                    break;
                }
        }
    }

    // 몬스터 초기화
    void MonsterAwake()
    {
        _ani.SetBool("isIdle", true);
        _ani.SetBool("isAttack", false);
        _ani.SetBool("isRun", false);

        transform.position = enemyOrigin;       // 처음 생성된 위치로 복귀

        this.GetComponent<SWS.splineMove>().StartMove();  // 웨이포인트 시작

        hp = maxHp;     // 체력 원래상태로 초기화
    }

    #region 좀비 인식거리 벗어날 시 행동
    // Attack애니메이션의 모델링 이벤트 함수에 적용 ~ 거기서 실행한다
    void IsAtkToRun()
    {
        float distance = Vector3.Distance(target.position, transform.position);

        if (distance > attakbleRange || PlayerState.instance.playerState == PlayerState.PLAYERSTATE.DEAD)
        {
            zombieState = ZOMBIESTATE.MOVE;
        }
    }
    #endregion

    #region 좀비 힛리커버리 관리
    public void EnemyDamage()
    {
        //--hp;
        int dmg = CryptoPlayerPrefs.GetInt("playerDmg");
        hp -= dmg;

        //Debug.Log("좀비체력: " + hp);
        _ani.SetTrigger("hit");
        zombieState = ZOMBIESTATE.DAMAGE;

        // 스테이지 진행에 따라 좀비 힛리커버리 증가(데미지 입을 시, 경직 시간 감소)
        float EnemyhitRecovery = 0.5f - 0.015f * CryptoPlayerPrefs.GetInt("currentStage");

        Invoke("IdleState", EnemyhitRecovery);

        SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_BloodStabHit, 0.0f, SoundManager.instance.volume_Sfx);
    }
    #endregion

    void IdleState()
    {
        zombieState = ZOMBIESTATE.IDLE;
    }

    public void EnemyInfo()
    {
        // ■■■■■■■■■■GameController의 SearchEnemy함수 작동 시, SendMessage로 실행됨■■■■■■■■■■
        if(gameObject.tag == "EnemyBoss_1")
        {
            EnemyManager.instance.BossHPBar.GetComponent<EnergyBar>().SetValueMax(maxHp);
            EnemyManager.instance.BossHPBar.GetComponent<EnergyBar>().SetValueMin(0);
            EnemyManager.instance.BossHPBar.GetComponent<EnergyBar>().SetValueCurrent(hp);
        }
        else
        {
            EnemyManager.instance.enemyHpBar.GetComponent<EnergyBar>().SetValueMax(maxHp);
            EnemyManager.instance.enemyHpBar.GetComponent<EnergyBar>().SetValueMin(0);
            EnemyManager.instance.enemyHpBar.GetComponent<EnergyBar>().SetValueCurrent(hp);
        }

        #region 좀비 이름에 따라 체력바 찾기 참고용
        /***
        // 각각의 독립적인 체력바
        if (name.Length == 7)       // 이름이 "Enemy_1자리숫자" 인 경우에는 "Enemy_"가 6글자이므로 다음 첫 글자가 spawnCnt로 체력바 이름과 일치한다
        {
            GameObject.Find("EnemyHP_" + this.name.Substring(6, 1)).GetComponent<EnergyBar>().SetValueMax(maxHP);        // name.Substring(a, b) 는 이름에서 a칸만큼 건너뛰고 b번째 string을 비교하는 것
            GameObject.Find("EnemyHP_" + this.name.Substring(6, 1)).GetComponent<EnergyBar>().SetValueMin(0);
            GameObject.Find("EnemyHP_" + this.name.Substring(6, 1)).GetComponent<EnergyBar>().SetValueCurrent(hp);
        }
        else                        // 그러나 이름이 "Enemy_2자리숫자" 인 경우에는 "Enemy_1"가 7글자이므로 6칸 건너뛰고 다음 첫 글자가 spawnCnt와 일치하지 않는다. 그러므로 6칸 건너뛰고 두번째 글자를 비교해야 함
        {
            GameObject.Find("EnemyHP_" + this.name.Substring(6, 2)).GetComponent<EnergyBar>().SetValueMax(maxHP);
            GameObject.Find("EnemyHP_" + this.name.Substring(6, 2)).GetComponent<EnergyBar>().SetValueMin(0);
            GameObject.Find("EnemyHP_" + this.name.Substring(6, 2)).GetComponent<EnergyBar>().SetValueCurrent(hp);
        }
        ***/
        #endregion
    }

    IEnumerator ZombieAtkSfx()
    {
        if(!isAtkSound)
        {
            isAtkSound = true;

            SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_ZombieAtk, 0.0f, SoundManager.instance.volume_Sfx);
            
            yield return new WaitForSeconds(SoundManager.instance.sound_ZombieAtk.length);

            isAtkSound = false;
        }
    }
}
