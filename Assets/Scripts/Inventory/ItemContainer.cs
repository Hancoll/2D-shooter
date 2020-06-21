using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer : MonoBehaviour
{
    [SerializeField] Item item;
    [SerializeField] int itemCount;
    bool isActive = true;

    public void TakeItem()
    {
        if (isActive)
        {
            if (GameManager.InventorySystem.AddItem(item, itemCount))
                Destroy(gameObject);
        }
    }

    public void SetEnable()
    {
        isActive = false;
        StartCoroutine(IEnableTime());
    }

    IEnumerator IEnableTime()
    {
        yield return new WaitForSeconds(1.2f);
        isActive = true;
    }

    public static void ThrowItem(Item item, int itemCount, Vector2 senderPosition, bool isCharacter = false)
    {
        GameObject currentItem = Instantiate(item.ItemPrefab, senderPosition, Quaternion.identity,GameManager.GameSpace);
        currentItem.transform.localScale = Vector3.one;
        CircleCollider2D circleCollider = currentItem.AddComponent<CircleCollider2D>();
        circleCollider.isTrigger = true;
        circleCollider.radius = 0.2f;

        ItemContainer itemContainer = currentItem.AddComponent<ItemContainer>();
        itemContainer.item = item;
        itemContainer.itemCount = itemCount;
        itemContainer.SetEnable();
    }
}
