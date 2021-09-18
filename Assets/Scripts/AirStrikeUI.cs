using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class AirStrikeUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject _activeStateObj;
    [SerializeField] private AirStrike _airStrike;
    [SerializeField] private int _airStrikeCost = 3;

    public bool IsActive { get; private set; }
    
    void Update(){
        IsActive = (LevelManager.Instance.CurrentEnergy >= _airStrikeCost && !_airStrike.IsStriking); 
        _activeStateObj.SetActive(!IsActive);
    }

    public void OnPointerClick(PointerEventData eventData) {
        if (IsActive){
            // sent plane to air strike            
            _airStrike.gameObject.SetActive(true);
            _airStrike.SetOnStrike();
            LevelManager.Instance.ReduceEnergy(_airStrikeCost);
        } else {
            Notifier.Show("Not Enough Energy!",new Vector3(0f, -2f, 0f));
        }
    }
}
