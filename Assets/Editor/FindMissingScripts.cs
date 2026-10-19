using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Window/Find Missing Scripts")]
    public static void ShowWindow()
    {
        GetWindow<FindMissingScripts>("Find Missing Scripts");
    }

    void OnGUI()
    {
        if (GUILayout.Button("Find Missing Scripts in Scene"))
        {
            FindInCurrentScene();
        }
    }

    void FindInCurrentScene()
    {
        GameObject[] goArray = FindObjectsOfType<GameObject>();
        int missingCount = 0;

        foreach (GameObject go in goArray)
        {
            Component[] components = go.GetComponents<Component>();

            foreach (Component component in components)
            {
                if (component == null)
                {
                    Debug.LogWarning("Missing script found in object: " + go.name, go);
                    missingCount++;
                }
            }
        }

        Debug.Log("Total missing scripts: " + missingCount);
    }
}
