using UnityEngine;
using UnityEngine.UI;

public class CrossFace : MonoBehaviour
{
    [Header("Animation Frames")]
    public Texture[] frames;
    public float frameRate = 10f; // Frames per second

    [Header("Color Tint")]
    public Color tintColor = Color.white;

    private RawImage rawImage;
    private int currentFrame = 0;
    private float timer = 0f;

    private void Start()
    {
        rawImage = GetComponent<RawImage>();

        if (rawImage == null)
        {
            Debug.LogError("CrossFace: No RawImage component found!");
            enabled = false;
            return;
        }

        if (frames == null || frames.Length == 0)
        {
            Debug.LogWarning("CrossFace: No frames assigned!");
            enabled = false;
            return;
        }

        // Set initial frame and color
        rawImage.texture = frames[0];
        rawImage.color = tintColor;
    }

    private void Update()
    {
        if (frames.Length == 0) return;

        // Update color tint
        rawImage.color = tintColor;

        // Cycle through frames
        timer += Time.deltaTime;

        if (timer >= 1f / frameRate)
        {
            timer = 0f;
            currentFrame = (currentFrame + 1) % frames.Length;
            rawImage.texture = frames[currentFrame];
        }
    }
}
