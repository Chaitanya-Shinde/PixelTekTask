using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CircleTarget : MonoBehaviour
{
    [SerializeField] int segments = 100;

    private LineRenderer line;
    public float Radius { get; private set; }

    void Awake()
    {
        line = GetComponent<LineRenderer>();
        line.loop = true;
    }

    public void SetRadius(float radius)
    {
        Radius = radius;
        DrawCircle();
    }

    void DrawCircle()
    {
        line.positionCount = segments;

        float angleStep = 360f / segments;

        for (int i = 0; i < segments; i++)
        {
            float angle = Mathf.Deg2Rad * angleStep * i;

            Vector3 pos = new Vector3( Mathf.Cos(angle) * Radius, 1.5f, Mathf.Sin(angle) * Radius );

            line.SetPosition(i, pos);
        }
    }
}