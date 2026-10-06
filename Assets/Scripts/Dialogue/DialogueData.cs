    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "New Dialogue",menuName = "Dialogue/Dialogue Data"
    )]
    public class DialogueData : ScriptableObject
    {
        [Header("Thông tin Dialogue")]
        [Tooltip("ID duy nhất của cuộc hội thoại")]
        public string dialogueID;

        [Header("Nội dung hội thoại")]
        public List<DialogueLine> lines = new List<DialogueLine>();
    }

    [System.Serializable]
    public class DialogueLine
    {
        [Header("Người nói")]
        public string speakerName;

        [Header("Nội dung")]
        [TextArea(2, 5)]
        public string dialogueText;

        [Header("Ảnh nhân vật")]
        public Sprite characterPortrait;
    }