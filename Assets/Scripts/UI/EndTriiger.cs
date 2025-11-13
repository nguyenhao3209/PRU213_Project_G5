using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [Header("WinCanvas")]
    [SerializeField] private GameObject winCanvas;  // Gán Canvas chiến thắng
    [SerializeField] private float delayToShow = 0.5f; // Thời gian trễ (tùy chọn)

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered) return;
        if (collision.CompareTag("Player"))
        {
            triggered = true;
            Debug.Log("Player reached the end point!");

            // Dừng player (nếu có Rigidbody2D)
            var rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
                rb.linearVelocity = Vector2.zero;

            // Hiện Canvas sau một khoảng ngắn
            Invoke(nameof(ShowWinCanvas), delayToShow);
        }
    }

    private void ShowWinCanvas()
    {
        if (winCanvas != null)
        {
            winCanvas.SetActive(true);
            Time.timeScale = 0f; // Dừng game
        }
    }
}
