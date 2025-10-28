using System.Collections;
using UnityEngine;

public class EnemySummoner : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Summon Settings")]
    public GameObject summonPrefab; // Prefab ที่จะถูกเสก
    public float summonInterval = 3f; // เวลาเว้นระหว่างการเสกแต่ละครั้ง
    public float summonRange = 5f; // ระยะที่ต้องให้ผู้เล่นอยู่ใกล้ถึงจะเริ่มเสก
    public Transform summonPoint; // จุดที่จะเสกออปเจกต์ (ถ้าไม่ใส่ จะเสกที่ตำแหน่งศัตรู)

    [Header("EXP & Heal Reward")]
    public int expReward = 20;
    public int healAmount = 2;

    [Header("Sound Effects")]
    public AudioSource audioSource;
    public AudioClip summonSound;
    public AudioClip dieSound;

    private Transform player;
    private bool isSummoning = false;

    void Awake()
    {
        currentHealth = maxHealth;
        player = GameObject.FindWithTag("Player").transform;
    }

    void Update()
    {
        if (player == null || currentHealth <= 0) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= summonRange)
        {
            if (!isSummoning)
                StartCoroutine(SummonRoutine());
        }
        else
        {
            // ถ้าออกนอกระยะ ให้หยุดการเสก
            if (isSummoning)
                StopAllCoroutines();

            isSummoning = false;
        }
    }

    IEnumerator SummonRoutine()
    {
        isSummoning = true;

        while (true)
        {
            // เล่นเสียง (ถ้ามี)
            if (audioSource != null && summonSound != null)
                audioSource.PlayOneShot(summonSound);

            // สร้างออปเจกต์ใหม่
            Vector3 spawnPos = summonPoint != null ? summonPoint.position : transform.position;
            Instantiate(summonPrefab, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(summonInterval);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (currentHealth <= 0)
            Die();
    }

    void Die()
    {
        if (audioSource != null && dieSound != null)
            audioSource.PlayOneShot(dieSound);

        // ให้ EXP และ Heal ผู้เล่น
        PlayerController playerCtrl = FindObjectOfType<PlayerController>();
        if (playerCtrl != null)
        {
            playerCtrl.GainExp(expReward);
            playerCtrl.Heal(healAmount);
        }

        Destroy(gameObject);
    }
}
