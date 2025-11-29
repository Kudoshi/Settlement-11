
using UnityEngine;

public class BoxInteractable : InteractableBase
{
    public override void Interact()
    {
        Debug.Log("Hi i am " + gameObject.name);
    }
}