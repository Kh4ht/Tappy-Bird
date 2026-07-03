using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class Clouds : MonoBehaviour
{
    private float Speed;

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Start()
    {
        SpriteRenderer Sprite = GetComponent<SpriteRenderer>();

        float minSpeed = 0.5f;
        float maxSpeed = 4;

        Speed = Random.Range(minSpeed, maxSpeed);

        Sprite.color = new Color(Sprite.color.r,
                                 Sprite.color.g,
                                 Sprite.color.b,
                                 Mathf.Lerp(0.03f, 0.3f, (Speed - minSpeed) / (maxSpeed - minSpeed)));
    }

    private void Update()
    {
        Move();

        DestroyCloud();
    }

    //////////////////////////////
    //////////////////////////////
    //////////////////////////////

    private void Move()
    {
        float Multiplier;

        float change = 0;

        if (FindAnyObjectByType<Obstacle>() != null)
        {
            change = (FindAnyObjectByType<Obstacle>().Speed - FindAnyObjectByType<Obstacle>().OriginalSpeed)
                                    / (FindAnyObjectByType<Obstacle>().MaxSpeed - FindAnyObjectByType<Obstacle>().OriginalSpeed);
        }

        Multiplier = Mathf.Lerp(1f, 2f, change);

        transform.position += new Vector3(-Speed * Multiplier * Time.deltaTime, 0);
    }

    private void DestroyCloud()
    {
        if (transform.position.x <= -Camera.main.orthographicSize * Camera.main.aspect - 2)
        {
            Destroy(gameObject);
        }
    }
}