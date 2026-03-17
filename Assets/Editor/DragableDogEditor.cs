using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(DragableDog))]
public class DragableDogEditor : Editor
{
    SerializedProperty targetCameraProp;
    SerializedProperty logDragEveryFrameProp;
    SerializedProperty applyTranslationProp;
    SerializedProperty applyRotationProp;
    SerializedProperty rootBoneProp;
    SerializedProperty unDragableNodesProp;
    SerializedProperty boneConfigsProp;

    void OnEnable()
    {
        targetCameraProp = serializedObject.FindProperty("targetCamera");
        logDragEveryFrameProp = serializedObject.FindProperty("logDragEveryFrame");
        applyTranslationProp = serializedObject.FindProperty("applyTranslation");
        applyRotationProp = serializedObject.FindProperty("applyRotation");
        rootBoneProp = serializedObject.FindProperty("RootBone");
        unDragableNodesProp = serializedObject.FindProperty("UnDragableNodes");
        boneConfigsProp = serializedObject.FindProperty("BoneConfigs");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(targetCameraProp);
        EditorGUILayout.PropertyField(logDragEveryFrameProp);
        EditorGUILayout.PropertyField(applyTranslationProp);
        EditorGUILayout.PropertyField(applyRotationProp);
        EditorGUILayout.PropertyField(rootBoneProp);
        EditorGUILayout.PropertyField(unDragableNodesProp, true);

        DrawBoneConfigs();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawBoneConfigs()
    {
        if (boneConfigsProp == null)
            return;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Bone Configs", EditorStyles.boldLabel);

        for (int i = 0; i < boneConfigsProp.arraySize; i++)
        {
            var element = boneConfigsProp.GetArrayElementAtIndex(i);
            var boneProp = element.FindPropertyRelative("bone");
            var centerZRotProp = element.FindPropertyRelative("centerZRot");
            var zRotRangeProp = element.FindPropertyRelative("zRotRange");

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.BeginHorizontal();
            element.isExpanded = EditorGUILayout.Foldout(element.isExpanded, $"Element {i}", true);
            if (GUILayout.Button("Remove", GUILayout.Width(64)))
            {
                boneConfigsProp.DeleteArrayElementAtIndex(i);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.EndVertical();
                break;
            }
            EditorGUILayout.EndHorizontal();

            if (element.isExpanded)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(boneProp, new GUIContent("Bone"));
                if (EditorGUI.EndChangeCheck())
                {
                    var bone = boneProp.objectReferenceValue as Transform;
                    if (bone != null)
                        centerZRotProp.floatValue = bone.localEulerAngles.z;
                }

                EditorGUILayout.PropertyField(centerZRotProp, new GUIContent("Center Z Rot"));
                EditorGUILayout.PropertyField(zRotRangeProp, new GUIContent("Z Rot Range"));
            }

            EditorGUILayout.EndVertical();
        }

        if (GUILayout.Button("Add Bone Config"))
            boneConfigsProp.InsertArrayElementAtIndex(boneConfigsProp.arraySize);
    }
}
