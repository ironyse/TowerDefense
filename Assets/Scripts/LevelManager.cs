using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance = null;
    public static LevelManager Instance { 
        get { 
            if (_instance == null){
                _instance = FindObjectOfType<LevelManager>();
            }
            return _instance;
        }
    }

    [SerializeField] private int _maxLives = 3;
    [SerializeField] private int _maxEnergies = 6;
    [SerializeField] private int _startingEnergy = 2;
    [SerializeField] private int _totalEnemy = 15;

    [SerializeField] private GameObject _panel;
    [SerializeField] private Text _statusInfo;
    [SerializeField] private Text _livesInfo;
    [SerializeField] private Text _totalEnemyInfo;

    [SerializeField] private Transform _towerUIParent;
    [SerializeField] private GameObject _towerUIPrefab;
    [SerializeField] private Transform _notifierPrefab;
    [SerializeField] private GameObject _explosionPrefab;

    [SerializeField] private Tower[] _towerPrefabs;
    [SerializeField] private Enemy[] _enemyPrefabs;    

    [SerializeField] private Transform[] _enemyPaths;    
    [SerializeField] private float _spawnDelay = 5f;
    [SerializeField] private float _energyRechargeDelay = 4f;

    [SerializeField] private List<Energy> _shownedEnergy = new List<Energy>();

    private List<Tower> _spawnedTowers = new List<Tower>();
    private List<Enemy> _spawnedEnemies = new List<Enemy>();
    private List<Bullet> _spawnedBullets = new List<Bullet>();    
    private List<GameObject> _explosions = new List<GameObject>();

    private int _currentLives;    
    private int _enemyCounter;
    private float _runningSpawnDelay;
    private float _runningRechargeDelay;

    private bool waitForSpace = false;
    public Transform startPanel;
    
    public int CurrentEnergy { get; private set; }
    public bool IsOver { get; private set; }
    public Transform NotifierPrefab { get { return _notifierPrefab; } }

    void Start(){
        SetCurrentLives(_maxLives);
        SetCurrentEnergy(_startingEnergy);
        SetTotalEnemy(_totalEnemy);
        InstantiateAllTowerUI();
    }
        
    void Update(){
        if (Input.GetKeyDown(KeyCode.Space) && !waitForSpace)
        {
            waitForSpace = true;
            startPanel.gameObject.SetActive(false);
        }

        if (!waitForSpace) return;

        if (Input.GetKeyDown(KeyCode.R)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        if (IsOver) return;
        
        _runningSpawnDelay -= Time.unscaledDeltaTime;
        _runningRechargeDelay -= Time.unscaledDeltaTime;

        //Spawn enemies
        if (_runningSpawnDelay <= 0f){
            //Spawn enemy
            SpawnEnemy(_enemyPaths[0].position);
            _runningSpawnDelay = _spawnDelay;
        }

        if (_runningRechargeDelay <= 0f && CurrentEnergy < _maxEnergies)
        {
            RechargeEnergy();
            _runningRechargeDelay = _energyRechargeDelay;
        }


        foreach(Enemy enemy in _spawnedEnemies) {
            if (!enemy.gameObject.activeSelf) {
                continue;
            }

            if (Vector2.Distance(enemy.transform.position, enemy.TargetPos)< 0.1f) {
                enemy.SetCurrentPathIndex(enemy.CurrentPathIndex + 1);
                if (enemy.CurrentPathIndex < _enemyPaths.Length) {
                    enemy.SetTargetPos(_enemyPaths[enemy.CurrentPathIndex].position);
                } else {
                    ReducesLives(1);
                    enemy.gameObject.SetActive(false);
                }
            } else {
                enemy.MoveToTarget();
                if (enemy.CanAttack) {
                    enemy.CheckNearbyTower(_spawnedTowers);
                    enemy.SeekTarget();
                    enemy.ShootTarget();
                }
                
            }

        }


        foreach(Tower tower in _spawnedTowers) {
            if (tower.gameObject.activeSelf) {
                tower.CheckNearbyEnemy(_spawnedEnemies);
                tower.SeekTarget();
                tower.ShootTarget();
            }            
        }
        
    }

    void InstantiateAllTowerUI() { 
        foreach(Tower tower in _towerPrefabs) {
            GameObject newTowerUIObj = Instantiate(_towerUIPrefab.gameObject, _towerUIParent);
            TowerUI newTowerUI = newTowerUIObj.GetComponent<TowerUI>();

            newTowerUI.SetTowerPrefab(tower);
            newTowerUI.transform.name = tower.name;
        }
    }

    // change to public so the method can be used by other object(when enemy dies, it spawn another enemy)
    public void SpawnEnemy(Vector2 lastPos,Enemy enemy = null, int lastPath = 0) {        
        string enemyString;
        Enemy enemyPrefab;

        bool canAttack; // enemy that spawned by other enemy can't attack

        if (enemy == null) {

            SetTotalEnemy(--_totalEnemy);
            if (_enemyCounter < 0) {
                bool isAllEnemyDestroyed = _spawnedEnemies.Find(e => e.gameObject.activeSelf) == null;
                if (isAllEnemyDestroyed) SetGameOver(true);

                return;
            }
            int randomIndex = Random.Range(0, _enemyPrefabs.Length);
            enemyString = (randomIndex + 1).ToString();
            enemyPrefab = _enemyPrefabs[randomIndex];
            canAttack = true;
        } else {
            enemyString = enemy.name;
            enemyPrefab = enemy;
            canAttack = false;
        }

        GameObject newEnemyObj = _spawnedEnemies.Find(e => !e.gameObject.activeSelf && e.name.Contains(enemyString))?.gameObject;

        if (newEnemyObj == null) {

            newEnemyObj = Instantiate(enemyPrefab.gameObject);
        }

        Enemy newEnemy = newEnemyObj.GetComponent<Enemy>();
        if (!_spawnedEnemies.Contains(newEnemy)) { 
            _spawnedEnemies.Add(newEnemy); 
        }

        newEnemy.transform.position = lastPos;
        newEnemy.SetTargetPos(_enemyPaths[lastPath].position);
        newEnemy.SetCurrentPathIndex(lastPath);
        newEnemy.SetEnemyCanAttack(canAttack); 
        newEnemy.gameObject.SetActive(true);
    }
    

    void RechargeEnergy(){
        SetCurrentEnergy(++CurrentEnergy);
    }    

    public void RegisterSpawnedTower(Tower tower) {
        _spawnedTowers.Add(tower);        
    }

    public void UnRegisterSpawnedTower(Tower tower) {
        _spawnedTowers.Remove(tower);
    }

    public Bullet GetBulletFromPool(Bullet prefab) {
        GameObject newBulletObj = _spawnedBullets.Find(b => !b.gameObject.activeSelf && b.name.Contains(prefab.name))?.gameObject;

        if (newBulletObj == null) newBulletObj = Instantiate(prefab.gameObject);

        Bullet newBullet = newBulletObj.GetComponent<Bullet>();
        if (!_spawnedBullets.Contains(newBullet)) {
            _spawnedBullets.Add(newBullet);
        }

        return newBullet;
    }

    public void ExplodeAt(Vector2 point, float radius, int damage){
        foreach(Enemy enemy in _spawnedEnemies) {

            if (enemy.gameObject.activeSelf) {

                if (Vector2.Distance(enemy.transform.position, point) <= radius) {
                    enemy.ReduceHealth(damage, true);
                }
            }
        }

        GameObject explosionObj = _explosions.Find(e => !e.gameObject.activeSelf && e.name.Contains("Explosion"))?.gameObject;
        if (explosionObj == null) {
            explosionObj = Instantiate(_explosionPrefab);
        }
        if (!_explosions.Contains(explosionObj)) {
            _explosions.Add(explosionObj);
        }

        explosionObj.transform.position = point;
        explosionObj.SetActive(true);
    }

    public void ReducesLives (int value) {
        SetCurrentLives(_currentLives - value);
        if (_currentLives <= 0) SetGameOver(false);
    }

    public void SetCurrentLives(int currentLives) {
        _currentLives = Mathf.Max(currentLives, 0);
        _livesInfo.text = $"Lives: {_currentLives}";
    }

    public void SetTotalEnemy(int totalEnemy) {
        _enemyCounter = totalEnemy;
        _totalEnemyInfo.text = $"Total Enemy: {Mathf.Max(_enemyCounter, 0)}";
    }

    public void SetGameOver(bool isWin) {
        IsOver = true;

        _statusInfo.text = isWin ? "You Win!" : "You Lose!";
        _panel.gameObject.SetActive(true);
    }

    public void SetCurrentEnergy(int value){
        CurrentEnergy = Mathf.Max(value, 0);
        foreach(Energy energy in _shownedEnergy) {
            energy.transform.GetChild(0).gameObject.SetActive(false);
        }

        for(int i=0; i < CurrentEnergy; i++) {
            _shownedEnergy[i].transform.GetChild(0).gameObject.SetActive(true);
        }
        
    }

    public void ReduceEnergy(int value) {
        SetCurrentEnergy(CurrentEnergy - value);
    }
    
    void OnDrawGizmos()
    {

        for (int i = 0; i < _enemyPaths.Length - 1; i++)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(_enemyPaths[i].position, _enemyPaths[i + 1].position);
        }        
    }
}
