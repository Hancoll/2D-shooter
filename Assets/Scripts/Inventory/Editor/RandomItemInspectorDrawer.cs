using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(RandomItemInspector))]
public class RandomItemInspectorDrawer : ItemInspectorDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 30 * 3;
    }
    
    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        Rect InspectorRect = rect;
        InspectorRect.y += 20;
        InspectorRect.height = 45;

        base.OnGUI(InspectorRect, property, label);

        var itemProperty = property.FindPropertyRelative("item");
        var chanceValue = property.FindPropertyRelative("chance");

        if (itemProperty.objectReferenceValue != null)
        {
            Rect ChanceRect = InspectorRect;
            ChanceRect.height = 20;
            ChanceRect.y += InspectorRect.height + 5;

            EditorGUIUtility.labelWidth = 100;
            chanceValue.intValue = EditorGUI.IntSlider(ChanceRect,"Drop chance", chanceValue.intValue, 1, 100);
        }

        else
            chanceValue.intValue = 0;
    }
}
