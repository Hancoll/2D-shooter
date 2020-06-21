using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour
{
    //Motion
    public float moveSpeed;
    bool isRight = true;//-1 — Character бежит влево, +1 — Character бежит вправо, 0 — стоит на месте.
    bool isRunning = false;
    Vector2 motionDirection;

    //Attack
    public GameObject bulletPrefab;
    public GameObject ItemObject;
    Vector2 attackDirection;

    public InteractiveObject interactiveObject;
    [SerializeField] int interactRange;
    [SerializeField] LayerMask interactiveMask;
    public Vector2 characterDirection = new Vector2(1, 0);//Направление персонажа для взаимодействия с окружением
    public event System.Action<InteractiveObject> OnSetIntarctiveObject;

    //Components
    CharacterStats characterStats;
    public Rigidbody2D Rigidbody { get; private set; }
    public Animator Animator { get; private set; }
    Animator itemAnimator;

    private void Awake()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
    }

    private void Start()
    {
        GameManager.InventoryController.eventOnChangeInputDirection += SetAttackDirection;
        GameManager.MotionController.eventOnChangeInputDirection += SetMotionDirection;
        GameManager.InventoryController.hasAttack += Attack;
        GameManager.InventoryController.onChangeItem += ChangeItem;
        characterStats.OnDied += OnDied;
    }

    private void FixedUpdate()
    {
        Rigidbody.MovePosition((Vector2)transform.position + motionDirection * moveSpeed * Time.fixedDeltaTime);

        Collider2D intaractiveCollider = Physics2D.OverlapCircle((Vector2)transform.position + (characterDirection * interactRange), interactRange, interactiveMask);

        if (intaractiveCollider != null && (interactiveObject != intaractiveCollider?.GetComponent<InteractiveObject>() || !GameManager.InventoryController.isInteractiveObject()))
        {
            interactiveObject = intaractiveCollider.GetComponent<InteractiveObject>();
            OnSetIntarctiveObject?.Invoke(interactiveObject);
        }

        else if (intaractiveCollider == null && interactiveObject != intaractiveCollider?.GetComponent<InteractiveObject>())
        {
            interactiveObject = null;
            OnSetIntarctiveObject?.Invoke(null);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Premises>())
        {
            collision.GetComponent<Premises>().ChangeForwardWallVisible(false);
            GameManager.CameraController.OnChangeLightingStatus(false);
        }

        if (collision.GetComponent<ItemContainer>())
        {
            collision.GetComponent<ItemContainer>().TakeItem();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Premises>())
        {
            collision.GetComponent<Premises>().ChangeForwardWallVisible(true);
            GameManager.CameraController.OnChangeLightingStatus(true);
        }
    }

    void OnDied()
    {
        GameManager.InventoryController.eventOnChangeInputDirection -= SetAttackDirection;
        GameManager.MotionController.eventOnChangeInputDirection -= SetMotionDirection;
        GameManager.InventoryController.hasAttack -= Attack;
        GameManager.InventoryController.onChangeItem -= ChangeItem;
    }

    void ChangeItem(Item newItem)
    {
        Destroy(ItemObject);

        if (newItem != null)
        {
            ItemObject = Instantiate(newItem.ItemPrefab, transform);
            itemAnimator = ItemObject.GetComponent<Animator>();
        }

        else
        {
            ItemObject = null;
            itemAnimator = null;
        }
    }

    #region Motion

    void ChangeMoveDirection(bool isRight)
    {
        this.isRight = isRight;

        if (!isRight)
            transform.localRotation = Quaternion.Euler(0, 180, 0);
        else
            transform.localRotation = Quaternion.Euler(0, 0, 0);
    }

    void SetMotionDirection(Vector2 direction)
    {
        if (!isRunning && direction != Vector2.zero)//Начать движение
        {
            if (direction.x > 0)
                ChangeMoveDirection(true);
            else if (direction.x < 0)
                ChangeMoveDirection(false);

            if (!isRunning)
            {
                isRunning = true;
                Animator.SetBool("isRunning", isRunning);
            }
        }

        else if(attackDirection == Vector2.zero && isRunning && direction != Vector2.zero)
        {
            characterDirection = direction;

            if (direction.x < 0)//Поворот налево
                ChangeMoveDirection(false);

            else if (direction.x >= 0 && direction != Vector2.zero)//Поворот направо
                ChangeMoveDirection(true);
        }

        else if (isRunning && direction == Vector2.zero)//Закончить движение
        {
            isRunning = false;
            Animator.SetBool("isRunning", isRunning);
        }

        motionDirection = direction;
    }

    #endregion

    #region Attack

    void SetAttackDirection(Vector2 direction)
    {
        attackDirection = direction;

        if (ItemObject != null)
        {
            Vector2 tanAngle;
            float angle;//Угол повотрота оружия в руках

            if (direction != Vector2.zero)
            {
                characterDirection = direction;

                if (isRight && direction.x < 0)
                    ChangeMoveDirection(false);
                else if(!isRight && direction.x >= 0)
                    ChangeMoveDirection(true);

                if (isRight)
                    tanAngle = new Vector2(direction.y, direction.x) * 0.5f * Mathf.PI;

                else
                    tanAngle = new Vector2(direction.y, -direction.x) * 0.5f * Mathf.PI;


                angle = Mathf.Atan2(tanAngle.x, tanAngle.y) * Mathf.Rad2Deg;
            }

            else
                angle = 0;

            ItemObject.transform.localEulerAngles = new Vector3(0, 0, angle);
        }
    }

    void Attack(ItemGun gun, ItemInventory bullet)
    {
        GameObject[] bullets;
        ItemBullet itemBullet = bullet.item as ItemBullet;
        float angleRotation;

        switch (gun.AttackType)
        {
            case AttackType.Burst: bullets = new GameObject[2]; break;

            case AttackType.Shot: bullets = new GameObject[6]; break;

            default: bullets = new GameObject[1]; break;
        }

        int bulletPositin = -bullets.Length / 2;

        for (int i = 0; i < bullets.Length; i++ , bulletPositin++)
        {
            //Создание GameObject'а
            bullets[i] = Instantiate(bulletPrefab, ItemObject.transform.GetChild(0).position, Quaternion.identity, GameManager.GameSpace);
            //Инициализация
            bullets[i].GetComponent<Bullet>().Initialize(gun.AttackDamage, gun.Knockback, GameManager.CharacterStats);
            bullets[i].GetComponent<SpriteRenderer>().sprite = itemBullet.BulletSprite;

            if (gun.AttackType == AttackType.Single)
            {
                angleRotation = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
                bullets[i].transform.localRotation = Quaternion.AngleAxis(angleRotation, Vector3.forward);
                bullets[i].GetComponent<Rigidbody2D>().AddForce(attackDirection * gun.BulletSpeed);
            }

            else if (gun.AttackType == AttackType.Burst)
            {
                angleRotation = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
                bullets[i].transform.localRotation = Quaternion.AngleAxis(angleRotation, Vector3.forward);
                bullets[i].GetComponent<Rigidbody2D>().AddForce(attackDirection * (gun.BulletSpeed - bulletPositin * 40));
            }

            else if (gun.AttackType == AttackType.Shot)
            {
                angleRotation = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
                bullets[i].transform.localRotation = Quaternion.AngleAxis(angleRotation, Vector3.forward);

                float bulletAngleRotation = bulletPositin * 4 * Mathf.Deg2Rad;

                Vector2 bulletDirection = new Vector2();
                bulletDirection.x = attackDirection.x * Mathf.Cos(bulletAngleRotation) - attackDirection.y * Mathf.Sin(bulletAngleRotation);
                bulletDirection.y = attackDirection.x * Mathf.Sin(bulletAngleRotation) + attackDirection.y * Mathf.Cos(bulletAngleRotation);

                bullets[i].GetComponent<Rigidbody2D>().AddForce(bulletDirection * gun.BulletSpeed);
            }
        }

        //x = x0 * cos(a) - y0 * sin(a)
        //y = x0 * sin(a) + y0 * cos(a)

        //GameObject newBullet = Instantiate(bulletPrefab, ItemObject.transform.GetChild(0).position, Quaternion.identity,GameManager.GameSpace);
        //newBullet.GetComponent<Bullet>().Initialize(gun.AttackDamage, GameManager.CharacterStats);

        //ItemBullet itemBullet = bullet.item as ItemBullet;
        //newBullet.GetComponent<SpriteRenderer>().sprite = itemBullet.BulletSprite;
        //itemAnimator.SetTrigger("Shot");
        //angleRotation = Mathf.Atan2(attackDirection.y, attackDirection.x) * Mathf.Rad2Deg;
        //newBullet.transform.localRotation = Quaternion.AngleAxis(angleRotation, Vector3.forward);
        //newBullet.GetComponent<Rigidbody2D>().AddForce(attackDirection * gun.BulletSpeed);

        itemAnimator.SetTrigger("Shot");
        bullet.AddValue(-1);
    }

    #endregion
}
