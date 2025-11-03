using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioClip checkpoint;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private UIManager uiManager;
    private CameraController camController;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
        uiManager = Object.FindFirstObjectByType<UIManager>();
        camController = Object.FindFirstObjectByType<CameraController>();
    }

    public void RespawnCheck()
    {
        if (currentCheckpoint == null)
        {
            // Không có checkpoint => hiện Game Over
            if (uiManager != null)
                uiManager.GameOver();
            else
                Debug.LogWarning("⚠️ PlayerRespawn: UIManager not found!");
            return;
        }

        // Hồi máu và reset animation
        playerHealth.Respawn();

        // Đưa player về checkpoint
        transform.position = currentCheckpoint.position;

        // Di chuyển camera về đúng room chứa checkpoint
        if (camController != null)
        {
            // Nếu checkpoint nằm trong room object (parent)
            Transform room = currentCheckpoint.parent != null ? currentCheckpoint.parent : currentCheckpoint;
            camController.MoveToNewRoom(room);
        }
        else
        {
            Debug.LogWarning("⚠️ PlayerRespawn: CameraController not found!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform;

            // Bật âm thanh checkpoint
            if (SoundManager.instance != null && checkpoint != null)
                SoundManager.instance.PlaySound(checkpoint);

            // Hiệu ứng checkpoint
            var anim = collision.GetComponent<Animator>();
            if (anim != null)
                anim.SetTrigger("activate");

            // Vô hiệu hóa collider để tránh kích hoạt lại
            collision.GetComponent<Collider2D>().enabled = false;

            Debug.Log($"✅ New checkpoint set: {currentCheckpoint.name}");
        }
    }
}
