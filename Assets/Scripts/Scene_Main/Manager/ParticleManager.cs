using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager instance;

    public GameObject concImpact;
    public GameObject sandImpact;
    public GameObject muzzleImpact;

    public GameObject bloodImpact;
    public GameObject deadEffect;

    public GameObject portalEff;

    //public GameObject 

    private void Awake()
    {
        instance = this;
    }
}
