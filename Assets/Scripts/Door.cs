using UnityEngine;

public class Door : MonoBehaviour
{
    [Header("Keys required to open")]
    public Key[] requiredKeys; // ต้องการกุญแจอะไรบ้าง

    [Header("Door Settings")]
    public bool isOpen = false;
    public AudioClip openSound;
    public GameObject openEffect; // เอฟเฟกต์ตอนประตูหาย (เช่น particle)

    private int keysCollected = 0;

    public void OnKeyCollected(Key key)
    {
        // ตรวจว่ากุญแจนี้เป็นส่วนหนึ่งของประตูนี้ไหม
        foreach (Key k in requiredKeys)
        {
            if (k == key)
            {
                keysCollected++;
                break;
            }
        }

        // ถ้าเก็บครบทุกดอกแล้ว → ประตูหาย
        if (keysCollected >= requiredKeys.Length)
        {
            OpenDoor();
        }
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        isOpen = true;

        Debug.Log($"Door {name} is now open!");

        // เอฟเฟกต์ตอนหาย
        if (openEffect != null)
            Instantiate(openEffect, transform.position, Quaternion.identity);

        if (openSound != null)
            AudioSource.PlayClipAtPoint(openSound, transform.position);

        // 🔥 ทำให้ประตูหายไปจากฉาก
        Destroy(gameObject);
    }
}
