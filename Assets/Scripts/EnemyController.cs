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

    [Header("EXP & Heal Reward")]
    public int expReward = 20; // ค่าประสบการณ์ที่ให้เมื่อถูกฆ่า
    public int healAmount = 2; // จำนวนเลือดที่ฟื้นคืนให้ผู้เล่น

    private Transform player;
    private bool isRetreating = false;
    private Vector2 moveDir;

    private Rigidbody2D rb;
    private Animator animator;
    float moveX;
    float moveY;
    bool isMoving;

    public AudioSource audioSource;
    public AudioClip hurtP;

    public AudioClip dieSound;

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
                // เก็บทิศทางไว้ใน Update (ไม่ใช้ MovePosition ตรงนี้)
                moveDir = (player.position - transform.position).normalized;

                if (animator != null)
                {
                    animator.SetFloat("moveX", moveDir.x);
                    animator.SetFloat("moveY", moveDir.y);
                    animator.SetBool("isMoving", true);
                }
            }
            else
            {
                if (animator != null)
                {
                    animator.SetBool("isMoving", false);
                    if (moveDir != Vector2.zero)
                    {
                        animator.SetFloat("moveX", moveDir.x);
                        animator.SetFloat("moveY", moveDir.y);
                    }
                }

                moveDir = Vector2.zero; // หยุด
            }
        }
    }

    void FixedUpdate()
    {
        if (!isRetreating && moveDir != Vector2.zero)
        {
            rb.MovePosition(rb.position + moveDir * moveSpeed * Time.fixedDeltaTime);
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
                if (audioSource != null && hurtP != null)
                audioSource.PlayOneShot(hurtP);
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
            if (audioSource != null && dieSound != null)
                audioSource.PlayOneShot(dieSound);
            Die();
        }
    }

    void Die()
    {
        // แจ้ง Player ว่าได้รับ EXP
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.GainExp(expReward);

            // 💖 ฟื้นเลือดให้ผู้เล่นตามจำนวนที่ตั้งไว้
            player.Heal(healAmount);
        }

        // เล่นเสียงตาย (ถ้ามี)
        if (audioSource != null && dieSound != null)
            audioSource.PlayOneShot(dieSound);

        // ทำลายตัวเอง
        Destroy(gameObject);
    }
}
