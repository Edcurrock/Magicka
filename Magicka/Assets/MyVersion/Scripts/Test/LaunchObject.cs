using UnityEngine;

public class LaunchObject : MonoBehaviour 
{

    [SerializeField] 
    int health = 5;

    [SerializeField]
    bool canLaunch = false;
    public bool CanLaunch
    {
        get {return canLaunch;}
        set {canLaunch = value;}
    }

    [SerializeField]
    Transform launchPos;
    [SerializeField]
    float heightPlayer;

    public ParticleSystem explosionPrefab;

    bool isReadyLaunch = false;

    public Transform target;

    Rigidbody2D _rigidBody;

    [SerializeField]
    float speed = 0f;


    Vector2 point1;
    Vector2 point2;
    Vector2 point3;

    float lenOfOX;
    float lenOfOY;

    float tParam = 0f;

    private void Start()
    {
        Mathf.Clamp01(tParam);
        // heightPlayer = launchPos.localPosition.y;
        _rigidBody = GetComponent<Rigidbody2D>();

   
    }

    private void Update() 
    {
        
        heightPlayer = launchPos.localPosition.y + launchPos.position.y / 2f;
        if(canLaunch)
        {
           
            _rigidBody.isKinematic = true;

            if(Vector2.Distance(transform.position, launchPos.position) > 0.5f)
            {
                tParam += Time.deltaTime * 0.5f;
                transform.position = Bezier.GetPos(point1, point2, point3, launchPos.position, tParam);
            }
            else
                isReadyLaunch = true;
        }
        else
        {
            _rigidBody.isKinematic = false;
            lenOfOX = Mathf.Abs(launchPos.position.x - transform.position.x);
            lenOfOY = Mathf.Abs(launchPos.position.y - transform.position.y);
            point1 = transform.position;
            point2 = new Vector2((point1.x + lenOfOX/2),
        point1.y + lenOfOY );
            point3 = new Vector2(point2.x * 2, point2.y);
            tParam = 0f;
        }
    }

    private void FixedUpdate() {
        if(isReadyLaunch)
        {
            Vector3 dir = target.position - transform.position;
            _rigidBody.AddForce(dir*2, ForceMode2D.Impulse);
            speed = _rigidBody.velocity.magnitude;
        }
    }
    private void OnCollisionEnter2D(Collision2D other) {
        isReadyLaunch = false;
        if(speed >= 50f && !canLaunch)
        {
            health--;
            if(health <= 0)
            {
                //Instantiate(explosionPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        int segments = 20;
        Vector3 startPos = transform.position;

        for (int i = 0; i < segments; i++)
        {
            float tParamSegment = (float)i / segments;
            Vector3 pointNew = Bezier.GetPos(point1, point2,
                    point3, launchPos.position, tParamSegment);
            Gizmos.DrawLine(startPos, pointNew);
            startPos = pointNew;
        }
    }
}