using UnityEngine;
using Unity.UI;
using JetBrains.Annotations;
using UnityEditor;

public class ItemBox : MonoBehaviour
{
    [SerializeField] private Item item;
    [SerializeField] private int amount;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public ItemBox(Item item, int amount) 
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

    public GameObject getGameObject()
    {
        return this.gameObject;
    }

    public Item getItem()
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

    public void setItem(Item item)
    {
        this.item = item;
    }

    public void setItemAmount(int amount)
    {
        this.amount = amount;
    }
}
