using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int _bulletPower;
    private float _bulletSpeed;
    private float _bulletSplashRadius;

    private dynamic _bulletTarget;

    private void FixedUpdate(){

        if (LevelManager.Instance.IsOver) return; //Skip code block if Is Over

        if (_bulletTarget != null) { 

            if (!_bulletTarget.gameObject.activeSelf || _bulletTarget == null) {

                gameObject.SetActive(false);
                _bulletTarget = null;
                return;
            }

            transform.position = Vector3.MoveTowards(transform.position, _bulletTarget.transform.position, _bulletSpeed * Time.fixedDeltaTime);

            Vector3 direction = _bulletTarget.transform.position - transform.position;
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, targetAngle - 90f));

        }        
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (_bulletTarget == null) return;

        if (collision.gameObject.Equals(_bulletTarget.gameObject)) {
            gameObject.SetActive(false);

            if (_bulletSplashRadius > 0f) {
                LevelManager.Instance.ExplodeAt(transform.position, _bulletSplashRadius, _bulletPower);
            } else {
                _bulletTarget.ReduceHealth(_bulletPower);
            }

            _bulletTarget = null;

        }
    }

    public void SetProperties(int bulletPower, float bulletSpeed, float bulletSplashRadius) {
        _bulletPower = bulletPower;
        _bulletSpeed = bulletSpeed;
        _bulletSplashRadius = bulletSplashRadius;
    }

    public void SetTargetEnemy(Enemy enemy) {
        _bulletTarget = enemy;
    }

    public void SetTargetTower(Tower tower)
    {
        _bulletTarget = tower;
    }
}
