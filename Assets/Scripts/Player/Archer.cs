using UnityEngine;

public class Archer : MonoBehaviour
{
    private bool _canShoot;
    private bool _isChargingBow;
    
    [Header("UI Variables")]
    [SerializeField] private UiManager uiManager;
    [SerializeField] private int linePointResolution;
    [SerializeField] private float linePointDistance;
    private Vector3[] linePositions;
    private LineRenderer _lineRenderer;

    [Header("Shot Variables")]
    [SerializeField] private GameObject shotPrefab;
    [SerializeField] private Transform arrowSpawnOrigin;
    [SerializeField] private float shotStrengthIncrease;
    [SerializeField] private float shotAngleIncrease;
    [SerializeField] private float startingAngle;
    [SerializeField] private float shotAngleMax;
    [SerializeField] private float shotAngleMin;
    [SerializeField] private float shotStrengthMax;
    [SerializeField] private float shotStrengthMin;
    [SerializeField] private bool isLookingRight;
    public ShotProjectile activeShot {get; private set;}
    private float _shotAngle;
    private float _shotStrength;
    private Vector2 _shotDirection;
    private Vector2 _shotVector;
    
    [Header("Sound Variables")]
    [SerializeField] private AudioSource _bowDrawAudioSource;
    [SerializeField] private AudioSource _bowShotAudioSource;
    [SerializeField] private AudioSource _bodyFallAudioSource;

    [Header("Input Variables")]
    [SerializeField] private string inputAxis;
    private float _verticalInput;
    private bool _buttonInput;
    
    [Header("Animation Variables")]
    [SerializeField] private GameObject handsPivot;
    private Animator _handsAnimator;
    private Animator _animator;
    
    // Start is called before the first frame update
    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _animator = GetComponent<Animator>();
        _handsAnimator = handsPivot.GetComponentInChildren<Animator>();
        linePositions = new Vector3[linePointResolution];
        _lineRenderer.positionCount = linePointResolution;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_canShoot) return;

        GetInputs();

        if (_isChargingBow) AimShot();
        else ChargeShot();

        GetShotVector();
        RotateHands();
        UpdateLineRenderer();
    }

    public void GetTurn()
    {
        _canShoot = true;
        _isChargingBow = true;
        activeShot = null;
        _shotAngle = startingAngle;
        _shotStrength = shotStrengthMin;
        _lineRenderer.enabled = true;
        TurnStart();
    }
    
    public void TurnStart()
    {
        _animator.SetTrigger("TurnStart");
    }

    private void GetInputs()
    {
        _verticalInput = Input.GetAxis(inputAxis);
        _buttonInput = Input.GetButtonDown("Fire1");
    }

    private void AimShot()
    {
        if (!isLookingRight) _verticalInput = -_verticalInput;
        
        _shotAngle += _verticalInput * shotAngleIncrease * Time.deltaTime;
        _shotAngle = Mathf.Clamp(_shotAngle, shotAngleMin, shotAngleMax);

        if (_buttonInput)
        {
            _animator.SetTrigger("DrawBow");
            _handsAnimator.SetTrigger("Draw");
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
            _handsAnimator.SetTrigger("Shoot");
            Shoot();
            _bowShotAudioSource.Play();
            uiManager.DisableControls();
            _lineRenderer.enabled = false;
        }
    }

    private void Shoot()
    {
        _canShoot = false;
        activeShot = Instantiate(shotPrefab, arrowSpawnOrigin.position, Quaternion.identity).GetComponent<ShotProjectile>();
        activeShot.ApplyShotForce(_shotVector);
        activeShot.GetSourceArcher(gameObject);
    }
    
    private void GetShotVector()
    {
        _shotDirection = new Vector2(Mathf.Cos(_shotAngle * Mathf.Deg2Rad), Mathf.Sin(_shotAngle * Mathf.Deg2Rad)).normalized;
        _shotVector = _shotDirection * _shotStrength;
    }

    private void RotateHands()
    {
        handsPivot.transform.eulerAngles = new Vector3(0, 0, _shotAngle);
    }

    private void UpdateLineRenderer()
    {
        Vector3 o = arrowSpawnOrigin.localPosition;
        float f = linePointDistance;
        for (int i = 0; i < linePointResolution; i++)
        {
            linePositions[i] = o + (Vector3)_shotVector * (f * 0.5f);
            if(!_isChargingBow) linePositions[i].y += 4f*Physics.gravity.y*f*f;
            
            if(!isLookingRight) linePositions[i].x = -linePositions[i].x;
            f+= linePointDistance;
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
