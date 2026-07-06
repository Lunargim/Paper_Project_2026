using UnityEditor;
using FMODUnity;

[CustomEditor(typeof(Audio3DEmitter))]
public class Audio3DEmitterEditor : StudioEventEmitterEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Audio3DEmitter - Occlusion", EditorStyles.boldLabel);

        serializedObject.Update();
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_useOcclusion"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_occlusionLayers"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_occlusionSmoothTime"));
        serializedObject.ApplyModifiedProperties();
    }
}
