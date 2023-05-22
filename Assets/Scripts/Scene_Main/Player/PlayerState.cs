using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    #region 변수들
    public enum PLAYERSTATE { AWAKE = 0, IDLE, MOVE, ATTACK, DAMAGE, RELOAD, DEAD }
    public PLAYERSTATE playerState = PLAYERSTATE.AWAKE;     // 초기 상태 지정

    bool isRun = false;     // 스텝 효과음 코루틴의 제어플래그

    public static PlayerState instance;
    #endregion

    private void Awake()
    {
        instance = this;
    }

    void Update()
    {
        switch (playerState)
        {
            case PLAYERSTATE.AWAKE:
                {
                    playerState = PLAYERSTATE.IDLE;
                    break;
                }
            case PLAYERSTATE.IDLE:
                {
                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        playerState = PLAYERSTATE.MOVE;
                    }
                    else if (GameController.instance.isAttack)
                    {
                        playerState = PLAYERSTATE.ATTACK;
                    }
                    else
                    {
                        playerState = PLAYERSTATE.IDLE;
                    }
                    break;
                }
            case PLAYERSTATE.MOVE:
                {
                    StartCoroutine(StepRun());

                    if(GameController.instance.isAttack)
                    {
                        playerState = PLAYERSTATE.ATTACK;
                    }
                    else if (!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                    {
                        playerState = PLAYERSTATE.IDLE;
                    }
                    break;
                }
            case PLAYERSTATE.ATTACK:
                {
                    if (!GameController.instance.isAttack)
                    {
                        playerState = PLAYERSTATE.IDLE;
                    }
                    break;
                }
            case PLAYERSTATE.DAMAGE:
                {
                    EZCameraShake.CameraShaker.Instance.ShakeOnce(2.0f, 2.0f, 0.0f, 0.5f);
                    
                    // EZCameraShake.CameraShaker.Instance.Shake(EZCameraShake.CameraShakePresets.Vibration);

                    if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
                    {
                        playerState = PLAYERSTATE.MOVE;
                    }
                    else if (GameController.instance.isAttack)
                    {
                        playerState = PLAYERSTATE.ATTACK;
                    }
                    //else playerState = PLAYERSTATE.IDLE;
                    
                    Invoke("PlayerIdle", 0.2f);

                    break;
                }
            case PLAYERSTATE.DEAD:
                {
                    SoundManager.instance.GetComponent<AudioSource>().mute = true;
                    SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_PlayerDie, 0.0f, SoundManager.instance.volume_Sfx);

                    GameObject deadEff = Instantiate(ParticleManager.instance.deadEffect, transform.position, transform.rotation);
                    Destroy(deadEff, 15f);

                    this.gameObject.SetActive(false);

                    FadePanel.isFadeOut = true;

                    break;
                }
        }
    }

    void PlayerIdle()
    {
        playerState = PLAYERSTATE.IDLE;
    }

    IEnumerator StepRun()
    {
        if (!isRun)
        {
            isRun = true;

            yield return new WaitForSeconds(0.3f);

            // 스텝 소리 3가지를 배열로 만들어 넣어놓고, 거기서 랜덤으로 뽑아서 재생하도록 만듦.
            AudioClip[] rndAudio = { SoundManager.instance.sound_Step_A, SoundManager.instance.sound_Step_B, SoundManager.instance.sound_Step_C };

            SoundManager.instance.PlaySfx(transform.position, rndAudio[Random.Range(0, 3)], 0.0f, SoundManager.instance.volume_Sfx);

            isRun = false;
        }
    }
}
