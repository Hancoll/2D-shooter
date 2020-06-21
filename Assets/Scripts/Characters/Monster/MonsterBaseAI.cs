using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public enum MonsterState
{
    None,
    /// <summary> Стоит на месте. </summary>
    Idle,
    /// <summary> Идёт к игроку. </summary>
    Targeting,
    /// <summary> Патрулирует. </summary>
    Patrol,
    /// <summary> Идёт к привязаной позиции и не может атаковать. </summary>
    MovingToAnchoredPosition,
    /// <summary> Атакует цель. </summary>
    Attack
}

public class MonsterBaseAI : MonoBehaviour
{
    [SerializeField] protected MonsterState currentState;
    public BaseStats Target { get; private set; } = null;
    Vector2 anchoredPosition;
    [HideInInspector] public Transform marker;//Объект к которому будет двигаться монстр при патрулировании

    [SerializeField] [Range(0,15)] float rageDistance = 5;
    float timeBetweenCharacterSearch = 1f;
    [SerializeField] int maxFollowingDistance = 20;//Дистанция на которой монстр прекращает преследование и идёт на место
    float timeBetweenCheckCharacterPosition = 0.3f;//Время между проверкой позиции игрока по отношению к привязаной позиции
    [SerializeField] float timeBeforePatrol = 2;//Задержка до начала патрулирования
    [SerializeField] float timeBetweenCheckOnReachedMarker = 0.4f;

    [SerializeField] Transform spriteContainer;
    bool isLeft = false;

    float attackRate;
    int damage;
    float moveSpeed;
    float endReachedDistance;//Дистанция до которой монстр преследует цель и после начинает атаковать

    public List<string> activeCoroutines = new List<string>();
    Coroutine characterSearch; //ICharacterFind()
    Coroutine checkCharacterPosition;//ICheckCharacterPosition()
    Coroutine delayBeforePatrol;//IDelayBeforePatrol()
    Coroutine checkOnReachedMarker;//ICheckOnReachedMarker()
    Coroutine attack;//IAttack()

    public Animator Animator { get; private set; }
    AIPath pathAgent;
    AIDestinationSetter destinationSetter;
    public Rigidbody2D Rigidbody { get; private set; }
    MonsterStats monsterStats;


    protected virtual void Awake()
    {
        monsterStats = GetComponent<MonsterStats>();
        destinationSetter = GetComponent<AIDestinationSetter>();
        pathAgent = GetComponent<AIPath>();
        Animator = GetComponent<Animator>();
        Rigidbody = GetComponent<Rigidbody2D>();
        marker = Instantiate(GameManager.MarkerPrefab, GameManager.MarkersContainer).transform;
        //Инициализация статов
        attackRate = monsterStats.AttackRate;
        damage = monsterStats.Damage;
        moveSpeed = monsterStats.MoveSpeed;
        endReachedDistance = (float)monsterStats.AttackDistance * 0.65f;
        //
        pathAgent.maxSpeed = moveSpeed;
        pathAgent.slowdownDistance = (float)monsterStats.AttackDistance;
    }

    protected virtual void Start()
    {
        ChangeState(MonsterState.Idle);
    }

    void Attack()
    {
        Vector2 direction = (transform.position - Target.transform.position).normalized;
        Target.GetDamage(damage, monsterStats, -direction * monsterStats.Knockback);
    }

    /// <summary> Развернуть спрайт монстра. </summary>
    public void ChangeSide(bool value)
    {
        if (value)
            spriteContainer.localRotation = Quaternion.Euler(Vector3.up * 180);
        else
            spriteContainer.localRotation = Quaternion.Euler(Vector3.zero);

        isLeft = value;
    }

    #region States

    public void ContinuePreActiveCoroutines()
    {
        foreach(string coroutineName in activeCoroutines)
        {
            switch(coroutineName)
            {
                case "IAttack":
                    attack = StartCoroutine(coroutineName);
                    break;

                case "ICharacterFind":
                    characterSearch = StartCoroutine(coroutineName);
                    break;

                case "ICheckCharacterPosition":
                    checkCharacterPosition = StartCoroutine(coroutineName);
                    break;

                case "ICheckOnReachedMarker":
                    checkOnReachedMarker = StartCoroutine(coroutineName);
                    break;

                case "IDelayBeforePatrol":
                    delayBeforePatrol = StartCoroutine(coroutineName);
                    break;
            }
        }
    }

    void ChangeState(MonsterState state)
    {
        if(currentState != state)
        {
            #region if currentState
            if (currentState == MonsterState.Idle)
            {
                if(characterSearch != null && state != MonsterState.Patrol)
                {
                    StopCoroutine(characterSearch);
                    activeCoroutines.Remove("ICharacterFind");
                    characterSearch = null;
                }

                if(delayBeforePatrol != null)
                {
                    StopCoroutine(delayBeforePatrol);
                    activeCoroutines.Remove("IDelayBeforePatrol");
                    delayBeforePatrol = null;
                }
            }

            else if(currentState == MonsterState.Targeting)
            {
                if (checkCharacterPosition != null && state != MonsterState.Attack)
                {
                    StopCoroutine(checkCharacterPosition);
                    activeCoroutines.Remove("ICheckCharacterPosition");
                    checkCharacterPosition = null;
                }
            }

            else if (currentState == MonsterState.Patrol)
            {
                if (characterSearch != null && state != MonsterState.Idle)
                {
                    StopCoroutine(characterSearch);
                    activeCoroutines.Remove("ICharacterFind");
                    characterSearch = null;
                }

                if (checkOnReachedMarker != null)
                {
                    StopCoroutine(checkOnReachedMarker);
                    activeCoroutines.Remove("ICheckOnReachedMarker");
                    checkOnReachedMarker = null;
                }
            }

            else if (currentState == MonsterState.MovingToAnchoredPosition)
            {
                if (checkOnReachedMarker != null)
                {
                    StopCoroutine(checkOnReachedMarker);
                    activeCoroutines.Remove("ICheckOnReachedMarker");
                    checkOnReachedMarker = null;
                }
            }

            else if(currentState == MonsterState.Attack)
            {
                if(checkCharacterPosition != null && state != MonsterState.Targeting)
                {
                    StopCoroutine(checkCharacterPosition);
                    activeCoroutines.Remove("ICheckCharacterPosition");
                    checkCharacterPosition = null;
                }


                if(attack != null)
                {
                    StopCoroutine(attack);
                    activeCoroutines.Remove("IAttack");
                    attack = null;
                }
            }
            #endregion

            #region if state
            if (state == MonsterState.Idle)
            {
                Target = null;
                destinationSetter.target = null;
                if (characterSearch == null)
                {
                    characterSearch = StartCoroutine(ICharacterFind());
                    activeCoroutines.Add("ICharacterFind");
                }

                delayBeforePatrol = StartCoroutine(IDelayBeforePatrol());
                activeCoroutines.Add("IDelayBeforePatrol");
            }

            else if (state == MonsterState.Targeting)
            {
                pathAgent.maxSpeed = moveSpeed;
                pathAgent.endReachedDistance = endReachedDistance;
                destinationSetter.target = Target.transform;

                if (checkCharacterPosition == null)
                {
                    checkCharacterPosition = StartCoroutine(ICheckCharacterPosition());
                    activeCoroutines.Add("ICheckCharacterPosition");
                }
            }

            else if (state == MonsterState.Patrol)
            {
                Target = null;
                pathAgent.maxSpeed = moveSpeed / 2;
                pathAgent.endReachedDistance = 1;
                if((Vector2)marker.position == anchoredPosition)
                {
                    int extraPosition = 2 * monsterStats.MoveSpeed;
                    int xPos = Random.Range(-9 - extraPosition, 9 + extraPosition);
                    int yPos = Random.Range(-9 - extraPosition, 9 + extraPosition);

                    SetMovingPosition(new Vector2(transform.position.x + xPos, transform.position.y + yPos));
                }                                                                                                                                     
                else
                    SetMovingPosition(anchoredPosition);
                checkOnReachedMarker = StartCoroutine(ICheckOnReachedMarker());
                activeCoroutines.Add("ICheckOnReachedMarker");
            }

            else if (state == MonsterState.MovingToAnchoredPosition)
            {
                Target = null;
                pathAgent.endReachedDistance = 0.2f;
                SetMovingPosition(anchoredPosition);
                checkOnReachedMarker = StartCoroutine(ICheckOnReachedMarker());
                activeCoroutines.Add("ICheckOnReachedMarker");
            }

            else if (state == MonsterState.Attack)
            {
                destinationSetter.target = null;

                if (attack != null)
                {
                    StopCoroutine(attack);
                    activeCoroutines.Remove("IAttack");
                    attack = null;
                }

                else
                {
                    attack = StartCoroutine(IAttack());
                    activeCoroutines.Add("IAttack");
                }
            }
            #endregion

            currentState = state;
        }
    }

    /// <summary> True если можно использовать его как таргет.</summary>
    public bool CheckNewTarget(BaseStats target)
    {
        if (CheckTarget(target) && target != Target)
            return true;
        else
            return false;
    }

    bool CheckTarget(BaseStats target)
    {
        if (Vector2.Distance(target.transform.position, anchoredPosition) < maxFollowingDistance && currentState != MonsterState.MovingToAnchoredPosition && !target.IsDied)
            return true;
        else
            return false;
    }

    public void SetTarget(BaseStats target)
    {
        this.Target = target;
        ChangeState(MonsterState.Targeting);
    }

    #endregion

    #region Navigation

    public void SetAnchoredPosition(Vector2 anchoredPosition)
    {
        this.anchoredPosition = anchoredPosition;
    }

    void SetMovingPosition(Vector2 position)
    {
        marker.position = position;
        destinationSetter.target = marker;
    }

    #endregion

    #region IEnumerators

    IEnumerator IAttack()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, Target.transform.position) > monsterStats.AttackDistance && CheckTarget(Target))
                ChangeState(MonsterState.Targeting);
            else if(!CheckTarget(Target))
                ChangeState(MonsterState.MovingToAnchoredPosition);

            yield return new WaitForSeconds(attackRate);

            if (Vector2.Distance(transform.position, Target.transform.position) <= monsterStats.AttackDistance && CheckTarget(Target))
                Attack();
        }
    }

    IEnumerator ICharacterFind()
    {
        while (true)
        {
            Collider2D characterCollider = Physics2D.OverlapCircle(transform.position, rageDistance, GameManager.CharacterLayerMask); 
            if (characterCollider != null)
            {
                RaycastHit2D collidersOnPath = Physics2D.Linecast(transform.position, characterCollider.transform.position, GameManager.InvisibleLayerMask);
                if(collidersOnPath.collider == null)
                {
                    Target = characterCollider.GetComponent<BaseStats>();
                    ChangeState(MonsterState.Targeting);
                }
            }

            yield return new WaitForSeconds(timeBetweenCharacterSearch);
        }
    }

    IEnumerator ICheckCharacterPosition()
    {
        while (true)
        {
            if (Target == null || Vector2.Distance(Target.transform.position, anchoredPosition) >= maxFollowingDistance)
                ChangeState(MonsterState.MovingToAnchoredPosition);
            else
            {
                if (Vector2.Distance(transform.position, Target.transform.position) < monsterStats.AttackDistance)
                    ChangeState(MonsterState.Attack);

                if (Target.transform.position.x - transform.position.x >= 0 && isLeft)
                    ChangeSide(false);
                else if (Target.transform.position.x - transform.position.x < 0 && !isLeft)
                    ChangeSide(true);
            }

            yield return new WaitForSeconds(timeBetweenCheckCharacterPosition);
        }
    }

    IEnumerator ICheckOnReachedMarker()
    {
        while (true)
        {
            if (pathAgent.reachedDestination)
            {
                if (currentState == MonsterState.Patrol)
                    ChangeState(MonsterState.Idle);

                else if (currentState == MonsterState.MovingToAnchoredPosition)
                    ChangeState(MonsterState.Idle);
            }

            else
            {
                if (marker.position.x - transform.position.x >= 0 && isLeft)
                    ChangeSide(false);
                else if (marker.position.x - transform.position.x < 0 && !isLeft)
                    ChangeSide(true);
            }

            yield return new WaitForSeconds(timeBetweenCheckOnReachedMarker);
        }
    }

    IEnumerator IDelayBeforePatrol()
    {
        yield return new WaitForSeconds(timeBeforePatrol);
        ChangeState(MonsterState.Patrol);
    }

    #endregion
}
