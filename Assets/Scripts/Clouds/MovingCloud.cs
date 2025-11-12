using UnityEngine;

public class MovingCloud : MonoBehaviour
{
    [Header("Cài đặt di chuyển")]
    public float moveSpeed = 1f;        // tốc độ di chuyển
    public float moveDistance = 1f;       // khoảng cách di chuyển từ vị trí gốc
    public bool moveVertically = true;    // true = lên/xuống, false = ngang

    private Vector3 startPos;             // vị trí ban đầu
    private int direction = 1;            // chiều di chuyển

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Tính toán di chuyển
        Vector3 movement = (moveVertically ? Vector3.up : Vector3.right) * moveSpeed * direction * Time.deltaTime;
        transform.position += movement;

        // Đổi chiều khi vượt khoảng cách
        if (Vector3.Distance(startPos, transform.position) >= moveDistance)
        {
            direction *= -1;
        }
    }
}
