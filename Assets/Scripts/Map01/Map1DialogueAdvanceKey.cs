
using UnityEngine;
using UnityEngine.InputSystem;

public class Map1DialogueAdvanceKey : MonoBehaviour
{
    private bool wasTalking;

    private void Update()
    {
        DialogueManager manager = DialogueManager.Instance;

        if (manager == null || !manager.IsTalking)
        {
            wasTalking = false;
            return;
        }

        // Bo qua frame dau khi hoi thoai vua mo.
        // Tranh bam F mot lan ma nhay qua 2 cau.
        if (!wasTalking)
        {
            wasTalking = true;
            return;
        }

        if (Keyboard.current != null &&
            Keyboard.current.fKey.wasPressedThisFrame)
        {
            manager.NextLine();
        }
    }
}
