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
    private float PUZZLE_MIN_SIZE = 216f;
    private float PUZZLE_MIN_COUNT = 3;
    private int rowCount, columnCount;
    private float tableWidth, tableHeight;
    public GameObject dragTargetPrefab;
    private GameObject[,] dragTargets;

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
    }

    private void generateDragTarget()
    {
        float left, top, columnEven, rowEven;
        left = -tableWidth / 2f;
        top = tableHeight / 2f;
        columnEven = tableWidth / columnCount;
        rowEven = tableHeight / rowCount;
        Debug.Log("left:"+ left+ " ,top:"+ top+ " ,columnEven:"+ columnEven+ " ,rowEven:"+ rowEven);
        if (null != dragTargets) {
            foreach (GameObject go in dragTargets) {
                Destroy(go);
            }
        }
        dragTargets = new GameObject[columnCount,rowCount];
        for (int x = 0; x < columnCount; x++)
        {
            for (int y = 0; y < rowCount; y++)
            {
                GameObject dragTarget = Instantiate(dragTargetPrefab,uISpriteLoader.transform);
                dragTarget.transform.localPosition = new Vector3(left + x * columnEven + columnEven / 2f, top - y * rowEven - rowEven / 2f);
                dragTargets[x, y] = dragTarget;
            }
        }
    }
    public void nextImage()
    {
        uISpriteLoader.gameObject.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(TABLE_WIDTH, TABLE_HEIGHT);
        loadTexture();
    }
}
