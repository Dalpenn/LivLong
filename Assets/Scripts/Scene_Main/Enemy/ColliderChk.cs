using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColliderChk : MonoBehaviour
{
    private void OnTriggerEnter(Collider collision)
    {
        switch (collision.gameObject.layer)
        {
            case 11:   //Player Layer가진 충돌체와 충돌할 경우 동작
                {
                    #region 적 종류에 따른 데미지
                    if (this.gameObject.tag == "Enemy_1")
                    {
                        int dmg = (5 + (int)(0.5 * CryptoPlayerPrefs.GetInt("currentStage") * CryptoPlayerPrefs.GetInt("currentStage")));
                        GameManager.instance.playerHp -= dmg;
                    }
                    else if (this.gameObject.tag == "Enemy_2")
                    {
                        int dmg = (10 + (int)((CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1)));
                        GameManager.instance.playerHp -= dmg;
                    }
                    else if (this.gameObject.tag == "EnemyBoss_1")
                    {
                        int dmg = (30 + (int)((CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1)));
                        GameManager.instance.playerHp -= dmg;
                    }
                    #endregion

                    //Debug.Log("플레이어 체력: " + GameManager.instance.playerHp);

                    GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(GameManager.instance.playerMaxHp);
                    GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMin(0);
                    GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(GameManager.instance.playerHp);

                    //  ShkerManager.instance.isShake = true;    // 쉐이크 카메라 동작

                    PlayerState.instance.playerState = PlayerState.PLAYERSTATE.DAMAGE;

                    SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_HandAtk, 0.0f, SoundManager.instance.volume_Sfx);

                    break;
                }
        }
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    switch (collision.gameObject.layer)
    //    {
    //        case 11:   //Player Layer가진 충돌체와 충돌할 경우 동작
    //            {
    //                if(this.gameObject.tag == "Enemy_1")
    //                {
    //                    int dmg = (5 + (int)(0.5 * CryptoPlayerPrefs.GetInt("currentStage") * CryptoPlayerPrefs.GetInt("currentStage")));
    //                    GameManager.instance.playerHp -= dmg;
    //                }
    //                else if(this.gameObject.tag == "Enemy_2")
    //                {
    //                    int dmg = (10 + (int)((CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1)));
    //                    GameManager.instance.playerHp -= dmg;
    //                }
    //                else if (this.gameObject.tag == "EnemyBoss_1")
    //                {
    //                    int dmg = (30 + (int)((CryptoPlayerPrefs.GetInt("currentStage") - 1) * (CryptoPlayerPrefs.GetInt("currentStage") - 1)));
    //                    GameManager.instance.playerHp -= dmg;
    //                }

    //                //Debug.Log("플레이어 체력: " + GameManager.instance.playerHp);

    //                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMax(GameManager.instance.playerMaxHp);
    //                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueMin(0);
    //                GameManager.instance.playerEnergyBar.GetComponent<EnergyBar>().SetValueCurrent(GameManager.instance.playerHp);

    //                //  ShkerManager.instance.isShake = true;    // 쉐이크 카메라 동작

    //                PlayerState.instance.playerState = PlayerState.PLAYERSTATE.DAMAGE;

    //                SoundManager.instance.PlaySfx(transform.position, SoundManager.instance.sound_HandAtk, 0.0f, SoundManager.instance.volume_Sfx);

    //                break;
    //            }
    //    }
    //}
}
