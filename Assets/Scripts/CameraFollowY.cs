using UnityEngine;

public class CameraFollowY : MonoBehaviour
{
    public Transform target;   // ผู้เล่น (Player) ที่จะให้กล้องตาม
    public float smoothSpeed = 0.125f; // ความนุ่มนวลเวลาเลื่อนกล้อง
    public float offsetY = 0f; // ระยะเยื้องจากผู้เล่น (ขึ้น/ลง)

    void LateUpdate()
    {
        if (target != null)
        {
            // ตำแหน่งกล้องปัจจุบัน
            Vector3 currentPos = transform.position;

            // ตำแหน่งที่กล้องควรจะอยู่ (ตามผู้เล่นเฉพาะแกน Y)
            Vector3 desiredPos = new Vector3(currentPos.x, target.position.y + offsetY, currentPos.z);

            // ทำให้เลื่อนอย่างนุ่มนวล
            Vector3 smoothedPos = Vector3.Lerp(currentPos, desiredPos, smoothSpeed);

            transform.position = smoothedPos;
        }
    }
}
