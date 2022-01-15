using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class CompleteAnim : MonoBehaviour
{
    public string completeText = "PuzzleSolved£¡";
    public Pusher pusherPrefab;

     void Start()
    {
        startAnim();
    }

    public void startAnim()
    {
        //string[] split = completeText.Split();
        //split.ToObservable().
        //Observable.Timer(TimeSpan.FromSeconds(0),TimeSpan.FromSeconds(1)).Subscribe(l=> {
        //    Instantiate(pusherPrefab, transform);
        //}).AddTo(this);
    }
     void Update()
    {
        
    }
}
