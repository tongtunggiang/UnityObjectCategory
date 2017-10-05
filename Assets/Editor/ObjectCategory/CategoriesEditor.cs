
using UnityEditor;
using UnityEngine;

public class CategoriesEditor
{
    public const string CATEGORY_PATH = "Assets/Resources/Category/Categories.asset";
    const string MENU_PATH = "GameObject/Categories/";

    [MenuItem(MENU_PATH + "View Current Categories")]
    public static void ViewCurrentCategories()
    {
        CategoryChoices categories = AssetDatabase.LoadAssetAtPath(CATEGORY_PATH, typeof(CategoryChoices)) as CategoryChoices;
        EditorUtility.FocusProjectWindow();

        Selection.activeObject = categories;
    }

    [MenuItem(MENU_PATH + "New Categories Set")]
    public static void CreateNewCagoriesSet()
    {
        CategoryChoices categories = ScriptableObject.CreateInstance<CategoryChoices>();

        AssetDatabase.CreateAsset(categories, CATEGORY_PATH);
        AssetDatabase.SaveAssets();


        EditorUtility.FocusProjectWindow();

        Selection.activeObject = categories;
    }
}