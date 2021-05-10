using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeCursor : MonoBehaviour
{
    public void SetCursor(Texture2D texture) 
    {
        Cursor.SetCursor(texture, new Vector2(0, 0), CursorMode.Auto);
    }

}
