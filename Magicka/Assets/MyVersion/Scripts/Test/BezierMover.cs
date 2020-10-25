using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BezierMover : MonoBehaviour
{ 
    Vector2 point1;

    Vector2 point2;

    Vector2 point3;

    [SerializeField]
    Transform point4 = null;
    
    float lenOfOX = 0f;
    float lenOfOY =0f;
    [SerializeField]
    Scrollbar tParamSlider;


    private void Start()
    {
        point1 = transform.position;
        lenOfOX = (point4.position.x - transform.position.x);
        lenOfOY = (point4.position.y - transform.position.y);
    }

    private void Update()
    {
      
        CoinMover();
    }

    
    void CoinMover()
    {
        point2 = new Vector2(transform.position.x + lenOfOX * 0.25f,
            transform.position.y + lenOfOY * 0.25f);
        point3 = new Vector2(point2.x * 2, point2.y);
        Vector2 dist = Bezier.GetPos(point1, point2, point3, point4.position, tParamSlider.value);
        transform.position = Vector2.Lerp(transform.position,dist,Time.deltaTime*2.5f);
       // transform.rotation = Quaternion.LookRotation(Bezier.GetRot(transform.position, point2,
          //          point3, point4.position, tParamSlider));
    }

    // void FixedUpdate()
    // {
    //     transform.position = Bezier.GetPos(point1.position, point2.position, point3.position, point4.position, tParamSlider);
    //     transform.rotation = Quaternion.LookRotation(Bezier.GetRot(point1.position, point2.position, 
    //                 point3.position, point4.position, tParamSlider));
    // }

    private void OnDrawGizmos()
    {
        int segments = 20;
        Vector3 startPos = transform.position;

        for (int i = 0; i < segments; i++)
        {
            float tParamSegment = (float)i / segments;
            Vector3 pointNew = Bezier.GetPos(transform.position, point2,
                    point3, point4.position, tParamSegment);
            Gizmos.DrawLine(startPos, pointNew);
            startPos = pointNew;
        }
    }
}
