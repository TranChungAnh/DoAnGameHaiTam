
using System;
using System.Collections;
using UnityEngine;

public class MemoryGlitchTransition : MonoBehaviour
{
    [Header("Glitch UI")]
    [SerializeField] private GameObject glitchPanel;
    [SerializeField] private Animator glitchAnimator;

    private bool isPlaying;

    public IEnumerator PlayTransition(Action onComplete = null)
    {
        if (isPlaying)
            yield break;

        if (glitchPanel == null || glitchAnimator == null)
        {
            Debug.LogError(
                "MemoryGlitchTransition: Chưa gán GlitchPanel hoặc GlitchAnimator!"
            );

            onComplete?.Invoke();
            yield break;
        }

        isPlaying = true;

        // Hiện màn hình nhiễu
        glitchPanel.SetActive(true);
        glitchAnimator.enabled = true;

        // Reset Animator để phát lại từ đầu
        glitchAnimator.Rebind();
        glitchAnimator.Update(0f);

        // Đợi Animator bắt đầu cập nhật
        yield return null;
        yield return null;

        // Chờ animation glitch chạy hết
        while (glitchAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }

        // Tắt màn hình nhiễu
        glitchPanel.SetActive(false);

        isPlaying = false;

        // Tiếp tục hội thoại hoặc kết thúc hội thoại
        onComplete?.Invoke();
    }
}
