using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float gridSize = 1f;   // ขนาดของช่อง Grid (Tile Size)

    private bool isMoving;
    private Vector2 input;
    private Vector2 lastDir;   // เก็บทิศทางล่าสุด

    private Animator animator;

    public LayerMask solidObjectsLayer;
    public LayerMask interactableLayer;
    public LayerMask battleLayer;

    public GameObject projectilePrefab;  // 🔥 Drag prefab มาวางใน Inspector
    public float projectileSpeed = 10f;

    private void Awake()
    {
        animator = GetComponent<Animator>();
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
        if (animator != null)
        {
            animator.SetBool("isMoving", true); 
        }
        //animator.SetBool("isMoving", isMoving);

        if (Input.GetKeyDown(KeyCode.Z))
            Interact();

        // คลิกขวาเพื่อเสก projectile
        if (Input.GetMouseButtonDown(1))
        SpawnProjectile();
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

        // 🔥 Snap to Grid ให้ตรงกับ Tile Size
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
        if (Physics2D.OverlapCircle(targetPos, 0.2f, solidObjectsLayer | interactableLayer) != null)
        {
            return false;
        }
        return true;
    }

    void SpawnProjectile()
    {
        if (projectilePrefab == null) return;

        Vector3 spawnPos = transform.position + new Vector3(lastDir.x, lastDir.y, 0) * 0.5f;

        // สร้างออปเจค
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);

        // ส่งทิศทางให้ projectile
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.SetDirection(lastDir, projectileSpeed);
        }
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
}
