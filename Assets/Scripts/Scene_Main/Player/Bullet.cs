using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public static Bullet instance;

    Transform tr;
    float bulletSpeed = 1800;
    Rigidbody rbody;

    float playerAtkRange = 0.6f;

    private void Awake()
    {
        instance = this;

        CryptoPlayerPrefs_HasKeyFloatFind("playerAtkRange", playerAtkRange = 0.6f);
        playerAtkRange = CryptoPlayerPrefs.GetFloat("playerAtkRange");
    }

    void Start()
    {
        tr = transform;     // 직접 참조 캐싱(Transform tr을 선언하고 Awake나 Start에서 넣어줌)
        rbody = GetComponent<Rigidbody>();

        rbody.AddForce(tr.forward * bulletSpeed,ForceMode.Force);        // 총알이 생성되면 알아서 직진하도록 설정

        Destroy(tr.gameObject, playerAtkRange);
    }

    //void Update()
    //{
    //    //float move = bulletSpeed * Time.deltaTime;
    //    //tr.Translate(tr.forward * move);
    //}

    #region 충돌한 레이어에 따라 다른 이펙트(파티클 효과) 발생하도록 설정
    private void OnCollisionEnter(Collision collision)
    {
        Vector3 rndRot = Vector3.right * Random.Range(200, 300);

        Destroy(tr.gameObject);

        int collLayer = collision.gameObject.layer;

        if(collLayer == LayerMask.NameToLayer("Stage"))
        {
            GameObject obj = Instantiate(ParticleManager.instance.concImpact, tr.position, tr.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);             // 생성된 총알 이펙트가 랜덤한 값으로 회전되어 발생하도록 설정(좀 더 현실적인 총알 이펙트를 위해)
        }
        else if (collLayer == LayerMask.NameToLayer("Ground"))
        {
            GameObject obj = Instantiate(ParticleManager.instance.sandImpact, tr.position, tr.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);

        }
        else if (collLayer == LayerMask.NameToLayer("Enemy"))
        {
            GameObject obj = Instantiate(ParticleManager.instance.bloodImpact, tr.position, tr.rotation);
            obj.transform.localRotation = Quaternion.Euler(rndRot);
            collision.gameObject.SendMessage("EnemyDamage", SendMessageOptions.DontRequireReceiver);     // 충돌체의 "EnemyDmged" 함수를 호출 (SendMessage는 public이 필요없다)
            //collision.gameObject.GetComponent<ZombieProcess>().EnemyDamage();        // GetComponent로 불러오려면 불러오려는 함수를 public화 해줘야 함
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