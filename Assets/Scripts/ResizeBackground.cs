using UnityEditor.UI;
using UnityEngine;

public class ResizeBackground : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    private float canvasScaleFactor;
    void Start()
    {
        canvas = GetComponent<Canvas>();
        canvasScaleFactor = canvas.scaleFactor;
        foreach (RectTransform t in GetComponentsInChildren<RectTransform>())
        {
            if (t.name != gameObject.name) 
            {
                Debug.Log("Resizing background element: " + t.name);
                t.sizeDelta *= canvasScaleFactor;
            }
        }
    }
}
