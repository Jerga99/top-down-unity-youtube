using UnityEngine;

[ExecuteInEditMode]
public class GridDrawer : MonoBehaviour
{
    public int gridSize = 10;            // Number of units in each direction
    public float gridSpacing = 1f;       // Distance between grid lines
    public Material lineMaterial;        // Material for the grid lines
    public Font numberFont;              // Font for the axis numbers
    public Color gridColor = Color.gray; // Color of the grid lines

    void Start()
    {
        DrawGrid();
    }

    void DrawGrid()
    {
        // Clear existing grid lines and numbers
        foreach (Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        // Draw lines parallel to the X-axis (varying Z)
        for (int i = -gridSize; i <= gridSize; i++)
        {
            float z = i * gridSpacing;
            Vector3 start = new Vector3(-gridSize * gridSpacing, 0, z);
            Vector3 end = new Vector3(gridSize * gridSpacing, 0, z);
            DrawLine(start, end, gridColor);

            // Label the Z-axis
            if (i != 0)
                CreateNumberLabel(new Vector3(0, 0, z), i.ToString());
        }

        // Draw lines parallel to the Z-axis (varying X)
        for (int i = -gridSize; i <= gridSize; i++)
        {
            float x = i * gridSpacing;
            Vector3 start = new Vector3(x, 0, -gridSize * gridSpacing);
            Vector3 end = new Vector3(x, 0, gridSize * gridSpacing);
            DrawLine(start, end, gridColor);

            // Label the X-axis
            if (i != 0)
                CreateNumberLabel(new Vector3(x, 0, 0), i.ToString());
        }

        // Label the origin
        CreateNumberLabel(new Vector3(0, 0, 0), "0");
    }

    float lineOffset = 0.0001f; // Small offset to prevent z-fighting

    void DrawLine(Vector3 start, Vector3 end, Color color)
    {
        start.y += lineOffset;
        end.y += lineOffset;
        GameObject lineObj = new GameObject("Line");
        lineObj.transform.parent = transform;

        LineRenderer lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = lineMaterial;
        lineRenderer.startWidth = 0.05f;
        lineRenderer.endWidth = 0.05f;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
        lineRenderer.useWorldSpace = true;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new GradientAlphaKey[] { new GradientAlphaKey(color.a, 0f), new GradientAlphaKey(color.a, 1f) }
        );
        lineRenderer.colorGradient = gradient;
    }

    void CreateNumberLabel(Vector3 position, string text)
    {
        GameObject textObj = new GameObject("NumberLabel");
        textObj.transform.parent = transform;
        textObj.transform.position = position + new Vector3(0, 0.01f, 0); // Slightly above ground to prevent z-fighting

        TextMesh textMesh = textObj.AddComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = 24;
        textMesh.characterSize = 0.2f;
        textMesh.anchor = TextAnchor.MiddleCenter;
        textMesh.alignment = TextAlignment.Center;
        textMesh.color = Color.black;

        if (numberFont != null)
        {
            textMesh.font = numberFont;
            textMesh.GetComponent<MeshRenderer>().material = numberFont.material;
        }

        // Rotate text to face upwards
        textObj.transform.rotation = Quaternion.Euler(90, 0, 0);
    }
}
