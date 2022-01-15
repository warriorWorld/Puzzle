using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UniRx;

public class CompleteAnim : MonoBehaviour
{
    public string completeText = "PuzzleSolved£¡";
    public GameObject pusherPrefab;

    public void startAnim()
    {
        char[] split = completeText.ToCharArray();
        Debug.Log("split£º" + split.Length);

        Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(0.3)).Zip<long, char, string>(split.ToObservable(), (l, t) => {
            return t.ToString();
        }).Subscribe(text =>
        {
            GameObject gameObject = Instantiate(pusherPrefab, transform);
            gameObject.GetComponent<Pusher>().setText(text);
        }).AddTo(this);
    }
}
