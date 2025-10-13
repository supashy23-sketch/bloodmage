using UnityEngine;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenuUI; // UI ของ Pause Menu
    private bool isPaused = false;

    void Start()
    {
        pauseMenuUI.SetActive(false); // ซ่อนเมนูตอนเริ่มเกม
    }

    // เรียกจากปุ่ม Pause (UI Button)
    public void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            Time.timeScale = 0f; // หยุดเวลาในเกม
            ScoreManager.Instance.StopTimer(); // หยุดตัวนับเวลา
            pauseMenuUI.SetActive(true); // โชว์เมนู
        }
    }

    // เรียกจากปุ่ม Resume (UI Button)
    public void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f; // เวลาเดินต่อ
            ScoreManager.Instance.StartTimer(); // นับเวลาต่อ
            pauseMenuUI.SetActive(false); // ซ่อนเมนู
        }
    }

    // เรียกจากปุ่มอีกอัน (UI Button) → Resume + ทำอย่างอื่น
    public void ResumeAndDoSomething()
    {
        ResumeGame(); // เล่นต่อก่อน
        // 🔹 เพิ่มโค้ดของคุณเองที่นี่ เช่น โหลดโฆษณา, บันทึกข้อมูล, ออกไปเมนู ฯลฯ
        Debug.Log("ทำฟังก์ชันพิเศษตรงนี้ได้เลย");
    }
}
