using UnityEngine;

public class ObjectOnLaunch : MonoBehaviour 
{
    [SerializeField]
    Transform bar;

    [SerializeField] 
    int health = 5;

    float healhDiv;

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

    private void Start()
    {
        healhDiv = bar.localScale.x / health;
        // heightPlayer = launchPos.localPosition.y;
        _rigidBody = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        if (health <= 0)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void FixedUpdate() {

        heightPlayer = launchPos.localPosition.y + launchPos.position.y / 2f;
        if (canLaunch)
        {
            _rigidBody.isKinematic = true;
            _rigidBody.velocity = Vector2.zero;
            if (Vector2.Distance(transform.position, new Vector2(transform.position.x, heightPlayer)) > 0.5f)
            {
                _rigidBody.position += Vector2.up * Time.deltaTime * 5;
            }
            else
                isReadyLaunch = true;
        }
        else
        {
            _rigidBody.isKinematic = false;
        }
        if(isReadyLaunch && target)
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
            bar.localScale = new Vector3(bar.localScale.x - healhDiv, bar.localScale.y,0);
            speed = 0f;
            health--;
        }
    }
}