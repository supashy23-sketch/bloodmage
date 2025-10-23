using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 moveDir;
    private float moveSpeed;

    private int damage = 1;

    [Header("Projectile Settings")]
    public float lifeTime = 3f;
    public string[] destroyTags; // ใส่ Tag ของสิ่งที่ชนแล้วให้หาย

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 direction, float speed)
    {
        moveDir = direction.normalized;
        moveSpeed = speed;

        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
    }

    // ถ้า Collider ไม่เป็น Trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        CheckCollision(collision.gameObject);
    }

    // ถ้า Collider เป็น Trigger
    void OnTriggerEnter2D(Collider2D collision)
    {
        CheckCollision(collision.gameObject);
    }

    void CheckCollision(GameObject obj)
    {
        foreach (string tag in destroyTags)
        {
            if (obj.CompareTag(tag))
            {
                // ถ้าโดน Enemy ลด HP
                if (tag == "Enemy")
                {
                    EnemyController enemy = obj.GetComponent<EnemyController>();
                    if (enemy != null)
                        enemy.TakeDamage(damage);
                }

                Destroy(gameObject);
                break;
            }
        }
    }

    public void SetDamage(int value)
    {
        damage = value;
    }




}
