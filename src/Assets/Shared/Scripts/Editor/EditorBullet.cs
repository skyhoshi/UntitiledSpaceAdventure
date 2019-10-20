using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(Bullet))]
public class EditorBullet : Editor
{
    public override void OnInspectorGUI()
    {
        var bullet = target as Bullet;
        GUIContent content = new GUIContent("Speed", "Projectile speed relative to firing object");

        //ditorGUILayout.FloatField("Speed",bullet.Speed);

        bullet.Speed = EditorGUILayout.FloatField(content, bullet.Speed);

        content.text = "Hit Effect Prefab";
        content.tooltip = "Effect that is spawned when bullet enters collision";
        bullet.BulletHitEffectPrefab = (Transform) EditorGUILayout.ObjectField(content, bullet.BulletHitEffectPrefab, typeof(Transform));

        content.text = "Destroy On Hit";
        content.tooltip = "When enters collision destroy this object";
        bullet.DestroyOnHit = EditorGUILayout.Toggle(content, bullet.DestroyOnHit);

        content.text = "Bullet Collision Settings";
        content.tooltip = "Allows you to set what layer the bullet is created in";
        bullet.BulletCollisionSetting = (BulletCollisionSettings) EditorGUILayout.EnumPopup(content, bullet.BulletCollisionSetting);

        if (bullet.BulletCollisionSetting == BulletCollisionSettings.UserDefinedLayer)
        {
            content.text = "Layer";
            content.tooltip = "Select the layer you want the bullet to spawn in";
            bullet.SpawnLayer.Set(EditorGUILayout.LayerField(content, bullet.SpawnLayer.LayerIndex));
        }

    }
}
