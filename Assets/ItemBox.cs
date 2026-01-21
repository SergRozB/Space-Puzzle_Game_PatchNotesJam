using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;

public class ItemBox : MonoBehaviour
{
    private Player player;
    private Item item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Item getItem()
    {
        return this.item;
    }

    public float getX()
    {
        return transform.position.x;
    }

    public float getY()
    {
        return transform.position.y;
    }
}
