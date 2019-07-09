using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class SetAction
{
    public ActionState PonitAI;
    [Range(0, 1)]
    public float RandomValue = 0.33f;
    //[HideInInspector]
    public int RangeNub;
}

public enum ActionState
{
    Idel,
    Walk,
    Attack
}
public class FaRMControl : MonoBehaviour
{

    [Header("寻路点组")]
    [SerializeField]
    private Transform PointGroup;
    private List<Transform> WayPoint = new List<Transform>();
    private Animator FarmAnimator;
    [Header("初始动作")]
    public ActionState MyActionType;
    public bool IsWalk;
    [Header("寻路速度")]
    [Range(1, 50)]
    public int Speed = 10;
    [Header("转弯速度")]
    [Range(1, 50)]
    public int TurnSpeed = 2;
    [Header("状态变换间隔")]
    [Range(6, 100)]
    public int Interval = 20;

    [HideInInspector]
    public Transform NowPoint;

    public void Start()
    {
        foreach (Transform child in PointGroup)
            WayPoint.Add(child);

        PointGroup.gameObject.SetActive(false);
        FarmAnimator = GetComponent<Animator>();
        UpdateWayPoint();
    }


    void Update()
    {
        if (IsWalk)
        {
            if (Vector3.Distance(transform.position, NowPoint.position) <= 0.2f)
            {
                IsWalk = false;
                SetPoint(CalculateAction(NowPoint.GetComponent<AIpoint>().MyAction));
            }
            Vector3 relativePos = NowPoint.position - transform.position;
            Quaternion rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(relativePos.normalized), Time.deltaTime * TurnSpeed);
            transform.rotation = rotation;
            transform.Translate(Vector3.forward * Speed * Time.deltaTime / 10);
        }
    }


    public ActionState CalculateAction(SetAction[] GetActionGroup)
    {
        ActionState CopAction = ActionState.Idel;
        int AddNub = 0;
        foreach (SetAction child in GetActionGroup)
        {
            AddNub += (int)(child.RandomValue * 100);
            child.RangeNub = AddNub;
        }


        int Nub = Random.Range(0, GetActionGroup.Last().RangeNub);

        foreach (SetAction child in GetActionGroup)
        {
            if (Nub < child.RangeNub)
            {
                CopAction = child.PonitAI;
                break;
            }
        }

        return CopAction;
    }



    void SetPoint(ActionState DoAction)
    {

        SetFalse();
        MyActionType = DoAction;
        switch (MyActionType)
        {
            case ActionState.Idel:
                FarmAnimator.SetBool("ToIdle", true);
                StartCoroutine("Wait", Random.Range(5, Interval));
                break;

            case ActionState.Walk:
                UpdateWayPoint();
                break;
            case ActionState.Attack:
                FarmAnimator.SetBool("ToAttack", true);
                StartCoroutine("Wait", Random.Range(2, Interval));
                break;
        }
    }


    IEnumerator Wait(int timewait)
    {
        yield return new WaitForSeconds(timewait);
        SetPoint(CalculateAction(NowPoint.GetComponent<AIpoint>().MyAction));
        FarmAnimator.SetBool("ToAttack", false);   
    }


    Dictionary<float, Transform> SearchWay = new Dictionary<float, Transform>();
    List<Transform> SortPoint = new List<Transform>();

    public void UpdateWayPoint()
    {
        SearchWay.Clear();
        SortPoint.Clear();
        foreach (Transform child in WayPoint)
        {
            SearchWay.Add(Vector3.Distance(transform.position, child.position), child);
        }

        Dictionary<float, Transform> SorthWay = SearchWay.OrderBy(o => o.Key).ToDictionary(o => o.Key, p => p.Value);

        foreach (Transform child in SorthWay.Values)
            SortPoint.Add(child);

        int a = (int)Time.time;
        NowPoint = a % 2 == 0 ? SortPoint[2] : SortPoint[1];
        IsWalk = true;
        FarmAnimator.SetBool("ToWalk", true);
    }


    void SetFalse()
    {
        FarmAnimator.SetBool("ToWalk", false);
        FarmAnimator.SetBool("ToAttack", false);
        FarmAnimator.SetBool("ToIdle", false);
    }
}
