using UnityEngine;

public class ItemScore : MonoBehaviour
{
    public int scoreValue = 10; // ตั้งค่าใน Inspector ได้

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่า Player เก็บไอเทม
        if (collision.CompareTag("Player"))
        {
            Debug.Log("ชน Player แล้ว + " + scoreValue + " คะแนน"); // ✅ ยืนยันว่าชนแล้ว
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddScore(scoreValue);
            }
            // ลบไอเทมออกจากฉาก
            Destroy(gameObject);
        }
    } 

}
