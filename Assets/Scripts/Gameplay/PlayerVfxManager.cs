using UnityEngine;

public class PlayerVfxManager : MonoBehaviour
{
    [Header("Particle Systems")]
    public ParticleSystem landDustParticle;
    public ParticleSystem slideParticle;
    public ParticleSystem sprintParticle;
    public ParticleSystem windParticle;

    [Header("Settings")]
    public float minLandSpeedForDust = 5f;

    private PlayerMovement playerMovement;
    private Rigidbody rb;
    private bool wasGrounded;
    private bool wasSliding;
    private float lastFallSpeed;
    private Vector3 landDustOriginalScale;
    private Vector3 slideOriginalScale;
    private Vector3 sprintOriginalScale;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        wasGrounded = true;
        wasSliding = false;

        if (landDustParticle != null)
            landDustOriginalScale = landDustParticle.transform.localScale;
        if (slideParticle != null)
            slideOriginalScale = slideParticle.transform.localScale;
        if (sprintParticle != null)
            sprintOriginalScale = sprintParticle.transform.localScale;
    }

    private void Update()
    {
        HandleWindVFXRotation();
        HandleLandDust();
        HandleSlideParticle();
        HandleSprintParticle();
        FixParticleScaling();
    }

    private void FixParticleScaling()
    {
        Vector3 parentScale = transform.localScale;

        if (landDustParticle != null)
        {
            landDustParticle.transform.localScale = new Vector3(
                landDustOriginalScale.x / parentScale.x,
                landDustOriginalScale.y / parentScale.y,
                landDustOriginalScale.z / parentScale.z
            );
        }

        if (slideParticle != null)
        {
            slideParticle.transform.localScale = new Vector3(
                slideOriginalScale.x / parentScale.x,
                slideOriginalScale.y / parentScale.y,
                slideOriginalScale.z / parentScale.z
            );
        }

        if (sprintParticle != null)
        {
            sprintParticle.transform.localScale = new Vector3(
                sprintOriginalScale.x / parentScale.x,
                sprintOriginalScale.y / parentScale.y,
                sprintOriginalScale.z / parentScale.z
            );
        }
    }

    private void HandleLandDust()
    {
        if (playerMovement == null || rb == null) return;

        if (!playerMovement.grounded)
        {
            lastFallSpeed = Mathf.Abs(rb.linearVelocity.y);
        }

        if (!wasGrounded && playerMovement.grounded)
        {
            if (lastFallSpeed > minLandSpeedForDust && landDustParticle != null)
            {
                ParticleSystem.MainModule main = landDustParticle.main;
                main.simulationSpace = ParticleSystemSimulationSpace.World;

                landDustParticle.transform.position = transform.position;
                landDustParticle.Play();
            }
        }

        wasGrounded = playerMovement.grounded;
    }

    private void HandleSlideParticle()
    {
        if (playerMovement == null || slideParticle == null) return;

        if (!wasSliding && playerMovement.isSliding)
        {
            slideParticle.Play();
            windParticle.Play();
        }
        else if (wasSliding && !playerMovement.isSliding)
        {
            slideParticle.Stop();
            windParticle.Stop();
        }

        wasSliding = playerMovement.isSliding;
    }

    private void HandleSprintParticle()
    {
        if (playerMovement == null || sprintParticle == null || rb == null) return;

        float speed = new Vector2(rb.linearVelocity.x, rb.linearVelocity.z).magnitude;
        bool shouldPlay = playerMovement.isSprinting && speed > 5f && playerMovement.grounded;

        if (shouldPlay && !sprintParticle.isPlaying)
        {
            sprintParticle.Play();
        }
        else if (!shouldPlay && sprintParticle.isPlaying)
        {
            sprintParticle.Stop();
        }
    }

    private void HandleWindVFXRotation()
    {
        //windParticle.transform.rotation = transform.rotation;
    }
}
