using Photon.Pun;
using System;
using System.Collections;
using UnityEngine;

public class SimpleBotAI : MonoBehaviour
{
    public event Action Attack;
    public event Action Throw;
    public event Action<int> Rotate;
    public event Action DodgeStart;
    public event Action DodgeEnd;

    [Header("Brain")]
    public float decisionInterval = 2f;

    [Header("Sensing")]
    public Transform eyePoint;
    public float adjacentX = 1.8f;
    public float maxSenseRange = 10f;
    public LayerMask standingHurtboxMask;

    [Header("Chaos knobs")]
    [Range(0, 1)] public float rerollDirectionChance = 0.6f;
    [Range(0, 1)] public float punchChanceIfAdjacent = 0.9f;
    [Range(0, 1)] public float throwChanceIfFar = 0.85f;
    [Range(0, 1)] public float crouchChanceIfDanger = 0.2f;
    public Vector2 crouchTapDurationRange = new Vector2(0.2f, 2f);

    // Optional: let some external trigger set this when a projectile is incoming
    public bool incomingProjectile;

    private int facing = 1;
    private float crouchUntil;

    private Coroutine brainRoutine;

    private bool gameStarted = false;

    public void Start()
    {
        if (brainRoutine == null) brainRoutine = StartCoroutine(BrainLoop());
    }

    public void OnGameStart()
    {
        gameStarted = true;
    }

    public void Disable()
    {
        if (brainRoutine != null)
        {
            StopCoroutine(brainRoutine);
            brainRoutine = null;
        }
    }

    private IEnumerator BrainLoop()
    {
        while (true)
        {
            yield return new WaitUntil(() => gameStarted);

            yield return new WaitForSeconds(decisionInterval);
            if (!PhotonNetwork.IsMasterClient) continue;

            // If currently in crouch tap, do nothing until it ends
            if (Time.time < crouchUntil) continue;

            // Chaos: choose direction often
            if (UnityEngine.Random.value < rerollDirectionChance)
            {
                int newFacing = UnityEngine.Random.value < 0.5f ? -1 : 1;
                if (newFacing != facing)
                {
                    facing = newFacing;
                    Rotate?.Invoke(facing); // <- emits "Rotate input"
                }
            }

            var startRaycastPosition = eyePoint.position + Vector3.right * facing;
            // Sense standing target in facing direction
            var hit = Physics2D.Raycast(startRaycastPosition, Vector2.right * facing, maxSenseRange, standingHurtboxMask);
            bool hasTarget = hit.collider != null;

            float dist = hasTarget ? hit.distance : float.PositiveInfinity;

            bool adjacent = hasTarget && dist <= adjacentX;

            bool far = hasTarget && dist > adjacentX;

            // Danger logic (simple)
            bool danger = incomingProjectile || adjacent;

            if (danger && UnityEngine.Random.value < crouchChanceIfDanger)
            {
                // Tap crouch: start then end after duration
                float dur = UnityEngine.Random.Range(crouchTapDurationRange.x, crouchTapDurationRange.y);
                DodgeStart?.Invoke();
                crouchUntil = Time.time + dur;
                StartCoroutine(EndCrouchAfter(dur));
                incomingProjectile = false;
                continue;
            }

            if (adjacent && UnityEngine.Random.value < punchChanceIfAdjacent)
            {
                Attack?.Invoke();
                continue;
            }

            if (far && UnityEngine.Random.value < throwChanceIfFar)
            {
                Throw?.Invoke();
                continue;
            }

            // If no standing target, sometimes flip anyway for chaos
            if (!hasTarget && UnityEngine.Random.value < 0.5f)
            {
                facing *= -1;
                Rotate?.Invoke(facing);
            }
        }
    }

    private IEnumerator EndCrouchAfter(float dur)
    {
        yield return new WaitForSeconds(dur);
        DodgeEnd?.Invoke();
    }

    // Call this from a trigger script if you want
    public void NotifyIncomingProjectile() => incomingProjectile = true;

    private void OnEnable()
    {
        if (brainRoutine == null) brainRoutine = StartCoroutine(BrainLoop());
    }

    private void OnDisable()
    {
        Disable();
    }
}
