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
  }


  public void AddCircle(CirclePuzzle circlePuzzle)
  {
    if(circlePuzzle.hasIn)
        return;
    circlePuzzle.hasIn = true;
    var mid = Instantiate(Line);
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
  
  
  
  
}
