using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 5;

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
        if (lifetime > 5f)
            gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        // ✅ Gộp toàn bộ logic xử lý va chạm từ cả hai nhánh

        // --- Level 3/4: Enemy & Boss (Health/BossHealth/AI/SlimeGirl)
        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {
            // SlimeGirl đặc biệt
            SlimeGirl slime = collision.GetComponent<SlimeGirl>();
            if (slime != null)
            {
                slime.TakeDamage(damage);
                return;
            }

            // Enemy AI thông thường
            EnemyAI2D enemy = collision.GetComponent<EnemyAI2D>();
            if (enemy != null)
                enemy.TakeDamage(damage);

            // Gây sát thương qua Health component
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
                targetHealth.TakeDamage(damage);

            // BossHealth (Level 3)
            BossHealth bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null)
                bossHealth.TakeDamage(damage);

            // BossController (Level 5)
            BossController_LV5 bossController = collision.GetComponent<BossController_LV5>();
            if (bossController != null)
                bossController.TakeDamage(damage);
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

    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    // Hàm gọi từ animation event “explode”
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
