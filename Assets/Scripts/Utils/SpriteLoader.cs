using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteLoader : MonoBehaviour
{
    public string url;
    //[LabelText("图片尺寸类型")]
    public LoadImageSizeType loadImageSizeType=LoadImageSizeType.GameObjectSize;
   // [EnableIf("@this.loadImageSizeType==LoadImageSizeType.Fixed||this.loadImageSizeType==LoadImageSizeType.AdapteToWidth")]
    public float width;
    //[EnableIf("@this.loadImageSizeType==LoadImageSizeType.Fixed||this.loadImageSizeType==LoadImageSizeType.AdapteToHeight")]
    public float height;
    //[LabelText("锚点")]
    public Vector2 anchor = new Vector2(0.5f, 0.5f);
    public float animDuration = 0.3f;
    private SpriteRenderer spriteRenderer;

     void Awake()
    {
         spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        loadImage();
    }

    public void setUrl(string url) {
        this.url = url;
        loadImage();
    }

    private void loadImage()
    {
        if (url != null && url != "")
        {
            SpriteUtil.loadImageToSprite(url, spriteRenderer, loadImageSizeType, width, height, anchor, animDuration, null);
        }
    }
}
