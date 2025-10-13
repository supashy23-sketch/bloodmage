using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyPatrolTopDown : MonoBehaviour
{
    public Transform[] waypoints; // จุด waypoint
    public float speed = 2.0f;    // ความเร็ว
    public float waitTime = 2.0f; // เวลาหยุดรอแต่ละ waypoint

    private int currentWaypointIndex = 0;
    private bool isWaiting = false;

    public string sceneName;

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

          // ถึง waypoint แล้ว
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

      // เดินไป waypoint ถัดไป (วนลูป)
      currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ตรวจสอบว่า Player เก็บไอเทม
        if (collision.CompareTag("Player"))
        {
            SceneManager.LoadScene(sceneName);
        }
    }
    
}
