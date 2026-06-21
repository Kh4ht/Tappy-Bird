using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    private GameManager gm;

    private float OriginalSpawnSpeed;

    [SerializeField] private GameObject[] clouds;

    [SerializeField] private GameObject Obstacle;

    [SerializeField] private float SpawnSpeed,
                                   MaxSpawnSpeed;

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Awake()
    {
        gm = GameManager.Ins;
    }

    private void Start()
    {
        OriginalSpawnSpeed = SpawnSpeed;

        timer = SpawnSpeed;

        CloudSpawnSpeed = Random.Range(0, 10f);

        transform.position = new Vector3(Camera.main.orthographicSize * Camera.main.aspect + 2, 0);
    }

    private void Update()
    {
        SpawnCloud();

        SpawnObstacle();
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private float CloudSpawnSpeed;
    private float timer2;
    private void SpawnCloud()
    {
        timer2 += Time.deltaTime;

        if (timer2 >= CloudSpawnSpeed && gm.IsLevelStarted)
        {
            Instantiate(clouds[Random.Range(0, clouds.Length)],
                        new Vector3(transform.position.x, Random.Range(-Camera.main.orthographicSize, Camera.main.orthographicSize)),
                        transform.rotation);

            timer2 = 0;

            CloudSpawnSpeed = Random.Range(0, 10f);
        }
    }

    private float timer = 9;
    private void SpawnObstacle()
    {
        SpawnSpeed = Mathf.Lerp(OriginalSpawnSpeed, MaxSpawnSpeed, (float)gm.Score / gm.Max_SpawnSpeed_Score);

        timer += Time.deltaTime;

        if (timer >= SpawnSpeed && gm.IsLevelStarted)
        {
            Instantiate(Obstacle, transform.position, transform.rotation);

            timer = 0;
        }
    }
}