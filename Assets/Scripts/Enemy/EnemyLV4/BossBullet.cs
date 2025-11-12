using UnityEngine;

public class BossBullet : MonoBehaviour
{
    [SerializeField] private float damage = 1f;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private AudioClip hitSound;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Health playerHealth = collision.GetComponent<Health>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                SoundManager.instance.PlaySound(hitSound);
            }
            Destroy(gameObject);
        }
    }
}
