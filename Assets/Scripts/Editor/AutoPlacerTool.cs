using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class AutoPlacerTool : EditorWindow
{
    private GameObject groundZone;
    private GameObject prefab;
    private int count = 10;
    private float radius = 10f;
    private float heightOffset = 0f;
    private bool useRandomRotation = true;
    private bool useRandomScale = false;
    private Vector2 scaleRange = new Vector2(0.8f, 1.2f);
    private List<GameObject> previewObjects = new List<GameObject>();
    private bool isPlacing = false;

    [MenuItem("Tools/Auto Placer")]
    public static void ShowWindow()
    {
        GetWindow<AutoPlacerTool>("Auto Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Placer Tool", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        groundZone = (GameObject)EditorGUILayout.ObjectField("Ground Zone", groundZone, typeof(GameObject), true);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        EditorGUILayout.Space();

        count = EditorGUILayout.IntSlider("Count", count, 1, 500);
        radius = EditorGUILayout.Slider("Radius", radius, 1f, 100f);
        heightOffset = EditorGUILayout.Slider("Height Offset", heightOffset, -10f, 10f);

        EditorGUILayout.Space();

        useRandomRotation = EditorGUILayout.Toggle("Random Rotation", useRandomRotation);
        useRandomScale = EditorGUILayout.Toggle("Random Scale", useRandomScale);

        if (useRandomScale)
        {
            EditorGUILayout.MinMaxSlider("Scale Range", ref scaleRange.x, ref scaleRange.y, 0.1f, 3f);
            EditorGUILayout.LabelField($"Scale: {scaleRange.x:F2} - {scaleRange.y:F2}");
        }

        EditorGUILayout.Space();

        if (EditorGUI.EndChangeCheck())
        {
            if (isPlacing)
                UpdatePreview();
        }

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button(isPlacing ? "Update Preview" : "Start Preview"))
        {
            if (!isPlacing)
            {
                isPlacing = true;
                CreatePreview();
            }
            else
            {
                UpdatePreview();
            }
        }

        if (GUILayout.Button("Clear Preview"))
        {
            ClearPreview();
            isPlacing = false;
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        if (GUILayout.Button("Place Objects", GUILayout.Height(40)))
        {
            PlaceObjects();
        }

        if (GUILayout.Button("Randomize Positions"))
        {
            if (isPlacing)
                UpdatePreview();
        }
    }

    private void CreatePreview()
    {
        if (prefab == null || groundZone == null) return;

        ClearPreview();

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position = randomPos;
            obj.transform.SetParent(groundZone.transform);
            obj.hideFlags = HideFlags.HideAndDontSave;

            if (useRandomRotation)
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            if (useRandomScale)
            {
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                obj.transform.localScale = Vector3.one * scale;
            }

            previewObjects.Add(obj);
        }

        SceneView.RepaintAll();
    }

    private void UpdatePreview()
    {
        ClearPreview();
        CreatePreview();
    }

    private void ClearPreview()
    {
        foreach (GameObject obj in previewObjects)
        {
            if (obj != null)
                DestroyImmediate(obj);
        }
        previewObjects.Clear();
        SceneView.RepaintAll();
    }

    private Vector3 GetRandomPosition()
    {
        if (groundZone == null) return Vector3.zero;

        Vector3 center = groundZone.transform.position;
        Vector2 randomCircle = Random.insideUnitCircle * radius;
        Vector3 randomPos = center + new Vector3(randomCircle.x, heightOffset, randomCircle.y);

        RaycastHit hit;
        if (Physics.Raycast(randomPos + Vector3.up * 100f, Vector3.down, out hit, 200f))
        {
            return hit.point + Vector3.up * heightOffset;
        }

        return randomPos;
    }

    private void PlaceObjects()
    {
        if (prefab == null || groundZone == null)
        {
            EditorUtility.DisplayDialog("Error", "Please assign both Ground Zone and Prefab", "OK");
            return;
        }

        ClearPreview();
        isPlacing = false;

        GameObject parent = new GameObject($"{prefab.name}_Group");
        parent.transform.position = groundZone.transform.position;
        Undo.RegisterCreatedObjectUndo(parent, "Place Objects");

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = GetRandomPosition();
            GameObject obj = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            obj.transform.position = randomPos;
            obj.transform.SetParent(parent.transform);

            if (useRandomRotation)
                obj.transform.rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            if (useRandomScale)
            {
                float scale = Random.Range(scaleRange.x, scaleRange.y);
                obj.transform.localScale = Vector3.one * scale;
            }

            Undo.RegisterCreatedObjectUndo(obj, "Place Objects");
        }

        Selection.activeGameObject = parent;
    }

    private void OnDestroy()
    {
        ClearPreview();
    }

    private void OnEnable()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    private void OnDisable()
    {
        ClearPreview();
        SceneView.duringSceneGui -= OnSceneGUI;
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        if (isPlacing && groundZone != null)
        {
            Handles.color = new Color(0, 1, 0, 0.2f);
            Handles.DrawSolidDisc(groundZone.transform.position, Vector3.up, radius);
            Handles.color = Color.green;
            Handles.DrawWireDisc(groundZone.transform.position, Vector3.up, radius);
        }
    }
}
