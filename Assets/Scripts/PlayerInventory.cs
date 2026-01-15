using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerInventory : MonoBehaviour
{
    public event Action onItemChangedCallback;

    // Inspector'da gorebilmek icin listeyi public yapiyoruz.
    public List<InventoryItem> items = new List<InventoryItem>();

    // --- GUNCEL VE AKILLI EKLEME FONKSIYONU ---
    public void AddItem(ItemData newItemData, int amount)
    {
        // --- GUVENLIK DUVARI ---
        // Eger gelen veri bossa, islem yapma ve hata verme.
        if (newItemData == null)
        {
            Debug.LogError("⚠️ HATA: Toplanan esyanin 'ItemData' kismi bos! Inspector'dan atama yapmayi unutmussun.");
            return; // Fonksiyondan cik, oyunu cokertme.
        }
        // 1. ADIM: YIGINLAMA (Stacking)
        // Eger esya istiflenebilir ise, once cantada aynisindan var mi diye bak.
        if (newItemData.isStackable)
        {
            foreach (InventoryItem item in items)
            {
                // Item doluysa (null degilse) VE verisi ayniysa VE daha yer varsa (99 olmadiysa)
                if (item.data != null && item.data == newItemData && item.quantity < newItemData.maxStackSize)
                {
                    item.quantity += amount;
                    // UI Guncelle ve bitir
                    if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
                    return;
                }
            }
        }

        // 2. ADIM: BOSLUK DOLDURMA (Senin Sorunu Cozen Kisim 🛠️)
        // Yiginlayamadik, o zaman bos bir yer (delik) var mi diye bak.
        foreach (InventoryItem item in items)
        {
            // Eger bu slotun verisi bossa (Sandiga esya koyunca burasi bosalmisti)
            if (item.data == null)
            {
                item.data = newItemData;
                item.quantity = amount;

                if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
                return; // Bosluga yerlestirdik, bitir.
            }
        }

        // 3. ADIM: YENI SLOT ACMA
        // Yiginlanacak yer yok, bosluk da yok. Listenin sonuna yeni yer ekle.
        InventoryItem newItem = new InventoryItem();
        newItem.data = newItemData;
        newItem.quantity = amount;
        items.Add(newItem);

        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
    }

    public void SwapItems(int indexA, int indexB)
    {
        // Listeyi guvenli sekilde genislet
        while (items.Count <= indexA || items.Count <= indexB)
        {
            items.Add(new InventoryItem());
        }

        // Takas islemi
        InventoryItem temp = items[indexA];
        items[indexA] = items[indexB];
        items[indexB] = temp;

        if (onItemChangedCallback != null) onItemChangedCallback.Invoke();
    }
}

[System.Serializable]
public class InventoryItem
{
    public ItemData data;
    public int quantity;
}