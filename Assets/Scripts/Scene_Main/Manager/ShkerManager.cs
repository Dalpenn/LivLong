using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShkerManager : MonoBehaviour
{
    public Camera shakeCamera;

    public bool isShake = false;

    public static ShkerManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void LateUpdate()
    {
        StartCoroutine(ShakeCamera(0.3f, 0.05f, 0.01f, 0f));
    }

    IEnumerator ShakeCamera(float endTime, float sensiX, float sensiY, float sensiZ)
    {
        if (isShake)
        {
            Vector3 pos = Vector3.zero;
            pos.x = Random.Range(-sensiX, sensiX);
            pos.y = Random.Range(-sensiY, sensiY);
            pos.z = Random.Range(-sensiZ, sensiZ);
            shakeCamera.transform.position += pos;
            yield return new WaitForSeconds(endTime);
            isShake = false;

            //Lerp() 선형보간함수
            shakeCamera.transform.localPosition = Vector3.Lerp(shakeCamera.transform.localPosition, Vector3.zero, Time.deltaTime * 10f);
        }
    }
}