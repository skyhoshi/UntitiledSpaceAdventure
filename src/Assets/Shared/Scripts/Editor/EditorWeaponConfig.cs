using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer (typeof(Weapon))]
class IngredientDrawer : PropertyDrawer 
{

    public void EditorLayourProperty(string _property, string _toolTipText = "")
    {
        SerializedProperty prop = property.FindPropertyRelative(_property);
        GUIContent Tooltip = new GUIContent(prop.displayName, _toolTipText);

        EditorGUILayout.BeginHorizontal();
        //EditorGUILayout.LabelField(Tooltip);
        EditorGUILayout.PropertyField(prop, Tooltip);
        
        EditorGUILayout.EndHorizontal();
    }

    private SerializedProperty property;

    public override void OnGUI(Rect position, SerializedProperty _property, GUIContent label)
    {

        property = _property;
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        

        EditorLayourProperty("BulletPrefab", "Bullet that is spawned");
        EditorLayourProperty("MuzzleFlashPrefab", "Muzzle flash that is spawned when firing");
        EditorLayourProperty("PortPreference", "What order / sequence to fire from the fire ports");
        EditorLayourProperty("ShotType");

        SerializedProperty type = property.FindPropertyRelative("ShotType");
        if (type.enumNames[type.enumValueIndex] == "Burst")
        {
            EditorLayourProperty("ShotNo");
            EditorLayourProperty("ShotDelay");
        }
        EditorLayourProperty("Cooldown");
       
    }
}


[CustomEditor(typeof(WeaponConfig))]
public class EditorWeaponConfig : Editor
{   // TODO: This wasn't working because the fields need to be set to the return value of EditorGUILayout.

    //public override void OnInspectorGUI()
    //{
    //    GUILayout("Editor");
    //    //var weaponObj = target as WeaponConfig;

    //    //EditorGUILayout.LabelField("Weapon Name :" + weaponObj.mWeapon.Name);

    //    //EditorGUILayout.BeginVertical();
    //    //EditorGUILayout.ObjectField("Bullet Prefab", weaponObj.BulletPrefab, typeof(Transform), false);
    //    //EditorGUILayout.ObjectField("MuzzleFlash Prefab", weaponObj.MuzzleFlashPrefab, typeof(Transform), false);
    //    //EditorGUILayout.EndVertical();

    //    ////
    //    //EditorGUILayout.EnumPopup("Port Preferences", weaponObj.PortPreference);
    //    //EditorGUILayout.EnumPopup("Type", weaponObj.ShotType);
    //    //if (weaponObj.mWeapon.ShotType == ShotType.Burst)
    //    //{
    //    //    EditorGUILayout.BeginHorizontal();
    //    //    EditorGUILayout.IntField("Num Shots", weaponObj.ShotNo);
    //    //    EditorGUILayout.FloatField("Shot Delay", weaponObj.ShotDelay);
    //    //    EditorGUILayout.EndHorizontal();
    //    //}
    //    //EditorGUILayout.FloatField("Cooldowmn", weaponObj.Cooldown);
    //}
}
