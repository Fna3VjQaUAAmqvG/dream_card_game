//修改线条使他动起来
using UnityEngine;

public class Line : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float offsetSpeed = 0.1f;
    private void Update()
    {
        if(lineRenderer != null)
        {
            var offset = lineRenderer.material.mainTextureOffset; //获取
            offset.x += offsetSpeed * Time.deltaTime; //修改
            lineRenderer.material.mainTextureOffset = offset; //写回
        }
    }
}
