using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private SpriteRenderer _healthBar;
    [SerializeField] private SpriteRenderer _healthFill;

    private int _currentHealth;

    public Vector3 TargetPos { get; private set; }
    public int CurrentPathIndex { get; private set; }

    // method from monobehavior, called everytime gameobject is enabled
    private void OnEnable(){
        _currentHealth = _maxHealth;
        _healthFill.size = _healthBar.size;
    }

    public void MoveToTarget(){
        transform.position = Vector3.MoveTowards(transform.position, TargetPos, _moveSpeed * Time.deltaTime);
    }

    public void SetTargetPos(Vector3 targetPos){
        TargetPos = targetPos;
        _healthBar.transform.parent = null;

        Vector3 distance = TargetPos - transform.position;
        if (Mathf.Abs(distance.y) > Mathf.Abs(distance.x)) { 

            if (distance.y > 0) {
                // Facing up
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            } else {
                // Facing down
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, -90f));
            }

        } else {

            if (distance.x > 0) {
                // Facing right
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            } else {
                // Facing left
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 180f));
            }

        }

        _healthBar.transform.parent = transform;
    }

    public void SetCurrentPathIndex(int currentIndex) {
        CurrentPathIndex = currentIndex;
    }

    public void ReduceHealth(int damage) {
        _currentHealth -= damage;        
        AudioPlayer.Instance.PlaySFX("hit-enemy");

        if (_currentHealth <= 0) {
            _currentHealth = 0;
            gameObject.SetActive(false);
            AudioPlayer.Instance.PlaySFX("enemy-die");
        }
        float fillRatio = (float) _currentHealth / _maxHealth;
        _healthFill.size = new Vector2(fillRatio * _healthBar.size.x, _healthBar.size.y);
    }
}
