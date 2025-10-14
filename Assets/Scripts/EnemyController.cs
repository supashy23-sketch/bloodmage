using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    private int currentHealth;
    public int attackPower = 1;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float chaseRange = 5f; // ระยะที่เริ่มตามผู้เล่น
    public float retreatTime = 1.5f; // เวลาเดินถอยหลังหลังชนผู้เล่น
    public float retreatDistance = 2f; // ระยะที่ถอยหลังแบบสุ่ม

    private Transform player;
    private bool isRetreating = false;
    private Vector2 moveDir;

    private Rigidbody2D rb;
    private Animator animator;

    void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        if (!isRetreating)
        {
            float distance = Vector2.Distance(transform.position, player.position);

            if (distance <= chaseRange)
            {
                // ตามผู้เล่น
                moveDir = (player.position - transform.position).normalized;
                rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);



                // อัปเดต animator
                if (animator != null)
                {
                    animator.SetFloat("moveX", moveDir.x);
                    animator.SetFloat("moveY", moveDir.y);
                    animator.SetBool("isMoving", true);
                }
            }
            else
            {
                if (animator != null) animator.SetBool("isMoving", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // ลด HP ผู้เล่น
            PlayerController playerCtrl = collision.gameObject.GetComponent<PlayerController>();
            if (playerCtrl != null)
            {
                playerCtrl.TakeDamage(attackPower);
            }

            // เริ่มถอยหลัง
            StartCoroutine(Retreat());
        }

        if (collision.gameObject.CompareTag("Projectile"))
        {
            Projectile projectile = collision.gameObject.GetComponent<Projectile>();
            if (projectile != null)
            {
                TakeDamage(1); // ลดเลือด 1 หน่วยเมื่อโดน projectile
                Destroy(collision.gameObject);
            }
        }
    }

    IEnumerator Retreat()
    {
        isRetreating = true;

        // สุ่มทิศทางถอยหลัง
        Vector2 retreatDir = Random.insideUnitCircle.normalized;
        float timer = 0f;

        while (timer < retreatTime)
        {
            rb.MovePosition(rb.position + retreatDir * moveSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        isRetreating = false;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        // ตัวอย่างทำลายตัวเอง
        Destroy(gameObject);
    }
}
