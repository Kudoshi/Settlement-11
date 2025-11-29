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

    private bool useColliderArea = true;
    private Collider groundCollider;

    private enum AreaShape { Circle, Square }
    private AreaShape areaShape = AreaShape.Circle;
    private float squareSize = 10f;

    [MenuItem("Tools/Auto Placer")]
    public static void ShowWindow()
    {
        GetWindow<AutoPlacerTool>("Auto Placer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Auto Placer Tool", EditorStyles.boldLabel);

        EditorGUI.BeginChangeCheck();

        // Object references
        groundZone = (GameObject)EditorGUILayout.ObjectField("Ground Zone", groundZone, typeof(GameObject), true);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        // Read collider from groundZone
        groundCollider = (groundZone != null) ? groundZone.GetComponent<Collider>() : null;

        EditorGUILayout.Space();

        count = EditorGUILayout.IntSlider("Count", count, 1, 500);

        // NEW: collider option UI
        useColliderArea = EditorGUILayout.Toggle("Use Ground Zone Collider", useColliderArea);

        // If collider mode is enabled AND collider exists → auto area
        if (useColliderArea && groundCollider != null)
        {
            EditorGUILayout.HelpBox("Using Ground Zone Collider as placement area.", MessageType.Info);
        }
        else
        {
            // Manual shape selection (fallback)
            areaShape = (AreaShape)EditorGUILayout.EnumPopup("Placement Shape", areaShape);

            if (areaShape == AreaShape.Circle)
            {
                radius = EditorGUILayout.Slider("Radius", radius, 1f, 100f);
            }
            else
            {
                squareSize = EditorGUILayout.Slider("Square Size", squareSize, 1f, 100f);
            }
        }

        heightOffset = EditorGUILayout.Slider("Height Offset", heightOffset, -10f, 10f);

        EditorGUILayout.Space();

        // Rotation / scale options
        useRandomRotation = EditorGUILayout.Toggle("Random Rotation", useRandomRotation);
        useRandomScale = EditorGUILayout.Toggle("Random Scale", useRandomScale);

        if (useRandomScale)
        {
            EditorGUILayout.MinMaxSlider("Scale Range", ref scaleRange.x, ref scaleRange.y, 0.1f, 3f);
            EditorGUILayout.LabelField($"Scale: {scaleRange.x:F2} - {scaleRange.y:F2}");
        }

        EditorGUILayout.Space();

        // If any variable changed → update preview
        if (EditorGUI.EndChangeCheck())
        {
            if (isPlacing)
                UpdatePreview();
        }

        // Buttons row
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

        // Place objects
        if (GUILayout.Button("Place Objects", GUILayout.Height(40)))
        {
            PlaceObjects();
        }

        // Randomize preview positions
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

        // try detect collider
        if (useColliderArea && groundCollider != null)
        {
            // S P H E R E   C O L L I D E R
            if (groundCollider is SphereCollider sphere)
            {
                Vector3 center = sphere.transform.TransformPoint(sphere.center);
                float r = sphere.radius * Mathf.Max(
                    sphere.transform.lossyScale.x,
                    sphere.transform.lossyScale.y,
                    sphere.transform.lossyScale.z);

                Vector2 randomCircle = Random.insideUnitCircle * r;
                return RaycastWithOffset(center + new Vector3(randomCircle.x, 0, randomCircle.y));
            }

            // B O X   C O L L I D E R
            if (groundCollider is BoxCollider box)
            {
                Vector3 center = box.transform.TransformPoint(box.center);
                Vector3 size = Vector3.Scale(box.size, box.transform.lossyScale);

                float halfX = size.x * 0.5f;
                float halfZ = size.z * 0.5f;

                float x = Random.Range(-halfX, halfX);
                float z = Random.Range(-halfZ, halfZ);

                Vector3 worldPos = center + new Vector3(x, 0, z);
                return RaycastWithOffset(worldPos);
            }
        }

        // F A L L B A C K: manual shape
        Vector3 fallbackCenter = groundZone.transform.position;

        if (areaShape == AreaShape.Circle)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            return RaycastWithOffset(fallbackCenter + new Vector3(r.x, 0, r.y));
        }
        else
        {
            float half = squareSize / 2f;
            float x = Random.Range(-half, half);
            float z = Random.Range(-half, half);
            return RaycastWithOffset(fallbackCenter + new Vector3(x, 0, z));
        }
    }

    private Vector3 RaycastWithOffset(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up * 100f, Vector3.down, out RaycastHit hit, 200f))
            return hit.point + Vector3.up * heightOffset;

        return pos + Vector3.up * heightOffset;
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

            if (areaShape == AreaShape.Circle)
            {
                Handles.DrawSolidDisc(groundZone.transform.position, Vector3.up, radius);
                Handles.color = Color.green;
                Handles.DrawWireDisc(groundZone.transform.position, Vector3.up, radius);
            }
            else // SQUARE
            {
                float half = squareSize / 2f;
                Vector3 c = groundZone.transform.position;

                Vector3[] verts = new Vector3[]
                {
                c + new Vector3(-half, 0, -half),
                c + new Vector3(-half, 0, half),
                c + new Vector3(half, 0, half),
                c + new Vector3(half, 0, -half)
                };

                Handles.DrawSolidRectangleWithOutline(verts, new Color(0, 1, 0, 0.1f), Color.green);
            }
        }
    }

}
