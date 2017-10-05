using UnityEngine;

public class CategoryChoices : ScriptableObject
{
    [SerializeField]
    string[] categories;

    public string[] GetCategories()
    {
        return categories;
    }
}
