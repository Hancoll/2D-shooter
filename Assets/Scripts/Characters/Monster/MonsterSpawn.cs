using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawn : MonoBehaviour
{
    [Range(0,10)] [SerializeField] int spawnRadius = 3;
    [SerializeField] float distanceBetweenSpawnPoint = 2;
    [SerializeField] float timeBetweenSpawn = 3;
    [SerializeField] int faultPosition = 2;
    Coroutine SpawnerCoroutine;

    [SerializeField] List<GameObject> monsterList;
    GameObject[,] aliveMonsters;

    static MonsterBaseAI bossMonsterBaseAI;

    private void Start()
    {
        int arrayCount = spawnRadius * 2 + 1;
        aliveMonsters = new GameObject[arrayCount, arrayCount];

        SpawnerCoroutine = StartCoroutine(IMonsterSpawn());
    }

    public static bool SpawnMonsterBoss(GameObject boss)
    {
        if(bossMonsterBaseAI == null)
        {
            for (int i = 1; i <= 8; i++)
            {
                float angle = (Mathf.PI / 4) * i;

                Vector2 pos = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * (GameManager.MinSpawnDistance + 3);
                pos += (Vector2)GameManager.CharacterTransform.position;


                Collider2D obstacleCollider = Physics2D.OverlapCircle(pos, 2f);

                if (obstacleCollider == null)
                {
                    GameObject newMonster = Instantiate(boss, pos, Quaternion.identity, GameManager.GameSpace);
                    bossMonsterBaseAI = newMonster.GetComponent<MonsterBaseAI>();
                    bossMonsterBaseAI.SetAnchoredPosition(pos);

                    return true;
                }
            }
        }

        return false;
    }

    void SpawnMonster()
    {
        int xPosition, yPosition;
        int x = 0, y = 0;

        for (xPosition = -spawnRadius; xPosition <= spawnRadius; xPosition++, x++, y = 0)
        {
            for (yPosition = -spawnRadius; yPosition <= spawnRadius; yPosition++, y++)
            {
                if (xPosition * xPosition + yPosition * yPosition <= spawnRadius * spawnRadius)
                {
                    if (aliveMonsters[x, y] == null)
                    {
                        float spawnPositionX = (xPosition * distanceBetweenSpawnPoint) + transform.position.x + Random.Range(-faultPosition, faultPosition);
                        float spawnPositionY = (yPosition * distanceBetweenSpawnPoint) + transform.position.y + Random.Range(-faultPosition, faultPosition);
                        Vector2 spawnPosition = new Vector2(spawnPositionX, spawnPositionY);

                        float distance = Vector2.Distance(spawnPosition, GameManager.CharacterTransform.position);

                        if (distance > GameManager.MinSpawnDistance && distance < GameManager.LoadingDistance)
                        {
                            Collider2D obstacleCollider = Physics2D.OverlapCircle(spawnPosition, 1.5f);

                            if (obstacleCollider == null)
                            {
                                GameObject newMonster = Instantiate(monsterList[0], spawnPosition, Quaternion.identity, GameManager.GameSpace);
                                newMonster.GetComponent<MonsterBaseAI>().SetAnchoredPosition(spawnPosition);

                                aliveMonsters[x, y] = newMonster;
                                return;
                            }

                            else
                                continue;
                        }

                        else
                            continue;
                    }
                }
            }
        }
    }

    IEnumerator IMonsterSpawn()
    {
        while (true)
        {
            SpawnMonster();

            yield return new WaitForSeconds(timeBetweenSpawn);
        }
    }
}
