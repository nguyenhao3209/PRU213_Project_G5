using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Boss Movement Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float attackRange = 3f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
            Debug.LogWarning("⚠️ Player chưa được gán trong BossAI!");
        else
            Debug.Log("✅ Boss đã nhận diện Player!");
    }

    private void Update()
    {
        if (player == null || rb == null) return;

        float distance = Vector2.Distance(transform.position, player.position);
        Debug.Log("📏 Khoảng cách đến player: " + distance);

        if (distance > attackRange)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();
        }

        FlipTowardsPlayer();
    }

    private void MoveTowardsPlayer()
    {
        Vector2 direction = (player.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(direction.x * moveSpeed, rb.linearVelocity.y);
        Debug.Log("🏃 Boss di chuyển với vận tốc: " + rb.linearVelocity);
    }

    private void StopMoving()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        Debug.Log("🛑 Boss dừng lại");
    }

    private void FlipTowardsPlayer()
    {
        if (player.position.x > transform.position.x)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
    }
}
