using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    [SerializeField] private int damage = 1;

    private float direction;
    private bool hit;
    private float lifetime;

    private Animator anim;
    private BoxCollider2D boxCollider;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (hit) return;

        float movement = speed * Time.deltaTime * direction;
        transform.Translate(movement, 0, 0);

        lifetime += Time.deltaTime;
        if (lifetime > 5f) gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hit = true;
        boxCollider.enabled = false;
        anim.SetTrigger("explode");

        if (collision.CompareTag("Enemy") || collision.CompareTag("Boss"))
        {

            SlimeGirl slime = collision.GetComponent<SlimeGirl>();
            if(slime != null)
            {
                slime.TakeDamage(damage);
                return; // dừng kiểm tra nếu là SlimeGirl
            }

            // ✅ Gây damage cho bất kỳ object nào có Health
            Health targetHealth = collision.GetComponent<Health>();
            if (targetHealth != null)
            {
                targetHealth.TakeDamage(damage);
            }

            EnemyAI2D enemy = collision.GetComponent<EnemyAI2D>();
            if (enemy != null)
                enemy.TakeDamage(damage);
        }
    }

    public void SetDirection(float _direction)
    {
        lifetime = 0;
        direction = _direction;
        gameObject.SetActive(true);
        hit = false;
        boxCollider.enabled = true;

        float localScaleX = transform.localScale.x;
        if (Mathf.Sign(localScaleX) != _direction)
            localScaleX = -localScaleX;

        transform.localScale = new Vector3(localScaleX, transform.localScale.y, transform.localScale.z);
    }

    // Hàm này gọi từ animation event explode
    private void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
