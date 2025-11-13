using UnityEngine;

public class OrbController : MonoBehaviour
{
    // Các thông số này bạn có thể chỉnh trong Inspector
    public float speed = 4f;        // Tốc độ bay (giá trị thấp để bay chậm như Galamoth)
    public int damage = 10;         // Sát thương gây ra
    public float lifetime = 10f;    // Tự hủy sau 10 giây (để nó bay được lâu)

    private Transform player;
    private Vector2 moveDirection; // Hướng bay cố định

    void Start()
    {
        // 1. Tự hủy sau khoảng thời gian 'lifetime'
        // Điều này rất quan trọng để quả cầu không tồn tại mãi mãi
        Destroy(gameObject, lifetime);

        // 2. Tìm người chơi MỘT LẦN DUY NHẤT
        player = GameObject.FindGameObjectWithTag("Player").transform;
        
        // 3. Tính toán hướng bay MỘT LẦN DUY NHẤT
        if (player != null)
        {
            // Tính hướng từ quả cầu (transform.position) đến người chơi (player.position)
            moveDirection = (player.position - transform.position).normalized;
        }
        else
        {
            // Nếu không tìm thấy người chơi, cứ bay sang phải
            moveDirection = Vector2.right; 
        }
    }

    void Update()
    {
        // 4. Di chuyển quả cầu theo hướng đã tính
        // Vì 'moveDirection' không bao giờ thay đổi, quả cầu sẽ bay thẳng
        transform.Translate(moveDirection * speed * Time.deltaTime);
    }

    // 5. Xử lý va chạm (vì ta dùng "Is Trigger")
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Kiểm tra xem có va phải người chơi không
        // (Hãy chắc chắn nhân vật của bạn có Tag là "Player")
        if (other.CompareTag("Player"))
        {
            // Gây sát thương cho người chơi
            Debug.Log("Đã trúng Player!");
            other.GetComponent<AlucardController>().TakeDamage(damage);
            
            // Tự hủy ngay lập tức khi trúng người chơi
            Destroy(gameObject);
        }
        
        // Bạn cũng có thể thêm logic tự hủy khi va vào tường (nếu tường có Tag là "Ground")
        if (other.CompareTag("Ground"))
        {
            Destroy(gameObject);
        }
    }
}