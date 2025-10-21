using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float gridSize = 1f;   // ขนาดของช่อง Grid (Tile Size)

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

    }

    public void HandleUpdate()
    {
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
                    StartCoroutine(Move(targetPos));
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
            if (footstepSource != null && !footstepSource.isPlaying)
            {
                footstepSource.clip = footstepSound;
                footstepSource.loop = true;
                footstepSource.Play();
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

    IEnumerator Move(Vector3 targetPos)
    {
        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 🔥 Snap to Grid ให้ตรงกับ Tile
        transform.position = new Vector3(
            Mathf.Round(transform.position.x / gridSize) * gridSize,
            Mathf.Round(transform.position.y / gridSize) * gridSize,
            transform.position.z
        );

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

    

}
