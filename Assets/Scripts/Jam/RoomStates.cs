using UnityEngine;
using DG.Tweening;
using System.Collections;

public class RoomStates : MonoBehaviour
{
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private RoomPlayerController playerController;

    private void Start()
    {
        if (playerController != null)
        {
            playerController.enabled = false;
        }

        StartCoroutine(IntroSequence());
    }


    private IEnumerator IntroSequence()
    {
        if (fadeCanvas != null)
        {
            fadeCanvas.alpha = 1f;
            fadeCanvas.DOFade(0f, fadeDuration).SetDelay(0.2f).OnComplete(() =>
            {
                fadeCanvas.gameObject.SetActive(false);
            });
        }

        yield return new WaitForSeconds(2.4f);

        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
}

