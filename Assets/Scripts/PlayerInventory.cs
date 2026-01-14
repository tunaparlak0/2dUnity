using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public event Action onItemChangedCallback; // <-- 2. BU SATIRI EKLE
    // C# List yapisi ile dinamik bir canta olusturuyoruz.
    // Inspector'da gorebilmek icin listeyi public yapiyoruz.
    public List<InventoryItem> items = new List<InventoryItem>();
    // Olay (Event) Tanimlayalim: "Envanter Degisti" olayi
    // Esya ekleme fonksiyonu (Disaridan cagirilacak)
    // string itemName yerine ItemData newItemData aliyoruz
    public void AddItem(ItemData newItemData, int amount)
    {
        bool itemFound = false;

        foreach (InventoryItem item in items)
        {
            // Isim kiyaslamasi yerine Data kiyaslamasi yapiyoruz
            if (item.data == newItemData)
            {
                item.quantity += amount;
                itemFound = true;
                break;
            }
        }

        if (!itemFound)
        {
            InventoryItem newItem = new InventoryItem();
            newItem.data = newItemData; // Veriyi ata
            newItem.quantity = amount;
            items.Add(newItem);
        }

        if (onItemChangedCallback != null)
            onItemChangedCallback.Invoke();
    }
}

// --- DATA MODEL ---
// Unity Inspector'da class'in icini gorebilmek icin bu satiri ekliyoruz:
[System.Serializable]
public class InventoryItem
{
    public ItemData data;  // ARTIK STRING DEGIL, DATA TUTUYORUZ
    public int quantity;
}