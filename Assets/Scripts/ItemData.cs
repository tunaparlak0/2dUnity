using UnityEngine;

// Eşya türlerini listeleyelim
public enum ItemType
{
    Resource,   // Odun, Tas vb.
    Tool,       // Capa, Balta, Kilic
    Seed,       // Tohum
    Consumable  // Yemek (Elma)
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;

    [Header("Tur Ayarlari")]
    public ItemType itemType; // <-- YENI: Bu esya ne tur?

    [Header("Yiginlama")]
    public bool isStackable = true;
    public int maxStackSize = 99;
}