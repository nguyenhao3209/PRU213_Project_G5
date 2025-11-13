using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BossController_Level6 : MonoBehaviour
{
    [Header("Boss Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 8f;
    public float attackRange = 5f;
    public float attackCooldown = 2f;
    public int maxHealth = 10;
    private int currentHealth;

    [Header("Attack")]
    public GameObject bulletPrefab;      // Prefab ??n
    public Transform firePoint;          // V? tr� b?n ??n

    [Header("UI")]
    public Slider healthBar;
    public Image fillImage;   // để đổi màu theo % máu
                              // Thanh m�u tr�n Canvas

    private Transform player;
    private Animator animator;
    private Rigidbody2D rb;
    private bool isFacingRight = true;
    private bool isAttacking = false;
    private float nextAttackTime = 0f;
    public GameObject deathEffect;


    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player").transform;

        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
        healthBar.value = currentHealth;
    }



    void Update()
    {
        if (currentHealth <= 0) return; // Boss ch?t th� d?ng

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= detectionRange && distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else if (distance <= attackRange)
        {
            Attack();
        }
        else
        {
            animator.SetBool("ClimbTrigger", true);
        }

        FlipTowardsPlayer();
    }

    void MoveTowardsPlayer()
    {
        animator.SetTrigger("MoveTrigger");
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        Vector2 newPos = Vector2.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        rb.MovePosition(newPos);
    }


    void Attack()
    {
        if (Time.time >= nextAttackTime && !isAttacking)
        {
            isAttacking = true;
            animator.SetTrigger("SpecialAttack");
            Invoke(nameof(FireBullet), 0.5f);
            nextAttackTime = Time.time + attackCooldown;
        } 
    }

    void FireBullet()
    {
        if (bulletPrefab && firePoint)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Vector2 direction = (player.position - firePoint.position).normalized;
            bullet.GetComponent<Rigidbody2D>().linearVelocity = direction * 6f;
        }
        isAttacking = false;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        StartCoroutine(UpdateHealthBarSmooth());

        if (currentHealth == 0)
        {
            Die();
        }
    }
    public IEnumerator UpdateHealthBarSmooth()
    {
        float currentValue = healthBar.value;
        float targetValue = currentHealth;

        while (Mathf.Abs(currentValue - targetValue) > 0.1f)
        {
            currentValue = Mathf.Lerp(currentValue, targetValue, Time.deltaTime * 8f);
            healthBar.value = currentValue;

            // Đổi màu thanh máu (xanh → vàng → đỏ)
            float t = currentHealth / (float)maxHealth;
            fillImage.color = Color.Lerp(Color.red, Color.green, t);

            yield return null;
        }

        healthBar.value = targetValue;
    }

    void Die()
    {
        animator.SetTrigger("DeathTrigger");
        rb.linearVelocity = Vector2.zero;
        rb.bodyType = RigidbodyType2D.Kinematic; // ✅ Thay cho isKinematic
        GetComponent<Collider2D>().enabled = false;
        healthBar.gameObject.SetActive(false);

        if (deathEffect != null)
            Instantiate(deathEffect, transform.position, Quaternion.identity);

        Destroy(gameObject, 2f);
    }


    void FlipTowardsPlayer()
    {
        if (player == null) return;

        // Đảo điều kiện
        if (player.position.x > transform.position.x && isFacingRight)
            Flip();
        else if (player.position.x < transform.position.x && !isFacingRight)
            Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}
