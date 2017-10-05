using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CategoryMask))]
public class CategoryMaskDrawer : PropertyDrawer
{
    string[] categories;

    void LoadCategories()
    {
        CategoryChoices serializedCategories = AssetDatabase.LoadAssetAtPath(CategoriesEditor.CATEGORY_PATH, typeof(CategoryChoices)) as CategoryChoices;
        categories = serializedCategories.GetCategories();
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        LoadCategories();

        EditorGUI.BeginProperty(position, label, property);

        // Draw label
        position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Create a dropdown mask values
        SerializedProperty maskValue = property.FindPropertyRelative("value");
        int currentMask = maskValue.intValue;

        int chosenMask = EditorGUI.MaskField(position, currentMask, categories);
        maskValue.intValue = chosenMask;

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
}