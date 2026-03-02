using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BallSpawner : MonoBehaviour
{
    [Header("References")]
    public GameObject ballPrefab;
    public TMP_InputField inputField;
    public Button goBtn;
    public TMP_Text errorText;
    public GameManager gameManager;

    [Header("Spawn Settings")]
    public float spawnHeight = 5f;

    [Tooltip("Extra gap between balls")]
    public float spacingPadding = 0.1f;

    [Tooltip("Absolute minimum spawn radius")]
    public float minimumRadius = 1.5f;

    [Tooltip("Time to wait for balls to settle")]
    public float physicsSettleTime = 3f;

    private PlayerInputActions inputActions;
    private List<GameObject> spawnedBalls = new();
    private bool spawningLocked = false;


    void Awake()
    {
        inputActions = new PlayerInputActions();
        inputActions.UI.Submit.performed += _ => SpawnBalls(); // spawn balls when go button is pressed

        inputField.onValueChanged.AddListener(_ => ValidateInput()); // to validate input whenever the field value changes
    }

    void OnEnable() => inputActions.Enable();
    void OnDisable() => inputActions.Disable();

    private void ValidateInput()
    {
        if (spawningLocked)
            return;

        if (!int.TryParse(inputField.text, out int count))
            return;


        if (count <= 0 || count > 100) //input value should be between 1 and 100
        {
            goBtn.interactable = false; // disable the go button if input is invalid
            errorText.text = "Please enter a number between 1 and 100.";
            return;
        }

        goBtn.interactable = true;
        errorText.text = string.Empty;
    }

    public void SpawnBalls()
    {
        if (spawningLocked)
            return;

        LockSpawner();

        ClearBalls(); //clear previously spawned balls

        if (!int.TryParse(inputField.text, out int count))
            return;

        count = Mathf.Clamp(count, 1, 100);

        
        float radius = CalculateRadius(count);
        float angleStep = 360f / count; // angle steps to evenly distribute balls in a circle

        for (int i = 0; i < count; i++) //for every ball
        {
            float angle = i * angleStep * Mathf.Deg2Rad; 

            Vector3 pos = new Vector3(
                Mathf.Cos(angle) * radius,
                spawnHeight,
                Mathf.Sin(angle) * radius
            );

            GameObject ball = Instantiate(ballPrefab, pos, Quaternion.identity); //spawn ball at the calculated position

            spawnedBalls.Add(ball); //add the spawned ball to the list for later cleanup
        }
        StartCoroutine(EvaluateAfterDrop(radius));

    }

    public float CalculateRadius(int count)
    {
        float ballDiameter = GetBallDiameter();

        float minSpacing = ballDiameter + spacingPadding;

        float spacingRadius = (count * minSpacing) / (2f * Mathf.PI); // radius is required so balls don't collide

        return Mathf.Max(spacingRadius, minimumRadius); // final radius respects minimum radius
    }


    IEnumerator EvaluateAfterDrop(float spawnedRadius)
    {
        yield return new WaitForSeconds(physicsSettleTime);

        float targetRadius = gameManager.targetRadius;

        // Check if any ball escaped circle
        foreach (var ball in spawnedBalls)
        {
            Vector3 flatPos = new Vector3(
                ball.transform.position.x,
                0,
                ball.transform.position.z);

            float distance = flatPos.magnitude;

            if (distance > targetRadius)
            {
                gameManager.LevelFailed();
                yield break;
            }
        }

        //evaluate the round based on how close the spawned radius is to the target radius
        gameManager.EvaluateRound(spawnedRadius);
    }


    float GetBallDiameter() //getting the diameter of the ball to calculate spacing between each balls
    {
        SphereCollider col = ballPrefab.GetComponent<SphereCollider>(); // get the sphere collider to calculate the diameter of the ball

        float scale = ballPrefab.transform.localScale.x;

        return col.radius * 2f * scale; 
    }

    public void ClearBalls() //function to clear previously spawned balls before spawning new ones
    {
        foreach (var ball in spawnedBalls)
            Destroy(ball);

        spawnedBalls.Clear(); // clear the list after destroying the balls
    }

    public void LockSpawner()
    {
        spawningLocked = true;
        goBtn.interactable = false;
    }

    public void UnlockSpawner()
    {
        spawningLocked = false;
        ValidateInput();
    }


}