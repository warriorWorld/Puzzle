using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class PuzzleManager : MonoBehaviour
{
    public static PuzzleManager instance;
    [SerializeField]
    RawImage rawImage;

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

    // Update is called once per frame
    void Update()
    {

    }

    void loadTexture()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        dialog.Filter = "JPGͼƬ|*.jpg|PNGͼƬ|*.png|JPEGͼƬ|*.jpeg";
        if (dialog.ShowDialog() == DialogResult.OK)
        {
            string path = dialog.FileName;
            Debug.Log("selected path:" + path);
            Texture2D texture2D = new Texture2D(1, 2);
            byte[] textureBytes = File.ReadAllBytes(path);
            texture2D.LoadImage(textureBytes);
            rawImage.texture = texture2D;
        }
    }
}
