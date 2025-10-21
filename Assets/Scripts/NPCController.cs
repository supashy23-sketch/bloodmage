using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, Interactable
{
    [SerializeField] Dialog dialog;
    public void Interact()
    {
        if (!DialogManager.Instance.IsDialogActive) // ป้องกันเปิดซ้อน
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }
}
