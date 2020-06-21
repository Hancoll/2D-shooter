using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum InventoryContainerType
{
    Inventory,
    ActiveInventory,//Инвентарь через который используются предметы
    Equipment,//Снаряжение 
    Storage,//К этому относится всё остаьное что может хранить в себе предметы: сундуки,коробки и т.д.
}

[System.Serializable]
public class ItemInspector
{
    public Item item;
    public int itemCount = 0;
}

public class ItemInventory
{
    public Item item;
    public int itemCount = 0;
    [HideInInspector] public InventoryContainer consumablesContainer;
    [HideInInspector] public int consumablesCellNumber;
    [HideInInspector] public InventoryContainer container;
    [HideInInspector] public int cellNumber;

    public void AddValue(int value)
    {
        container.AddValue(cellNumber, value);
    }

    public void ConsumablesAddValue(int value)
    {
        consumablesContainer.AddValue(consumablesCellNumber, value);
    }
}

public class InventoryContainer : MonoBehaviour
{
    public Transform containerTransform;
    public InventoryContainerType inventoryContainerType;
    public ItemInspector[] items;
    [HideInInspector] public InventoryCell[] inventoryCells;
    InventorySystem inventorySystem;

    private void Awake()
    {
        inventorySystem = GameManager.InventorySystem;
        CreateInventoryCells();
    }

    #region Cells

    /// <summary> Создаёт ячейки в инвентаре. </summary>
    void CreateInventoryCells()
    {
        inventoryCells = new InventoryCell[items.Length];

        for (int i = 0; i < items.Length; i++)
        {
            GameObject cell = Instantiate(inventorySystem.cellPrefab, containerTransform);
            InventoryCell inventoryCell;

            if (inventoryContainerType == InventoryContainerType.ActiveInventory)
                inventoryCell = cell.AddComponent<ActiveInventoryCell>();
            else if(inventoryContainerType == InventoryContainerType.Equipment)
            {
                inventoryCell = cell.AddComponent<EquipmentInventoryCell>();
                EquipmentInventoryCell equipmentCell = inventoryCell as EquipmentInventoryCell;

                switch (i)
                {
                    case 0:
                        equipmentCell.equipmentType = EquipmentType.Head;
                        break;

                    case 1:
                        equipmentCell.equipmentType = EquipmentType.Body;
                        break;

                    case 2:
                        equipmentCell.equipmentType = EquipmentType.Shoulders;
                        break;

                    case 3:
                        equipmentCell.equipmentType = EquipmentType.Belt;
                        break;

                    default:
                        equipmentCell.equipmentType = EquipmentType.Boosters;
                        break;
                }
            }

            else
                inventoryCell = cell.AddComponent<InventoryCell>();

            inventoryCell.CellInitialize(i, this, inventorySystem.OnSelectItem, inventorySystem.OnDeselectItem, inventorySystem.OnDragItem);
            inventoryCell.itemInventory.container = this;
            inventoryCells[i] = inventoryCell;

            if(items[i].item != null)
            {
                AddItem(items[i].item, items[i].itemCount,i);
                UpdateInventoryCell(i);
            }                    
        }
    }

    public void UpdateInventoryCells()
    {
        for (int i = 0; i < items.Length; i++)
        {
            UpdateInventoryCell(i);
        }
    }

    public void UpdateInventoryCell(int cellNumber)
    {
        if (inventoryCells[cellNumber].itemInventory.item != null)
        {
            inventoryCells[cellNumber].ChangeItemSprite(inventoryCells[cellNumber].itemInventory.item.ItemSprite);
            items[cellNumber].item = inventoryCells[cellNumber].itemInventory.item;
        }

        else
        {
            inventoryCells[cellNumber].ChangeItemSprite(null);
            items[cellNumber].item = null;
        }

        if (inventoryCells[cellNumber].itemInventory.itemCount > 0 && inventoryCells[cellNumber].itemInventory.item.IsStacked)
            inventoryCells[cellNumber].ChangeItemCountText(inventoryCells[cellNumber].itemInventory.itemCount);
        else
            inventoryCells[cellNumber].ChangeItemCountText();

        items[cellNumber].itemCount = inventoryCells[cellNumber].itemInventory.itemCount;
    }

    #endregion

    /// <summary> Добавить предмет в определённую ячейку инвентаря. </summary>
    public void AddItem(Item newItem, int count, int cellNumber)
    {
        inventoryCells[cellNumber].itemInventory.item = newItem;
        inventoryCells[cellNumber].itemInventory.itemCount = count;
        inventoryCells[cellNumber].itemInventory.container = this;
        inventoryCells[cellNumber].itemInventory.cellNumber = cellNumber;

        UpdateInventoryCell(cellNumber);
    }


    public bool FindBullets(out InventoryContainer bulletContainer,out int cellNumber)
    {
        for (int i = 0;i < items.Length; i++)
        {
            if(inventoryCells[i].itemInventory.item is ItemBullet)
            {
                bulletContainer = inventoryCells[i].itemInventory.container;
                cellNumber = inventoryCells[i].itemInventory.cellNumber;
                return true;
            }
        }

        bulletContainer = null;
        cellNumber = -1;
        return false;
    }


    public void AddValue(int cellNumber, int value)
    {
        ItemInventory itemInventory = inventoryCells[cellNumber].itemInventory;

        if (itemInventory.item.IsStacked)
        {
            if (itemInventory.itemCount + value <= 0)
            {
                itemInventory.item = null;
                itemInventory.itemCount = 0;
            }

            else if (itemInventory.itemCount + value <= itemInventory.item.MaxCountInStack)
                itemInventory.itemCount += value;

            else
            {
                int remains = (itemInventory.itemCount + value) - itemInventory.item.MaxCountInStack;
                itemInventory.itemCount = itemInventory.item.MaxCountInStack;
                GameManager.InventorySystem.AddItem(itemInventory.item, remains);
                //itemInventory.container.AddItem(itemInventory.item, remains);
            }

            itemInventory.container.UpdateInventoryCell(itemInventory.cellNumber);
        }

        else
        {
            itemInventory.item = null;
            itemInventory.itemCount = 0;
            itemInventory.container.UpdateInventoryCell(itemInventory.cellNumber);
        }
    } 
}