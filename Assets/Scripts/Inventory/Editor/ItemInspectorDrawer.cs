using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ItemInspector))]
public class ItemInspectorDrawer : PropertyDrawer
{  
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 20f * 2;
    }

    public override void OnGUI(Rect rect, SerializedProperty property, GUIContent label)
    {
        var itemProperty = property.FindPropertyRelative("item");
        var itemCountProperty = property.FindPropertyRelative("itemCount");

        EditorGUIUtility.wideMode = true;
        rect.height = rect.height / 2 - 2;

        rect.y += 4;
        EditorGUIUtility.labelWidth = 100;
        itemProperty.objectReferenceValue = (Item)EditorGUI.ObjectField(rect, "Item", itemProperty.objectReferenceValue, typeof(Item), false);

        if (itemProperty.objectReferenceValue != null)
        {
            Item item = itemProperty.objectReferenceValue as Item;

            rect.y += rect.height + 1;
            EditorGUIUtility.labelWidth = 140;

            if (item.IsStacked)
                itemCountProperty.intValue = EditorGUI.IntSlider(rect, " ", itemCountProperty.intValue, 1, item.MaxCountInStack);
            else
            {
                rect.x += 230;
                itemCountProperty.intValue = 1;
                EditorGUI.LabelField(rect, "1");
            }
        }

        else
            itemCountProperty.intValue = 0;
    }
}
