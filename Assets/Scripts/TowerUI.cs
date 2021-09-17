using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerUI : MonoBehaviour, IBeginDragHandler, IDragHandler,IEndDragHandler
{
    [SerializeField] private Image _towerIcon;
    private Tower _towerPrefab;
    private Tower _currentSpawnedTower;

    public void SetTowerPrefab(Tower tower){
        _towerPrefab = tower;
        _towerIcon.sprite = tower.GetTowerHeadIcon();
    }    

    public void OnBeginDrag(PointerEventData eventData){        

        // Check if current energy is greater than or equals tower's energy cost than continue the code 
        if (LevelManager.Instance.CurrentEnergy >= _towerPrefab.EnergyCost) {
            GameObject newTowerObj = Instantiate(_towerPrefab.gameObject);
            _currentSpawnedTower = newTowerObj.GetComponent<Tower>();
            _currentSpawnedTower.ToggleOrderInLayer(true);
        } else {
            Notifier.Show(new Vector3(0f, -2f, 0f));
        }
        
    }

    public void OnDrag(PointerEventData eventData){
        if (!_currentSpawnedTower) return; 
        Camera mainCamera = Camera.main;
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = -mainCamera.transform.position.z;
        Vector3 targetPos = Camera.main.ScreenToWorldPoint(mousePos);

        _currentSpawnedTower.transform.position = targetPos;
    }

    public void OnEndDrag(PointerEventData eventData){
        if (!_currentSpawnedTower) return;
        if (_currentSpawnedTower.PlacePos == null) {
            Destroy(_currentSpawnedTower.gameObject);
        } else{
            _currentSpawnedTower.LockPlacement();
            _currentSpawnedTower.ToggleOrderInLayer(false);
            LevelManager.Instance.RegisterSpawnedTower(_currentSpawnedTower);
            LevelManager.Instance.ReduceEnergy(_currentSpawnedTower.EnergyCost); // reduce energy by tower variant energy cost
            _currentSpawnedTower = null;
        }
    }
}
