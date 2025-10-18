using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonHoverScale : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Glow Background")]
    public GameObject glowBackground; // ใส่ Image Glow ที่อยู่หลังปุ่ม
    public float scaleUp = 1.2f;      // ขยายปุ่มตอน Hover
    public float speed = 10f;         // ความเร็วการขยาย
    public float fadeSpeed = 5f;      // ความเร็ว Fade In/Out

    private Vector3 originalScale;
    private bool isHovering = false;
    private CanvasGroup glowCanvasGroup;

    void Start()
    {
        originalScale = transform.localScale;

        if (glowBackground != null)
        {
            glowCanvasGroup = glowBackground.GetComponent<CanvasGroup>();
            if (glowCanvasGroup == null)
                glowCanvasGroup = glowBackground.AddComponent<CanvasGroup>();

            glowCanvasGroup.alpha = 0f; // ซ่อน Glow ตอนเริ่ม
            glowBackground.SetActive(true); // เปิดไว้ แต่โปร่งใส 0
        }
    }

    void Update()
    {
        // ขยาย/หดปุ่ม
        transform.localScale = Vector3.Lerp(transform.localScale,
            isHovering ? originalScale * scaleUp : originalScale,
            Time.deltaTime * speed);

        // Fade In/Out Glow
        if (glowCanvasGroup != null)
        {
            float targetAlpha = isHovering ? 1f : 0f;
            glowCanvasGroup.alpha = Mathf.Lerp(glowCanvasGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
    }
}
