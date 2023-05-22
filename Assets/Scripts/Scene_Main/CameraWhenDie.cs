using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraWhenDie : MonoBehaviour
{
    public static CameraWhenDie instance;

    Transform tr;
    public Transform player;

    private void Awake()
    {
        tr = transform;
    }

    private void Update()
    {
        tr.LookAt(player);
        tr.RotateAround(player.position, new Vector3(0, 1, 0), 20 * Time.deltaTime);
    }
}
