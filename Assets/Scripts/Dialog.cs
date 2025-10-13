using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogChoice
{
    public string choiceText;
    public Dialog nextDialog;
    public string statToChange;
    public int changeAmount;

    // เงื่อนไข Choice
    public string requiredStat;       // stat ที่ต้องมี
    public int requiredValue;         // ต้องมากกว่าหรือเท่ากับ

    // รีเซ็ต oneTimeDialog ของ NPC
    public bool resetOneTimeDialog = false; // รีเซ็ต Dialog พิเศษ
    public string targetNPCID; // NPC ที่จะรีเซ็ต

}
[CreateAssetMenu(menuName = "Dialog/Dialog Asset")]

public class Dialog : ScriptableObject
{
    [TextArea(2, 5)]
    public List<string> lines;           // ข้อความหลายบรรทัด

    public List<DialogChoice> choices;   // ตัวเลือก (อาจว่างได้ ถ้าเป็นบทพูดธรรมดา)

}
