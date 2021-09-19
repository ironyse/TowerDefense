using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    private Tower _placedTower;
    
    
    private void OnTriggerEnter2D(Collider2D collision) {        
        if (_placedTower != null && _placedTower.placed && _placedTower.IsDestroyed) {            
            _placedTower = null;            
        }

        if (_placedTower == null) {
            Tower tower = collision.GetComponent<Tower>();
            if (tower != null) {
                tower.SetPlacePos(transform.position);
                _placedTower = tower;                
            } 
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (_placedTower == null) return;

        if (!_placedTower.placed)
        {
            _placedTower.SetPlacePos(null);
            _placedTower = null;
        }
        
    }

}
