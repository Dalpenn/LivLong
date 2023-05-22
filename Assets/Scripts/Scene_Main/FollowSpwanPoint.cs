using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowSpwanPoint : MonoBehaviour
{
    Transform tr;

    void Start()
    {
        tr = transform;
    }

    void Update()
    {
        tr.position = GameController.instance.muzzlePoint.position;
        tr.rotation = GameController.instance.muzzlePoint.rotation;
    }
}
