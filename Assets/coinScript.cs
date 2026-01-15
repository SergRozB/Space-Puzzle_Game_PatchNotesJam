using UnityEngine;
using Unity.UI;

public class coinScript : MonoBehaviour
{
    private player playerScript;
    public static int score;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Destroy(this.gameObject);
            score += 1;
            playerScript = collision.GetComponent<player>();
            playerScript.scoreText.text = score.ToString();

        }
    }
}
