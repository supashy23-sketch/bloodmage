using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    [Header("ตั้งชื่อ Scene ที่ต้องการไป")]
    public string sceneName;

    [Header("คลิกปุ่มซ้ายเพื่อเปลี่ยนซีน")]
    public bool changeOnClick = true;

    private void OnMouseDown()
    {
        if (changeOnClick && !string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    // สำหรับเรียกใช้งานผ่านปุ่ม UI (Button) ใน Inspector
    public void ChangeScene()
    {
        if (!string.IsNullOrEmpty(sceneName))
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

