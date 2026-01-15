using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ChestUI : MonoBehaviour
{
    public Transform itemsParent;   // Grid (Kutularin dizilecegi yer)
    public GameObject slotPrefab;   // Slot Kalibi
    public GameObject chestPanel;   // Acip kapatacagimiz panelin kendisi

    // O an hangi sandiga bakiyoruz?
    private Chest currentChest;
    private List<GameObject> slots = new List<GameObject>();

    // Singleton (Her yerden ulasabilmek icin kisa yol)
    public static ChestUI instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        // Baslangista paneli kapat
        chestPanel.SetActive(false);
    }

    public void OpenChest(Chest chest)
    {
        currentChest = chest;
        chestPanel.SetActive(true);
        UpdateUI();

        // --- YENI EKLENEN KISIM ---
        // Sandik acilinca oyuncunun cantasi da acilsin
        if (InventoryUI.instance != null)
        {
            InventoryUI.instance.inventoryPanel.SetActive(true);
            InventoryUI.instance.UpdateUI(); // Cantayi guncelle ki bos gorunmesin
        }
    }

    public void CloseChest()
    {
        // KONTROL 1: Sandik Paneli (ChestPanel) hala var mi?
        if (chestPanel != null)
        {
            chestPanel.SetActive(false);
        }

        currentChest = null;

        // KONTROL 2: Envanter Sistemi ve Paneli hala var mi?
        if (InventoryUI.instance != null && InventoryUI.instance.inventoryPanel != null)
        {
            InventoryUI.instance.inventoryPanel.SetActive(false);
        }
    }

    public void UpdateUI()
    {
        // Once eski slotlari temizle (Cunku farkli sandiklarin kapasitesi farkli olabilir)
        foreach (Transform child in itemsParent)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        // Yeni slotlari olustur
        for (int i = 0; i < currentChest.chestItems.Count; i++)
        {
            GameObject newSlot = Instantiate(slotPrefab, itemsParent);
            // Yeni slot olusturduktan hemen sonra:
            InventorySlot slotScript = newSlot.GetComponent<InventorySlot>();

            // SETUP cagrisi:
            // Listemiz: currentChest.chestItems
            // Index: i
            // isChest: true (Evet bu bir sandik)
            slotScript.Setup(currentChest.chestItems, i, true);

            // --- ONEMLI: Transfer icin buraya birazdan ayar cekecegiz ---
            // Simdilik sadece gosterme kismini yapiyoruz.

            if (currentChest.chestItems[i] != null)
            {
                slotScript.SetItem(currentChest.chestItems[i]);
            }
            else
            {
                slotScript.SetItem(null);
            }

            slots.Add(newSlot);
        }
    }
}