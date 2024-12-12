using UnityEngine;

public class Archer : MonoBehaviour
{
    private bool _canShoot;
    private bool _isChargingBow;
    public bool _shotFired {get; private set;}
    
    private float _shotAngle;
    private float _shotStrength;
    private Vector2 _shotVector;
    
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private Transform arrowSpawnOrigin;
    private LineRenderer _lineRenderer;

    [SerializeField] private float shotStrengthIncrease;
    [SerializeField] private float shotAngleIncrease;
    [SerializeField] private float startingAngle;
    [SerializeField] private float shotAngleMax;
    [SerializeField] private float shotAngleMin;
    [SerializeField] private float shotStrengthMax;
    [SerializeField] private float shotStrengthMin;
    [SerializeField] private bool isLookingRight;

    private float _verticalInput;
    private bool _buttonInput;
    public ShotProjectile activeShot {get; private set;}
    private Animator _animator;
    [SerializeField] private AudioSource _bowDrawAudioSource;
    [SerializeField] private AudioSource _bowShotAudioSource;
    [SerializeField] private AudioSource _bodyFallAudioSource;

    [SerializeField] private string inputAxis;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canShoot) return;

        GetInputs();

        if (_isChargingBow) AimShot();
        else ChargeShot();

        GetShotVector();
        UpdateLineRenderer();
    }

    public void GetTurn()
    {
        _animator.SetTrigger("TurnStart");
        _canShoot = true;
        _isChargingBow = true;
        activeShot = null;
        _shotAngle = startingAngle;
        _shotStrength = shotStrengthMin;
    }

    private void GetInputs()
    {
        _verticalInput = Input.GetAxis(inputAxis);
        _buttonInput = Input.GetButtonUp("Fire1");
    }

    private void AimShot()
    {
        if (!isLookingRight) _verticalInput = -_verticalInput;
        
        _shotAngle += _verticalInput * shotAngleIncrease * Time.deltaTime;
        _shotAngle = Mathf.Clamp(_shotAngle, shotAngleMin, shotAngleMax);

        if (_buttonInput)
        {
            _animator.SetTrigger("DrawBow");
            _bowDrawAudioSource.Play();
            _isChargingBow = !_isChargingBow;
            uiManager.NextControls();
        }
    }

    private void ChargeShot()
    {
        _shotStrength += _verticalInput * shotStrengthIncrease * Time.deltaTime;
        _shotStrength = Mathf.Clamp(_shotStrength, shotStrengthMin, shotStrengthMax);
        if (_buttonInput)
        {
            _animator.SetTrigger("Shoot");
            Shoot();
            _bowShotAudioSource.Play();
            uiManager.DisableControls();
        }
    }

    private void Shoot()
    {
        _canShoot = false;
        activeShot = Instantiate(shotPrefab, arrowSpawnOrigin.position, Quaternion.identity).GetComponent<ShotProjectile>();
        activeShot.ApplyShotForce(_shotVector);
        activeShot.GetSourceArcher(gameObject);

        _shotAngle = 0f;
        _shotStrength = 0f;

    }
    
    private void GetShotVector()
    {
        _shotVector = new Vector2(Mathf.Cos(_shotAngle * Mathf.Deg2Rad), Mathf.Sin(_shotAngle * Mathf.Deg2Rad)).normalized * _shotStrength;
    }

    private void UpdateLineRenderer()
    {
        Vector3[] linePositions = new Vector3[2];

        linePositions[0] = arrowSpawnOrigin.position;
        linePositions[1] = arrowSpawnOrigin.position + (new Vector3(_shotVector.x, _shotVector.y, 0) * Mathf.Max(0.02f, _shotStrength * .00005f));

        if(!isLookingRight)
        {
            linePositions[1].x = -linePositions[1].x;
        }

        _lineRenderer.SetPositions(linePositions);
    }

    public void GetHit()
    {
        _animator.SetTrigger("Hit");
        _animator.SetTrigger("Death");
        _bodyFallAudioSource.Play();
    }

    public void ResetArrow()
    {
        activeShot = null;
    }
}
