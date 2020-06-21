using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootContainer : MonoBehaviour
{
    public RandomItemInspector[] items;

    public void DropItems(Vector2 dropPosition)
    {
        int randomNumber = Random.Range(1, 100);

        for(int i = 0; i < items.Length; i++)
        {
            if(items[i].chance > 0 && items[i].chance <= randomNumber)
                ItemContainer.ThrowItem(items[i].item, items[i].itemCount, dropPosition);
        }
    }
}

[System.Serializable]
public class RandomItemInspector : ItemInspector
{
    [HideInInspector] public int chance;
}
