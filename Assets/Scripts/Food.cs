using UnityEngine;


public class Food : MonoBehaviour
{
    public Collider2D gridArea;

    public GameObject GrowFood;
    public GameObject UnGrowFood;
    [Range(50, 90)]
    public float growPossibility = 70;

    public GameObject SlowerFood;
    public GameObject SpeedUpFood;
    [Range(10, 90)]
    public float speedUpPossibility = 50;

    private Snake snake;

    private void Awake()
    {
        snake = FindObjectOfType<Snake>();
    }

    private void Start()
    {
        SpawnFood();
        SpawnSpeedFood();
    }

    public void SpawnFood()
    {
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));

        

        // Prevent the food from spawning on the snake
        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y) {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        int posibility = Random.Range(0, 100);

        if(posibility <= growPossibility)
        {
            GameObject f = Instantiate(GrowFood, new Vector2(x, y), Quaternion.identity);
        }
        else
        {
            GameObject f = Instantiate(UnGrowFood, new Vector2(x, y), Quaternion.identity);
        }

    }

    public void SpawnSpeedFood()
    {
        Bounds bounds = gridArea.bounds;

        // Pick a random position inside the bounds
        // Round the values to ensure it aligns with the grid
        int x = Mathf.RoundToInt(Random.Range(bounds.min.x, bounds.max.x));
        int y = Mathf.RoundToInt(Random.Range(bounds.min.y, bounds.max.y));



        // Prevent the food from spawning on the snake
        while (snake.Occupies(x, y))
        {
            x++;

            if (x > bounds.max.x)
            {
                x = Mathf.RoundToInt(bounds.min.x);
                y++;

                if (y > bounds.max.y)
                {
                    y = Mathf.RoundToInt(bounds.min.y);
                }
            }
        }

        int posibility = Random.Range(0, 100);

        if (posibility <= speedUpPossibility)
        {
            GameObject f = Instantiate(SpeedUpFood, new Vector2(x, y), Quaternion.identity);
        }
        else
        {
            GameObject f = Instantiate(SlowerFood, new Vector2(x, y), Quaternion.identity);
        }

    }


}
