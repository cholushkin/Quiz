using UnityEngine;
using UnityEngine.UI;

public class ImageSwitch : MonoBehaviour
{
    public Image Destination;
    public Texture2D[] ImagePool;

    public void Switch(int index)
    {
        var tex = ImagePool[index];
        Destination.overrideSprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
    }
}
