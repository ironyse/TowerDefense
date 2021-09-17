using UnityEngine;

public class TowerPlacement : MonoBehaviour
{
    [SerializeField]private Tower _placedTower;

    // mengganti OnTriggerEnter jadi OnTriggerStay karena terjadi bug dimana tower masih bisa ditempatkan di tempat yang sama
    private void OnTriggerStay2D(Collider2D collision){

        // melakukan pengecekan terhadap tower yang hancur/non-aktif
        if (_placedTower != null && !_placedTower.gameObject.activeSelf) {
            _placedTower = null;
        }

        if (_placedTower != null) return; // Kalau sudah ada tower yang dipasang, skip the next block of code

        
        Tower tower = collision.GetComponent<Tower>();
        if (tower != null)
        {
            tower.SetPlacePos(transform.position);
            _placedTower = tower;
        }
    }

    private void OnTriggerExit2D(Collider2D collision){
        if (_placedTower == null) return;

        _placedTower.SetPlacePos(null);
        _placedTower = null;
    }
    
}
