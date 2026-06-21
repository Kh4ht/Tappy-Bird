using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private GameManager gm;

    private Camera cam;
    [HideInInspector] public float OriginalSpeed;

    [SerializeField] private Transform Down;
    public Transform Top;
    public GameObject Point;
    public float Speed,
                 MaxSpeed,
                 MaxRotation;

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Awake()
    {
        cam = Camera.main;
        gm = GameManager.Ins;
    }

    private void Start()
    {
        OriginalSpeed = Speed;

        TopPosition();

        ObstaclePosition();

        Rotation();
    }

    private void Update()
    {
        Move();

        DestroyObstacle();
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Move()
    {
        Speed = Mathf.Lerp(OriginalSpeed, MaxSpeed, (float)gm.Score / gm.Max_SpawnSpeed_Score);

        transform.position += new Vector3(-Speed * Time.deltaTime, 0);
    }

    private void Rotation()
    {
        float Zrot = Mathf.Lerp(0, MaxRotation, (float)gm.Score / gm.Max_Rotation_Score);

        transform.rotation = Quaternion.Euler(transform.rotation.x, transform.rotation.y, Random.Range(-Zrot, Zrot));
    }

    private void ObstaclePosition()
    {
        float offset = 2 - Top.localPosition.y;

        transform.position = new Vector3(transform.position.x, Random.Range(-(cam.orthographicSize - 2), cam.orthographicSize - 2 + offset));
    }

    private void TopPosition()
    {
        float pos = Mathf.Lerp(2, 0, (float)gm.Score / gm.Max_TopPos_Score);

        Top.localPosition = new Vector3(Top.localPosition.x, pos);
    }

    private void DestroyObstacle()
    {
        if (transform.position.x <= -cam.orthographicSize * cam.aspect - 2)
        {
            Destroy(gameObject);
        }
    }
}