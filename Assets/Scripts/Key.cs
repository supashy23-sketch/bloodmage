using UnityEngine;

public class Key : MonoBehaviour
{
    [Header("Door(s) that this key unlocks")]
    public Door[] linkedDoors; // ประตูที่กุญแจนี้ผูกอยู่

    [Header("FX")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // แจ้งประตูทั้งหมดว่ากุญแจนี้ถูกเก็บแล้ว
            foreach (Door door in linkedDoors)
            {
                if (door != null)
                    door.OnKeyCollected(this);
            }

            // เอฟเฟกต์ตอนเก็บ
            if (pickupEffect != null)
                Instantiate(pickupEffect, transform.position, Quaternion.identity);

            if (pickupSound != null)
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject); // ลบกุญแจออก
        }
    }
}
