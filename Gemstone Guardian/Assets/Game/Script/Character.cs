using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private CharacterController _cc;
    public float MoveSpeed = 5f;
    private Vector3 _movementVelocity;
    private bool isFrozen = false;
    private PlayerInput _playerInput;

    private float _verticalVelocity;

    public bool hasShield;

    public float fireSpellDamageRadius = 10f;
    public int fireSpellDamageAmount = 30;
    public float freezeDuration = 10f; // Duration of the freeze effect
    public float freezeRadius = 10f;

    public float Gravity = -9.8f;

    private Animator _animator;

    public bool isPlayer = true;

    private float attackStartTime;

    private Health _health;

    public int Coin;

    private DamageCaster _damageCaster;

    private Vector3 impactOnCharacter;

    public float SlideSpeed = 1f;

    public bool IsInvincible;
    public float invincibleDuration = 0.2f;

    private bool isHealing = false;

    private bool isFireSpell = false;
    private bool isFreezeSpell = false;
    public bool isBlocking = false;
    public bool isRageSpell = false;



    private MaterialPropertyBlock _materialPropertyBlock;
    private SkinnedMeshRenderer _skinnedMeshRenderer;

    public GameObject ItemToDrop;


    public float AttackSlideDuration = 0.4f;

    public float AttackSlideSpeed = 0.06f;

    private UnityEngine.AI.NavMeshAgent _navMeshAgent;

    private Transform TargetPlayer;

    public enum CharacterState
    {
        Normal, Attacking, Dead, BeingHit, Slide, Spawn, Kick, Punch
    }


    public float SpawnDuration = 2f;

    private float currentSpawnTime;


    public CharacterState CurrentState;
 
    void Start()
    {
        // Rotate 180 degrees around the Y-axis
        transform.Rotate(0, 180, 0);
    }


    // Call this method to freeze the enemy
    public void Freeze()
    {
        Debug.Log("Freezing Enemy");
        isFrozen = true;
        // Additional logic to stop enemy movement and actions
    }

    // Call this method to unfreeze the enemy
    public void Unfreeze()
    {
        Debug.Log("UnFreezing Enemy");
        isFrozen = false;
        // Additional logic to resume enemy movement and actions
    }


    private void Awake()
    {
        _cc = GetComponent<CharacterController>();
        
        _animator = GetComponent<Animator>();

        _health = GetComponent<Health>();

        _damageCaster = GetComponentInChildren<DamageCaster>();

        _skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _skinnedMeshRenderer.GetPropertyBlock(_materialPropertyBlock);

        if (!isPlayer)
        {
            _navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
            TargetPlayer = GameObject.FindWithTag("Player").transform;
            _navMeshAgent.speed = MoveSpeed;
            SwitchStateTo(CharacterState.Spawn);
        } else {
            _playerInput = GetComponent<PlayerInput>();
        }
    }


    private void CalculateEnemyMovement()
    {

        if (Vector3.Distance(TargetPlayer.position, transform.position) >= _navMeshAgent.stoppingDistance)
        {
            _navMeshAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0.2f);
        } else
        {
            _navMeshAgent.SetDestination(TargetPlayer.position);
            _animator.SetFloat("Speed", 0f);

            SwitchStateTo(CharacterState.Attacking);
        }
    }



    private void CalculatePlayerMovement()
    {
        if(_playerInput.AttackKeyDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Attacking);
            return;
        }
        else if (_playerInput.PunchKeyDown)
        {
            SwitchStateTo(CharacterState.Punch);
            return; // Return early to ensure no movement happens while punching
        }
        else if (_playerInput.KickKeyDown)
        {
            SwitchStateTo(CharacterState.Kick);
            return; // Return early to ensure no movement happens while kicking
        }
        else if (_playerInput.SpaceKeyDown && _cc.isGrounded)
        {
            SwitchStateTo(CharacterState.Slide);
            return;
        }
        _movementVelocity.Set(-_playerInput.HorizontalInput, 0f, -_playerInput.VerticalInput);
        _movementVelocity.Normalize();
        //_movementVelocity = Quaternion.Euler(0, -45f, 0) * _movementVelocity;
        _animator.SetFloat("Speed", _movementVelocity.magnitude);
        _movementVelocity = _movementVelocity * MoveSpeed * Time.deltaTime;

        if (_movementVelocity != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(_movementVelocity);
        }

        RotateToCursor();
    }

    private void FixedUpdate()
    {

        if (isFrozen)
        {
            Debug.Log("Enemy is Frozen so switching to Normal");
            SwitchStateTo(CharacterState.Normal);
            return;
        }

        switch (CurrentState)
        {
            case CharacterState.Normal:
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                } else
                {
                    CalculateEnemyMovement();
                }
                break;
            case CharacterState.Attacking:
                if (isPlayer)
                {
                    if(Time.time < attackStartTime + AttackSlideDuration)
                    {
                        float timePassed = Time.time - attackStartTime;
                        float lerpTime = timePassed / AttackSlideDuration;
                        _movementVelocity = Vector3.Lerp(transform.forward * AttackSlideSpeed, Vector3.zero, lerpTime);
                    }
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                _movementVelocity = transform.forward * SlideSpeed * Time.deltaTime;
                break;
            case CharacterState.Spawn:
                currentSpawnTime = currentSpawnTime - Time.deltaTime;
                if(currentSpawnTime <= 0)
                {
                    SwitchStateTo(CharacterState.Normal);
                }
                break;
            case CharacterState.Punch:
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                }
                break;

            case CharacterState.Kick:
                if (isPlayer)
                {
                    CalculatePlayerMovement();
                }
                break;
        }

        if (impactOnCharacter.magnitude > 0.2f)
        {
            _movementVelocity = impactOnCharacter * Time.deltaTime;
        }
        impactOnCharacter = Vector3.Lerp(impactOnCharacter, Vector3.zero, Time.deltaTime * 5);
         
        if (isPlayer)
        {
            if (_playerInput.HealKeyDown && !isHealing)
            {
                _playerInput.HealKeyDown = false;
                StartCoroutine(HealOverTime(10, 2, 5));  // Duration 10s, heal every 2s, 5 health
            }

            if (_playerInput.FireSpellKeyDown && !isFireSpell)
            {
                _playerInput.FireSpellKeyDown = false;
                StartCoroutine(FireSpell());
            }

            if (_playerInput.FreezeSpellKeyDown && !isFreezeSpell)
            {
                _playerInput.FreezeSpellKeyDown = false;
                StartCoroutine(FreezeSpell());
            }

            if (_playerInput.RageSpellKeyDown && !isRageSpell)
            {
                _playerInput.RageSpellKeyDown = false;
                StartCoroutine(RageSpell());
            }

            if (_playerInput.BlockSpellKeyDown && !isBlocking)
            {
                _playerInput.BlockSpellKeyDown = false;
                StartCoroutine(BlockSpell());
            }

            if (_cc.isGrounded == false)
            {
                _verticalVelocity = Gravity;
            }
            else
            {
                _verticalVelocity = Gravity * 0.3f;
            }
            _movementVelocity += _verticalVelocity * Vector3.up * Time.deltaTime;

            _cc.Move(_movementVelocity);
            _movementVelocity = Vector3.zero;
        } else
        {
            if(CurrentState != CharacterState.Normal)
            {
                _cc.Move(_movementVelocity);
                _movementVelocity = Vector3.zero;
            }
        }
        
    }

    public void SwitchStateTo(CharacterState newState)
    {

        if (isPlayer)
        {
            _playerInput.ClearCache();
        }

        switch (CurrentState){
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:
                if(_damageCaster != null)
                {
                    DisableDamageCaster();
                }
                break;
            case CharacterState.Dead:
                return;
            case CharacterState.BeingHit:
                break;
            case CharacterState.Slide:
                break;
            case CharacterState.Spawn:
                IsInvincible = false;
                break;

        }

// Entering New State
        switch (newState)
        {
            case CharacterState.Normal:
                break;
            case CharacterState.Attacking:

                if (!isPlayer)
                {
                    Quaternion newRotation = Quaternion.LookRotation(TargetPlayer.position - transform.position);
                    transform.rotation = newRotation;
                }

                _animator.SetTrigger("Attack");

                if (isPlayer)
                {
                    attackStartTime = Time.time;
                    RotateToCursor();
                }
                break;
            case CharacterState.Dead:
                _cc.enabled = false;
                _animator.SetTrigger("Dead");
                StartCoroutine(MaterialDissolve());
                break;
            case CharacterState.BeingHit:
                _animator.SetTrigger("BeingHit");
                if (isPlayer)
                {
                    IsInvincible = true;
                    StartCoroutine(DelayCancelInvincible());
                }
                break;
            case CharacterState.Slide:
                _animator.SetTrigger("Slide");
                break;
            case CharacterState.Spawn:
                IsInvincible = true;
                currentSpawnTime = SpawnDuration;
                StartCoroutine(MaterialAppear());
                break;
            case CharacterState.Punch:
                _animator.SetTrigger("Punch");
                break;

            case CharacterState.Kick:
                _animator.SetTrigger("Kick");
                break;

        }

        CurrentState = newState;

        //Debug.Log("Switched to " + CurrentState);
    }



    public void PunchAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }

    public void PowerUpAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }


    public void SideKickAnimationEnds()
    {
        Debug.Log("Side Kick Animation Ends");
        SwitchStateTo(CharacterState.Normal);
    }

    public void SlideAnimationEnds()
    {
        SwitchStateTo(CharacterState.Normal);
    }


    public void BeingHitAnimationEnds()
    {
        Debug.Log("Being Hit Animation Ends");
        SwitchStateTo(CharacterState.Normal);
    }

    IEnumerator DelayCancelInvincible()
    {
        yield return new WaitForSeconds(invincibleDuration);
        IsInvincible = false;
    }



    public void ApplyDamage(int damage, Vector3 attackerPos = new Vector3())
    {

        if(isPlayer && (isBlocking|| isRageSpell))
        {
            Debug.Log("Not applying Damage");
            return;
        }
        if (IsInvincible)
        {
            return;
        }

        Debug.Log("Appyling Damage");

        if (_health != null)
        {
            _health.ApplyDamage(damage);
        }

        if (!isPlayer)
        {
            GetComponent<EnemyVFXManager>().PlayBeingHitVFX(attackerPos);
        }
        StartCoroutine(MaterialBlink());

        if (isPlayer)
        {
            SwitchStateTo(CharacterState.BeingHit);
            AddImpact(attackerPos, 10f);
        } else
        {
            AddImpact(attackerPos, 2.5f);
        }

    }


    private void AddImpact(Vector3 attackerPos, float force)
    {
        Vector3 impactDir = transform.position - attackerPos;
        impactDir.Normalize();
        impactDir.y = 0;
        impactOnCharacter = impactDir * force;
    }

    public void AttackAnimationEnds()
    {
        //Debug.Log("Attack Animation Ends");
        SwitchStateTo(CharacterState.Normal);
    }


    public void EnableDamageCaster()
    {
        _damageCaster.EnableDamageCaster();
    }

    public void DisableDamageCaster()
    {
        _damageCaster.DisableDamageCaster();
    }

    IEnumerator MaterialBlink()
    {
        _materialPropertyBlock.SetFloat("_blink", 0.4f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        yield return new WaitForSeconds(0.2f);

        _materialPropertyBlock.SetFloat("_blink", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }

    IEnumerator MaterialAppear()
    {
        float dissolveTimeDuration = SpawnDuration;
        float currentDissolveTime = 0;
        float dissolveHight_start = -10f;
        float dissolveHight_target = 20f;
        float dissolveHight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        _materialPropertyBlock.SetFloat("_enableDissolve", 0f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
    }


    IEnumerator MaterialDissolve()
    {
        yield return new WaitForSeconds(2);

        float dissolveTimeDuration = 2f;
        float currentDissolveTime = 0;
        float dissolveHight_start = 20f;
        float dissolveHight_target = -10f;
        float dissolveHight;

        _materialPropertyBlock.SetFloat("_enableDissolve", 1f);
        _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);

        while (currentDissolveTime < dissolveTimeDuration)
        {
            currentDissolveTime += Time.deltaTime;
            dissolveHight = Mathf.Lerp(dissolveHight_start, dissolveHight_target, currentDissolveTime / dissolveTimeDuration);
            _materialPropertyBlock.SetFloat("_dissolve_height", dissolveHight);
            _skinnedMeshRenderer.SetPropertyBlock(_materialPropertyBlock);
            yield return null;
        }

        DropItem();

    }

    public void DropItem()
    {
        if (ItemToDrop != null)
        {
            Instantiate(ItemToDrop, transform.position, Quaternion.identity);
        }
    }

    public void PickUpItem(PickUp item)
    {
        switch (item.Type)
        {
            case PickUp.PickUpType.Heal:
                AddHealth(item.Value);
                break;
            case PickUp.PickUpType.Coin:
                Debug.Log("Pickup Coin");
                AddCoin(item.Value);
                break;
        }
    }

    public void RotateToTarget()
    {
        if (CurrentState != CharacterState.Dead)
        {
            transform.LookAt(TargetPlayer, Vector3.up);
        }
    }

    private void AddHealth(int health)
    {
        _health.AddHealth(health);
        GetComponent<PlayerVFXManager>().PlayHealVFX();
    }

    private void AddCoin(int coin)
    {
        Coin += coin;
    }

    private void OnDrawGizmos()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitResult;

        //if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        //{
        //    Vector3 cursorPos = hitResult.point;
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawWireSphere(cursorPos, 1);
        //}
    }

    private void RotateToCursor()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        //RaycastHit hitResult;

        //if (Physics.Raycast(ray, out hitResult, 1000, 1 << LayerMask.NameToLayer("CursorTest")))
        //{
        //    Vector3 cursorPos = hitResult.point;
        //    transform.rotation = Quaternion.LookRotation(cursorPos - transform.position, Vector3.up);
        //}
    }

    private IEnumerator HealOverTime(float duration, float healInterval, int healAmount)
    {
        isHealing = true;
        Debug.Log("Healing Started");
        GetComponent<PlayerVFXManager>().PlayHealSpell();  // Play healing Spell

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            _health.AddHealth(healAmount);
            // Check if health is at or exceeds max, then stop healing
            if (_health.CurrentHealth >= _health.MaxHealth)
            {
                Debug.Log("Health full, stopping healing.");
                break;  // Exit the loop
            }

            yield return new WaitForSeconds(healInterval);
            elapsedTime += healInterval;
        }

        Debug.Log("Healing Ended");
        GetComponent<PlayerVFXManager>().StopHealSpell();  // Stop healing Spell
        isHealing = false;
    }

    private IEnumerator FireSpell()
    {
        Debug.Log("Playing Fire Spell");
        isFireSpell = true;
        GetComponent<PlayerVFXManager>().PlayFireSpell();
        ApplyDamageToEnemies(fireSpellDamageRadius, fireSpellDamageAmount);
        yield return new WaitForSeconds(5);
        GetComponent<PlayerVFXManager>().StopFireSpell();
        isFireSpell = false;
    }

    private IEnumerator FreezeSpell()
    {
        Debug.Log("Playing Freeze Spell");
        isFreezeSpell = true;
        GetComponent<PlayerVFXManager>().PlayFreezeSpell();
        CastFreezeSpell(freezeRadius);
        yield return new WaitForSeconds(10);
        GetComponent<PlayerVFXManager>().StopFreezeSpell();
        isFreezeSpell = false;
    }


    private IEnumerator BlockSpell()
    {
        Debug.Log("Playing Block Spell");
        isBlocking = true;
        // Activate VFX for blocking
        GetComponent<PlayerVFXManager>().PlayBlockSpell();
        // Optionally set a timer to deactivate the block spell // 5 seconds duration
        yield return new WaitForSeconds(5f);
        isBlocking = false;
        // Optionally stop the VFX for blocking
        GetComponent<PlayerVFXManager>().StopBlockSpell();
    }


    private IEnumerator RageSpell()
    {
        Debug.Log("Playing Rage Spell");
        isRageSpell = true;
        // Activate VFX for blocking
        GetComponent<PlayerVFXManager>().PlayRageSpell();
        // Optionally set a timer to deactivate the block spell // 5 seconds duration
        yield return new WaitForSeconds(5f);
        isRageSpell = false;
        // Optionally stop the VFX for blocking
        GetComponent<PlayerVFXManager>().StopRageSpell();
    }

    private void ApplyDamageToEnemies(float radius, int damage)
    {
        Debug.Log("Entering Damange to Enemies in Firespell");
        Vector3 adjustedPosition = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);

        Collider[] hitColliders = Physics.OverlapSphere(adjustedPosition, radius);

        foreach (var hitCollider in hitColliders)
        {
            Character enemyCharacter = hitCollider.GetComponent<Character>();
            if (enemyCharacter != null && !enemyCharacter.isPlayer)
            {
                int finalDamage = damage;
                if (isRageSpell)
                {
                    finalDamage *= 2; // Apply double damage if launched during Rage Spell
                }
                Debug.Log("Applying Damage with Fire Spell");
                enemyCharacter.ApplyDamage(finalDamage, transform.position);
            }
        }
    }

    public void CastFreezeSpell(float radius)
    {
        Debug.Log("Casting Freeze Spell");

        Vector3 adjustedPosition = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);
        // Find all enemies within the freeze radius and apply the freeze effect
        Collider[] hitColliders = Physics.OverlapSphere(adjustedPosition, radius);


        foreach (var hitCollider in hitColliders)
        {
            Character enemyCharacter = hitCollider.GetComponent<Character>();
            if (enemyCharacter != null && !enemyCharacter.isPlayer)
            {
                Debug.Log("Applying Freeze Effect");
                StartCoroutine(ApplyFreezeEffect(enemyCharacter));
            }
        }
    }

    private IEnumerator ApplyFreezeEffect(Character character)
    {
        Debug.Log("Inside Freeze Effect");
        character.Freeze(); // Implement this method in your enemy script
        yield return new WaitForSeconds(freezeDuration);
        character.Unfreeze(); // Implement this method in your enemy script
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red; // Set the color of the Gizmo
        Vector3 adjustedPosition = new Vector3(transform.position.x, transform.position.y + 5, transform.position.z);

        Gizmos.DrawWireSphere(adjustedPosition, fireSpellDamageRadius); // Draw a wireframe sphere
    }

}
