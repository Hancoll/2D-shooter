using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Create EquipmentItem")]
[System.Serializable]
public class EquipmentItem : Item
{
    public EquipmentType equipmentType;
}
