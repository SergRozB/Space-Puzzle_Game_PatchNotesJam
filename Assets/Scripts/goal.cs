using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;

public class Goal : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI timeText;
    [SerializeField] private Player mainPlayerScript;
    private bool goalReached = false;
    [SerializeField] private GameObject successCanvas;
    [SerializeField] private TMPro.TextMeshProUGUI successTimeText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!goalReached && mainPlayerScript.fired)
        {
            float startTime = mainPlayerScript.startTime;
            timeText.text = "Time Elapsed: " + (Time.time - startTime).ToString("F2");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            goalReached = true;
            successCanvas.SetActive(true);
            successTimeText.text = timeText.text;
            mainPlayerScript.gameObject.SetActive(false);
            // Additional logic for when the player reaches the goal can be added here
        }
    }
}
