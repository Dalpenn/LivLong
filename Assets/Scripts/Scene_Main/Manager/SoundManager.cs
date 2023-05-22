using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    #region 변수들
    public float volume_Sfx = 1.0f;
    public float volume_Gun = 1.0f;
    public float volume_BGM = 1.0f;

    public bool isSfxMute = false;      // 음소거 기능

    //public AudioSource sound_bgm;         // BGM 전용

    public AudioClip sound_Fire;

    public AudioClip sound_Step_A;      // 플레이어 걷는 소리 3가지
    public AudioClip sound_Step_B;
    public AudioClip sound_Step_C;

    public AudioClip sound_ZombieAtk;       // 적 공격 소리
    public AudioClip sound_BloodStabHit;    // 적이 맞을 때 소리
    public AudioClip sound_HandAtk;         // 적한테 플레이어가 맞을 때 소리
    public AudioClip sound_BloodDeath;

    public AudioClip sound_GameStartVoice;

    public AudioClip sound_LvUP;

    public AudioClip sound_OpenPortal;

    public AudioClip sound_ShopOpenClose;

    public AudioClip sound_PlayerDie;

    public static SoundManager instance;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    // 동적 생성 전용 효과음
    public void PlaySfx(Vector3 pos, AudioClip sfx, float delayed, float volume)
    {
        if(isSfxMute)
        {
            return;     // isSfxMute가 true면 여기서 끊음
        }

        StartCoroutine(PlaySfxIE(pos, sfx, delayed, volume));
    }

    #region PlaySfxIE(pos, clip, delayed, volume) 의 기능
    /*******************************************************
    [ PlaySfxIE(pos, clip, delayed, volume) 의 기능 ]
    1. (직접 값 입력)delayed만큼 기다렸다가 아래 순서로 동작 시작
    2. "Sfx" 이름의 빈 오브젝트를 생성
    3. Sfx에 AudioSource를 추가
    4. (직접 값 입력)Sfx의 위치를 pos로 설정
    5. (직접 값 입력)재생할 사운드를 clip으로 설정
    6. (직접 값 입력)재생할 사운드의 볼륨을 volume으로 설정
    7. 사운드 재생
    8. 사운드클립의 재생이 완전히 끝난 후, Sfx오브젝트 파괴
    *******************************************************/
    #endregion

    IEnumerator PlaySfxIE(Vector3 pos, AudioClip sfx, float delayed, float volume)
    {
        yield return new WaitForSeconds(delayed);

        GameObject sfxObj = new GameObject("Sfx");      // 빈 오브젝트를 생성 후, 이름을 Sfx로 변경
        AudioSource _aud = sfxObj.AddComponent<AudioSource>();

        sfxObj.transform.position = pos;

        // Inspector창 AudioSource의 다양한 기능들
        _aud.clip = sfx;                // 재생할 오디오 파일

        _aud.minDistance = 5.0f;        // 소리가 들리는 최소거리 ~ 최대거리
        _aud.maxDistance = 10.0f;

        _aud.volume = volume;           // 소리 볼륨
        _aud.Play();                    // 소리 재생

        Destroy(sfxObj, sfx.length);    // 클립이 끝까지 재생된 후, 파괴됨
    }
}
