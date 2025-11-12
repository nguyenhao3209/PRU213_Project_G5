using UnityEngine;

public class BossBullet_LV5 : MonoBehaviour
{
    public int damage = 1;       // sát thương gây cho player
    public float lifetime = 2f;   // tự hủy sau 3 giây

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Nếu trúng Player
        if (collision.CompareTag("Player"))
        {
            // Giảm máu Player
            Health player = collision.GetComponent<Health>();
            if (player != null)
            {
                player.TakeDamage(damage);
            }

            // Hủy viên đạn
            Destroy(gameObject);
        }

        // Nếu trúng tường hoặc vật cản
        if (collision.CompareTag("Ground") || collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}
