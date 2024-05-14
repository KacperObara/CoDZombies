using H3MP.Scripts;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PlayerBody))]
public class PlayerBodyEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Verify"))
        {
            ((PlayerBody)target).Verify();
        }
    }
}
