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

        // ✅ Hợp nhất toàn bộ xử lý va chạm cho Enemy + Boss từ cả 2 nhánh

        // --- Enemy (Level 3, 4)
        if (collision.CompareTag("Enemy"))
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

            // Health component chung
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
                targetHealth.TakeDamage(damage);
        }

        // --- Boss (Level 5, 6)
        if (collision.CompareTag("Boss"))
        {
            // Boss level 5
            BossController_LV5 boss5 = collision.GetComponent<BossController_LV5>();
            if (boss5 != null)
                boss5.TakeDamage(damage);

            // Boss level 6
            BossController_LV6 boss6 = collision.GetComponent<BossController_LV6>();
            if (boss6 != null)
                boss6.TakeDamage(damage);

            // BossHealth (nếu có)
            BossHealth bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null)
                bossHealth.TakeDamage(damage);
        }

        // Sau khi gây sát thương → vô hiệu hoá viên đạn
        gameObject.SetActive(false);
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

    // Gọi từ animation event "explode"
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
