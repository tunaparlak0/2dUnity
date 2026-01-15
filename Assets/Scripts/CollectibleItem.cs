using UnityEngine;

public class CollectibleItem : MonoBehaviour
{
    public ItemData itemData; // <-- YENI: String yerine Data dosyasi isteyecegiz
    public int amount = 1;

    void Start()
    {
        // 1. Sahne'de "Collectibles" adinda bir obje var mi diye ara
        GameObject parentObject = GameObject.Find("Collectibles");

        // 2. Eger varsa, beni onun cocugu yap
        if (parentObject != null)
        {
            transform.SetParent(parentObject.transform);
        }
    }
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