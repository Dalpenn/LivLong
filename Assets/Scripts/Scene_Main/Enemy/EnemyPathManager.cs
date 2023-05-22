using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathManager : MonoBehaviour
{
    public SWS.PathManager[] enemyPath;

    public static EnemyPathManager instance;
    
    void Awake()
    {
        instance = this;
    }
}
