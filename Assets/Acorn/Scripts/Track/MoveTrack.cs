using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class MoveTrack : MonoBehaviour
{
    LineRenderer lineRenderer;
    // 使用List储存前几个位置的坐标点
    // 由于绘制过程中需要遍历坐标,所以不能直接使用队列
    // 而是用List进行模拟
    List<Vector3> pointList = new List<Vector3>();
    [SerializeField]
    [Tooltip("最大记录多少坐标点")]
    int pointSize = 20;
    // Start is called before the first frame update
    void Start()
    {
        InitPointList();
        lineRenderer = GetComponent<LineRenderer>();
        // lineRenderer.positionCount = 0;
        // 初始化LineRenderer的坐标点数量
        lineRenderer.positionCount = pointSize;
    }

    private void FixedUpdate()
    {
        UpdatePointList();
        DrawLine();
    }

    /// <summary>
    /// 模拟队列的方式,更新坐标点列表
    /// </summary>
    private void UpdatePointList()
    {
        pointList.RemoveAt(pointSize - 1);
        pointList.Insert(0, transform.position);
    }

    private void DrawLine()
    {
        for (int i = 0; i < pointSize; i++)
        {
            lineRenderer.SetPosition(i, pointList[i]);
        }
    }

    private void InitPointList()
    {
        for (int i = 0; i < pointSize; i++)
        {
            pointList.Add(transform.position);
        }
    }
}
