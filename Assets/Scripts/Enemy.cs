using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private int _shootPower = 1;
    [SerializeField] private float _shootDistance = 1f;
    [SerializeField] private float _shootDelay = 5f;
    [SerializeField] private float _bulletSpeed = 1f;

    // enemy behavior type
    [SerializeField] private bool _canAttacking = false;
    [SerializeField] private bool _canRunAndGun = false;
    [SerializeField] private bool _canSpawningEnemies = false;
    [SerializeField] private int _spawnNumber = 0;
    [SerializeField] private Enemy _spawnedType;

    [SerializeField] private SpriteRenderer _enemyHead;
    [SerializeField] private SpriteRenderer _healthBar;
    [SerializeField] private SpriteRenderer _healthFill;

    [SerializeField] private Bullet _bulletPrefab;

    private int _currentHealth;
    private float _runningShootDelay;
    private Tower _targetTower;
    private Quaternion _targetRotation;

    public Vector3 TargetPos { get; private set; }
    public int CurrentPathIndex { get; private set; }
    public bool CanAttack { get { return _canAttacking; } }
    public bool CanRunAndGun { get { return _canRunAndGun; } }

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

    public void CheckNearbyTower(List<Tower> towers) {
        if (_targetTower != null) { 
            if (!_targetTower.gameObject.activeSelf || Vector3.Distance(transform.position, _targetTower.transform.position) > _shootDistance) {
                _targetTower = null;
            } else {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Tower nearbyTower = null;

        foreach (Tower tower in towers) {
            float distance = Vector3.Distance(transform.position, tower.transform.position);
            if (distance > _shootDistance) {
                continue;
            }

            if (distance < nearestDistance) {
                nearestDistance = distance;
                nearbyTower = tower;
            }

        }
        _targetTower = nearbyTower;
    }

    public void ShootTarget() {
        if (_targetTower == null) return;

        _runningShootDelay -= Time.unscaledDeltaTime;
        if (_runningShootDelay <= 0f) {
            bool headHasAimed = Mathf.Abs(_enemyHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) < 10f;
            if (!headHasAimed) return;

            Bullet bullet = LevelManager.Instance.GetBulletFromPool(_bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.SetProperties(_shootPower, _bulletSpeed, 0f);
            bullet.SetTargetTower(_targetTower);
            bullet.gameObject.SetActive(true);

            _runningShootDelay = _shootDelay;
        }
    }

    public void SeekTarget() {
        if (_targetTower == null) return;

        Vector3 direction = _targetTower.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle));

        _enemyHead.transform.rotation = Quaternion.RotateTowards(_enemyHead.transform.rotation, _targetRotation, Time.deltaTime * 180f);
    }
}
