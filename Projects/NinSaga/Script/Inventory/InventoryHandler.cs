using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    public int maxItems = 20;
    public List<Item> itemList = new List<Item>();

    // Add an item to the inventory
    public void AddItem(string itemName, int quantity)
    {
        Item existingItem = FindItemByName(itemName);

        if (existingItem != null)
        {
            // If the item already exists, increase the quantity
            existingItem.quantity += quantity;
        }
        else if (itemList.Count < maxItems)
        {
            // If the item doesn't exist and there's space, add the new item
            Item newItem = new Item(itemName, quantity);
            itemList.Add(newItem);
        }
        else
        {
            Debug.Log("Inventory is full!");
        }
    }

    // Remove an item from the inventory (or reduce its quantity)
    public void RemoveItem(string itemName, int quantity)
    {
        Item existingItem = FindItemByName(itemName);

        if (existingItem != null)
        {
            existingItem.quantity -= quantity;
            if (existingItem.quantity <= 0)
            {
                itemList.Remove(existingItem);
            }
        }
    }

    // Find an item in the inventory by name
    public Item FindItemByName(string name)
    {
        return itemList.Find(item => item.itemName == name);
    }

    // Display the inventory
    public void DisplayInventory()
    {
        Debug.Log("Player Inventory:");
        foreach (Item item in itemList)
        {
            Debug.Log(item.itemName + " x" + item.quantity);
        }
    }
}
