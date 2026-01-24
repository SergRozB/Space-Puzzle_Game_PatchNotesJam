using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerInputManager : MonoBehaviour
{
    public InputActionReference pause;
    public InputActionReference control;
    [SerializeField] private InputActionReference scroll;
    public bool isPaused = false;
    [SerializeField] private GameObject[] inventory;
    [SerializeField] private bool isInventoryFull = false;
    [SerializeField] private int inventorySize = 8;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform leftInventoryTransform;
    [SerializeField] private Transform rightInventoryTransform;
    [SerializeField] private Transform inventorySlotsParent;
    private GameObject[] inventorySlotGameObjects;
    private float inventorySideCushion = 50f;
    [SerializeField] private Sprite selectedItemSlotSprite;
    [SerializeField] private Sprite noItemInSlotSprite;
    [SerializeField] private int selectedSlot = 0;
    private bool changedSlot = false;
    [SerializeField] private GameObject selectedItem;
    [SerializeField] private float itemSpriteScale = 5f;
    public List<Vector3> itemBoxPositions = new List<Vector3>();

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
        SelectInventorySlot();
    }

    private void SetInventorySlots() 
    {
        float xDifference = rightInventoryTransform.position.x - leftInventoryTransform.position.x;
        float inventorySlotWidth = inventorySlotPrefab.transform.localScale.x * 100f;
        float distanceBetween = (xDifference - (inventorySideCushion*2) - inventorySlotWidth) / (inventorySize-1);
        float startingCushion = inventorySideCushion + (inventorySlotWidth/2);
        float yChange = 85f;
        for (int i = 0; i < inventorySize; i++)
        {

            Vector3 slotPosition = leftInventoryTransform.gameObject.GetComponent<RectTransform>().position + new Vector3(startingCushion+distanceBetween * (i), yChange, 0);
            inventorySlotGameObjects[i] = Instantiate(inventorySlotPrefab, Vector3.zero, Quaternion.identity, inventorySlotsParent);
            RectTransform slotRectTransform = inventorySlotGameObjects[i].GetComponent<RectTransform>();
            slotRectTransform.position = slotPosition;
            UnityEngine.UI.Image slotImage = inventorySlotGameObjects[i].GetComponent<UnityEngine.UI.Image>();
            slotImage.sprite = noItemInSlotSprite;
            slotImage.rectTransform.localScale = new Vector3(itemSpriteScale, itemSpriteScale, 0);
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
                GameObject itemInInventory = Instantiate(item, inventorySlotGameObjects[i].transform.position, Quaternion.identity, inventorySlotsParent);
                itemInInventory.transform.localScale = new Vector3(50f, 50f, 0);
                itemInInventory.AddComponent<UnityEngine.UI.Image>();
                UnityEngine.UI.Image image = itemInInventory.GetComponent<UnityEngine.UI.Image>();
                image.sprite = item.GetComponent<SpriteRenderer>().sprite;
                image.SetNativeSize();
                image.rectTransform.localScale = new Vector3(itemSpriteScale/2, itemSpriteScale/2, 0);
            }
        }
    }

    private void SelectInventorySlot() 
    {
        if(scroll.action.ReadValue<float>() > 0 && control.action.ReadValue<float>() != 1) 
        {
            selectedSlot = (selectedSlot + 1) % inventorySize;
            changedSlot = true;
        }

        else if(scroll.action.ReadValue<float>() < 0 && control.action.ReadValue<float>() != 1) 
        {
            selectedSlot = (selectedSlot - 1 + inventorySize) % inventorySize;
            changedSlot = true;
        }

        if(changedSlot) 
        {
            for (int i = 0; i < inventorySize; i++)
            {
                UnityEngine.UI.Image slotImage = inventorySlotGameObjects[i].GetComponent<UnityEngine.UI.Image>();
                if (i == selectedSlot)
                {
                    selectedItem = inventory[i];
                    slotImage.sprite = selectedItemSlotSprite;
                    slotImage.rectTransform.localScale = new Vector3(itemSpriteScale, itemSpriteScale, 0);
                }
                else
                {
                    slotImage.sprite = noItemInSlotSprite;
                    slotImage.rectTransform.localScale = new Vector3(itemSpriteScale, itemSpriteScale, 0);
                }
            }
            changedSlot = false;
        }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("ItemBox"))
        {
            ItemBox itemBoxScript = collision.gameObject.GetComponent<ItemBox>();
            itemBoxScript.wasPickedUp = true;
            AddToInventory(itemBoxScript.getItem().getGameObject());
            itemBoxPositions.Add(collision.gameObject.transform.position);
            if (!isInventoryFull) 
            {
                collision.gameObject.SetActive(false);
            }
        }
    }
}
