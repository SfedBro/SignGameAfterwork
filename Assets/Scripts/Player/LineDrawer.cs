using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Unity.Barracuda;

[System.Serializable]
public class ClassMapping
{
    public string className;
    public int classIndex;
}

public class LineDrawer : MonoBehaviour
{
    [Header("Drawing Settings")]
    public LineRenderer brushPrefab;
    public Camera cam;
    public float minDistanceForNewPoint = 0.1f;
    public float secondsForLineToLive = 3f;
    
    [Header("Classification Settings")]
    public NNModel modelAsset;
    public float confidenceThreshold = 0.5f;
    

    [SerializeField] private PlayerAttack playerAttack;
    
    List<string> elements = new List<string>
    {
        "Air1",    
        "Air2",    
        "Air3",    
        "Earth1",  
        "Earth2",  
        "Earth3",  
        "Fire1",   
        "Fire2",   
        "Fire3",   
        "Water1",  
        "Water2",  
        "Water3"   
    };

    private LineRenderer currentLine;
    private IWorker _worker;
    private Model _runtimeModel;
    int num = 1000;

    private void Start()
    {
        
        // Initialize Barracuda model
        if (modelAsset != null)
        {
            _runtimeModel = ModelLoader.Load(modelAsset);
            _worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, _runtimeModel);
        }
    }

    void Update() {
        Draw();
    }

    void Draw() {
        if (Input.GetMouseButtonDown(0)) {
            currentLine = Instantiate(brushPrefab);
            currentLine.transform.SetParent(transform, false);
            currentLine.SetPosition(0, GetMouseLocalPos());
        } 
        else if (Input.GetMouseButton(0)) {
            if (currentLine == null || DistanceToLastPoint(GetMouseLocalPos()) < minDistanceForNewPoint) 
                return;
                
            ++currentLine.positionCount;
            int posIndex = currentLine.positionCount - 1;
            currentLine.SetPosition(posIndex, GetMouseLocalPos());
        } 
        else if (Input.GetMouseButtonUp(0)) {
            StartCoroutine(RemoveLineAfter(currentLine, secondsForLineToLive));
            
            // Save and classify the line
            byte[] pngData = SaveLineToPNG(currentLine);
            string element;
            if (modelAsset != null)
            {
                element = ClassifyDrawing(pngData, num++);
                playerAttack.HandleInput(element);
                Debug.Log("Predictred class" + element + " with confidence" );
            }
            else {
                Debug.Log("Model is not assigned");

            }
            currentLine = null;
        }
    }

    byte[] SaveLineToPNG(LineRenderer line)
    {
        Bounds bounds = new Bounds(line.GetPosition(0), Vector3.zero);
        for (int i = 1; i < line.positionCount; i++)
        {
            bounds.Encapsulate(line.GetPosition(i));
        }

        float padding = 0.5f;
        bounds.Expand(padding);

        int width = Mathf.CeilToInt(bounds.size.x * 100);
        int height = Mathf.CeilToInt(bounds.size.y * 100);

        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        Color lineColor = Color.black;
        for (int i = 0; i < line.positionCount - 1; i++)
        {
            Vector3 start = line.GetPosition(i);
            Vector3 end = line.GetPosition(i + 1);

            Vector2 startTexCoord = new Vector2(
                (start.x - bounds.min.x) / bounds.size.x * width,
                (start.y - bounds.min.y) / bounds.size.y * height
            );
            Vector2 endTexCoord = new Vector2(
                (end.x - bounds.min.x) / bounds.size.x * width,
                (end.y - bounds.min.y) / bounds.size.y * height
            );

            DrawLineOnTexture(texture, startTexCoord, endTexCoord, lineColor, line.startWidth * 100);
        }

        texture.Apply();

        byte[] bytes = texture.EncodeToPNG();
        Destroy(texture);
        
        /*
        string directoryPath = Path.Combine(Application.dataPath, "Casts");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string filePath = Path.Combine(directoryPath, "water2-" + num + ".png");
        File.WriteAllBytes(filePath, bytes);
        Debug.Log("Line saved to " + filePath);
        */
        
        return bytes;
    }

    string ClassifyDrawing(byte[] pngData, int num)
    {
        Debug.Log("Start classfy " + num + "image");
        // Convert PNG byte array to Texture2D
        Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        texture.LoadImage(pngData);
        
        // Preprocess and create input tensor
        using Tensor inputTensor = PreprocessTexture(texture);
        
        // Execute the model
        _worker.Execute(inputTensor);
        
        // Get the output
        using Tensor outputTensor = _worker.PeekOutput();
        float[] predictions = outputTensor.ToReadOnlyArray();
        
        // Find best prediction
        int predictedClass = -1;
        float maxConfidence = 0f;
        
        for (int i = 0; i < predictions.Length; i++)
        {
            if (predictions[i] > maxConfidence)
            {
                maxConfidence = predictions[i];
                predictedClass = i;
            }
        }
        
        // Clean up
        UnityEngine.Object.Destroy(texture);
        
        // Display results
        
        if (! ((maxConfidence >= confidenceThreshold && predictedClass >= 0)))
        {
            Debug.Log( "Unknown drawing");
            return "None";
        }
        
        return elements[predictedClass];
    }

    Tensor PreprocessTexture(Texture2D texture)
    {
        // Resize and convert to grayscale
        Texture2D processedTexture = ResizeAndGrayscale(texture, 128, 128);
        
        // Create tensor
        TensorShape shape = new TensorShape(1, 128, 128, 1);
        float[] tensorData = new float[shape.length];
        
        // Fill tensor data (normalized 0-1)
        Color[] pixels = processedTexture.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            tensorData[i] = pixels[i].grayscale;
        }
        
        UnityEngine.Object.Destroy(processedTexture);
        return new Tensor(shape, tensorData);
    }

    Texture2D ResizeAndGrayscale(Texture2D source, int width, int height)
    {
        // Create render texture
        RenderTexture rt = RenderTexture.GetTemporary(width, height, 0, RenderTextureFormat.ARGB32);
        Graphics.Blit(source, rt);
        
        // Create texture and read pixels
        Texture2D result = new Texture2D(width, height, TextureFormat.RGBA32, false);
        RenderTexture.active = rt;
        result.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        result.Apply();
        
        // Convert to grayscale
        Color[] pixels = result.GetPixels();
        for (int i = 0; i < pixels.Length; i++)
        {
            float gray = pixels[i].grayscale;
            pixels[i] = new Color(gray, gray, gray);
        }
        result.SetPixels(pixels);
        result.Apply();
        
        // Clean up
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(rt);
        
        return result;
        
    }

    void DrawLineOnTexture(Texture2D tex, Vector2 start, Vector2 end, Color color, float width)
    {
        int x0 = Mathf.RoundToInt(start.x);
        int y0 = Mathf.RoundToInt(start.y);
        int x1 = Mathf.RoundToInt(end.x);
        int y1 = Mathf.RoundToInt(end.y);

        int dx = Mathf.Abs(x1 - x0);
        int dy = Mathf.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;

        while (true)
        {
            DrawCircle(tex, x0, y0, Mathf.RoundToInt(width / 2), color);

            if (x0 == x1 && y0 == y1) break;
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    void DrawCircle(Texture2D tex, int cx, int cy, int r, Color color)
    {
        int x, y, px, nx, py, ny, d;
        
        for (x = 0; x <= r; x++)
        {
            d = (int)Mathf.Ceil(Mathf.Sqrt(r * r - x * x));
            for (y = 0; y <= d; y++)
            {
                px = cx + x;
                nx = cx - x;
                py = cy + y;
                ny = cy - y;

                if (px >= 0 && px < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(px, py, color);
                if (nx >= 0 && nx < tex.width && py >= 0 && py < tex.height)
                    tex.SetPixel(nx, py, color);
                if (px >= 0 && px < tex.width && ny >= 0 && ny < tex.height)
                    tex.SetPixel(px, ny, color);
                if (nx >= 0 && nx < tex.width && ny >= 0 && ny < tex.height)
                    tex.SetPixel(nx, ny, color);
            }
        }
    }

    Vector3 GetMouseLocalPos() {
        Vector3 mouseDrawerPos = GetMouseWorldPos();
        Vector3 localPos = transform.InverseTransformPoint(mouseDrawerPos);
        return localPos;
    }

    float DistanceToLastPoint(Vector3 pos) {
        if (currentLine == null) return 0;
        return Vector3.Distance(currentLine.GetPosition(currentLine.positionCount - 1), pos);
    }

    IEnumerator RemoveLineAfter(LineRenderer line, float secondsToWait) {
        yield return new WaitForSeconds(secondsToWait);
        Destroy(line.gameObject);
    }

    public Vector3 GetMouseWorldPos() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane xy = new(Vector3.forward, new Vector3(0, 0, transform.position.z));
        xy.Raycast(ray, out float distance);
        return ray.GetPoint(distance);
    }
}