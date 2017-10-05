using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectCategory : MonoBehaviour
{
    [SerializeField]
    CategoryMask category;

    public int CategoriesValue { get { return category.Value; } }

    private void Awake()
    {
        GameObjectCategoryHelpers.RegisterGameObject(gameObject, this);
    }

    private void OnDestroy()
    {
        GameObjectCategoryHelpers.DeregisterGameObject(gameObject, this);
    }    
}

public static class GameObjectCategoryHelpers
{
    private static Dictionary<int, List<GameObject>> categoriesGameObjectDictionary = new Dictionary<int, List<GameObject>>();

    private static List<string> existingCategories
    {
        get
        {
            if (_existingCategories == null)
            {
                string[] serializedCategories = ((CategoryChoices)(ScriptableObject.Instantiate(Resources.Load("Category/Categories")))).GetCategories();

                _existingCategories = new List<string>();
                foreach (string category in serializedCategories)
                {
                    _existingCategories.Add(category);
                }
            }
            return _existingCategories;
        }
    }

    // Never to be used directly
    private static List<string> _existingCategories = null;

    private static int PowerOfTwo(int power)
    {
        return 1 << power; // returns the power 2^i;
    }

    /// <summary>
    /// Tell the category manager that there is a newly created gameObject that needs to
    /// keep track on its categories.
    /// </summary>
    public static void RegisterGameObject(GameObject gameObject, ObjectCategory categories)
    {
        for (int i = 0; i < existingCategories.Count; i++)
        {
            int categoryValue = PowerOfTwo(i);
            if ((categoryValue & categories.CategoriesValue) != 0)
            {
                if (!categoriesGameObjectDictionary.ContainsKey(categoryValue))
                {
                    categoriesGameObjectDictionary[categoryValue] = new List<GameObject>();
                }

                List<GameObject> objectsInCategory = categoriesGameObjectDictionary[categoryValue];
                objectsInCategory.Add(gameObject);

#if SHOW_CATEGORIES_DEBUG_INFO
                Debug.Log("Registered game object " + gameObject + " to category " + existingCategories[i]);
#endif
            }
        }
    }

    /// <summary>
    /// The manager can safely ignore of the gameObject's categories
    /// </summary>
    public static void DeregisterGameObject(GameObject gameObject, ObjectCategory categories)
    {
        for (int i = 0; i < existingCategories.Count; i++)
        {
            int categoryValue = PowerOfTwo(i);
            if ((categoryValue & categories.CategoriesValue) != 0)
            {
                if (!categoriesGameObjectDictionary.ContainsKey(categoryValue))
                    continue;

                categoriesGameObjectDictionary[categoryValue].Remove(gameObject);

#if SHOW_CATEGORIES_DEBUG_INFO
                Debug.Log("OnDestroy: Deregistered game object " + gameObject + " to category " + existingCategories[i]);
#endif
            }
        }
    }

    public static bool IsOfCategory(this GameObject gameObject, CategoryMask categories)
    {
        foreach (KeyValuePair<int, List<GameObject>> dictElement in categoriesGameObjectDictionary)
        {
            if ((dictElement.Key & categories.Value) != 0)
                return true;
        }

        return false;
    }

    public static bool IsOfCategory(this GameObject gameObject, string category)
    {
        int index = existingCategories.FindIndex(s => s == category);
        if (index == -1) // no such category
            return false;

        int categoryValue = PowerOfTwo(index);

        // There is no game object of this category being registered
        if (!categoriesGameObjectDictionary.ContainsKey(categoryValue))
            return false;

        if (categoriesGameObjectDictionary[categoryValue] == null)
            return false;

        if (categoriesGameObjectDictionary[categoryValue].Count <= 0)
            return false;

        return categoriesGameObjectDictionary[categoryValue].Contains(gameObject);
    }

    /// <summary>
    /// Get the first found game object of this category.
    /// </summary>
    /// <param name="category">The input category</param>
    /// <param name="includeInactiveGameObject">If set to true, the first non-null game object would be returned regardless of its active state</param>
    /// <returns>The first found game object.</returns>
    public static GameObject FindGameObjectOfCategory(string category, bool includeInactiveGameObject = false)
    {
        int index = existingCategories.FindIndex(s => s == category);
        if (index == -1) // no such category
            return null;

        int categoryValue = PowerOfTwo(index);
        if (!categoriesGameObjectDictionary.ContainsKey(categoryValue))
            return null;

        foreach (GameObject obj in categoriesGameObjectDictionary[categoryValue])
        {
            if (obj != null)
            {
                if (includeInactiveGameObject || obj.activeInHierarchy)
                    return obj;
            }
        }

        return null;
    }

    /// <summary>
    /// Get all game objects of this category.
    /// </summary>
    /// <param name="category">The input category</param>
    /// <param name="includeInactiveGameObject">If set to true, the first non-null game object would be returned regardless of its active state</param>
    /// <returns>The array containing non-null game objects.</returns>
    public static GameObject[] FindGameObjectsOfCategory(string category, bool includeInactiveGameObject = false)
    {
        int index = existingCategories.FindIndex(s => s == category);
        if (index == -1) // no such category
            return new GameObject[0];

        int categoryValue = PowerOfTwo(index);
        if (!categoriesGameObjectDictionary.ContainsKey(categoryValue))
            return new GameObject[0];

        List<GameObject> gameObjectsOfCategory = new List<GameObject>();
        foreach (GameObject obj in categoriesGameObjectDictionary[categoryValue])
        {
            if (obj != null)
            {
                if (includeInactiveGameObject || obj.activeInHierarchy)
                    gameObjectsOfCategory.Add(obj);
            }
        }
        return gameObjectsOfCategory.ToArray();
    }
}