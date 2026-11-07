using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class DialogueData : ScriptableObject
{
    [Header("Thông tin Dialogue")]
    [Tooltip("ID duy nhất của cuộc hội thoại")]
    public string dialogueID;

    [Header("Nội dung hội thoại")]
    public List<DialogueLine> lines = new List<DialogueLine>();
}

public enum DialogueDisplayMode
{
    NormalDialog,   // Trạng thái 1: Có khung, panel, avatar đầy đủ
    FloatingText    // Trạng thái 2: Chỉ có chữ chạy từng chữ, không nền
}

[System.Serializable]
public class DialogueLine
{
    [Header("Kiểu hiển thị")]
    public DialogueDisplayMode displayMode = DialogueDisplayMode.NormalDialog;

    [Header("Người nói (Chỉ dùng cho Normal Dialog)")]
    public string speakerName;

    [Header("Nội dung")]
    [TextArea(2, 5)]
    public string dialogueText;

    [Header("Ảnh nhân vật & Giao diện (Chỉ dùng cho Normal Dialog)")]
    [Tooltip("Ảnh nhân vật (Portrait)")]
    public Sprite characterPortrait;

    [Tooltip("Ảnh nền (Background)")]
    public Sprite backgroundSprite;

  

}