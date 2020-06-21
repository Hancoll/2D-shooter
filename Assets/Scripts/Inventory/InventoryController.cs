using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum InputType
{
    Direction,
    NonDirection,
}

public class InventoryController : MobileController
{
    public int currentItemIndex;//Номер ячейки активного инвентаря
    InventoryContainer activeInventory;
    public Sprite aimIco, eatIco, defaultIco;
    public InputType currentInputType;
    //public delegate void Attack(ItemInventory bullet, float attackDamage,  float bulletSpeed);
    public delegate void Attack(ItemGun gun, ItemInventory bullet);
    public event Attack hasAttack;
    public event System.Action<Item> onChangeItem;
    Coroutine attackRoutine;

    InteractiveObject interactiveObject;//Объект взаимодействия
    
    protected override void Start()
    {
        base.Start();
        activeInventory = GameManager.ActiveInventoryContainer;
        GameManager.CharacterController.OnSetIntarctiveObject += ChangeInteractiveObject;
        SwitchCurrentItem(currentItemIndex);
    }

    /// <summary>Установить объект взаимодействия.</summary>
    void ChangeInteractiveObject(InteractiveObject interactiveObject)
    {
        if(interactiveObject != null && inputDirection == Vector2.zero)
        {
            this.interactiveObject = interactiveObject;
            ChangeInteractiveController();
        }

        else if(this.interactiveObject != null)
        {
            this.interactiveObject = null;
            SwitchCurrentItem(currentItemIndex);
        }
    }

    public bool isInteractiveObject()
    {
        if (interactiveObject != null)
            return true;
        else
            return false;
    }

    #region OnPointer

    public override void OnPointerUp(PointerEventData ped)
    {
        if (currentInputType == InputType.Direction)
            base.OnPointerUp(ped);
    }

    public override void OnPointerDown(PointerEventData ped)
    {
        if(interactiveObject != null)
        {
            interactiveObject.Execute();
        }

        else if (activeInventory.inventoryCells[currentItemIndex].itemInventory.item != null)
        {
            if (activeInventory.inventoryCells[currentItemIndex].itemInventory.item is ItemGun)
            {
                base.OnPointerDown(ped);

                ItemGun itemGun = activeInventory.inventoryCells[currentItemIndex].itemInventory.item as ItemGun;

                for (int i = 0; i < 2; i++)
                {
                    if (activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer != null
                        && activeInventory.inventoryCells[currentItemIndex].itemInventory.item.Execute(activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer.inventoryCells[activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesCellNumber].itemInventory)
                        && inputDirection != Vector2.zero)
                    {
                        if (attackRoutine == null)
                            attackRoutine = StartCoroutine(IAttack(itemGun, itemGun.AttackRate));

                        break;
                    }

                    else if (!FindBullets())
                        GameManager.GameUIManager.SendGameMessage("Ran out of bullets . . .");
                }
            }

            else
            {
                if (activeInventory.inventoryCells[currentItemIndex].itemInventory.item.Execute())
                {
                    activeInventory.inventoryCells[currentItemIndex].itemInventory.AddValue(-1);
                    activeInventory.inventoryCells[currentItemIndex].itemInventory.container.UpdateInventoryCell(activeInventory.inventoryCells[currentItemIndex].itemInventory.cellNumber);
                }

                if (activeInventory.inventoryCells[currentItemIndex].itemInventory.itemCount == 0)
                    SwitchCurrentItem(currentItemIndex);//Обновление текущего предмета
            }
        }

        else
            SwitchCurrentItem(currentItemIndex);//Обновление текущего предмета
    }

    public override void OnDrag(PointerEventData ped)
    {
        if(currentInputType == InputType.Direction)
            base.OnDrag(ped);
    }

    #endregion

    #region ItemVisual

    ///<summary>  Смена активируемого предмета </summary>
    public void SwitchCurrentItem(int cellNumber)
    {
        if (activeInventory.inventoryCells[cellNumber].itemInventory.item != null)
        {
            onChangeItem?.Invoke(activeInventory.inventoryCells[cellNumber].itemInventory.item);

            if (isInteractiveObject())
            {
                //Установить иконку управления на взаимодействие
                ChangeItemController(InputType.NonDirection, null);
            }

            else if (activeInventory.inventoryCells[cellNumber].itemInventory.item is ItemGun)
            {
                ChangeItemController(InputType.Direction, activeInventory.inventoryCells[cellNumber].itemInventory.item);
                FindBullets();
            }

            else
                ChangeItemController(InputType.NonDirection, activeInventory.inventoryCells[cellNumber].itemInventory.item);
        }

        else
        {
            onChangeItem?.Invoke(null);
            ChangeItemController(InputType.NonDirection, null);
        }

        currentItemIndex = cellNumber;
    }

    void ChangeItemController(InputType inputType,Item item)
    {
        currentInputType = inputType;

        if (inputType == InputType.Direction && item is ItemGun)
            handle.GetComponent<Image>().sprite = aimIco;

        else if(item is ItemEat)
            handle.GetComponent<Image>().sprite = eatIco;

        else
            handle.GetComponent<Image>().sprite = defaultIco;
    }

    void ChangeInteractiveController()
    {
        currentInputType = InputType.NonDirection;
        //Поменять спрайт
        handle.GetComponent<Image>().sprite = defaultIco;
    }

    #endregion

    #region Attack

    bool FindBullets()
    {
        InventoryContainer bulletContainer;
        int bulletCellNumber;

        if (activeInventory.inventoryCells[currentItemIndex].itemInventory != null)
        {
            if (GameManager.InventoryContainer.FindBullets(out bulletContainer, out bulletCellNumber)
                || GameManager.ActiveInventoryContainer.FindBullets(out bulletContainer, out bulletCellNumber))
            {
                activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer = bulletContainer;
                activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesCellNumber = bulletCellNumber;
                return true;
            }

            else
            {
                activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer = null;
                activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesCellNumber = -1;
                return false;
            }
        }

        else
            return false;
    }

    void StopAttackRoutine()
    {
        if (attackRoutine != null)
        {
            StopCoroutine(attackRoutine);
            attackRoutine = null;
        }
    }

    IEnumerator IAttack(ItemGun itemGun, float attackeRate)
    {
        yield return new WaitForSeconds(attackeRate);

        while (true)
        {
            if (inputDirection != Vector2.zero)
            {
                if (activeInventory.inventoryCells[currentItemIndex].itemInventory.item.Execute(activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer.inventoryCells[activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesCellNumber].itemInventory))
                {
                    hasAttack?.Invoke(itemGun, activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer.inventoryCells[activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesCellNumber].itemInventory);
                }

                else if (FindBullets())
                {
                    hasAttack?.Invoke(itemGun, activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesContainer.inventoryCells[activeInventory.inventoryCells[currentItemIndex].itemInventory.consumablesCellNumber].itemInventory);
                }

                else
                {
                    GameManager.GameUIManager.SendGameMessage("Ran out of bullets . . .");
                    StopAttackRoutine();
                }

            }

            yield return new WaitForSeconds(attackeRate);

            if (inputDirection == Vector2.zero)
            {
                StopAttackRoutine();
            }
        }
    }

    #endregion
}
