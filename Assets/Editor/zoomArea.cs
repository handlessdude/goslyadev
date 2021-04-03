using UnityEngine;
using UnityEditor;

//ONLY MODIFIED BY GOSLYADEV, NOT WRITTEN ENTIRELY
// source: https://gist.github.com/MattRix/7b4da243dacd43025bc6eae2ee3c3a1a
public static class RectExtensions
{
    public static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }

    public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.xMax *= scale;
        result.yMin *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
    {
        return rect.ScaleSizeBy(scale, rect.center);
    }
    public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale.x;
        result.xMax *= scale.x;
        result.yMin *= scale.y;
        result.yMax *= scale.y;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
}

public class EditorZoomArea
{
    private const float kEditorWindowTabHeight = 21.0f;
    private static Matrix4x4 _prevGuiMatrix;

    public static Rect Begin(float zoomScale, Rect screenCoordsArea)
    {
        return Begin(zoomScale, screenCoordsArea, screenCoordsArea.TopLeft());
    }

    static void Test()
    {
        Rect r = new Rect(0, 0, 10, 5);
        float sc = 2f;
        Rect r2 = r.ScaleSizeBy(1.0f / sc, r.center);
        Debug.Log(r2.TopLeft());
    }

    //TODO: implement decent pivot point scaling
    public static Rect Begin(float zoomScale, Rect screenCoordsArea, Vector2 pivotPoint)
    {
        //pivotPoint = new Vector2(400f, 300f);
        GUI.EndGroup();        // End the group Unity begins automatically for an EditorWindow to clip out the window tab. This allows us to draw outside of the size of the EditorWindow.
        //Test();
        Rect clippedArea = screenCoordsArea.ScaleSizeBy(1.0f / zoomScale, pivotPoint);
        clippedArea.y += kEditorWindowTabHeight;
        GUI.BeginGroup(clippedArea);
        _prevGuiMatrix = GUI.matrix;

        
        Matrix4x4 translation = Matrix4x4.TRS(-clippedArea.TopLeft()/zoomScale, Quaternion.identity, Vector3.one);
        Matrix4x4 scale = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1.0f));
        GUIUtility.ScaleAroundPivot(new Vector2(zoomScale, zoomScale), pivotPoint/zoomScale);
        //GUI.matrix = GUI.matrix * Matrix4x4.Translate(pivotPoint/zoomScale);
        //GUI.matrix = translation * scale * translation.inverse * GUI.matrix;
        //GUI.matrix = scale * GUI.matrix;

        return clippedArea;
    }

    public static void End()
    {
        GUI.matrix = _prevGuiMatrix;
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0.0f, kEditorWindowTabHeight, Screen.width, Screen.height));
    }
}
