using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 6f;
    public int damage = 1;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().TakeDamage(damage);
            Destroy(gameObject);
        }

    }
}
