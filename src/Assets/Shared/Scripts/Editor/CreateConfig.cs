using UnityEngine;
using System.Collections;
using UnityEditor;


public class CreateConfig : MonoBehaviour {

    [MenuItem("Assets/Create/HitConfigs/Default")]
    public static void CreateNewConfig()
    {
        ////HitConfigScriptable config = ScriptableObject.CreateInstance<HitConfigScriptable>();

        //AssetDatabase.CreateAsset(config, "Assets/NewHitConfig.asset");
        //AssetDatabase.SaveAssets();

        //EditorUtility.FocusProjectWindow();
        //Selection.activeObject = config;
    }

    [MenuItem("Assets/Create/HitConfigs/Armored")]
    public static void CreateNewArmoredConfig()
    {
        //HitConfigScriptable config = ScriptableObject.CreateInstance<HitConfigScriptable>();

        //AssetDatabase.CreateAsset(config, "Assets/NewHitConfig.asset");
        //AssetDatabase.SaveAssets();

        //EditorUtility.FocusProjectWindow();
        //Selection.activeObject = config;
    }
}
