using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLineCtrl : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform airplane;
    [SerializeField]
    [Tooltip("线段的密度")]
    private float density=2f;
    [SerializeField]
    [Tooltip("动画频率")]
    private float frequency = 0.03f;
    [SerializeField]
    [Tooltip("爬行速度")]
    private float moveSpeed = 0.04f;
    [SerializeField]
    [Tooltip("飞机飞行速度")]
    private float flySpeed = 0.01f;
    private Material lineMt;
    private Vector2 lineTiling;
    private Vector2 lineOffset;
    private int mainTexProperty;
    private float lineLen;
    private float timer = 0f;
    private Camera mainCam;
    private Vector3 targetPos;
    //是否已到达目的地
    private bool isReachedTarget;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
        lineMt = lineRenderer.material;
        // 缓存属性id，防止下面设置属性的时候重复计算名字的哈希
        mainTexProperty = Shader.PropertyToID("_MainTex");
        lineTiling = new Vector2(20, 0);
        lineOffset = new Vector2(0, 0);
        lineMt.SetTextureScale(mainTexProperty, lineTiling);
        lineMt.SetTextureOffset(mainTexProperty, lineOffset);
    }

    // Update is called once per frame
    void Update()
    {
        //实时计算tiling,保证无论line有多长都是相同长度的线段
        lineLen = (lineRenderer.GetPosition(1) - lineRenderer.GetPosition(0)).magnitude;
        lineTiling = new Vector2(lineLen * density, 0);
        lineMt.SetTextureScale(mainTexProperty, lineTiling);

        timer += Time.deltaTime;
        if (timer >= frequency)
        {
            timer = 0f;
            lineOffset -= new Vector2(moveSpeed, 0);
            lineMt.SetTextureOffset(mainTexProperty, lineOffset);
        }

        lineRenderer.SetPosition(0, airplane.position);

        //正常情况下,飞机的逻辑不应该写在这个类里.
        //这里只是演示
        if (Input.GetMouseButtonDown(0))
        {
            var screenPos = Input.mousePosition;
            //屏幕坐标转世界坐标,z轴为与摄像机的距离
            targetPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 10));
            //这里用up是因为飞机的朝向的方向是y轴的方向，如果你的飞机的朝向是z轴的，则用forward
            airplane.up = targetPos - airplane.position;
            lineRenderer.SetPosition(1, targetPos);
            isReachedTarget = false;
            lineRenderer.enabled = true;
        }
        if (!isReachedTarget)
        {
            // 飞机飞向目标的
            airplane.position += airplane.up * flySpeed;
            // 检测是否到达目标坐标
			if (Vector3.Dot(airplane.up, targetPos - airplane.position) < 0)
			{
			    airplane.position = targetPos;
			    isReachedTarget = true;
			    lineRenderer.enabled = false;
			}
        }
    }
}
