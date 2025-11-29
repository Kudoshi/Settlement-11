using Kudoshi.Utilities;
using TMPro;
using UnityEngine;

public class PlayerFirefly : Singleton<PlayerFirefly>
{
    [SerializeField] private int _fireflyMax;
    [SerializeField] private TextMeshProUGUI _fireflyText;

    private int _fireflyCounter;

    public int FireflyCounter { get => _fireflyCounter; }

    private void Awake()
    {
        _fireflyCounter = 0;
    }

    public void AdjustFireflies(int firefly)
    {
        _fireflyCounter += firefly;

        Debug.Log("Firefly Counter: " + _fireflyCounter);
    }

    public void UseAllFireflies()
    {
        _fireflyCounter -= 50;
        Debug.Log("All fireflies used. Counter reset to 0.");
    }

    private void Update()
    {
        _fireflyText.text = _fireflyCounter.ToString() + " / " + _fireflyMax.ToString();
    }
}