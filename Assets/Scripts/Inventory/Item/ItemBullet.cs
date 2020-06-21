using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bullet", menuName = "Create ItemBullet")]
[System.Serializable]
public class ItemBullet : Item
{
    public Sprite BulletSprite { get { return bulletSprite; } }
    [SerializeField] protected Sprite bulletSprite;
}
