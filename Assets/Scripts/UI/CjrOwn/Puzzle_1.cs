using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

public class Puzzle_1 : MonoBehaviour
{
  static Puzzle_1 _instance;
  public CirclePuzzle current;
  public bool isTouched = false;
  public List<CirclePuzzle> haveDown;
  public GameObject Line;
  private GameObject Step;
  public CanvasGroup canvasGroup;

  public Sprite Ori;
  public Sprite Lighting;
  
  
  [FormerlySerializedAs("LineStack")] public List<GameObject> LineList;
  
  public Stack<GameObject> LineStack;
  
  public float moveDistance;
  public static Puzzle_1 instance
  {
    get
    {
      if(_instance == null)
        _instance = FindObjectOfType<Puzzle_1>();
      return _instance;
    }
  }

  public bool isResolved;

  private void Update()
  {
    if(canvasGroup.alpha <= 0.05f)
      gameObject.SetActive(false);
  }

  private void Awake()
  {
    isResolved = false;
   LineStack = new Stack<GameObject>();
    LineList = new List<GameObject>();
    haveDown = new List<CirclePuzzle>();//<CirclePuzzle>();
    current = null;
    canvasGroup = GetComponent<CanvasGroup>();
  }


  public void AddCircle(CirclePuzzle circlePuzzle)
  {
    if(circlePuzzle.hasIn)
        return;
    circlePuzzle.hasIn = true;
    var mid = Instantiate(Line);
    mid.GetComponent<LineRenderer>().positionCount = 2;
    mid.GetComponent<LineRenderer>().startWidth = 0.2f;
    mid.GetComponent<LineRenderer>().endWidth = 0.2f;
    mid.GetComponent<LineRenderer>().useWorldSpace = true;
    mid.GetComponent<Puzzle_line>().UpID=LineList.Count + 1;
    mid.GetComponent<LineRenderer>().SetPosition(0, circlePuzzle.transform.position);
    mid.GetComponent<LineRenderer>().SetPosition(1, circlePuzzle.transform.position);
    if (LineStack.Count > 0)
    {
      Vector3 midVector3=circlePuzzle.transform.position;
      midVector3.z -= 2;
      LineStack.Peek().GetComponent<LineRenderer>().SetPosition(1, midVector3);
      var midd = LineStack.Peek().GetComponent<LineRenderer>();
      Vector3 dir=(midd.GetPosition(1)-midd.GetPosition(0)).normalized;
      float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
      midd.transform.rotation=Quaternion.Euler(0, 0, angle);  
      var rb = LineStack.Peek().AddComponent<Rigidbody2D>();
      rb.bodyType = RigidbodyType2D.Static;
      var col= LineStack.Peek().AddComponent<BoxCollider2D>();
      col.isTrigger = true;
      col.size = new Vector2(col.size.x,0.2f);
      col.includeLayers = 255;
    }
    LineList.Add(mid);
    LineStack.Push(mid);
    current = circlePuzzle;
    haveDown.Add(circlePuzzle);
  }

  public void Init()
  {
    if (!isResolved)
    {
      Puzzle_1.instance.isTouched = false;
      LineList.Clear();
      foreach (var item in haveDown)
      {
        item.hasIn = false;
        if(item.isFirst)
          continue;
        item.circleImage.color = Color.white;
        item.circleImage.sprite = Ori;
        
      }
      haveDown.Clear();
      foreach (var item in LineStack)
      {
        Destroy(item.gameObject);
      }
      LineStack.Clear();
    }
  }

  public void CheckFinished()
  {
    
    
    
    
    if(haveDown.Count == 7)
      isResolved = true;
  }

  public void CancelClick()
  {
    canvasGroup.DOFade(0, 2);
    foreach (var item in LineStack)
    {
      Destroy(item.gameObject);
    }
    LineStack.Clear();
  }
}
