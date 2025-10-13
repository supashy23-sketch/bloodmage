using UnityEngine;

public class WarningSign : MonoBehaviour
{
    [Header("เวลาที่ป้ายอยู่บนฉาก (วินาที)")]
    public float lifeTime = 3f;

    private void OnEnable()
    {
        // เริ่มจับเวลาเมื่อป้ายถูกแอกทีฟ
        Invoke("DestroySelf", lifeTime);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
