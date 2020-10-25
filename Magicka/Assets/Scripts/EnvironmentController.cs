using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using DG.Tweening;
using UnityEngine.Experimental.Rendering.Universal;
using Cinemachine;

public class EnvironmentController : MonoBehaviour
{
    [SerializeField] SpriteShapeController spriteShape = default;
    public bool canUse;
    int node;
    Vector3 startPosition, endPosition;
    float startHeight;
    public float environmentChangeDuration = 1.5f;

    //Effects
    public ParticleSystem environmentParticle;
    public Light2D environmentLight;

    private void Start()
    {
        canUse = true;
    }
    public void AddPoint(Vector3 worldPosition)
    {
        if (!canUse)
            return;

        endPosition = spriteShape.transform.InverseTransformPoint(worldPosition);
        node = 0;
        float distance = Mathf.Infinity;
        for (int i = 0; i < spriteShape.spline.GetPointCount(); i++)
        {
            Vector3 pointPosition = spriteShape.spline.GetPosition(i);
            float tempDistance = Vector3.Distance(endPosition, pointPosition);
            if (tempDistance < distance)
            {
                distance = tempDistance;
                node = i;
            }
        }

        startPosition = spriteShape.spline.GetPosition(node);
        startHeight = spriteShape.spline.GetHeight(node);

        Sequence tween = DOTween.Sequence()
        .AppendCallback(() => canUse = false)
        .Append(DOTween.To(AnimatePosition, 0, 1, 0.5f).SetEase(Ease.OutBack))
        .AppendInterval(environmentChangeDuration)
        .Append(DOTween.To(AnimatePosition, 1, 0, 0.5f))
        .AppendCallback(() => canUse = true);

        //effects
        if (environmentParticle == null)
            return;

        environmentParticle.transform.localPosition = new Vector3(startPosition.x, startPosition.y + .5f, 0);
        environmentParticle.Play();

        environmentLight.transform.localPosition = new Vector3(startPosition.x, startPosition.y + .5f, 0);
        DOVirtual.Float(0,6,.1f,(x)=> environmentLight.intensity = x).OnComplete(()=> DOVirtual.Float(6, 0, .2f, (x) => environmentLight.intensity = x));

        if (GetComponent<CinemachineImpulseSource>())
            GetComponent<CinemachineImpulseSource>().GenerateImpulse();

    }

    void AnimatePosition(float t)
    {
        spriteShape.spline.SetPosition(node, Vector3.Lerp(startPosition, endPosition, t));
        spriteShape.spline.SetHeight(node, Mathf.Lerp(startHeight, .2f, t));
    }

}
