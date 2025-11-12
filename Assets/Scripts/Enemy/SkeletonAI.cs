using UnityEngine;

public class SkeletonAI : MonoBehaviour
{
    public float speed = 2f;
    public float attackRange = 1.5f;
    public float detectionRange = 6f;
    public int damage = 1;

    private Transform player;
    private Animator anim;
    private bool isAttacking = false;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < detectionRange && distance > attackRange)
        {
            // Di chuyển tới player
            MoveTowardsPlayer();
        }
        else if (distance <= attackRange)
        {
            // Tấn công
            Attack();
        }
        else
        {
            // Idle
            anim.Play("Idle_0");
        }
    }

    void MoveTowardsPlayer()
    {
        if (isAttacking) return;

        anim.Play("Walk"); // Tên animation di chuyển
        Vector2 target = new Vector2(player.position.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Lật hướng mặt theo Player
        if (player.position.x > transform.position.x)
            transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
        else
            transform.localScale = new Vector3(-Mathf.Abs(transform.localScale.x), transform.localScale.y, transform.localScale.z);
    }

    void Attack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            anim.Play("Attack"); // Tên animation chém

            // Gây sát thương (giả định)
            Debug.Log("Skeleton chém trúng Player!");

            Invoke(nameof(ResetAttack), 1f); // Delay tấn công
        }
    }

    void ResetAttack()
    {
        isAttacking = false;
    }
}

