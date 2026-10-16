/*To replicate the behavior of Blender’s MeasureIt add-on in Unity (dynamic measurement lines, arrow end-caps, and text overlays in 3D space), you cannot simply export them from Blender because MeasureIt generates its visualizations using Blender's viewport drawing APIs (not actual mesh geometry).
Instead, you can rebuild this functionality dynamically in Unity using LineRenderer, a custom script, and TextMeshPro for the text.

Step 1: Create the Arrow/Measurement Shader (Optional but Recommended)

To prevent your lines and arrows from clipping into your 3D geometry (so they always display cleanly on top like an overlay), create a material using a shader with ZTest Always enabled. If you just want standard behavior, a simple Unlit Color material will do.

Step 2: Write the Measurement Script

Attach this script to an empty GameObject in Unity to control your start point, end point, line width, and arrow settings.
csharp*/

using UnityEngine;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class UnityMeasureIt : MonoBehaviour
{
    [Header("Points")]
    public Transform startPoint;
    public Transform endPoint;

    [Header("Line Settings")]
    public float lineWidth = 0.05px;
    public Color measurementColor = Color.yellow;
    
    [Header("Text settings")]
    public TextMeshPro textMesh;
    public string unitSuffix = "m";
    public float textOffsetBuffer = 0.2f;

    [Header("Arrow Cap Settings")]
    public MeshFilter startArrowCap;
    public MeshFilter endArrowCap;
    public float arrowScale = 0.2f;

    private LineRenderer lineRenderer;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        
        // Configure LineRenderer properties
        lineRenderer.useWorldSpace = true;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        
        // Ensure material handles colors correctly
        lineRenderer.startColor = measurementColor;
        lineRenderer.endColor = measurementColor;
    }

    void Update()
    {
        if (startPoint == null || endPoint == null) return;

        Vector3 pStart = startPoint.position;
        Vector3 pEnd = endPoint.position;
        Vector3 direction = (pEnd - pStart).normalized;
        float distance = Vector3.Distance(pStart, pEnd);

        // 1. Draw the Line
        lineRenderer.SetPosition(0, pStart);
        lineRenderer.SetPosition(1, pEnd);

        // 2. Position and Update Text
        if (textMesh != null)
        {
            // Center the text between points
            textMesh.transform.position = Vector3.Lerp(pStart, pEnd, 0.5f) + (Vector3.up * textOffsetBuffer);
            
            // Format to 2 decimal places matching 1 unit = 1 meter
            textMesh.text = $"{distance:F2} {unitSuffix}"; 
            
            // Optional: Make text face the camera
            textMesh.transform.rotation = Quaternion.LookRotation(textMesh.transform.position - Camera.main.transform.position);
        }

        // 3. Position and Rotate Arrow Caps
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            if (startArrowCap != null)
            {
                startArrowCap.transform.position = pStart;
                // Rotate 180 degrees so the arrow points outward/backward
                startArrowCap.transform.rotation = lookRotation * Quaternion.Euler(0, 180, 0); 
                startArrowCap.transform.localScale = Vector3.one * arrowScale;
            }

            if (endArrowCap != null)
            {
                endArrowCap.transform.position = pEnd;
                endArrowCap.transform.rotation = lookRotation;
                endArrowCap.transform.localScale = Vector3.one * arrowScale;
            }
        }
    }
}

/*Step 3: Set Up the Scene Components

1. The Caps: Create a simple 3D Cone or Pyramidal mesh to act as your arrow. Save it as a Prefab.
2. The Structure: Create an Empty GameObject named Measurement_Segment. Attach the UnityMeasureIt script to it.
3. Add Children: Inside your empty object, instantiate:
	• Two empty GameObjects (Start_Point and End_Point).
	• A TextMeshPro - Text component (configured as a 3D object, not UI canvas).
	• Two instances of your Arrow Prefab.
4. Link Fields: Drag and drop these newly created components into the matching Inspector slots on the script.*/
