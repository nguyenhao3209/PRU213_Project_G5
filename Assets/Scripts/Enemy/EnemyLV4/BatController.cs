using UnityEngine;

public class BatController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public Transform pointA;
    public Transform pointB;
    public float verticalAmplitude = 0.5f; // biên độ dao động
    public float verticalFrequency = 2f;   // tốc độ dao động

    [Header("Damage Settings")]
    public int damage = 10;

    private Transform targetPoint;
    private float baseY; // vị trí Y gốc để tính dao động

    private void Start()
    {
        // Bắt đầu bay từ A → B
        targetPoint = pointB;
        baseY = transform.position.y;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (pointA == null || pointB == null) return;

        // Lấy vị trí hiện tại
        Vector2 currentPosition = transform.position;

        // Di chuyển theo trục X về phía targetPoint
        float step = moveSpeed * Time.deltaTime;
        float newX = Mathf.MoveTowards(currentPosition.x, targetPoint.position.x, step);

        // Dao động lên xuống theo hình sin
        float newY = baseY + Mathf.Sin(Time.time * verticalFrequency) * verticalAmplitude;

        // Cập nhật vị trí
        transform.position = new Vector2(newX, newY);

        // Khi đến gần điểm đích → đổi hướng
        if (Mathf.Abs(transform.position.x - targetPoint.position.x) < 0.05f)
        {
            // Chuyển điểm bay (A <-> B)
            targetPoint = (targetPoint == pointA) ? pointB : pointA;

            // Lật hướng sprite (quay đầu)
            Vector3 localScale = transform.localScale;
            localScale.x = -localScale.x;
            transform.localScale = localScale;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health player = collision.gameObject.GetComponent<Health>();
            if (player != null)
                player.TakeDamage(damage);
        }
    }
}
