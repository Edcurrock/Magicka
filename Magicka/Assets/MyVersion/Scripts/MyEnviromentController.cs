using UnityEngine;
using DG.Tweening;
using UnityEngine.U2D;
using System;

public class MyEnviromentController : MonoBehaviour
{
    public bool canUse;
    [SerializeField] SpriteShapeController spriteShape = default;
    Vector3 startPosition, endPosition;
    int node;
    float startHeight;
    public float environmentChangeDuration = 1.5f;

    private void Start()
    {
        canUse = true;
    }

    public void AddPoint(Vector3 worldPosition) 
    {
        if(!canUse)
            return;
        endPosition = spriteShape.transform.InverseTransformPoint(worldPosition);
        node = FindClosestPoint();
        startPosition = spriteShape.spline.GetPosition(node);
        startHeight = spriteShape.spline.GetHeight(node);

        Sequence tween = DOTween.Sequence()
        .AppendCallback(() => canUse = false)
        .Append(DOTween.To(AnimatePosition, 0, 1, 0.5f).SetEase(Ease.OutBack))
        .AppendInterval(environmentChangeDuration)
        .Append(DOTween.To(AnimatePosition, 1, 0, 0.5f))
        .AppendCallback(() => canUse = true);
    }

    private int FindClosestPoint()
    {
        node = 0;
        float distance = Mathf.Infinity;
        for(int i = 0; i < spriteShape.spline.GetPointCount(); i++)
        {
            var point = spriteShape.spline.GetPosition(i);
            var curDistance = Vector2.Distance(endPosition, point);
            if(curDistance < distance)
            {
                distance = curDistance;
                node = i;
            }
            
        }

        return node;
    }


    void AnimatePosition(float t)
    {
        spriteShape.spline.SetPosition(node, Vector3.Lerp(startPosition, endPosition, t));
        spriteShape.spline.SetHeight(node, Mathf.Lerp(startHeight, .2f, t));
    }
}