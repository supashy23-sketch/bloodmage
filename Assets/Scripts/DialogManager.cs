using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [SerializeField] GameObject dialogBox;
    [SerializeField] Text dialogText;
    [SerializeField] int lettersPerSecond;

    public event Action OnShowDialog;
    public event Action OnHideDialog;
    public static DialogManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private Dialog dialog;
    private int currentLine = 0;
    private bool isTyping;
    private bool isDialogActive; // ใช้ตรวจสอบสถานะของบทสนทนา
    public bool IsDialogActive => isDialogActive;

    // แสดงบทสนทนา
    public IEnumerator ShowDialog(Dialog dialog)
    {
        if (isDialogActive) yield break; // ถ้าบทสนทนายังทำงานอยู่ ให้หยุดฟังก์ชันนี้

        isDialogActive = true; // ตั้งสถานะเป็นกำลังมีบทสนทนา
        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        currentLine = 0;

        yield return StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
    }

    // จัดการการอัปเดตการกดปุ่ม
    public void HandleUpdate()
    {
        if (Input.GetKeyUp(KeyCode.E) && !isTyping && isDialogActive)
        {
            currentLine++;
            if (currentLine < dialog.Lines.Count)
            {
                StartCoroutine(TypeDialog(dialog.Lines[currentLine]));
            }
            else
            {
                dialogBox.SetActive(false);
                isDialogActive = false; // ตั้งสถานะบทสนทนาเสร็จสิ้น
                OnHideDialog?.Invoke();
            }
        }
    }

    // พิมพ์ข้อความทีละตัวอักษร
    private IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / Mathf.Max(1, lettersPerSecond));
        }
        isTyping = false;
    }
}