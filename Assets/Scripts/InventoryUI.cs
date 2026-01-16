using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Referanslari")]
    public GameObject inventoryPanel; // "I" ile acilan buyuk panel
    public Transform inventoryGrid;   // Cantanin slotlarinin dizilecegi yer
    public Transform hotbarGrid;      // Hotbarin slotlarinin dizilecegi yer
    public GameObject slotPrefab;     // Kutu kalibi

    [Header("Secim Gorseli")]
    public GameObject selectorFrame;  // Secili kutunun etrafindaki cerceve

    PlayerInventory inventory;

    // Tum slotlari tek listede tutalim (Ilk 8'i hotbar, gerisi canta)
    List<GameObject> allSlots = new List<GameObject>();

    int totalSlotCount = 24; // 8 Hotbar + 16 Canta
    int selectedSlotIndex = 0; // Su an hangi slot secili?

    // --- EKLEME 1: Singleton Instance ---
    public static InventoryUI instance;

    // --- EKLEME 2: Awake Fonksiyonu (Start'tan once calisir) ---
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerInventory>();
        inventory.onItemChangedCallback += UpdateUI;

        // --- SLOTLARI OLUSTURMA ---
        for (int i = 0; i < totalSlotCount; i++)
        {
            GameObject newSlot;

            // Eger ilk 8 slottan biriyse -> Hotbar'a koy
            if (i < 8)
            {
                newSlot = Instantiate(slotPrefab, hotbarGrid);
            }
            // Degilse -> Cantaya koy
            else
            {
                newSlot = Instantiate(slotPrefab, inventoryGrid);
            }

            allSlots.Add(newSlot);
        }

        inventoryPanel.SetActive(false); // Cantayi gizle
        UpdateSelection(); // Ilk secimi yap
        UpdateUI();
    }

    void Update()
    {
        // "I" tusu -> Canta ac/kapa
        if (Keyboard.current.iKey.wasPressedThisFrame)
        {
            inventoryPanel.SetActive(!inventoryPanel.activeSelf);
            if (inventoryPanel.activeSelf) UpdateUI();
        }

        // --- SAYI TUSLARI ILE SECIM (1-8) ---
        if (Keyboard.current.digit1Key.wasPressedThisFrame) SelectSlot(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) SelectSlot(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) SelectSlot(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) SelectSlot(3);
        if (Keyboard.current.digit5Key.wasPressedThisFrame) SelectSlot(4);
        if (Keyboard.current.digit6Key.wasPressedThisFrame) SelectSlot(5);
        if (Keyboard.current.digit7Key.wasPressedThisFrame) SelectSlot(6);
        if (Keyboard.current.digit8Key.wasPressedThisFrame) SelectSlot(7);
    }

    void SelectSlot(int index)
    {
        selectedSlotIndex = index;
        UpdateSelection();
    }

    void UpdateSelection()
    {
        if (selectorFrame != null && allSlots.Count > selectedSlotIndex)
        {
            selectorFrame.transform.SetParent(allSlots[selectedSlotIndex].transform);
            selectorFrame.transform.localPosition = Vector3.zero;
            selectorFrame.SetActive(true);
        }
    }

    // --- EKLEME 3: Guncellenmis UpdateUI ---
    public void UpdateUI()
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            InventorySlot slotScript = allSlots[i].GetComponent<InventorySlot>();

            // ESKI KOD: Buradaki if-else bloklarini sildik.
            // YENI KOD: Tek satirda "Setup" cagiriyoruz.
            // Setup fonksiyonu; resmi, sayiyi ve hangi listeye ait oldugunu kendi icinde hallediyor.

            slotScript.Setup(inventory.items, i, false);
            // inventory.items -> Oyuncunun esya listesi
            // i -> Sira numarasi
            // false -> "Bu bir sandik degil" demek (Cunku bu script oyuncunun cantasi)
        }
    }
    // Su an secili olan slotun icindeki esyanin VERISINI dondur
    public ItemData GetSelectedItem()
    {
        // Secili slot gecersiz mi?
        if (selectedSlotIndex >= allSlots.Count) return null;

        InventorySlot slot = allSlots[selectedSlotIndex].GetComponent<InventorySlot>();

        // Slot bos mu?
        if (slot.myOwnerList == null || slot.myIndex >= slot.myOwnerList.Count) return null;

        InventoryItem item = slot.myOwnerList[slot.myIndex];

        // Item var mi?
        if (item != null) return item.data;

        return null;
    }
}