using System;
using Extensions;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(DoubleRangeAttribute))]
public class DoubleRangeAttributeDrawer : PropertyDrawer
{
    private float nameCharWidth = 9f;
    private Vector2 fieldRangeWidth = new Vector2(40f, 100f);
    private float fieldWidthPercent = .1f;
    private float spacing = 10f;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        DoubleRangeAttribute range = attribute as DoubleRangeAttribute;

        float nameWidth = property.displayName.Length * nameCharWidth;
        float valueWidth = position.width - nameWidth - spacing;
        float valueStartPosition = position.position.x + nameWidth + spacing;
        float fieldWidth = Mathf.Clamp(valueWidth * fieldWidthPercent, fieldRangeWidth.x, fieldRangeWidth.y);
        
        Rect rect_Name = new Rect(position.position, new Vector2(nameWidth, position.height));
        
        Rect rect_minValue = new Rect(new Vector2(valueStartPosition, position.position.y), new Vector2(fieldWidth, position.height));
        
        Rect rect_Slider = new Rect(new Vector2(valueStartPosition + fieldWidth + spacing, position.position.y), 
            new Vector2(valueWidth - 2 * (fieldWidth + spacing), position.height));
        Rect rect_maxValue = new Rect(new Vector2(rect_Slider.position.x + rect_Slider.width + spacing, position.position.y),
            new Vector2(fieldWidth, position.height));
        
        GUI.Label(rect_Name, property.displayName);

        float min;
        float max;
        switch (property.propertyType)
        {
            case SerializedPropertyType.Vector2:
                min = property.vector2Value.x;
                max = property.vector2Value.y;
                
                EditorGUI.MinMaxSlider(rect_Slider, ref min, ref max, range.Min, range.Max);
                min = EditorGUI.FloatField(rect_minValue, min);
                max = EditorGUI.FloatField(rect_maxValue, max);
                property.vector2Value = new Vector2(min, max);
                break;
            case SerializedPropertyType.Vector2Int:
                min = property.vector2IntValue.x;
                max = property.vector2IntValue.y;
                
                EditorGUI.MinMaxSlider(rect_Slider, ref min, ref max, range.Min, range.Max);
                min = EditorGUI.FloatField(rect_minValue, min);
                max = EditorGUI.FloatField(rect_maxValue, max);
                property.vector2IntValue = new Vector2Int(Convert.ToInt32(min), Convert.ToInt32(max));
                break;
            default:
                EditorGUI.LabelField(position, label.text, "Use Range only with Vector2 or Vector2Int types.");
                return;
        }
    }
}
