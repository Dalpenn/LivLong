using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseLook : MonoBehaviour
{
    public float sensi = 700;
    float rotationX = 0.0f;
    float rotationY = 0.0f;

    void Update()
    {
        float x = Input.GetAxis("Mouse X");
        float y = Input.GetAxis("Mouse Y");

        rotationX += x * sensi * Time.deltaTime;
        rotationY += y * sensi * Time.deltaTime;

        if (rotationY < -24) rotationY = -24;
        else if (rotationY  > 24) rotationY = 24;

        //유니티 회전은 오일러공식을 사용
        transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
    }
}
