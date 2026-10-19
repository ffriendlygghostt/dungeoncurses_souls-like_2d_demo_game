// Assets/Editor/PrefabPainterWindow.cs
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

public class PrefabPainterWindow : EditorWindow
{
    [SerializeField] private GameObject prefab;

    private static bool painting;
    private static GameObject ghost;
    private static SpriteRenderer ghostSR;
    private static GameObject cachedPrefab;
    private static bool flipX;
    private static Vector3 lastGhostPosition;


    /*──────── MENU ────────*/
    [MenuItem("Tools/Prefab Painter (F2)")]
    public static void Open() => GetWindow<PrefabPainterWindow>("Prefab Painter");

    [Shortcut("Prefab Painter/Toggle Paint", KeyCode.F2)]
    private static void ShortcutToggle() => TogglePainting();

    /*──────── GUI ─────────*/
    private void OnGUI()
    {
        GUILayout.Label("Prefab Painter", EditorStyles.boldLabel);
        prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

        GUI.enabled = prefab != null;
        if (GUILayout.Button(painting ? "Stop (Esc / F2)" : "Start Painting (F2)"))
            TogglePainting();
        GUI.enabled = true;
    }

    /*── TOGGLE ──*/
    private static void TogglePainting()
    {
        painting = !painting;

        if (painting)
        {
            SceneView.duringSceneGui += OnSceneGUI;
            CreateGhost();
            UpdateGhost(Vector3.zero, GetWindow<PrefabPainterWindow>().prefab);
        }
        else
        {
            SceneView.duringSceneGui -= OnSceneGUI;
            DestroyGhost();
        }
        SceneView.RepaintAll();
    }

    /*── SCENE GUI ──*/
    private static void OnSceneGUI(SceneView sv)
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        var e = Event.current;
        if (e == null) return;

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        { TogglePainting(); e.Use(); return; }

        PrefabPainterWindow window = GetWindow<PrefabPainterWindow>();
        if (window.prefab == null) return;

        /* --- КЛАВИШИ ФЛИПА --- */
        if (e.type == EventType.KeyDown)
        {
            bool flipLeft = e.keyCode == KeyCode.Comma;          // ,
            bool flipRight = e.keyCode == KeyCode.Period;         // .

            if (flipLeft || flipRight)
            {
                flipX = flipLeft;
                UpdateGhost(ghost ? ghost.transform.position : Vector3.zero, window.prefab);
                e.Use();
            }
        }

        /* --- ПОЗИЦИЯ КУРСОРА --- */
        if (e.type == EventType.MouseMove)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 pos = ray.origin + ray.direction * (-ray.origin.z / ray.direction.z);
            pos.z = 0;

            if (ghost == null || (ghost.transform.position - pos).sqrMagnitude > 0.0001f)
                UpdateGhost(pos, window.prefab);
        }

        /* --- СОЗДАНИЕ ПРЕФАБА --- */
        if (e.type == EventType.MouseDown && e.button == 0 && !e.alt && !e.control)
        {
            GameObject go = (GameObject)PrefabUtility.InstantiatePrefab(window.prefab);
            Undo.RegisterCreatedObjectUndo(go, "Paint Prefab");
            go.transform.position = ghost.transform.position;

            Vector3 s = go.transform.localScale;
            s.x = flipX ? -Mathf.Abs(s.x) : Mathf.Abs(s.x);
            go.transform.localScale = s;

            Selection.activeObject = null;
            GUIUtility.keyboardControl = 0;
            EditorGUI.FocusTextInControl(null);
            e.Use();

        }

        if (e.type == EventType.MouseMove)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
            Vector3 pos = ray.origin + ray.direction * (-ray.origin.z / ray.direction.z);
            pos.z = 0;

            // Удерживаем Shift для движения только по X
            if (e.shift && ghost != null)
            {
                pos.y = lastGhostPosition.y; // фиксируем Y
            }

            if (ghost == null || (ghost.transform.position - pos).sqrMagnitude > 0.0001f)
            {
                UpdateGhost(pos, window.prefab);
                lastGhostPosition = ghost.transform.position;
            }
        }


        /* Блокируем стандартные контролы только на MouseMove / MouseDown */
        if (e.type == EventType.MouseMove || e.type == EventType.MouseDown)
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
    }


    /*── GHOST ──*/
    private static void CreateGhost()
    {
        DestroyGhost();
        ghost = new GameObject("Ghost") { hideFlags = HideFlags.HideAndDontSave };
        ghostSR = ghost.AddComponent<SpriteRenderer>();
        ghostSR.color = new Color(1, 1, 1, 0.4f);
    }

    private static void UpdateGhost(Vector3 pos, GameObject currentPrefab)
    {
        if (ghost == null) CreateGhost();
        ghost.transform.position = pos;

        // scale‑флип
        Vector3 sc = ghost.transform.localScale;
        sc.x = flipX ? -Mathf.Abs(sc.x) : Mathf.Abs(sc.x);
        ghost.transform.localScale = sc;

        if (currentPrefab == null) return;

        if (cachedPrefab != currentPrefab)
        {
            cachedPrefab = currentPrefab;
            if (cachedPrefab.TryGetComponent(out SpriteRenderer pr))
            {
                ghostSR.sprite = pr.sprite;
                ghostSR.sortingLayerID = pr.sortingLayerID;
                ghostSR.sortingOrder = pr.sortingOrder;
            }
            else ghostSR.sprite = null;
        }
    }


    private static void DestroyGhost()
    {
        if (ghost != null) DestroyImmediate(ghost);
        ghost = null; ghostSR = null; cachedPrefab = null; flipX = false;
    }

    /*── автоподхват префаба из Project ──*/
    private void OnSelectionChange()
    {
        var obj = Selection.activeObject as GameObject;
        if (obj == null) return;

        var type = PrefabUtility.GetPrefabAssetType(obj);
        bool isPrefab = type == PrefabAssetType.Regular || type == PrefabAssetType.Variant;

        if (isPrefab)
        {
            prefab = obj;
            Repaint();
            if (painting)
                UpdateGhost(ghost ? ghost.transform.position : Vector3.zero, prefab);
        }
    }

    // ───── Shortcut Flip ─────────
    [Shortcut("Prefab Painter/Flip Left", KeyCode.Comma)]
    private static void FlipLeft() { if (!painting) return; flipX = true; RefreshGhost(); }

    [Shortcut("Prefab Painter/Flip Right", KeyCode.Period)]
    private static void FlipRight() { if (!painting) return; flipX = false; RefreshGhost(); }

    private static void RefreshGhost()
    {
        UpdateGhost(ghost ? ghost.transform.position : Vector3.zero,
                    GetWindow<PrefabPainterWindow>().prefab);
    }




}
#endif