using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemRare
{
    White,//
    Green,//
    Blue,//
    Violet,//
    Red,//
    Gold,//
    Pearl,//
}

[CreateAssetMenu(fileName = "Item", menuName = "Create Item")]
[System.Serializable]
public class Item : ScriptableObject
{
    public string ItemName { get { return itemName; } }
    [SerializeField] protected string itemName;
    public Sprite ItemSprite { get { return itemSprite; } }
    [SerializeField] protected Sprite itemSprite;
    public GameObject ItemPrefab { get { return itemPrefab; } }
    [SerializeField] protected GameObject itemPrefab;//Объект который будет отображаться в руках при выборе ячейки с предметом.
    public ItemRare ItemRare { get { return itemRare; } }
    [SerializeField] protected ItemRare itemRare = ItemRare.White;

    public bool IsStacked { get { return isStacked; } }
    [SerializeField] protected bool isStacked;
    public int MaxCountInStack { get { return maxCountInStack; } }
    [SerializeField] protected int maxCountInStack = 1;

    public virtual bool Execute(ItemInventory item)
    {
        return false;
    }

    public virtual bool Execute()
    {
        return false;
    }
}
