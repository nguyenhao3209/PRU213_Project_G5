using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("Projectile Settings")]
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

        // ✅ Hợp nhất xử lý cho Enemy và Boss từ Level 4 → 6

        // ---- ENEMY ----
        if (collision.CompareTag("Enemy"))
        {
            // SlimeGirl (Level 4)
            SlimeGirl slime = collision.GetComponent<SlimeGirl>();
            if (slime != null)
            {
                slime.TakeDamage(damage);
                Deactivate();
                return;
            }

            // EnemyAI2D (Level 4)
            EnemyAI2D enemyAI = collision.GetComponent<EnemyAI2D>();
            if (enemyAI != null)
            {
                enemyAI.TakeDamage(damage);
                Deactivate();
                return;
            }

            // Health (fallback cho enemy thường)
            Health enemyHealth = collision.GetComponent<Health>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Deactivate();
                return;
            }
        }

        // ---- BOSS ----
        if (collision.CompareTag("Boss"))
        {
            // BossHealth (Level 4)
            BossHealth bossHealth = collision.GetComponent<BossHealth>();
            if (bossHealth != null)
            {
                bossHealth.TakeDamage(damage);
                Deactivate();
                return;
            }

            // BossController_LV5
            BossController_LV5 boss5 = collision.GetComponent<BossController_LV5>();
            if (boss5 != null)
            {
                boss5.TakeDamage(damage);
                Deactivate();
                return;
            }

            // BossController_LV6
            BossController_LV6 boss6 = collision.GetComponent<BossController_LV6>();
            if (boss6 != null)
            {
                boss6.TakeDamage(damage);
                Deactivate();
                return;
            }
        }

        // ✅ fallback — bất kỳ object nào có Health component
        Health generic = collision.GetComponent<Health>();
        if (generic != null)
        {
            generic.TakeDamage(damage);
        }

        Deactivate();
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

    // Gọi từ animation event “explode”
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
