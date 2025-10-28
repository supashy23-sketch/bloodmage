using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrolTopDown : MonoBehaviour
{
    public Transform[] waypoints;
    public float speed = 2.0f;
    public float waitTime = 2.0f;
    public int damageToPlayer = 1; // 💥 ดาเมจที่ทำกับผู้เล่น

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;
    
    public AudioSource audioSource;
    public AudioClip hurtP;


    void Start()
    {
        if (waypoints.Length > 0)
        {
            StartCoroutine(Patrol());
        }
    }

    IEnumerator Patrol()
    {
        while (true)
        {
            if (!isWaiting)
            {
                MoveTowardsWaypoint();

                if (Vector3.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
                {
                    StartCoroutine(WaitAtWaypoint());
                }
            }
            yield return null;
        }
    }

    void MoveTowardsWaypoint()
    {
        if (waypoints.Length == 0) return;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, step);
    }

    IEnumerator WaitAtWaypoint()
    {
        isWaiting = true;
        yield return new WaitForSeconds(waitTime);
        isWaiting = false;
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }

    // 💥 ตรวจจับการชนกับ Player
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // อย่าลืมตั้ง Tag ให้ Player ด้วย
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                player.TakeDamage(damageToPlayer);
                if (audioSource != null && hurtP != null)
                audioSource.PlayOneShot(hurtP);
            }
        }
    }
}
