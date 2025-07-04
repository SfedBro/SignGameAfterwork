using System.Collections;
using UnityEngine;
using System.IO;

public class LineDrawer : MonoBehaviour
{
    public LineRenderer brushPrefab;
    public Camera cam;
    public float minDistanceForNewPoint = 0.1f;
    public float secondsForLineToLive = 3f;
    public string savePath = "SavedLines";

    LineRenderer currentLine;
    
    void Update() {
        Draw();
    }

    void Draw() {
        if (Input.GetMouseButtonDown(0)) {
            currentLine = Instantiate(brushPrefab);
            currentLine.transform.SetParent(transform, false);
            currentLine.SetPosition(0, GetMouseLocalPos());
        } else if (Input.GetMouseButton(0)) {
            if (DistanceToLastPoint(GetMouseLocalPos()) < minDistanceForNewPoint) return;
            ++currentLine.positionCount;
            int posIndex = currentLine.positionCount - 1;
            currentLine.SetPosition(posIndex, GetMouseLocalPos());
        } else if (Input.GetMouseButtonUp(0)) {
            StartCoroutine(RemoveLineAfter(currentLine, secondsForLineToLive));
            SaveLineToPNG(currentLine);
            currentLine = null;
        }
    }

    void SaveLineToPNG(LineRenderer line)
    {
        // Get the bounds of the line
        Bounds bounds = new Bounds(line.GetPosition(0), Vector3.zero);
        for (int i = 1; i < line.positionCount; i++)
        {
            bounds.Encapsulate(line.GetPosition(i));
        }

        // Add some padding
        float padding = 0.5f;
        bounds.Expand(padding);

        // Calculate texture dimensions based on line bounds
        int width = Mathf.CeilToInt(bounds.size.x * 100);
        int height = Mathf.CeilToInt(bounds.size.y * 100);

        // Create a texture with white background
        Texture2D texture = new Texture2D(width, height, TextureFormat.ARGB32, false);
        Color[] pixels = new Color[width * height];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.white;
        }
        texture.SetPixels(pixels);
        texture.Apply();

        // Draw the line onto the texture
        for (int i = 0; i < line.positionCount - 1; i++)
        {
            Vector3 start = line.GetPosition(i);
            Vector3 end = line.GetPosition(i + 1);

            // Convert world positions to texture coordinates
            Vector2 startTexCoord = new Vector2(
                (start.x - bounds.min.x) / bounds.size.x * width,
                (start.y - bounds.min.y) / bounds.size.y * height
            );
            Vector2 endTexCoord = new Vector2(
                (end.x - bounds.min.x) / bounds.size.x * width,
                (end.y - bounds.min.y) / bounds.size.y * height
            );

            // Draw line segment on texture
            DrawLineOnTexture(texture, startTexCoord, endTexCoord, line.startColor, line.startWidth * 100);
        }

        // Apply the changes
        texture.Apply();

        // Encode texture to PNG
        byte[] bytes = texture.EncodeToPNG();
        Destroy(texture);
        
        // Ensure directory exists
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        
        // Save file
        string fileName = System.DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".png";
        File.WriteAllBytes(Path.Combine(savePath, fileName), bytes);
        
        Debug.Log("Line saved to " + Path.Combine(savePath, fileName));
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
            // Draw a circle at each point with the line width
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