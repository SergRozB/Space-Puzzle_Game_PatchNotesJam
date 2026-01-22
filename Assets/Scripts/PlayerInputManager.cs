using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine.UI;

public class PlayerInputManager : MonoBehaviour
{
    public InputActionReference pause;
    public InputActionReference move;
    public bool isPaused = false;
    [SerializeField] private GameObject[] inventory;
    [SerializeField] private bool isInventoryFull = false;
    [SerializeField] private int inventorySize = 8;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject inventorySlot;
    [SerializeField] private Transform leftInventoryTransform;
    [SerializeField] private Transform rightInventoryTransform;
    [SerializeField] private Transform inventorySlotsParent;
    private GameObject[] inventorySlotGameObjects;
    private float inventorySideCushion = 50f;
    [SerializeField] private Sprite selectedItemSlotSprite; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        inventory = new GameObject[inventorySize];
        inventorySlotGameObjects = new GameObject[inventorySize];
        SetInventorySlots();
    }

    // Update is called once per frame
    void Update()
    {
        isPaused = pause.action.triggered ? !isPaused : isPaused;
        transform.Translate(move.action.ReadValue<Vector2>() * Time.deltaTime * moveSpeed);
    }

    private void SetInventorySlots() 
    {
        float xDifference = rightInventoryTransform.position.x - leftInventoryTransform.position.x;
        float inventorySlotWidth = inventorySlot.transform.localScale.x * 100f;
        float distanceBetween = (xDifference - (inventorySideCushion*2) - inventorySlotWidth) / (inventorySize-1);
        float startingCushion = inventorySideCushion + (inventorySlotWidth/2);
        for (int i = 0; i < inventorySize; i++)
        {
            Vector3 slotPosition = leftInventoryTransform.position + new Vector3(startingCushion+distanceBetween * (i), 0, 0);
            inventorySlotGameObjects[i] = Instantiate(inventorySlot, slotPosition, Quaternion.identity, inventorySlotsParent);
        }
    }

    private void AddToInventory(GameObject g) 
    {
        for(int i = 0; i < inventorySize; i++) 
        {
            if(inventory[i] == null) 
            {
                // Add item to inventory
                inventory[i] = g;
                isInventoryFull = false;
                AddSpriteToInventory(g, i);
                return;
            }
        }

        isInventoryFull = true;
    }

    private void AddSpriteToInventory(GameObject item, int index) 
    {
        for(int i = 0; i < inventorySize; i++) 
        {
            if(i == index) 
            {
                GameObject itemInInventory = Instantiate(item, inventorySlotGameObjects[i].transform.position, Quaternion.identity, inventorySlotGameObjects[i].transform);
                itemInInventory.transform.localScale = new Vector3(50f, 50f, 0);
                itemInInventory.AddComponent<UnityEngine.UI.Image>();
                UnityEngine.UI.Image image = itemInInventory.GetComponent<UnityEngine.UI.Image>();
                image.sprite = item.GetComponent<SpriteRenderer>().sprite;

            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            AddToInventory(collision.gameObject);
            if(!isInventoryFull) 
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
}
