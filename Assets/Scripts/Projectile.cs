using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Vector2 moveDir;
    private float moveSpeed;

    [Header("Projectile Settings")]
    public float lifeTime = 3f; // เวลาอยู่สูงสุดก่อนลบตัวเอง (วินาที)
    public LayerMask destroyOnLayer; // Layer ที่ชนแล้วให้ทำลาย เช่น SolidObject, Enemy

    void Start()
    {
        // 🔥 กันเสกทิ้งไว้เยอะเกินไปจนหน่วง
        Destroy(gameObject, lifeTime);
    }

    public void SetDirection(Vector2 direction, float speed)
    {
        moveDir = direction.normalized;
        moveSpeed = speed;
    }

    void Update()
    {
        // เคลื่อนที่ในทิศทางที่ตั้งไว้
        transform.Translate(moveDir * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // 🔍 ตรวจว่าชน Layer ที่เราต้องการให้หายหรือไม่
        if (((1 << collision.gameObject.layer) & destroyOnLayer) != 0)
        {
            Destroy(gameObject);
        }
    }
}
