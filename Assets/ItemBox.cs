using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;

public class ItemBox : MonoBehaviour
{
    private Player player;
    private int item;
    private int amount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public ItemBox(int item, int amount) 
    {
        this.item = item;
        this.amount = amount;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int getItem()
    {
        return this.item;
    }

    public int getItemAmount()
    {
        return this.amount;
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
