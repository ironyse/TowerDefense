using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public bool IsExploded {get;private set;}
    
    private float _runningExplosionTimer;

    private void OnEnable()
    {
        _runningExplosionTimer = 5f;
    }

    void Update(){
        _runningExplosionTimer -= Time.unscaledDeltaTime;

        if (_runningExplosionTimer <= 0f)
        {
            gameObject.SetActive(false);
        }
    }
}
