using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundControll : MonoBehaviour
{
    private Image image;
    public Sprite enableSprite;
    public Sprite disableSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    // Start is called before the first frame update
    void Start()
    {
        bool soundOpen = PlayerPrefs.GetInt(ShareKeys.SOUND_STATE_KEY) == 0;
        changeSprite(soundOpen);
    }

    public void toggleSound() {
        bool soundOpen = PlayerPrefs.GetInt(ShareKeys.SOUND_STATE_KEY)==0;
        soundOpen = !soundOpen;

        PlayerPrefs.SetInt(ShareKeys.SOUND_STATE_KEY, soundOpen ? 0 : 1);
        changeSprite(soundOpen);
    }

    private void changeSprite(bool soundOpen) {
        image.sprite = soundOpen ? enableSprite : disableSprite;
    }
}
