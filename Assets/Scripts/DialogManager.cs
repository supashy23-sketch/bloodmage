using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject dialogBox;
    public Text dialogText;
    public GameObject choiceBox;
    public Transform choiceContainer;
    public Button choiceButtonPrefab;

    public int lettersPerSecond = 30;

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
    private bool showingChoices = false;

    public IEnumerator ShowDialog(Dialog dialog)
    {
        yield return new WaitForEndOfFrame();

        OnShowDialog?.Invoke();

        this.dialog = dialog;
        dialogBox.SetActive(true);
        choiceBox.SetActive(false);

        StartCoroutine(TypeDialog(dialog.lines[0]));
    }

    public void HandleUpdate()
    {
        if (showingChoices) return;

        if (Input.GetKeyUp(KeyCode.Z) && !isTyping)
        {
            currentLine++;
            if (currentLine < dialog.lines.Count)
                StartCoroutine(TypeDialog(dialog.lines[currentLine]));
            else
            {
                if (dialog.choices != null && dialog.choices.Count > 0)
                    ShowChoices();
                else
                    CloseDialog();
            }
        }
    }

    private void ShowChoices()
    {
        showingChoices = true;
        choiceBox.SetActive(true);

        // ลบปุ่มเก่า
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (var choice in dialog.choices)
        {
            // ตรวจสอบเงื่อนไข stat
            if (!string.IsNullOrEmpty(choice.requiredStat))
            {
                int current = GameStatsManager.Instance.GetStat(choice.requiredStat);
                if (current < choice.requiredValue) continue;
            }

            var button = Instantiate(choiceButtonPrefab, choiceContainer);
            button.GetComponentInChildren<Text>().text = choice.choiceText;

            button.onClick.AddListener(() =>
            {
                // ปรับ stat
                if (!string.IsNullOrEmpty(choice.statToChange))
                    GameStatsManager.Instance.ChangeStat(choice.statToChange, choice.changeAmount);

                // รีเซ็ต oneTimeDialog ของ NPC
                if (choice.resetOneTimeDialog && !string.IsNullOrEmpty(choice.targetNPCID))
                {
                    // เพิ่มค่า _oneTimeReset = 1 เฉพาะครั้งนี้
                    GameStatsManager.Instance.ChangeStat(choice.targetNPCID + "_oneTimeReset", 1);
                }
                // ไป Dialog ถัดไป
                if (choice.nextDialog != null)
                {
                    dialog = choice.nextDialog;
                    currentLine = 0;
                    choiceBox.SetActive(false);
                    showingChoices = false;
                    StartCoroutine(TypeDialog(dialog.lines[0]));
                }
                else
                {
                    CloseDialog();
                }
            });
        }
    }

    private void CloseDialog()
    {
        dialogBox.SetActive(false);
        currentLine = 0;
        showingChoices = false;
        OnHideDialog?.Invoke();
    }

    public IEnumerator TypeDialog(string line)
    {
        isTyping = true;
        dialogText.text = "";
        foreach (var letter in line.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / lettersPerSecond);
        }
        isTyping = false;
    }
}
