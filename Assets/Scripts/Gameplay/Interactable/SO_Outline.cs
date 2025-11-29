
using UnityEngine;

[CreateAssetMenu(fileName = "SO_Outline", menuName = "Scriptable Objects/SO_OutlineSetting")]
public class SO_Outline : ScriptableObject
{
    public OutlineSetting GlobalOutlineSetting;
}

[System.Serializable]
public class OutlineSetting
{
    public Outline.Mode OutlineMode = Outline.Mode.OutlineAll;
    public Color OutlineColor = Color.yellow;
    [Range(0.01f, 10.0f)]
    public float OutlineWidth = 10.0f;
}