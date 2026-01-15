using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Chest : MonoBehaviour
{
    public List<InventoryItem> chestItems = new List<InventoryItem>();
    public int capacity = 16; // 4x4 istedin

    [Header("Gorseller")]
    public Sprite closedSprite;
    public Sprite openSprite;

    private SpriteRenderer spriteRenderer;
    private bool isPlayerInZone;
    private bool isOpen = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = closedSprite; // Baslangicta kapali

        // Listeyi bosaltip hazirla
        chestItems.Clear();
        for (int i = 0; i < capacity; i++) chestItems.Add(new InventoryItem());
    }

    void Update()
    {
        if (!isPlayerInZone) return;

        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (!isOpen) Open();
            else Close();
        }
    }

    void Open()
    {
        isOpen = true;
        spriteRenderer.sprite = openSprite; // Kapagi ac
        ChestUI.instance.OpenChest(this); // UI'yi ac
    }

    public void Close()
    {
        isOpen = false;
        spriteRenderer.sprite = closedSprite; // Kapagi kapa
        ChestUI.instance.CloseChest(); // UI'yi kapa
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInZone = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInZone = false;
            Close(); // Uzaklasinca otomatik kapat
        }
    }
}