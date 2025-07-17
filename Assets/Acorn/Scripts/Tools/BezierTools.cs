using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Acorn.Tools
{
    public class BezierTools
    {
        /// <summary>
        /// 计算二阶贝塞尔曲线上的点
        /// </summary>
        /// <param name="p0">起点</param>
        /// <param name="p1">控制点</param>
        /// <param name="p2">终点</param>
        /// <param name="t">插值参数 [0, 1]</param>
        public static Vector2 CalculateQuadraticBezierPoint(Vector2 p0, Vector2 p1, Vector2 p2, float t)
        {
            // 确保t在[0,1]范围内
            t = Mathf.Clamp01(t);

            // 贝塞尔曲线公式：B(t) = (1-t)²P₀ + 2(1-t)tP₁ + t²P₂
            float u = 1 - t;
            float uSquared = u * u;
            float tSquared = t * t;

            Vector2 position = uSquared * p0;          // (1-t)² * P₀
            position += 2 * u * t * p1;               // 2*(1-t)*t * P₁
            position += tSquared * p2;                // t² * P₂

            return position;
        }
        
        // 计算三次贝塞尔曲线上的点
        private Vector2 CalculateBezierPoint(float t, Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
        {
            float u = 1 - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            Vector2 point = uuu * p0; // (1-t)^3 * P0
            point += 3 * uu * t * p1; // 3*(1-t)^2*t * P1
            point += 3 * u * tt * p2; // 3*(1-t)*t^2 * P2
            point += ttt * p3;        // t^3 * P3

            return point;
        }

    }
}