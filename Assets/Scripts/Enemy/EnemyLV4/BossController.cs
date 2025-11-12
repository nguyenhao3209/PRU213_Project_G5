using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform firePoint;     // Vị trí bắn đạn
    [SerializeField] private GameObject bulletPrefab; // Prefab đạn boss
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private AudioClip attackSound;

    private Animator anim;
    private Transform player;
    private Health health;
    private float cooldownTimer;
    private bool isDead = false;
    private bool isAttacking = false; // ✅ thêm để tránh spam trigger attack

    private void Awake()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        // ✅ Ngăn gọi Die() lặp lại
        if (isDead || health == null || health.currentHealth <= 0)
        {
            if (!isDead)
                Die();
            return;
        }

        // Không tấn công nếu đang trong animation attack
        if (isAttacking) return;

        cooldownTimer += Time.deltaTime;

        if (cooldownTimer >= attackCooldown && player != null)
        {
            cooldownTimer = 0;
            Attack();
        }
    }

    private void Attack()
    {
        if (isDead) return;

        isAttacking = true; // ✅ để tránh tấn công lặp
        anim.ResetTrigger("Attack"); // tránh trigger chồng
        anim.SetTrigger("Attack");

        // Delay bắn để khớp animation
        Invoke(nameof(ShootProjectile), 0.3f);

        // Cho phép tấn công lại sau khi animation Attack kết thúc (~1 giây)
        Invoke(nameof(ResetAttack), 1f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void ShootProjectile()
    {
        if (isDead || bulletPrefab == null || player == null) return;

        // ✅ Kiểm tra SoundManager tồn tại để tránh NullRef
        if (SoundManager.instance != null && attackSound != null)
            SoundManager.instance.PlaySound(attackSound);

        // Tạo đạn và bắn hướng player
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.velocity = direction * bulletSpeed;

        // Lật hướng boss
        if ((direction.x > 0 && transform.localScale.x < 0) ||
            (direction.x < 0 && transform.localScale.x > 0))
        {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
        }
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        anim.ResetTrigger("Attack");
        anim.SetTrigger("Die");

        if (TryGetComponent(out Collider2D col))
            col.enabled = false;
        if (TryGetComponent(out Rigidbody2D rb))
            rb.simulated = false;

        // ✅ tránh lỗi "MissingReference" bằng cách cancel Invoke
        CancelInvoke();

        Destroy(gameObject, 1.5f);
    }
}
