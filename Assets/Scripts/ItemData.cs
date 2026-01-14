using UnityEngine;

// Bu satir, Unity'nin sag tik menusune "Create Item" secenegi ekler
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;  // Esyanin adi
    public Sprite icon;      // Esyanin resmi (Iste aradigimiz sey!)
    [TextArea]
    public string description; // Ileride kullaniriz (Aciklama)
}