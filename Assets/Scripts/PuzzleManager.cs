using System;
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
    public RectTransform solvedGroup, unsolvedGroup;
    public RectTransform topLine;
    public RectTransform bottomLine;
    private int solvedCount=0;
    public Image puzzleImage;
    public CompleteAnim completeAnim;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    private void loadTextureFromPC()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "JPGͼƬ|*.jpg|PNGͼƬ|*.png|JPEGͼƬ|*.jpeg";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string path = dialog.FileName;
            preparePuzzle(path);
        }
    }


    private void preparePuzzle(string path) {
        Debug.Log("selected path:" + path);
        NativeCaller.showLoading();
        uISpriteLoader.setURL(path, () =>
        {
            measurePuzzlePiece();
            adjustBottomLine();
            //生成
            generateDragTarget();
            generateDragItem();
            NativeCaller.dismissLoading();
        });
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
            //TODO 不满足最小尺寸要求
            nextImage();
            return;
        }

        //计算需要的item数量
        rowCount = (tableHeight % PUZZLE_MIN_SIZE == 0||tableHeight%PUZZLE_MIN_SIZE<=PUZZLE_MIN_SIZE/2) ? (int)(tableHeight / PUZZLE_MIN_SIZE) : (int)(tableHeight / PUZZLE_MIN_SIZE) + 1;
        columnCount = (tableWidth % PUZZLE_MIN_SIZE == 0) ? (int)(tableWidth / PUZZLE_MIN_SIZE) : (int)(tableWidth / PUZZLE_MIN_SIZE) + 1;
        Debug.Log("rowCount:" + rowCount + " ,columnCount:" + columnCount);
    }

    private void adjustBottomLine() {
        Debug.Log("bottom:"+bottomLine.position.ToString()+" ,"+bottomLine.anchoredPosition.ToString()+" ,"+bottomLine.localPosition.ToString());
        bottomLine.anchoredPosition = new Vector2(0, tableHeight - rowCount * PUZZLE_MIN_SIZE) ;
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
        int id=0;
        for (int x = 0; x < columnCount; x++)
        {
            float lastY=0;
            for (int y = 0; y < rowCount; y++)
            {
                GameObject dragTarget = Instantiate(dragTargetPrefab, uISpriteLoader.transform);
                lastY = y == 0 ? top - PUZZLE_MIN_SIZE / 2f : lastY - PUZZLE_MIN_SIZE;
                dragTarget.transform.localPosition = new Vector3(left + x * columnEven + columnEven / 2f,lastY);
                dragTargets[x, y] = dragTarget;
                DragTarget dragTargetScript = dragTarget.GetComponent<DragTarget>();
                dragTargetScript.setId(id);
                dragTargetScript.setPuzzleManager(this);
                id++;
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
        int id = 0;
        List<int> randomTemp = new List<int>();
        for (int i = 0; i < columnCount * rowCount; i++)
        {
            randomTemp.Add(i);
        }
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                //获取随机值
                int randomTempIndex = new System.Random(DateTime.Now.Millisecond).Next(randomTemp.Count);
                int index = randomTemp[randomTempIndex];
                randomTemp.RemoveAt(randomTempIndex);
                Debug.Log("getBornPoint childCount:" + dragItemsTable.transform.childCount + " ,index:" + index);
                //生成
                GameObject dragItem = Instantiate(getDragItemPrefabByCoordinates(x, y), getDragTargetByIndex(index));
                Dragable dragable = dragItem.GetComponent<Dragable>();
                Image image = dragable.image;

                //0,0点为左下角，无论是被裁的还是裁的结果都是
                image.sprite = Sprite.Create(texture2D, getDragItemRect(image.GetComponent<RectTransform>(),texture2D.height,x,y,fixedMaxSize,fixedMinSize,sizeRatio), anchor);
                image.preserveAspect = true;

                dragable.setFutureParent(solvedGroup,unsolvedGroup);
                dragItems[x, y] = dragItem;
                dragable.setId(id);
                id++;
            }
        }
    }

    private Transform getDragTargetByIndex(int index) {
        int y = index / columnCount;
        int x = index % columnCount;
        return dragTargets[x, y].transform;
    }

    //0,0点为左下角，无论是被裁的还是裁的结果都是
    private Rect getDragItemRect(RectTransform imageRectTransform,float textureHeight,int x, int y, float fixedMaxSize, float fixedMinSize,float sizeRatio)
    {
        float startX, startY, width, height;

        startX = x * fixedMinSize;
        width = (x == columnCount - 1) ? fixedMinSize : fixedMaxSize;//因为所有R的宽度都是小值
        startY = textureHeight - fixedMaxSize * (y + 1) + (fixedMaxSize - fixedMinSize) * y;//y的开始点是最下边，所以是顶点坐标-大值再去掉拼图的凸起即大值-小值
        if (y == rowCount - 1) {
            //最底下的那个拼图没有Y方向没有凸起
            startY += (fixedMaxSize - fixedMinSize);
        }
        height = (y == rowCount - 1) ? fixedMinSize : fixedMaxSize;
        //超出图片界限处理
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

    public void solvedOne()
    {
        solvedCount++;
        if (solvedCount >= rowCount * columnCount) {
            Debug.Log("Puzzle Over");
            //全部拼完，检查是否正确
            if (checkPuzzleCompleted())
            {
                //GAME OVER 
                Debug.Log("Game Over");
                foreach (GameObject go in dragItems)
                {
                    Destroy(go);
                }
                dragItems = null;
                puzzleImage.enabled = true;
                topLine.gameObject.SetActive(false);
                bottomLine.gameObject.SetActive(false);
                completeAnim.startAnim();
            }
        }
    }
    private bool checkPuzzleCompleted() {
        foreach(GameObject gameObject in dragItems)
        {
            Dragable dragable = gameObject.GetComponent<Dragable>();
            if (!dragable.isCorrect())
            {
                return false;
            }
        }
        return true;
    }

    public void unsolvedOne()
    {
        solvedCount--;
    }

    public void onStartClick()
    {
        NativeCaller.loadNextImage();
    }

    public void onCameraClick()
    {
        NativeCaller.camera();
    }

    //PC端调用
    public void nextImage()
    {
        resetValue();
        loadTextureFromPC();
    }

    //ANDROID端调用
    public void nextAndroidImage(string path)
    {
        resetValue();
        preparePuzzle(path);
    }

    private void resetValue()
    {
        uISpriteLoader.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(TABLE_WIDTH, TABLE_HEIGHT);
        solvedCount = 0;
        puzzleImage.enabled = false;
        topLine.gameObject.SetActive(true);
        bottomLine.gameObject.SetActive(true);
    }
}
