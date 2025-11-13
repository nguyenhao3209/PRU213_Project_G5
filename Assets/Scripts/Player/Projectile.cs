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

    if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
    {
        // 🧪 Trường hợp đặc biệt SlimeGirl (LV4)
        SlimeGirl slime = collision.GetComponent<SlimeGirl>();
        if (slime != null)
        {
            slime.TakeDamage(damage);
            Invoke(nameof(Deactivate), 0.3f);
            return;
        }

        // 🧱 Level 3 (enemy cổ điển dùng Health)
        if (collision.CompareTag("Enemy"))
            collision.GetComponent<Health>()?.TakeDamage(damage);

        // 🧱 Level 3 Boss
        if (collision.CompareTag("Boss"))
            collision.GetComponent<BossHealth>()?.TakeDamage(damage);

        // 🧱 Level 4 trở đi: đối tượng có Health hoặc AI riêng

        // Có Health → dùng Health
        Health targetHealth = collision.GetComponent<Health>();
        if (targetHealth != null)
            targetHealth.TakeDamage(damage);

        // Có AI EnemyAI2D → damage AI
        EnemyAI2D enemyAI = collision.GetComponent<EnemyAI2D>();
        if (enemyAI != null)
            enemyAI.TakeDamage(damage);
    }

    // 🔥 Tắt viên đạn sau hiệu ứng nổ
    Invoke(nameof(Deactivate), 0.3f);
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
