using UnityEngine;
using TMPro;

public class Notifier : MonoBehaviour
{
    private static TextMeshPro textMesh;

    private static float disapperTimer = 1f;
    private static Color textColor;    

    private static Notifier _instance = null;

    public static Notifier Show(string text, Vector3 position) {
        if (_instance == null) {
            Transform notifierTransform = Instantiate(LevelManager.Instance.NotifierPrefab, position, Quaternion.identity);
            _instance = notifierTransform.GetComponent<Notifier>();
        } else
        {
            _instance.gameObject.SetActive(false);
        }

        textMesh.text = text;                     
        _instance.gameObject.SetActive(true);
        
        return _instance;
    }

    private void OnEnable()
    {
        textColor.a = 1;
        disapperTimer = 1f;
        textMesh.color = textColor;
    }

    private void Awake() {
        textMesh = transform.GetComponent<TextMeshPro>();
        textColor = textMesh.color;  
    }

    void Update() {
        disapperTimer -= Time.deltaTime;
        if (disapperTimer < 0f) {
            float disappearSpeed = 2f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0) {
                gameObject.SetActive(false);
            }
        }
    }
}
