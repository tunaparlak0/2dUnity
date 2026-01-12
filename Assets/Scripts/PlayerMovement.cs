using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Hız ayarı
    Rigidbody2D rb;
    Vector2 movement;

    void Start()
    {
        // Rigidbody'yi otomatik bul
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Klavyeden yönleri al
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");
    }

    void FixedUpdate()
    {
        // Hareketi uygula
        rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
    }
}
