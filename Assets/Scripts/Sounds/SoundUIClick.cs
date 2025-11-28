using UnityEngine;

public class SoundUIClick : MonoBehaviour
{
    [SerializeField] private string SoundClickFx;

    public void UI_PlaySoundFx()
    {
        SoundManager.Instance.PlaySound(SoundClickFx);
    }
}
