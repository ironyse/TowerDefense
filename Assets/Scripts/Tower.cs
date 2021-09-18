using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    // Component
    [SerializeField] private SpriteRenderer _towerPlace;
    [SerializeField] private SpriteRenderer _towerHead;
    [SerializeField] private SpriteRenderer _healthBar;
    [SerializeField] private SpriteRenderer _healthFill;

    // Properti
    [SerializeField] private int _energyCost = 1; // cost for build tower
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _shootPower = 1;
    [SerializeField] private float _shootDistance = 1f;
    [SerializeField] private float _shootDelay = 5f;
    [SerializeField] private float _bulletSpeed = 1f;
    [SerializeField] private float _bulletSplashRadius = 0f;

    [SerializeField] private Bullet _bulletPrefab;
    private float _runningShootDelay;
    private Enemy _targetEnemy;
    private Quaternion _targetRotation;
    private int _currentHealth;

    public int EnergyCost { get { return _energyCost; } }

    public Sprite GetTowerHeadIcon(){
        return _towerHead.sprite;
    }
        
    public Vector2? PlacePos { get; private set; }

    public void SetPlacePos(Vector2? newPos){
        PlacePos = newPos;
    }

    public void LockPlacement() {
        transform.position = (Vector2)PlacePos;
        _currentHealth = _maxHealth;
        _healthFill.size = _healthBar.size;
    }

    public void ToggleOrderInLayer(bool toFront) {
        int orderInLayer = toFront ? 2 : 0;
        _towerPlace.sortingOrder = orderInLayer;
        _towerHead.sortingOrder = orderInLayer;
    }

    public void ReduceHealth(int damage) {
        _currentHealth -= damage;
        
        if(_currentHealth <= 0) {
            _currentHealth = 0;
            gameObject.SetActive(false);
            LevelManager.Instance.UnRegisterSpawnedTower(transform.GetComponent<Tower>());
            
        }

        float fillRatio = (float)_currentHealth / _maxHealth;
        _healthFill.size = new Vector2(fillRatio * _healthBar.size.x, _healthBar.size.y);
    }

    public void CheckNearbyEnemy(List<Enemy> enemies) { 
        if(_targetEnemy != null) { 
            if (!_targetEnemy.gameObject.activeSelf || Vector3.Distance(transform.position, _targetEnemy.transform.position) > _shootDistance) {
                _targetEnemy = null;
            } else {
                return;
            }
        }

        float nearestDistance = Mathf.Infinity;
        Enemy nearbyEnemy = null;

        foreach(Enemy enemy in enemies) {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance > _shootDistance) {
                continue;
            }
            // adding statement, because the tower still targeting inactive enemy game object
            if (distance < nearestDistance && enemy.gameObject.activeSelf) {
                nearestDistance = distance;
                nearbyEnemy = enemy;
            }
        }

        _targetEnemy = nearbyEnemy;

    }

    public void ShootTarget() {
        if (_targetEnemy == null) return;        

        _runningShootDelay -= Time.unscaledDeltaTime;
        if (_runningShootDelay <= 0f) {
            bool headHasAimed = Mathf.Abs(_towerHead.transform.rotation.eulerAngles.z - _targetRotation.eulerAngles.z) < 10f;
            if (!headHasAimed) return;

            Bullet bullet = LevelManager.Instance.GetBulletFromPool(_bulletPrefab);
            bullet.transform.position = transform.position;
            bullet.SetProperties(_shootPower, _bulletSpeed, _bulletSplashRadius);
            bullet.SetTargetEnemy(_targetEnemy);
            bullet.gameObject.SetActive(true);

            _runningShootDelay = _shootDelay;
        }
    }

    public void SeekTarget() {
        if (_targetEnemy == null) return;        

        Vector3 direction = _targetEnemy.transform.position - transform.position;
        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _targetRotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));

        _towerHead.transform.rotation = Quaternion.RotateTowards(_towerHead.transform.rotation, _targetRotation, Time.deltaTime * 180f);
    }
}
