using System;
using System.Collections;
using System.Collections.Generic;
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

  private void Awake()
  {
    isResolved = false;
   LineStack = new Stack<GameObject>();
    LineList = new List<GameObject>();
    haveDown = new List<CirclePuzzle>();//<CirclePuzzle>();
    current = null;
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
    mid.GetComponent<LineRenderer>().SetPosition(0, circlePuzzle.transform.position);
    mid.GetComponent<LineRenderer>().SetPosition(1, circlePuzzle.transform.position);
    if (LineStack.Count > 0)
    {
      Debug.Log(circlePuzzle.name);
      Debug.LogWarning(LineStack.Peek().GetComponent<LineRenderer>().GetPosition(0));
      Debug.LogWarning(LineStack.Peek().GetComponent<LineRenderer>().GetPosition(1));
      Vector3 midVector3=circlePuzzle.transform.position;
      midVector3.z -= 2;
      LineStack.Peek().GetComponent<LineRenderer>().SetPosition(1, midVector3);
    }
    LineList.Add(mid);
    LineStack.Push(mid);
    current = circlePuzzle;
    haveDown.Add(circlePuzzle);
    LineStack.Peek().GetComponent<Puzzle_line>().lineRenderer.SetPosition(0,current.transform.position);
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
        if(item.CANNEED)
          item.circleImage.color = Color.white;
        
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
  
  
}
