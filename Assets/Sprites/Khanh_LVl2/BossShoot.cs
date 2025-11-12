using UnityEngine;

public class BossShoot : MonoBehaviour
{
    public Transform firePoint;      // Nơi bắn ra đạn (child FirePoint)
    public GameObject bulletPrefab;  // Prefab viên đạn BossBullet
    public float attackCooldown = 2f; // Thời gian giữa các lần bắn
    private float cooldownTimer;
    private Transform player;
    private Animator anim;
    private bool facingRight = true;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Tăng thời gian cooldown
        cooldownTimer += Time.deltaTime;

        // Nếu có player và đủ thời gian cooldown thì tấn công
        if (player != null && cooldownTimer >= attackCooldown)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            if (distance < 8f)
            {
                anim.SetTrigger("Attack");
                cooldownTimer = 0;
            }
        }

        // Luôn xoay mặt theo hướng Player
        if (player != null)
        {
            if (player.position.x > transform.position.x && !facingRight)
                Flip();
            else if (player.position.x < transform.position.x && facingRight)
                Flip();
        }
    }

    // 🔥 Hàm này được gọi trong Animation Event (Boss_Attack)
    public void Shoot()
    {
        if (bulletPrefab == null || firePoint == null || player == null)
        {
            Debug.LogError("⚠️ FirePoint hoặc BulletPrefab chưa được gán!");
            return;
        }

        // Tạo viên đạn tại FirePoint
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        // Tính hướng bay từ Boss đến Player
        Vector2 direction = (player.position - firePoint.position).normalized;

        // Gán vận tốc cho đạn
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        rb.velocity = direction * 6f; // tốc độ 6, có thể chỉnh

        // Xoay sprite của đạn cho đúng hướng
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        bullet.transform.rotation = Quaternion.Euler(0, 0, angle);

        Debug.Log("🚀 Boss bắn đạn về hướng: " + direction);
    }

    // Hàm xoay mặt Boss
    private void Flip()
    {
        facingRight = !facingRight;
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
