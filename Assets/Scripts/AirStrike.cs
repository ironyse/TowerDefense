using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirStrike : MonoBehaviour
{
    [SerializeField] private int _explosionDamage = 3;
    [SerializeField] private float _explosionRange = 1.25f;
    [SerializeField] private float _moveSpeed = 1f;

    [SerializeField] private Transform[] _airStrikePaths;
    [SerializeField] private Vector3 _defaultPos;

    public Vector3 TargetPos { get; private set; }
    public int CurrentPathIndex { get; private set; }
    public bool IsStriking { get; private set; }

    private void OnEnable(){
        transform.position = _airStrikePaths[0].position + _defaultPos;
        TargetPos = _airStrikePaths[1].position + _defaultPos;
        CurrentPathIndex = 0;
    }

    void Update()
    {
        if (LevelManager.Instance.IsOver || !IsStriking) return;

        if (Vector2.Distance(transform.position, TargetPos) < 0.1f){
            Striking();
            CurrentPathIndex++;            
            if (CurrentPathIndex < _airStrikePaths.Length) {                
                TargetPos = _airStrikePaths[CurrentPathIndex].position + _defaultPos;                
            } else {                
                gameObject.SetActive(false);
                IsStriking = false;
            }
        } else {
            MoveToTarget();            
        }
        
    }

    void MoveToTarget(){
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, _moveSpeed * Time.deltaTime);        
    }

    void Striking(){
        // do the strike for every path

        if (CurrentPathIndex != 0 && CurrentPathIndex != _airStrikePaths.Length-1){
            LevelManager.Instance.ExplodeAt(_airStrikePaths[CurrentPathIndex].position, _explosionRange, _explosionDamage);
        }
    }

    public void SetOnStrike() {
        IsStriking = true;
    }

    
}
