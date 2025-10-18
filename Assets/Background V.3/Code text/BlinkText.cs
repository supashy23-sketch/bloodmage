using UnityEngine;
using UnityEngine.UI;

public class BlinkText : MonoBehaviour
{
    public Text targetText; // ลาก Text UI มาวางใน Inspector
    public float blinkSpeed = 1f; // ความเร็วในการกระพริบ (1 = ช้า, 5 = เร็ว)

    void Update()
    {
        if (targetText != null)
        {
            // เปลี่ยนความโปร่งใสตามเวลา (sin wave)
            Color c = targetText.color;
            c.a = Mathf.Abs(Mathf.Sin(Time.time * blinkSpeed));
            targetText.color = c;
        }
    }
}
