using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ActiveInventoryCell : InventoryCell
{
    public override void OnPointerUp(PointerEventData ped)
    {
        if (InventorySystem.inventoryIsOpen)
            base.OnPointerUp(ped);
    }

    public override void OnPointerDown(PointerEventData ped)
    {
        if (InventorySystem.inventoryIsOpen)
            base.OnPointerDown(ped);

        else
            GameManager.InventoryController.SwitchCurrentItem(cellNumber);
    }

    public override void OnDrag(PointerEventData ped)
    {
        if (InventorySystem.inventoryIsOpen)
            base.OnDrag(ped);
    }
}
