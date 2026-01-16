using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class FarmManager : MonoBehaviour
{
    [Header("Bilesenler")]
    public Tilemap groundTilemap; // Zemin (Toprak/Cim)
    public Tilemap cropTilemap;   // <-- YENI: Bitkiler bu katmanda olacak
    public Transform playerTransform; // <-- YENI: Oyuncunun konumu lazim

    [Header("Ayarlar")]
    public float interactionRange = 2.0f; // Kac metre uzaga erisebilir?

    [Header("Gorseller")]
    public TileBase plowedSoilTile;
    public TileBase plantedTile;
    public TileBase ripeCropTile;

    [Header("Urunler")]
    public ItemData productItem;

    // Hafiza
    private Dictionary<Vector3Int, Crop> activeCrops = new Dictionary<Vector3Int, Crop>();

    private Vector3Int selectedCell;
    private PlayerInventory playerInventory;

    void Start()
    {
        // UYARI DUZELTME: FindFirstObjectByType kullaniyoruz artik.
        playerInventory = Object.FindFirstObjectByType<PlayerInventory>();
    }

    void Update()
    {
        // K tusu ile gun atlatma (TEST)
        if (Keyboard.current.kKey.wasPressedThisFrame) AdvanceDay();

        if (Mouse.current == null) return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        mouseWorldPos.z = 0;
        selectedCell = groundTilemap.WorldToCell(mouseWorldPos);

        // --- MESAFE KONTROLU ---
        // Oyuncu ile farenin tikladigi yer arasindaki mesafeyi olc
        // Not: selectedCell'in merkezini bulmak icin "GetCellCenterWorld" kullaniyoruz
        Vector3 cellWorldPos = groundTilemap.GetCellCenterWorld(selectedCell);
        float distance = Vector3.Distance(playerTransform.position, cellWorldPos);

        // Eger mesafe 2 birimden fazlaysa, tiklamalari iptal et
        if (distance > interactionRange) return;


        // --- E TUSU: HASAT ---
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            TryHarvest();
        }

        // --- SOL TIK: EKME VE CAPALAMA ---
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            UseToolOrSeed();
        }
    }

    void TryHarvest()
    {
        if (activeCrops.ContainsKey(selectedCell))
        {
            Crop clickedCrop = activeCrops[selectedCell];

            if (clickedCrop.growthStage >= 1)
            {
                if (playerInventory != null) playerInventory.AddItem(productItem, 1);

                activeCrops.Remove(selectedCell);

                // GORSEL DUZELTME: Bitkiyi silerken CROPS katmanini temizle
                cropTilemap.SetTile(selectedCell, null);
                // Alttaki toprak (groundTilemap) oldugu gibi "Surulmus" kalsin

                Debug.Log("🍅 Hasat basarili!");
            }
        }
    }

    void UseToolOrSeed()
    {
        ItemData currentItem = InventoryUI.instance.GetSelectedItem();
        if (currentItem == null) return;

        // Bitki olan yere tekrar islem yapma
        if (activeCrops.ContainsKey(selectedCell)) return;

        // 1. ÇAPA
        if (currentItem.itemType == ItemType.Tool && (currentItem.itemName.Contains("Çapa") || currentItem.itemName.Contains("Hoe")))
        {
            // Çapa GROUND (Zemin) katmanini degistirir
            groundTilemap.SetTile(selectedCell, plowedSoilTile);
        }

        // 2. TOHUM EKME
        else if (currentItem.itemType == ItemType.Seed)
        {
            // Zemin katmaninda ne var?
            TileBase tileHere = groundTilemap.GetTile(selectedCell);

            // Sadece Surulmus Topraga ekebiliriz
            if (tileHere == plowedSoilTile && !activeCrops.ContainsKey(selectedCell))
            {
                // GORSEL DUZELTME: Tohumu CROPS (Bitki) katmanina ekle!
                // Boylece altta kahverengi toprak gorunmeye devam eder.
                cropTilemap.SetTile(selectedCell, plantedTile);

                if (playerInventory != null) playerInventory.RemoveItem(currentItem, 1);

                Crop newCrop = new Crop("Domates", ripeCropTile);
                activeCrops.Add(selectedCell, newCrop);
                Debug.Log("🌱 Tohum ekildi.");
            }
        }
    }

    public void AdvanceDay()
    {
        Debug.Log("🌞 Gun dogdu!");
        List<Vector3Int> cropPositions = new List<Vector3Int>(activeCrops.Keys);

        foreach (Vector3Int pos in cropPositions)
        {
            Crop crop = activeCrops[pos];
            crop.daysOld++;

            if (crop.daysOld >= 1)
            {
                crop.growthStage = 1;
                // Buyumeyi de CROPS katmaninda guncelle
                cropTilemap.SetTile(pos, crop.harvestTile);
            }
        }
    }
}

public class Crop
{
    public string cropName;
    public int growthStage;
    public int daysOld;
    public TileBase harvestTile;

    public Crop(string name, TileBase finalTile)
    {
        cropName = name;
        growthStage = 0;
        daysOld = 0;
        harvestTile = finalTile;
    }
}