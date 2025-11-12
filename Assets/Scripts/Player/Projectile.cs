using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;

    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;

        float movementSpeed = speed * Time.deltaTime * direction;
        transform.Translate(movementSpeed, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5f) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        // ✅ Gây damage cho các loại đối tượng khác nhau tùy theo level

        // Trường hợp Level 3 (Enemy, Boss cổ điển)
        if (collision.CompareTag("Enemy"))
            collision.GetComponent<Health>()?.TakeDamage(damage);

        if (collision.CompareTag("Boss"))
            collision.GetComponent<BossHealth>()?.TakeDamage(damage);

        // Trường hợp Level 4 trở đi (đa dạng đối tượng có Health hoặc AI riêng)
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            // SlimeGirl đặc biệt
            SlimeGirl slime = collision.GetComponent<SlimeGirl>();
            if (slime != null)
            {
                slime.TakeDamage(damage);
                return;
            }

            // Gây damage cho bất kỳ object nào có Health
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }

            // EnemyAI2D (nếu có)
            EnemyAI2D enemy = collision.GetComponent<EnemyAI2D>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    // Hàm gọi từ animation event “explode”
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
