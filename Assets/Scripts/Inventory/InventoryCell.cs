using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryCell : MonoBehaviour,IPointerDownHandler,IPointerUpHandler,IDragHandler
{
    public int cellNumber;
    public InventoryContainer inventoryContainer;
    Image itemIcoImage;
    Text itemCountText;

    public event System.Action<ItemInventory, Vector2, InventoryContainer , InventoryCell[],  int> onSelected;
    public System.Action<object, Vector2, InventoryContainer, InventoryCell[]> onDeselected;
    public System.Action<Vector2> onDrag;

    public ItemInventory itemInventory;

    bool isSelected = false;

    public void CellInitialize(int cellNumber, InventoryContainer inventoryContainer,
        System.Action<ItemInventory, Vector2, InventoryContainer, InventoryCell[], int> onSelected,
        System.Action<object, Vector2, InventoryContainer, InventoryCell[]> onDeselected,
        System.Action<Vector2> onDrag)
    {
        this.cellNumber = cellNumber;
        this.inventoryContainer = inventoryContainer;

        this.onSelected += onSelected;
        this.onDeselected += onDeselected;
        this.onDrag += onDrag;

        itemInventory = new ItemInventory();
        itemInventory.cellNumber = this.cellNumber;

        for (int i = 0; i < transform.childCount; i++)
        {
            if (itemIcoImage = transform.GetChild(i).gameObject?.GetComponent<Image>()) break;
        }

        itemCountText = transform.GetComponentInChildren<Text>();
    }

    public void ChangeItemSprite(Sprite newSprite)
    {
        if(newSprite != null)
        {
            itemIcoImage.sprite = newSprite;
            itemIcoImage.color = Color.white;
        }

        else
        {
            itemIcoImage.sprite = null;
            itemIcoImage.color = Color.clear;
        }
    }

    public void ChangeItemCountText(int count) => itemCountText.text = count.ToString();
    public void ChangeItemCountText() => itemCountText.text = "";

    #region OnPointer

    public virtual void OnPointerDown(PointerEventData ped)
    {
        if (itemInventory.item != null)
        {
            isSelected = true;
            onSelected.Invoke(itemInventory, ped.position, inventoryContainer, inventoryContainer.inventoryCells, cellNumber);
        }
    }

    public virtual void OnDrag(PointerEventData ped)
    {
        if(isSelected)
        {
            RectTransformUtility.ScreenPointToWorldPointInRectangle(GetComponent<RectTransform>(), ped.position, ped.pressEventCamera, out Vector3 position);
            onDrag.Invoke(position);
        }
    }

    public virtual void OnPointerUp(PointerEventData ped)
    {
        if(isSelected)
        {
            InventoryCell cell;
            if (ped?.pointerEnter?.GetComponentInParent<InventoryCell>())
            {
                cell = ped.pointerEnter.GetComponentInParent<InventoryCell>();
                onDeselected.Invoke(cell, ped.position, cell.inventoryContainer, cell.inventoryContainer.inventoryCells);
            }

            else if(ped?.pointerEnter?.GetComponent<RectTransform>())
                onDeselected.Invoke(ped?.pointerEnter, ped.position, new InventoryContainer(), new InventoryCell[0]);

            else
                onDeselected.Invoke(null, ped.position, new InventoryContainer(), new InventoryCell[0]);

            isSelected = false;
        }
    }

    #endregion
}


