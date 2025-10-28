using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float runMultiplier = 2f; // วิ่งเร็วขึ้น
    public float gridSize = 1f;

    private bool isMoving;
    private Vector2 input;
    private Vector2 lastDir;   // เก็บทิศทางล่าสุด

    private Animator animator;

    [Header("Collision Layers")]
    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask battleLayer;

    [Header("Projectile Settings")]
    public GameObject projectilePrefab;  // 🔥 Drag prefab มาวางใน Inspector
    public float projectileSpeed = 10f;
    public AudioSource audioSource;         // 🔊 แหล่งเล่นเสียง
    public AudioSource footstepSource;    // ✅ ใช้เฉพาะเสียงเดิน
    public AudioClip shootSound;            // 🔥 เสียงยิง
    public AudioClip footstepSound;      // เสียงเดิน

    [Header("EXP & Level System")]
    public int currentExp = 0;
    public int currentLevel = 1;
    public int[] expToNextLevel = { 0, 100, 200, 300 }; // แต่ละเลเวลต้องใช้เท่านี้
    public ExperienceUI expUI; // ผูกใน Inspector

    private int projectileDamage = 1; // ดาเมจของกระสุนตามเลเวล
   
    

    [Header("Health Settings")]
    public int maxHealth = 10;
    private int currentHealth;

    public HealthUI healthUI;

    [Header("Low Health Warning")]
    public AudioSource lowHealthAudio;
    public float lowHealthThreshold = 25f;
    public float flashSpeed = 2f;
    public CanvasGroup redOverlay; // ใช้ CanvasGroup แทน Image เพื่อปรับความโปร่งได้เนียนกว่า

    private bool isLowHealthEffectActive = false;
    public string sceneName;
  

    private void Awake()
    {
        animator = GetComponent<Animator>();

        currentHealth = maxHealth;

        if (healthUI != null)
            healthUI.SetMaxHealth(maxHealth);
        
        if (expUI != null)
        {
            expUI.SetMaxExp(expToNextLevel[currentLevel]);
            expUI.SetExp(currentExp);
            expUI.SetLevelText(currentLevel);
        }

    }

    public void HandleUpdate()
    {
        bool isRunning = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            // กันไม่ให้เดินทแยง
            if (input.x != 0) input.y = 0;

            if (input != Vector2.zero)
            {
                // อัปเดตทิศทางอนิเมชัน
                animator.SetFloat("moveX", input.x);
                animator.SetFloat("moveY", input.y);

                lastDir = input;

                // คำนวณตำแหน่งเป้าหมายทีละ Grid
                var targetPos = transform.position;
                targetPos.x += input.x * gridSize;
                targetPos.y += input.y * gridSize;

                if (IsWalkable(targetPos))
                {
                    StartCoroutine(Move(targetPos, isRunning));
                }
            }
        }

        // ปุ่มโต้ตอบ (เช่น พูดคุย, ตรวจของ)
        if (Input.GetKeyDown(KeyCode.E))
            Interact();

        // คลิกขวาเพื่อเสก projectile
        if (Input.GetMouseButtonDown(1))
            SpawnProjectile();

        // ✅ อัปเดตสถานะอนิเมชันให้ตรงกับการเคลื่อนไหวจริง
        animator.SetBool("isMoving", isMoving);
        // ✅ เสียงเดิน
        if (isMoving)
        {
            if (footstepSource != null)
            {
                if (!footstepSource.isPlaying)
                {
                    footstepSource.clip = footstepSound;
                    footstepSource.loop = true;
                    footstepSource.Play();
                }

                // 🎵 ปรับความเร็วเสียงตามความเร็ววิ่ง
                footstepSource.pitch = isRunning ? 2f : 1f;
            }
        }
        else
        {
            if (footstepSource != null && footstepSource.isPlaying)
                footstepSource.Stop();
        }
        

    }

    void Interact()
    {
        var facingDir = new Vector3(lastDir.x, lastDir.y);
        var interactPos = transform.position + facingDir * gridSize;

        var collider = Physics2D.OverlapCircle(interactPos, 0.2f, interactableLayer);
        if (collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
        }
    }

    IEnumerator Move(Vector3 targetPos, bool isRunning)
    {
        isMoving = true;

        // 🏃‍♂️ ปรับความเร็วตามสถานะวิ่ง
        float speed = isRunning ? moveSpeed * runMultiplier : moveSpeed;

        // 🎬 ปรับความเร็วแอนิเมชัน
        animator.speed = isRunning ? 2f : 1f;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
            yield return null;
        }

        // 🔥 Snap กลับให้ตรง Grid
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );

        // ❗ รีเซ็ต animator.speed กลับเป็นปกติ (กันแอนิเมชันอื่นเร็วผิดเวลา)
        animator.speed = 1f;
        isMoving = false;
        CheckForEncounters();
    }

    private bool IsWalkable(Vector3 targetPos)
    {
        // ถ้าตำแหน่งเป้าหมายชนสิ่งกีดขวางหรือวัตถุโต้ตอบ — เดินไม่ได้
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    void SpawnProjectile()
    {
        if (projectilePrefab == null) return;

        // 🔊 เล่นเสียงตอนยิง
        if (audioSource != null && shootSound != null)
        audioSource.PlayOneShot(shootSound);

        // จุดกำเนิดกระสุนอยู่ข้างหน้าผู้เล่นเล็กน้อย
        Vector3 spawnPos = transform.position + new Vector3(lastDir.x, lastDir.y, 0) * 0.5f;

        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // ส่งทิศทางให้ projectile
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.SetDirection(lastDir, projectileSpeed);
            p.SetDamage(projectileDamage);
        }

        TakeDamage(1);
    }

    private void CheckForEncounters()
    {
        if (Physics2D.OverlapCircle(transform.position, 0.2f, battleLayer) != null)
        {
            if (Random.Range(1, 101) <= 20)
            {
                Debug.Log("A battle has started!");
            }
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);

        if (currentHealth <= lowHealthThreshold && !isLowHealthEffectActive)
            StartCoroutine(LowHealthEffect());
        else if (currentHealth > lowHealthThreshold && isLowHealthEffectActive)
            StopLowHealthEffect();

        if (currentHealth <= 0)
        {
            SceneManager.LoadScene(sceneName);
            Debug.Log("Player is out of health!");
            // ใส่โค้ด Game Over หรือ disable movement ตรงนี้ได้
        }
    }

    IEnumerator LowHealthEffect()
    {
        isLowHealthEffectActive = true;

        // เล่นเสียงเตือนแบบลูป
        if (lowHealthAudio != null && !lowHealthAudio.isPlaying)
        {
            lowHealthAudio.loop = true;
            lowHealthAudio.Play();
        }

        float t = 0f;
        while (isLowHealthEffectActive)
        {
            t += Time.deltaTime * flashSpeed;
            float alpha = Mathf.PingPong(t, 1f) * 0.5f; // กระพริบ 0 → 0.5
            if (redOverlay != null)
                redOverlay.alpha = alpha;

            yield return null;
        }
    }

    void StopLowHealthEffect()
    {
        isLowHealthEffectActive = false;

        if (lowHealthAudio != null && lowHealthAudio.isPlaying)
            lowHealthAudio.Stop();

        if (redOverlay != null)
            redOverlay.alpha = 0f;
    }

    public void GainExp(int amount)
    {
        if (currentLevel >= 4) return; // ถึงเลเวลสูงสุดแล้ว

        currentExp += amount;
        if (expUI != null)
            expUI.SetExp(currentExp);

        // เช็คเลเวลอัพ
        if (currentExp >= expToNextLevel[currentLevel])
        {
            currentExp = 0;
            currentLevel++;
            LevelUp();
        }
    }

    void LevelUp()
    {
        // อัปเดต UI
        if (expUI != null)
        {
            if (currentLevel < expToNextLevel.Length)
                expUI.SetMaxExp(expToNextLevel[currentLevel]);
            expUI.SetLevelText(currentLevel);
            expUI.SetExp(currentExp);
        }

        // เพิ่มพลังโปรเจคไทล์ตามเลเวล
        projectileDamage = Mathf.Clamp(currentLevel, 1, 4);

        Debug.Log("Level Up! Now Level " + currentLevel + ", Damage = " + projectileDamage);
    }
    
    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        if (healthUI != null)
            healthUI.SetHealth(currentHealth);

        // ถ้าเลือดเกิน threshold ให้หยุดเอฟเฟกต์เตือน
        if (currentHealth > lowHealthThreshold && isLowHealthEffectActive)
            StopLowHealthEffect();

        Debug.Log($"Healed {amount} HP! Current HP: {currentHealth}");
    }

    

}
