using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMoveController : MonoBehaviour
{
    #region 변수들
    [HideInInspector]public float playerMoveSpeed = 6.5f;
    [HideInInspector]public float jumpSpeed = 10.0f;
    [HideInInspector]public float gravity = 20.0f;

    public Transform camPos;         //  메인카메라 위치

    CharacterController cc;

    Vector3 moveDir = Vector3.zero;  //플레이어 기준 위치

    float yVelocty = 0;              //점프에 활용되는 변수
    #endregion

    private void Awake()
    {
        #region 보안코드 저장 및 불러오기
        CryptoPlayerPrefs_HasKeyFloatFind("playerMoveSpeed", playerMoveSpeed = 6.5f);
        playerMoveSpeed = CryptoPlayerPrefs.GetFloat("playerMoveSpeed");
        #endregion
    }

    void Start()
    {
        cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        MoveStart();
        Jump();
    }

    // 카메라와 관련된 코드는 LateUpdate에 넣는다
    private void LateUpdate()
    {
        Camera.main.transform.position = camPos.position;
        transform.rotation = Camera.main.transform.rotation;
    }

    // 이동 함수 정의
    void MoveStart()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        moveDir.x = x;
        moveDir.y = 0;
        moveDir.z = z;

        moveDir = transform.TransformDirection(moveDir);    // TransformDirection : 로컬 이동벡터를 씬의 글로벌값으로 변환시키는 기능

        moveDir *= playerMoveSpeed;                         // moveDir = moveDir * moveSpeed

        yVelocty -= gravity * Time.deltaTime;               // y축으로 항상 일정속도 가지도록 하여 중력 설정
        moveDir.y = yVelocty;

        cc.Move(moveDir * Time.deltaTime);                  // 실시간 움직임 위해서 Time.deltaTime값도 포함
    }

    // 점프 정의
    void Jump()
    {
        if(cc.isGrounded)
        {
            if (Input.GetButtonDown("Jump"))  //스페이스바
            {
                yVelocty = jumpSpeed;
            }
        }
    }

    //캐릭터 컨트롤러 충돌 체크
    //private void OnControllerColliderHit(ControllerColliderHit hit)
    //{
    //    //Debug.Log("캐릭터 컨트롤러 충돌 체크");
    //    //Debug.Log(hit.gameObject.name);
    //}

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
