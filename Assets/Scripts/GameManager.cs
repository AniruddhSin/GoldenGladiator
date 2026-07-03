using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private PlayerInputHandler playerInputHandler;

    [SerializeField] private GameObject pausePanel;
    private bool isPaused = false;
    [SerializeField] private ObjectPool meteorPool;
    public ObjectPool MeteorPool => meteorPool;
    [SerializeField] private Spawner spawnerL;
    [SerializeField] private Spawner spawnerM;
    [SerializeField] private Spawner spawnerR;
    private List<Spawner> spawners;
    private int currentPhase = 1;

    [SerializeField] private Enemy[] enemies;
    private int numActiveEnemies = 0;

    [SerializeField] private Boss boss;
    private float bossRevealTime = 5f;
    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
    void Start()
    {
        spawners = new List<Spawner>{spawnerL, spawnerM, spawnerR};
        //spawners start deactivated already

        foreach (Enemy e in enemies)
        {
            e.gameObject.SetActive(false);
        }

        StartCoroutine(BossLoop());
    }

    private IEnumerator BossLoop()
    {
        while (currentPhase < 3)
        {
            yield return StartCoroutine(RunPhase(currentPhase));
        }
    }

    private IEnumerator RunPhase(int phase)
    {
        yield return new WaitForSeconds(bossRevealTime);
        boss.FadeBoss(true);
        yield return new WaitForSeconds(1f);
        ActivateEnemies();
        ActivateRandomSpawner(phase);
        yield return new WaitUntil(() => numActiveEnemies == 0);
        boss.FadeBoss(false);
    }

    public void NextPhase()
    {
        if (currentPhase < 3)
        {
            currentPhase += 1;        
        }
    }
    public void TogglePause()
    {
        isPaused = !isPaused;
        if (isPaused)
        {
            playerInputHandler.disableInput();
        }
        else
        {
            playerInputHandler.enableInput();
        }
        pausePanel.SetActive(isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void EnemyDied(Enemy enemy)
    {
        numActiveEnemies -= 1;
        enemy.gameObject.SetActive(false);
    }
    void ActivateEnemies()
    {
        foreach (Enemy e in enemies)
        {
            e.gameObject.SetActive(true);
            e.respawn();
        }
        numActiveEnemies = enemies.Length;
    }
    void ActivateRandomSpawner(int numSpawners)
    {
        for (int i = 0; i < numSpawners; i += 1)
        {
            int randomSpawner = Random.Range(0,spawners.Count);
            Spawner s = spawners[randomSpawner];
            while (s.isFiring)
            {
                randomSpawner = (randomSpawner + 1) % spawners.Count;
                s = spawners[randomSpawner];
            }
            s.isFiring = true;

        }
    }
    void DeactivateSpawners()
    {
        foreach (Spawner s in spawners)
        {
            s.isFiring = false;
        }
    }
}
