using UnityEngine;

public class CameraFollowXY : MonoBehaviour
{
    public Transform target;          // ผู้เล่น (Player)
    public float smoothSpeed = 0.125f; // ความนุ่มนวลเวลาเลื่อนกล้อง
    public Vector2 offset = Vector2.zero; // ระยะเยื้องจากผู้เล่น (แนวนอน/แนวตั้ง)

    void LateUpdate()
    {
        if (target != null)
        {
            // ตำแหน่งกล้องปัจจุบัน
            Vector3 currentPos = transform.position;

            // ตำแหน่งที่กล้องควรจะอยู่ (ตามผู้เล่นทั้ง X และ Y)
            Vector3 desiredPos = new Vector3(
                target.position.x + offset.x,
                target.position.y + offset.y,
                currentPos.z // ไม่เปลี่ยนระยะกล้อง (Z)
            );

            // ทำให้เลื่อนอย่างนุ่มนวล
            Vector3 smoothedPos = Vector3.Lerp(currentPos, desiredPos, smoothSpeed);

            transform.position = smoothedPos;
        }
    }
}
