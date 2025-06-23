using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntLineCtrl : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private Transform anchor;
    private Material lineMt;
    private Vector2 lineTiling;
    private Vector2 lineOffset;
    private int mainTexProperty;

    // Start is called before the first frame update
    void Start()
    {
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
        lineRenderer.SetPosition(0, anchor.position);
    }
}
