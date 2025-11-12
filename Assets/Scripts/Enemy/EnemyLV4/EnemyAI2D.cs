using UnityEngine;
using System.Collections; // Cần cho IEnumerator

public class EnemyAI2D : MonoBehaviour
{
    [Header("References")]
    public Transform player;
    public Animator animator;

    [Header("Settings")]
    public float moveSpeed = 2f;
    public float detectionRange = 10f; // phạm vi phát hiện player
    public float damage = 1f;
    public float attackCooldown = 1f;

    [Header("Movement Limit")]
    public Transform leftLimit;
    public Transform rightLimit;

    [Header("Health Settings")]
    public int maxHealth = 1; // số lần trúng fireball để chết

    private int currentHealth;
    private Rigidbody2D rb;
    private bool movingLeft = true;
    private bool isDead = false;
    private float lastAttackTime;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;

        // Tìm player tự động nếu chưa gán
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
                player = p.transform;
        }
    }

    private void FixedUpdate()
    {
        if (isDead) return;

        if (player != null && Vector2.Distance(new Vector2(transform.position.x, transform.position.y), new Vector2(player.position.x, player.position.y)) <= detectionRange)
        {
            ChasePlayer();
        }
        else
        {
            Patrol();
        }
    }

    private void Patrol()
    {
        animator.SetBool("isMoving", true);

        if (movingLeft)
        {
            if (transform.position.x > leftLimit.position.x)
            {
                Move(-1);
            }
            else
            {
                movingLeft = false;
            }
        }
        else
        {
            if (transform.position.x < rightLimit.position.x)
            {
                Move(1);
            }
            else
            {
                movingLeft = true;
            }
        }
    }

    private void ChasePlayer()
    {
        animator.SetBool("isMoving", true);

        Vector2 direction = new Vector2(player.position.x, player.position.y) - new Vector2(transform.position.x, transform.position.y);
        direction = direction.normalized;
        direction.y = 0;

        rb.MovePosition(rb.position + direction * moveSpeed * Time.fixedDeltaTime);

        // Quay hướng mặt theo hướng di chuyển
        if (direction.x > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (direction.x < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void Move(int dir)
    {
        rb.MovePosition(rb.position + Vector2.right * dir * moveSpeed * Time.fixedDeltaTime);

        // Quay mặt slime
        if (dir > 0)
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else if (dir < 0)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDead) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 contactPoint = collision.GetContact(0).point;
            Vector2 center = GetComponent<Collider2D>().bounds.center;

            // Nếu player từ trên nhảy xuống -> slime chết
            if (contactPoint.y > center.y + 0.1f)
            {
                Die();

                // Player bật ngược lên
                Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
                if (playerRb != null)
                    playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 8f);
            }
            else
            {
                // Player bị trúng bên hông -> trừ máu
                if (Time.time - lastAttackTime > attackCooldown)
                {
                    Health health = collision.gameObject.GetComponent<Health>();
                    if (health != null)
                        health.TakeDamage(damage);
                    lastAttackTime = Time.time;
                }
            }
        }
        else if (collision.gameObject.CompareTag("Projectile"))
        {
            // Nhận damage từ fireball
            TakeDamage(1);
            Destroy(collision.gameObject); // tắt fireball
        }
    }

    public void TakeDamage(int dmg)
    {
        if (isDead) return;

        currentHealth -= dmg;

        StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        sr.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        sr.color = Color.white;
    }

    public void Die()
    {
        if (isDead) return;
        isDead = true;

        StartCoroutine(DieRoutine());
    }

    private IEnumerator DieRoutine()
    {
        // Chớp 2-3 lần trước khi biến mất
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        int flashes = 1;
        float flashDuration = 0.1f;

        for (int i = 0; i < flashes; i++)
        {
            sr.color = Color.red;
            yield return new WaitForSeconds(flashDuration);
            sr.color = Color.white;
            yield return new WaitForSeconds(flashDuration);
        }

        if (animator != null)
            animator.SetTrigger("die");

        rb.linearVelocity = Vector2.zero;
        rb.isKinematic = true;
        GetComponent<Collider2D>().enabled = false;

        Destroy(gameObject, 0.5f);
    }
}
