using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EquipmentType
{
    Head,//Шлем
    Body,//Броня
    Shoulders,//Плечи
    Belt,//Пояс
    Boosters//Усилители
}

public class EquipmentInventoryCell : InventoryCell
{
    public EquipmentType equipmentType;
}
