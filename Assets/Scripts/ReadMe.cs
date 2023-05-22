#region 게임 조작 설명
/********************************************************************

- 이동 : WASD
- 공격 : 마우스 좌클릭

********************************************************************/
#endregion

#region 터치패드로 전환 시 변경사항
/********************************************************************
1. EasyTouchControlsCanvas 활성화
2. Player안에 MainCamera 넣기
3. MainCamera의 CameraMouseLook 스크립트 비활성화
4. Player의 PlayerMoveController 스크립트 비활성화
********************************************************************/
#endregion

#region 컨텐츠 추가 시 참고
/********************************************************************
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■  적 추가  ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
- Animation Controller생성해서 애니메이션 설정(트리거 및 Loop)
- 좀비 prefab에 Controller(Animator), Character Controller, ZombieProcess(C#), splineMove(C#) 추가
- 만들어놓은 Enemy Prefab ~ 팔에 빈 게임오브젝트 추가 ~ Collider, Rigidbody추가 ~ 달린 Collider에 ColliderChk 스크립트 추가
- EnemyManager 스크립트 ~ EnemySpawn의 Instantiate에서 enemy생성배열
- EnemyManager(Main씬) 배열에 추가할 좀비 프리팹 추가

■■■■■■■■■■■■■■■■■■■■■■■■■■■ 상점 품목 추가 ■■■■■■■■■■■■■■■■■■■■■■■■■■■
- 원하는 스크립트에 보안코드 추가(저장 및 로드)
- GameManager에 변수들 추가
- GameManager ~ Start에 출력항목 추가
- ShopManager에 변수들 및 함수 추가
********************************************************************/
#endregion

#region 스테이지 관리 메커니즘
/********************************************************************
1. 적군 모두 죽임
2. 다음 스테이지 이동 포탈이 나타남
(GameManager : 포탈 생성되며 플래그 A를 Off)
3. 다음 스테이지로 이동 및 스테이지 로드.
(GameController : 포탈에 들어가며 clearStage +1)
(StageManager의 Awake : if문에 Off된 플래그A가 걸림 : currentStage +1)
(StageManager의 Start : 저장된 currentStage 정보 갱신)
********************************************************************/
#endregion

#region 상점 시스템
/********************************************************************
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ 초기 상태 ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
playerDmg = 1
atkLevel = 1
atkPreview = 1

■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ 처음 업글 ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
1.playerDmgLevel = atkLevel
2.playerDmg = playerDmgLevel - 1의 데미지테이블 수치로 변함
(playerDmg = playerDmgLevel[0] 이 됨)
3.cur_atkLabel.text가 playerDmg로 바뀜(2)
4.atkPreview + 1, atkLevel.text + 1

playerDmg = 2
atkLevel = 2
atkPreview = 2

■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ 마지막 업글 ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
playerDmg = 8
atkLevel = 5
atkPreview = 5

1.playerDmgLevel = atkLevel
2.playerDmg = playerDmgLevel - 1의 데미지테이블 수치로 변함
(playerDmg = playerDmgLevel[0] 이 됨)
3.cur_atkLabel.text가 playerDmg로 바뀜(2)
4.atkPreview + 1, atkLevel.text + 1

■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■  Tables  ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■
playerDmgLevel[6]
2 / 3 / 4 / 6 / 8 / 10

playerMaxHPTable[6]
125 / 150 / 175 / 200 / 250 / 300
********************************************************************/
#endregion

#region Scripts(Main Scene)
/********************************************************************
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ Scripts ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

[Player]
- FollowSpawnPoint : 총알 발사 이펙트(총구)
- Bullet : 플레이어 공격 사거리 및 탄속
- GameController : 플레이어 공격속도 및 적 조준 시 체력바 출력 / 플레이어의 포탈 이동(이 때, clearStage를 증가시킴)
- GameManager : 플레이어 레벨, 경험치, 돈, 데미지, 체력, 스테이지 진행도 저장(Gate생성 시, titleOnlyStage를 조정하여 StageManager에서 clearStage, currentStage를 수정할 수 있게 만듦)
- PlayerState : 플레이어 상태 설정(시작상태, 기본, 이동, 공격 시, 데미지 입을 시)
- PlayerMoveController : 플레이어 이동속도 저장

---------------------------------------------------------------------

[Enemy(Zombies)]
- ColliderChk : Damage : 적 팔에 달린 충돌체의 플레이어와 충돌 시 효과(적의 공격데미지 조절)
- EnemyManager : 적 및 보스 소환
- EnemyPathManager : 적 이동경로 설정
- GameController : HPBar출력
- ZombieProcess : 스테이지에 따라 적 체력, 이동속도, hitRecovery 업그레이드 / 적 죽일 시 킬카운트, 경험치, 골드 획득, 다음 스테이지 포탈 생성 / 적 웨이포인트 설정

---------------------------------------------------------------------

[Effect]
- BloodEffect : 피격 시, 플레이어 화면 피범벅 효과

---------------------------------------------------------------------

[Camera]
- CameraMouseLook : 마우스 움직임에 따라 카메라 x, y축 이동
- ShkerManager : 카메라 지진 효과 생성함수

---------------------------------------------------------------------

[Manager]
- GameManager : 플레이어 레벨, 경험치, 돈, 데미지, 체력, 스테이지 진행도 저장
- EnemyManager : 적 및 보스 소환
- EnemyPathManager : 적 이동경로 설정
- PlayerMoveController : 플레이어 이동속도 저장
- 
- PlayerState : 플레이어 상태 설정(시작상태, 기본, 이동, 공격 시, 데미지 입을 시)
- GameController : 플레이어 공격속도 및 적 조준 시 체력바 출력
- ZombieProcess : 스테이지에 따라 적 체력, 이동속도, hitRecovery 업그레이드 / 적 죽일 시 킬카운트, 경험치, 골드 획득, 다음 스테이지 포탈 생성 / 적 웨이포인트 설정
- ParticleManager : 파티클 모음
- SoundManager : 사운드 모음 및 재생함수
- ShkerManager : 카메라 지진 효과 생성함수

---------------------------------------------------------------------

[ETC]
- DayAndNight : 메인 스테이지의 밤낮 효과 설정
- ParticleManager : 파티클 모음
- SoundManager : 사운드 모음 및 재생함수

---------------------------------------------------------------------

[보안코드 사용 스크립트]
- Bullet : 플레이어 총알 사거리 저장
- GameManager : 플레이어 레벨, 경험치, 돈, 데미지, 체력, 스테이지 진행도 저장
- GameController : 플레이어 공격속도 저장
- PlayerMoveController : 플레이어 이동속도 저장
- TitleManager : 스테이지 진행도 저장

********************************************************************/
#endregion

#region Scripts(Title, Loading, Shop, Stage Scene)
/********************************************************************
■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■ Scripts ■■■■■■■■■■■■■■■■■■■■■■■■■■■■■■

[Title]
- TitleManager : StartGame버튼 누를 때 기능 / 스테이지 진행도 저장 및 로드
- StageIconActive : 스테이지 잠금 및 잠금해제 / 스테이지 클릭 시, 오픈할 스테이지 정보 저장

---------------------------------------------------------------------

[Loading]
- 로드할 씬을 추가.
- 로딩바 생성 및 출력.

---------------------------------------------------------------------

[Shop]
- 

---------------------------------------------------------------------

[Stage]
- clearStage, currentStage값 설정 및, 로드하는 Stage설정.

---------------------------------------------------------------------

[보안코드 사용 스크립트]
- 

********************************************************************/
#endregion

#region 게임 완성 전에 바꿀 것들
/********************************************************************
- 터치패드 활성화
- 
********************************************************************/
#endregion