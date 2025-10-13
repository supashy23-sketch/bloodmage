using UnityEngine;

public class SpawnBird : MonoBehaviour
{
    [Header("GameObject ที่จะเปิดใช้งานเมื่อ Player ชน")]
    public GameObject birdToActivate1; // กำหนดใน Inspector
    public GameObject birdToActivate2; // กำหนดใน Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log("Player ชนแล้ว -> เปิด Bird ทั้งสอง");

            if (birdToActivate1 != null)
            {
                birdToActivate1.SetActive(true);
            }

            if (birdToActivate2 != null)
            {
                birdToActivate2.SetActive(true);
            }

            // ทำลายตัวเองหลังจากชน (ถ้าไม่อยากลบออก ให้ลบคำสั่งนี้)
            Destroy(gameObject);
        }
    }
}
