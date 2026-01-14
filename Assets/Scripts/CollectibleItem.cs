using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemData itemData; // <-- YENI: String yerine Data dosyasi isteyecegiz
    public int amount = 1;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory playerInv = other.GetComponent<PlayerInventory>();

            if (playerInv != null)
            {
                // String degil, Datayi gonderiyoruz
                playerInv.AddItem(itemData, amount);
                Destroy(gameObject);
            }
        }
    }
}