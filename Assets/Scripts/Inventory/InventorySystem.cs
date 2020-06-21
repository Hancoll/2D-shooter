using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static bool inventoryIsOpen;
    public GameObject cellPrefab;
    InventoryContainer activeInventoryContainer;
    InventoryContainer inventoryContainer;
    InventoryController inventoryController;
    GameObject itemNamePanel;

    public RectTransform selectedInventoryItemTransform;//Иконка при перетаскивании предмета
    public int selectedCellNumber = -1;//Ячейка с которой взаимодействует игрок при перетаскивании
    public ItemInventory selectedInventoryItem;//Данные о предмете который перетаскивают

    InventoryCell[] OnSelectdCells;
    InventoryContainer OnSelectedContainer;
    InventoryCell[] OnDeselectedCells;
    InventoryContainer OnDeselectedContainer;

    [SerializeField] Color WhiteItemColor;
    [SerializeField] Color GreenItemColor;
    [SerializeField] Color BlueItemColor;
    [SerializeField] Color VioletItemColor;
    [SerializeField] Color RedItemColor;
    [SerializeField] Color GoldItemColor;
    [SerializeField] Color PearlItemColor;

    private void Start()
    {

        inventoryContainer = GameManager.InventoryContainer;
        activeInventoryContainer = GameManager.ActiveInventoryContainer;
        inventoryController = GameManager.InventoryController;
        itemNamePanel = GameManager.ItemNamePanel;
        inventoryController.onChangeItem += SetItemName;
    }

    /// <summary> Добавить предмет в инвентарь. </summary>
    public bool AddItem(Item newItem, int count)
    {
        for (int i = 0; i < inventoryContainer.items.Length; i++)
        {
            if (inventoryContainer.inventoryCells[i].itemInventory.item == null)
            {
                inventoryContainer.AddItem(newItem, count, i);
                return true;
            }

            else if (inventoryContainer.inventoryCells[i].itemInventory.item == newItem &&
                    inventoryContainer.inventoryCells[i].itemInventory.itemCount < inventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack)
            {
                if (inventoryContainer.inventoryCells[i].itemInventory.itemCount + count <= inventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack)
                {
                    inventoryContainer.inventoryCells[i].itemInventory.itemCount += count;
                }

                else if (inventoryContainer.inventoryCells[i].itemInventory.itemCount + count > inventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack)
                {
                    int remains = (inventoryContainer.inventoryCells[i].itemInventory.itemCount + count) - inventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack;
                    inventoryContainer.inventoryCells[i].itemInventory.itemCount = inventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack;
                    AddItem(newItem, remains);
                }

                inventoryContainer.UpdateInventoryCell(i);
                return true;
            }
        }

        for (int i = 0; i < activeInventoryContainer.items.Length; i++)
        {
            if (activeInventoryContainer.inventoryCells[i].itemInventory.item == null)
            {
                activeInventoryContainer.AddItem(newItem, count, i);
                return true;
            }

            else if (activeInventoryContainer.inventoryCells[i].itemInventory.item == newItem &&
                    activeInventoryContainer.inventoryCells[i].itemInventory.itemCount < activeInventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack)
            {
                if (activeInventoryContainer.inventoryCells[i].itemInventory.itemCount + count <= activeInventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack)
                {
                    activeInventoryContainer.inventoryCells[i].itemInventory.itemCount += count;
                }

                else if (activeInventoryContainer.inventoryCells[i].itemInventory.itemCount + count > activeInventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack)
                {
                    int remains = (activeInventoryContainer.inventoryCells[i].itemInventory.itemCount + count) - activeInventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack;
                    activeInventoryContainer.inventoryCells[i].itemInventory.itemCount = activeInventoryContainer.inventoryCells[i].itemInventory.item.MaxCountInStack;
                    AddItem(newItem, remains);
                }

                activeInventoryContainer.UpdateInventoryCell(i);
                return true;
            }
        }

        return false;
    }

    void SetItemName(Item item)
    {
        if(item != null)
        {
            ItemRare itemRare = item.ItemRare;
            Color textColor;

            switch (itemRare)
            {
                case ItemRare.Green:
                    textColor = GreenItemColor;
                    break;

                case ItemRare.Blue:
                    textColor = BlueItemColor;
                    break;

                case ItemRare.Violet:
                    textColor = VioletItemColor;
                    break;

                case ItemRare.Red:
                    textColor = RedItemColor;
                    break;

                case ItemRare.Gold:
                    textColor = GoldItemColor;
                    break;

                case ItemRare.Pearl:
                    textColor = PearlItemColor;
                    break;

                default:
                    textColor = WhiteItemColor;
                    break;
            }

            itemNamePanel.GetComponent<Text>().text = item.ItemName;
            itemNamePanel.GetComponent<Text>().color = textColor;
            itemNamePanel.GetComponent<Animator>().SetTrigger("onChangeItem");
        }
    }

    public void OnSelectItem(ItemInventory itemInventory, Vector2 mousePosition,InventoryContainer inventoryContainer, InventoryCell[] inventoryCells, int cellNumber)
    {
        OnDeselectedCells = inventoryCells;
        OnSelectedContainer = inventoryContainer;

        selectedCellNumber = cellNumber;
        selectedInventoryItem = itemInventory;
        OnDeselectedCells[cellNumber].itemInventory = new ItemInventory();
        OnSelectedContainer.UpdateInventoryCell(cellNumber);

        selectedInventoryItemTransform.gameObject.SetActive(true);
        selectedInventoryItemTransform.GetComponent<Image>().sprite = selectedInventoryItem.item.ItemSprite;
        OnDragItem(mousePosition);

        if (inventoryContainer == activeInventoryContainer && inventoryController.currentItemIndex == itemInventory.cellNumber)
            inventoryController.SwitchCurrentItem(inventoryController.currentItemIndex);
    }

    public void OnDragItem(Vector2 mousePosition)
    {
        Vector3 pos = mousePosition + new Vector2(-14, 14);
        selectedInventoryItemTransform.position = (Vector2)pos;
        selectedInventoryItemTransform.transform.localPosition =
            new Vector3(selectedInventoryItemTransform.transform.localPosition.x,selectedInventoryItemTransform.transform.localPosition.y, 0);
    }

    public void OnDeselectItem(object deselectObject, Vector2 mousePosition, InventoryContainer inventoryContainer, InventoryCell[] inventoryCells)
    {
        InventoryCell inventoryCell;

        if (deselectObject is InventoryCell)
            inventoryCell = deselectObject as InventoryCell;
        else
            inventoryCell = null;


        if (inventoryCell != null && selectedInventoryItem.item is EquipmentItem && (inventoryCell is EquipmentInventoryCell || OnSelectedContainer.inventoryCells[selectedCellNumber] is EquipmentInventoryCell))
        {
            OnDeselectedContainer = inventoryContainer;
            EquipmentInventoryCell equipmentCell = inventoryCell as EquipmentInventoryCell;
            EquipmentItem equipmentItem = selectedInventoryItem.item as EquipmentItem;

            if (equipmentCell &&  equipmentCell.equipmentType == equipmentItem.equipmentType)
            {              
                if (inventoryCell.itemInventory.item == null)
                    OnDeselectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, inventoryCell.cellNumber);

                else if (inventoryCell.itemInventory.item != selectedInventoryItem.item)
                {
                    OnSelectedContainer.AddItem(inventoryCell.itemInventory.item, inventoryCell.itemInventory.itemCount, selectedCellNumber);
                    OnDeselectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, inventoryCell.cellNumber);
                }
            }

            else if (!equipmentCell && inventoryCell.itemInventory.item == null)
                OnDeselectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, inventoryCell.cellNumber);

            else
                OnSelectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, selectedCellNumber);
        }
                
        else if (inventoryCell != null && !(inventoryCell is EquipmentInventoryCell))
        {
            OnDeselectedContainer = inventoryContainer;

            if (inventoryCell.itemInventory.item == null)
                OnDeselectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, inventoryCell.cellNumber);

            else if (inventoryCell.itemInventory.item != selectedInventoryItem.item || !selectedInventoryItem.item.IsStacked || inventoryCell.itemInventory.itemCount == selectedInventoryItem.item.MaxCountInStack)
            {
                OnSelectedContainer.AddItem(inventoryCell.itemInventory.item, inventoryCell.itemInventory.itemCount, selectedCellNumber);
                OnDeselectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, inventoryCell.cellNumber);
            }

            else if (inventoryCell.itemInventory.item == selectedInventoryItem.item && selectedInventoryItem.item.IsStacked)
            {
                if (inventoryCell.itemInventory.itemCount + selectedInventoryItem.itemCount <= selectedInventoryItem.item.MaxCountInStack)
                    inventoryCell.itemInventory.itemCount += selectedInventoryItem.itemCount;

                else
                {
                    int remains = (inventoryCell.itemInventory.itemCount + selectedInventoryItem.itemCount) - selectedInventoryItem.item.MaxCountInStack;
                    inventoryCell.itemInventory.itemCount = selectedInventoryItem.item.MaxCountInStack;
                    OnSelectedContainer.AddItem(selectedInventoryItem.item, remains, selectedCellNumber);
                }

                OnDeselectedContainer.UpdateInventoryCell(inventoryCell.cellNumber);
            }
        }

        else if (deselectObject != null)
            OnSelectedContainer.AddItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, selectedCellNumber);

        else
            ItemContainer.ThrowItem(selectedInventoryItem.item, selectedInventoryItem.itemCount, GameManager.CharacterTransform.position);


        if (inventoryContainer == activeInventoryContainer && inventoryController.currentItemIndex == inventoryCell.cellNumber)
            inventoryController.SwitchCurrentItem(inventoryController.currentItemIndex);

        else if (OnSelectedContainer == activeInventoryContainer && inventoryController.currentItemIndex == selectedInventoryItem.cellNumber)
            inventoryController.SwitchCurrentItem(inventoryController.currentItemIndex);

        selectedInventoryItemTransform.gameObject.SetActive(false);

        selectedInventoryItem = new ItemInventory();
        selectedCellNumber = -1;
    }
}
