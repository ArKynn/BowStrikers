using UnityEngine;

public class ShotProjectile : MonoBehaviour
{
    [SerializeField] private AudioClip GroundHitSound;
    [SerializeField] private AudioClip PlayerHitSound;
    
    private GameManager gm;
    private Rigidbody2D _rb;
    private Collider2D _col;
    private Collider2D _collider;
    private bool _flying = true;

    private GameObject sourceArcher;
    private AudioSource _audioSource;
    
    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _col = GetComponent<Collider2D>();
        gm = FindObjectOfType<GameManager>();
        _audioSource = GetComponentInChildren<AudioSource>();
    }

    private void FixedUpdate()
    {
        if(_flying) RotateProjectile();
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject == sourceArcher) return;
        StopProjectile();

        var playerhit = other.gameObject.GetComponent<Archer>();
        
        gm.ShotHit(playerhit);
        PlaySound(playerhit != null);
        if(playerhit) GetComponent<SpriteRenderer>().enabled = false;
    }

    public void ApplyShotForce(Vector2 shotVector)
    {
        _rb.AddForce(shotVector, ForceMode2D.Impulse);
    }

    public void GetSourceArcher(GameObject archer)
    {
        sourceArcher = archer;
    }

    private void RotateProjectile()
    {
        var dir = _rb.velocity;
        var angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        var q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, q, 360); 
    }

    private void StopProjectile()
    {
        _rb.velocity = Vector2.zero;
        _rb.isKinematic = true;
        _col.isTrigger = true;
        _rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        _flying = false;
    }

    private void PlaySound(bool PlayerHit)
    {
        _audioSource.PlayOneShot(PlayerHit ? PlayerHitSound : GroundHitSound);
    }
}
