using UnityEngine;

public class BossController : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float bulletSpeed = 6f;
    [SerializeField] private AudioClip attackSound;

    private Animator anim;
    private Transform player;
    private Health health;
    private float cooldownTimer;
    private bool isDead = false;
    private bool isAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        health = GetComponent<Health>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    private void Update()
    {
        if (isDead || health == null) return;

        // Boss chết
        if (health.currentHealth <= 0)
        {
            Die();
            return;
        }

        // Không tấn công nếu đang trong animation attack
        if (isAttacking) return;

        cooldownTimer += Time.deltaTime;

        // Boss chỉ bắn khi player trong phạm vi nhất định
        if (cooldownTimer >= attackCooldown && player != null && IsPlayerInRange())
        {
            cooldownTimer = 0;
            Attack();
        }
    }

    private bool IsPlayerInRange()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        return distance < 12f; // bạn có thể chỉnh phạm vi tấn công
    }

    private void Attack()
    {
        if (isDead) return;

        isAttacking = true;
        anim.ResetTrigger("Attack");
        anim.SetTrigger("Attack");

        Invoke(nameof(ShootProjectile), 0.4f);
        Invoke(nameof(ResetAttack), 1.5f);
    }

    private void ResetAttack()
    {
        isAttacking = false;
    }

    private void ShootProjectile()
    {
        if (isDead || bulletPrefab == null || player == null) return;

        if (SoundManager.instance != null && attackSound != null)
            SoundManager.instance.PlaySound(attackSound);

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Vector2 direction = (player.position - firePoint.position).normalized;

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = direction * bulletSpeed;

        // Lật hướng Boss theo hướng Player
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
        CancelInvoke();
        anim.SetTrigger("Die");

        if (TryGetComponent(out Collider2D col))
            col.enabled = false;
        if (TryGetComponent(out Rigidbody2D rb))
            rb.simulated = false;

        Destroy(gameObject, 1.5f);
    }
}
