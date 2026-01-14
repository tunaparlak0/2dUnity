using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI; // Image icin sart!
using System.Collections.Generic; // Listeler icin

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemsParent;   // Grid izgarasi
    public GameObject slotPrefab;   // Kutu kalibi

    PlayerInventory inventory;

    // Olusturdugumuz slotlari bu listede tutacagiz ki sonra tekrar ulasabilelim
    List<GameObject> slots = new List<GameObject>();

    public int totalSlotCount = 24; // Kac kutumuz olacak? (8x3 = 24)

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        inventory.onItemChangedCallback += UpdateUI;

        // --- YENI KISIM: Baslangista bos kutulari olustur ---
        for (int i = 0; i < totalSlotCount; i++)
        {
            // Kutuyu olustur ve Grid'in icine at
            GameObject newSlot = Instantiate(slotPrefab, itemsParent);
            // Bu kutuyu listemize kaydet
            slots.Add(newSlot);
        }
        // ----------------------------------------------------

        inventoryPanel.SetActive(false);
        // Baslangista bir kere calistir ki kutularin ici bos gözüksün
        UpdateUI();
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf)
            {
                UpdateUI();
            }
        }
    }

    // --- YENI GUNCELLEME MANTIGI ---
    public void UpdateUI()
    {
        // DIKKAT: Artik eski slotlari silmiyoruz! (Destroy yok)

        // 24 kutunun hepsini tek tek geziyoruz
        for (int i = 0; i < slots.Count; i++)
        {
            GameObject currentSlot = slots[i];
            Image iconImage = currentSlot.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI amountText = currentSlot.transform.Find("AmountText").GetComponent<TextMeshProUGUI>();

            // Eger bu sira numarasinda (i) envanterde bir esya varsa:
            if (i < inventory.items.Count)
            {
                InventoryItem item = inventory.items[i];

                // Ikonu aktif et ve resmini koy
                iconImage.enabled = true;
                iconImage.sprite = item.data.icon;

                // Sayiyi aktif et ve yaz
                amountText.enabled = true;
                amountText.text = item.quantity.ToString();
            }
            else
            {
                // Eger bu sirada esya yoksa, kutu bos gorunmeli
                // Ikonu ve yaziyi gizle
                iconImage.enabled = false;
                amountText.enabled = false;
            }
        }
    }

    void OnDestroy()
    {
        if (inventory != null)
        {
            inventory.onItemChangedCallback -= UpdateUI;
        }
    }
}