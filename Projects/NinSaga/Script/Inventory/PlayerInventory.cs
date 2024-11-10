using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public InventoryHandler Inventory;

    void Start()
    {
        // Simulate adding items to the inventory
        Inventory.AddItem("Kunai", 5);
        Inventory.AddItem("Shuriken", 3);
        Inventory.AddItem("Explosive Tag", 2);
        Inventory.AddItem("Smoke Bomb", 2);
        Inventory.AddItem("Bandage", 3);

        // Display inventory in the console
        Inventory.DisplayInventory();
    }

    void Update()
    {
        // Testing the item function
        if (Input.GetKeyDown(KeyCode.U))
        {
            Inventory.RemoveItem("Kunai", 1);
            Debug.Log("Threw 1 Kunai");
            Inventory.DisplayInventory();
        }
    }
}
