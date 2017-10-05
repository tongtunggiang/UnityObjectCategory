using System;
using UnityEngine;

[Serializable]
public class CategoryMask
{
    [SerializeField] private int value;

    public int Value { get { return value; } }
}