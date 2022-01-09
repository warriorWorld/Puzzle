using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public Canvas canvas;
    public static PuzzleManager instance;
    public UISpriteLoader uISpriteLoader;
    private float TABLE_WIDTH = 1080, TABLE_HEIGHT = 1920;
    private float PUZZLE_MIN_SIZE = 216f, PUZZLE_MAX_SIZE = 282f;
    private float PUZZLE_MIN_COUNT = 3;
    private int rowCount, columnCount;
    private float tableWidth, tableHeight;
    public GameObject dragItemsTable;
    public GameObject dragTargetPrefab;
    private GameObject[,] dragTargets, dragItems;
    public GameObject ltPrefab, mtPrefab, rtPrefab, lmPrefab, mmPrefab, rmPrefab, lbPrefab, mbPrefab, rbPrefab;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        loadTexture();
    }

    private void loadTexture()
    {
#if UNITY_EDITOR
        loadTextureFromPC();
        return;
#endif
        loadTextureFromAndroid();
    }

    private void loadTextureFromPC()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "JPGͼƬ|*.jpg|PNGͼƬ|*.png|JPEGͼƬ|*.jpeg";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string path = dialog.FileName;
            Debug.Log("selected path:" + path);
            uISpriteLoader.setURL(path, () =>
            {
                measurePuzzlePiece();
            });
        }
    }

    private void loadTextureFromAndroid()
    {

    }

    private void measurePuzzlePiece()
    {
        //计算table宽高
        RectTransform rectTransform = uISpriteLoader.gameObject.GetComponent<RectTransform>();
        tableWidth = rectTransform.sizeDelta.x;
        tableHeight = rectTransform.sizeDelta.y;
        Debug.Log("width:" + tableWidth + " ,height:" + tableHeight);
        if (tableWidth < PUZZLE_MIN_COUNT * PUZZLE_MIN_SIZE || tableHeight < PUZZLE_MIN_SIZE * PUZZLE_MIN_COUNT)
        {
            //不满足最小尺寸要求
            nextImage();
            return;
        }

        //计算需要的item数量
        rowCount = (tableHeight % PUZZLE_MIN_SIZE == 0) ? (int)(tableHeight / PUZZLE_MIN_SIZE) : (int)(tableHeight / PUZZLE_MIN_SIZE) + 1;
        columnCount = (tableWidth % PUZZLE_MIN_SIZE == 0) ? (int)(tableWidth / PUZZLE_MIN_SIZE) : (int)(tableWidth / PUZZLE_MIN_SIZE) + 1;
        Debug.Log("rowCount:" + rowCount + " ,columnCount:" + columnCount);

        //生成
        generateDragTarget();
        generateDragItem();
    }

    private void generateDragTarget()
    {
        float left, top, columnEven, rowEven;
        left = -tableWidth / 2f;
        top = tableHeight / 2f;
        columnEven = tableWidth / columnCount;
        rowEven = tableHeight / rowCount;
        Debug.Log("left:" + left + " ,top:" + top + " ,columnEven:" + columnEven + " ,rowEven:" + rowEven);
        if (null != dragTargets)
        {
            foreach (GameObject go in dragTargets)
            {
                Destroy(go);
            }
        }
        dragTargets = new GameObject[columnCount, rowCount];
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                GameObject dragTarget = Instantiate(dragTargetPrefab, uISpriteLoader.transform);
                dragTarget.transform.localPosition = new Vector3(left + x * columnEven + columnEven / 2f, top - y * rowEven - rowEven / 2f);
                dragTargets[x, y] = dragTarget;
            }
        }
    }

    private void generateDragItem()
    {
        if (null != dragItems)
        {
            foreach (GameObject go in dragItems)
            {
                Destroy(go);
            }
        }
        dragItems = new GameObject[columnCount, rowCount];
        Texture2D texture2D = uISpriteLoader.gameObject.GetComponent<Image>().sprite.texture;
        Debug.Log("texture2D size:" + texture2D.width + "," + texture2D.height);
        float sizeRatio = uISpriteLoader.GetComponent<RectTransform>().sizeDelta.x / texture2D.width;
        float fixedMaxSize = PUZZLE_MAX_SIZE / sizeRatio;
        float fixedMinSize = PUZZLE_MIN_SIZE / sizeRatio;
        Vector2 anchor = new Vector2(0.5f, 0.5f);
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                GameObject dragItem = Instantiate(getDragItemPrefabByCoordinates(x, y), dragItemsTable.transform);
                Image image = dragItem.GetComponent<Dragable>().image;

                //0,0点为左下角，无论是被裁的还是裁的结果都是
                image.sprite = Sprite.Create(texture2D, getDragItemRect(image.GetComponent<RectTransform>(),texture2D.height,x,y,fixedMaxSize,fixedMinSize,sizeRatio), anchor);
                image.preserveAspect = true;

                dragItems[x, y] = dragItem;
            }
        }
    }

    //0,0点为左下角，无论是被裁的还是裁的结果都是
    private Rect getDragItemRect(RectTransform imageRectTransform,float textureHeight,int x, int y, float fixedMaxSize, float fixedMinSize,float sizeRatio)
    {
        float startX, startY, width, height;

        startX = x * fixedMinSize;
        width = (x == columnCount - 1) ? fixedMinSize : fixedMaxSize;//因为所有R的宽度都是小值
        startY = textureHeight - fixedMaxSize* (y+1)+(fixedMaxSize-fixedMinSize)*y;//y的开始点是最下边，所以是顶点坐标-大值再去掉拼图的凸起即大值-小值
        height = (y == rowCount - 1) ? fixedMinSize : fixedMaxSize;
        if (startY < 0)
        {
            height = height + startY;
            imageRectTransform.localPosition -= new Vector3(0,startY/2*sizeRatio,0);
            Debug.Log("startY:" + startY + " ,height" + height+ " ,sizeRatio:"+ sizeRatio);
            startY = 0;
        }
        return new Rect(startX,startY,width,height);
    }

    private GameObject getDragItemPrefabByCoordinates(int x, int y)
    {
        if (x == 0)
        {
            if (y == 0)
            {
                return ltPrefab;
            }
            else if (y == rowCount - 1)
            {
                return lbPrefab;
            }
            else
            {
                return lmPrefab;
            }
        }
        else if (x == columnCount - 1)
        {
            if (y == 0)
            {
                return rtPrefab;
            }
            else if (y == rowCount - 1)
            {
                return rbPrefab;
            }
            else
            {
                return rmPrefab;
            }
        }
        else
        {
            if (y == 0)
            {
                return mtPrefab;
            }
            else if (y == rowCount - 1)
            {
                return mbPrefab;
            }
            else
            {
                return mmPrefab;
            }
        }
    }

    public void nextImage()
    {
        uISpriteLoader.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(TABLE_WIDTH, TABLE_HEIGHT);
        loadTexture();
    }
}
