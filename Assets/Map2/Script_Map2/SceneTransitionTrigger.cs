
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransitionTrigger : MonoBehaviour
{
    [Header("Scene đích")]
    [SerializeField] private string destinationScene = "Map2_RungVong_TramNghienCuu";

    private bool isTransitioning;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isTransitioning)
            return;

        if (!other.CompareTag("Player"))
            return;

        isTransitioning = true;
        SceneManager.LoadScene(destinationScene);
    }
}
