using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Snake : MonoBehaviour
{
    readonly int MAX_SPEED = 20;
    readonly int MIN_SPEED = 10;

    public Transform segmentPrefab;
    public Vector2Int direction = Vector2Int.right;

    [Range(10, 20)]
    public float speed = 15f;
    public float speedMultiplier = 1f;

    [Range(1, 5)]
    public float speedIncreaser = 5f;
    [Range(1, 5)]
    public float speedDecreaser = 5f;
    
    [Range(5, 10)]
    public float EffectDuration = 6f;


    [Range(3, 6)]
    public int initialSize = 4;
    public bool moveThroughWalls = false;

    private List<Transform> segments = new List<Transform>();
    //[SerializeField]
    private Vector2Int input;
    private float nextUpdate;

    private int score = 0;

    public Food foodManager;

    public TMP_Text SpeedText;
    public TMP_Text ScoreText;

    private void Start()
    {
        ResetState();
    }

    private void Update()
    {
        // Only allow turning up or down while moving in the x-axis
        if (direction.x != 0f)
        {
            if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            {
                input = Vector2Int.up;
            } 
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) 
            {
                input = Vector2Int.down;
            }
        }
        // Only allow turning left or right while moving in the y-axis
        else if (direction.y != 0f)
        {
            if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) 
            {
                input = Vector2Int.right;
            } else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) 
            {
                input = Vector2Int.left;
            }
        }
    }

    private void FixedUpdate()
    {
        SpeedText.text = "Speed: " + speed;
        ScoreText.text = "Score: " + score;


        // Wait until the next update before proceeding
        if (Time.time < nextUpdate) {
            return;
        }

        // Set the new direction based on the input
        if (input != Vector2Int.zero) {
            direction = input;
        }

        // Set each segment's position to be the same as the one it follows. We
        // must do this in reverse order so the position is set to the previous
        // position, otherwise they will all be stacked on top of each other.
        for (int i = segments.Count - 1; i > 0; i--) {
            segments[i].position = segments[i - 1].position;
        }

        // Move the snake in the direction it is facing
        // Round the values to ensure it aligns to the grid
        int x = Mathf.RoundToInt(transform.position.x) + direction.x;
        int y = Mathf.RoundToInt(transform.position.y) + direction.y;
        transform.position = new Vector2(x, y);

        // Set the next update time based on the speed
        nextUpdate = Time.time + (1f / (speed * speedMultiplier));
    }

    public void Grow()
    {
        Transform segment = Instantiate(segmentPrefab);
        segment.position = segments[segments.Count - 1].position;
        segments.Add(segment);
    }

    public void UnGrow()
    {      
        if(segments.Count >= initialSize)
        {
            Transform temp = segments[segments.Count - 1];
            segments.RemoveAt(segments.Count - 1);
            Destroy(temp.gameObject);
        }
    }

    public void SpeedUp()
    {
        if (speed < MAX_SPEED)
        {
            speed += speedIncreaser;
        }
    }

    public void Slow()
    {
        if (speed > MIN_SPEED)
        {
            speed -= speedDecreaser;
        }
    }

    public void ResetState()
    {
        direction = Vector2Int.right;
        transform.position = Vector3.zero;

        // Start at 1 to skip destroying the head
        for (int i = 1; i < segments.Count; i++) {
            Destroy(segments[i].gameObject);
        }

        // Clear the list but add back this as the head
        segments.Clear();
        segments.Add(transform);

        // -1 since the head is already in the list
        for (int i = 0; i < initialSize - 1; i++) {
            Grow();
        }
    }

    public bool Occupies(int x, int y)
    {
        foreach (Transform segment in segments)
        {
            if (Mathf.RoundToInt(segment.position.x) == x &&
                Mathf.RoundToInt(segment.position.y) == y) {
                return true;
            }
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Grow"))
        {
            score++;
            Grow();
            Destroy(other.gameObject);
            foodManager.SpawnFood();
            //input = -input;
        }
        else if (other.gameObject.CompareTag("UnGrow"))
        {
            score++;
            UnGrow();
            Destroy(other.gameObject);
            foodManager.SpawnFood();
            //input = -input;
        }
        else if (other.gameObject.CompareTag("SpeedUp"))
        {
            score++;
            SpeedUp();
            Destroy(other.gameObject);
            foodManager.SpawnSpeedFood();
            //input = -input;
        }
        else if (other.gameObject.CompareTag("Slow"))
        {
            score++;
            Slow();
            Destroy(other.gameObject);
            foodManager.SpawnSpeedFood();
            //input = -input;
        }
        else if (other.gameObject.CompareTag("Obstacle"))
        {
            ResetState();
        }
        else if (other.gameObject.CompareTag("Wall"))
        {
            if (moveThroughWalls) {
                Traverse(other.transform);
            } else {
                ResetState();
            }
        }
    }

    private void Traverse(Transform wall)
    {
        Vector3 position = transform.position;

        if (direction.x != 0f) {
            position.x = Mathf.RoundToInt(-wall.position.x + direction.x);
        } else if (direction.y != 0f) {
            position.y = Mathf.RoundToInt(-wall.position.y + direction.y);
        }

        transform.position = position;
    }

}
