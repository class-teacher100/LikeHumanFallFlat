using UnityEditor;
using UnityEngine;

public static class SetupHelper
{
    [MenuItem("Game/Setup Layers")]
    static void SetupLayers()
    {
        var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
        var layers = tagManager.FindProperty("layers");

        SetLayer(layers, 6, "Ground");
        SetLayer(layers, 7, "Character");
        SetLayer(layers, 8, "Grabbable");

        tagManager.ApplyModifiedProperties();

        // Disable Character-Character collisions
        Physics.IgnoreLayerCollision(7, 7, true);

        Debug.Log("Layers configured: Ground(6), Character(7), Grabbable(8)");
    }

    static void SetLayer(SerializedProperty layers, int index, string name)
    {
        var layer = layers.GetArrayElementAtIndex(index);
        if (string.IsNullOrEmpty(layer.stringValue))
            layer.stringValue = name;
    }

    [MenuItem("Game/Create Character Definition Asset")]
    static void CreateCharacterDefinition()
    {
        var asset = ScriptableObject.CreateInstance<CharacterDefinition>();
        AssetDatabase.CreateAsset(asset, "Assets/Settings/DefaultCharacterDefinition.asset");
        AssetDatabase.SaveAssets();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
        Debug.Log("Created DefaultCharacterDefinition.asset in Assets/Settings/");
    }

    [MenuItem("Game/Setup Test Scene")]
    static void SetupTestScene()
    {
        // Ensure layers exist first
        SetupLayers();

        // Find or create character definition
        var defGuids = AssetDatabase.FindAssets("t:CharacterDefinition");
        CharacterDefinition def;
        if (defGuids.Length > 0)
        {
            def = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(AssetDatabase.GUIDToAssetPath(defGuids[0]));
        }
        else
        {
            CreateCharacterDefinition();
            defGuids = AssetDatabase.FindAssets("t:CharacterDefinition");
            def = AssetDatabase.LoadAssetAtPath<CharacterDefinition>(AssetDatabase.GUIDToAssetPath(defGuids[0]));
        }

        // Create TestSceneBuilder in scene
        var builderObj = new GameObject("TestSceneBuilder");
        var builder = builderObj.AddComponent<TestSceneBuilder>();

        var field = typeof(TestSceneBuilder).GetField("_characterDefinition",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (field != null)
            field.SetValue(builder, def);

        Debug.Log("TestSceneBuilder added to scene. Press Play to build the test environment.");
    }
}
