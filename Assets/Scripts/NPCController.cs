using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [Header("NPC Settings")]
    public string npcID;               // unique ID ของ NPC
    public Dialog defaultDialog;       
    public Dialog oneTimeDialog;
    public bool repeatable = true;     // ถ้า false → คุยครั้งเดียว

    public void Interact()
    {
        bool hasTalked = GameStatsManager.Instance.GetStat(npcID + "_hasTalked") > 0;
    bool canPlayOneTime = GameStatsManager.Instance.GetStat(npcID + "_oneTimeReset") > 0;

    Dialog dialogToShow = defaultDialog;

    // เล่น oneTimeDialog เฉพาะถ้าได้รับการรีเซ็ตจาก Choice
    if (canPlayOneTime)
    {
        if (oneTimeDialog != null)
        {
            dialogToShow = oneTimeDialog;
            // รีเซ็ต flag หลังเล่น
            GameStatsManager.Instance.ChangeStat(npcID + "_oneTimeReset", -1);
        }
    }
    else if (!hasTalked)
    {
        // เล่น oneTimeDialog รอบแรก
        if (oneTimeDialog != null)
        {
            dialogToShow = oneTimeDialog;
            if (!repeatable)
                GameStatsManager.Instance.ChangeStat(npcID + "_hasTalked", 1);
        }
    }

    StartCoroutine(DialogManager.Instance.ShowDialog(dialogToShow));

    // กำหนด hasTalked ถ้า NPC ไม่ repeatable
    if (!repeatable)
        GameStatsManager.Instance.ChangeStat(npcID + "_hasTalked", 1);
    }

    // ฟังก์ชันเรียกจาก Choice เพื่อรีเซ็ต oneTimeDialog
    public void ResetOneTimeDialog()
    {
        GameStatsManager.Instance.ChangeStat(npcID + "_oneTimeReset", 1);
    }
}
