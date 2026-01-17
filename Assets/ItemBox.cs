using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;

public class itemBox : MonoBehaviour
{
    private player playerScript;
    private inventory inventory;
    private item item;
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
            inventory.add(this.item);
            playerScript = collision.GetComponent<player>();
            playerScript.scoreText.text = score.ToString();

        }
    }
}
