using UnityEngine;
using TMPro;

public class Notifier : MonoBehaviour
{
    public static Notifier Show(Vector3 position) {
        Transform notifierTransform = Instantiate(LevelManager.Instance.NotifierPrefab, position, Quaternion.identity);
        Notifier notifier = notifierTransform.GetComponent<Notifier>();
        return notifier;
    }

    private TextMeshPro textMesh;
    private float disapperTimer = 1f;
    private Color textColor;

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
                Destroy(gameObject);
            }
        }
    }
}
