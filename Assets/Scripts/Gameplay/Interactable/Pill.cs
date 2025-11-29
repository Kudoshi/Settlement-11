
using System;
using UnityEngine;

public class Pill : InteractableBase
{
    public override void Interact()
    {
        ConsumePill();
        Destroy(gameObject);
    }

    private void ConsumePill()
    {
        Debug.Log("Increase Sanity");
        SanityManager.Instance.IncreaseSanity(20f);
    }
}