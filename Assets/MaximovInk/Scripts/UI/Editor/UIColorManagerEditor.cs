using UnityEditor;
using MaximovInk;
using System.Linq;
using UnityEngine;
using MaximovInk.UI;

[CustomEditor(typeof(UIColorManager))]
public class UIColorManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var ui = (UIColorManager)target;

        if (ui.scheme == null)
            return;
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical(EditorStyles.helpBox);

        EditorGUILayout.LabelField("colors:" + ui.scheme.colors.Count.ToString());

        for (int i = 0; i < ui.scheme.colors.Count; i++)
        {
            var elem = ui.scheme.colors[i];
            var newC = EditorGUILayout.ColorField(elem.Key, elem.Color);
            var key = elem.Key;

            ui.scheme.colors[i] = new StyleC() { Key = key, Color = newC };
        }
        ui.tint = EditorGUILayout.ColorField("Tint", ui.tint);

        if (GUILayout.Button("Apply"))
        {
            EditorUtility.SetDirty(ui.scheme);
            AssetDatabase.SaveAssets();
        }

        EditorGUILayout.EndVertical();

        if (EditorGUI.EndChangeCheck())
            ui.UpdateChange();
    }

    private static GameObject InstantiateFromPrefab(string resourcesPath, MenuCommand menuCommand)
    {
        GameObject go = PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(resourcesPath)) as GameObject;
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
        return go;
    }

    [MenuItem("GameObject/MUI/Panel", false, 10)]
    public static void CreatePanel(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MPanel.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Input", false, 10)]
    public static void CreateInput(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MInput.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Window", false, 10)]
    public static void CreateWindow(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MWindow.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Text", false, 10)]
    public static void CreateText(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MText.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Button", false, 10)]
    public static void CreateButton(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MButton.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Toggle", false, 10)]
    public static void CreateToggle(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MToggle.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Slider", false, 10)]
    public static void CreateSlider(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MSlider.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Scrollbar", false, 10)]
    public static void CreateScrollbar(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MScrollbar.prefab", menuCommand);
    }

    [MenuItem("GameObject/MUI/Scrollrect", false, 10)]
    public static void CreateScrollrect(MenuCommand menuCommand)
    {
        InstantiateFromPrefab("Assets/MaximovInk/UI/MScrollrect.prefab", menuCommand);
    }
}