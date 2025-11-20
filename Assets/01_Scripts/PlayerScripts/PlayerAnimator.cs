using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    public Animator animator;

    // Animator parameter names as constants
    private const string IsWalking = "isWalking";
    private const string IsSitting = "isSitting";
    private const string IsLaying = "isLaying";
    private const string IsWorking = "isWorking";
    private const string PickUp = "PickUp";
    private const string WalkSpeed = "walkSpeed";
    private const string IsAlive = "isAlive";

    // Enum to manage current player state
    private enum PlayerState { Idle, Walking, Sitting, Laying, Working }
    private PlayerState currentState = PlayerState.Idle;

    void Start()
    {
        // Initialize animator, if not assigned from Inspector
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }

    /// <summary>
    /// Resets all animation states to false.
    /// </summary>
    private void ResetAllStates()
    {
        animator.SetBool(IsWalking, false);
        animator.SetBool(IsSitting, false);
        animator.SetBool(IsLaying, false);
        animator.SetBool(IsWorking, false);
    }

    /// <summary>
    /// Switches to the specified animation state and ensures only one state is active.
    /// </summary>
    /// <param name="newState">The new animation state to switch to.</param>
    /// <summary>
    /// Switches to the specified animation state and ensures only one state is active.
    /// </summary>
    /// <param name="newState">The new animation state to switch to.</param>
    private void ChangeState(PlayerState newState) // Add the 'private' keyword
    {
        if (currentState == newState) return; // No need to change if already in the desired state

        ResetAllStates();
        currentState = newState;

        switch (newState)
        {
            case PlayerState.Walking:
                animator.SetBool(IsWalking, true);
                break;
            case PlayerState.Sitting:
                animator.SetBool(IsSitting, true);
                break;
            case PlayerState.Laying:
                animator.SetBool(IsLaying, true);
                break;
            case PlayerState.Working:
                animator.SetBool(IsWorking, true);
                break;
            case PlayerState.Idle:
            default:
                // No action for idle state
                break;
        }
    }


    /// <summary>
    /// Plays the walking animation.
    /// </summary>
    public void PlayWalkingAnimation(bool isWalking)
    {
        if (animator == null) return; // Safety check

        if (isWalking)
        {
            ChangeState(PlayerState.Walking);
        }
        else if (currentState == PlayerState.Walking)
        {
            ChangeState(PlayerState.Idle);
        }
    }

    /// <summary>
    /// Plays the sitting animation.
    /// </summary>
    public void PlaySitAnimation(bool isSitting)
    {
        if (animator == null) return; // Safety check

        if (isSitting)
        {
            ChangeState(PlayerState.Sitting);
        }
        else if (currentState == PlayerState.Sitting)
        {
            ChangeState(PlayerState.Idle);
        }
    }

    /// <summary>
    /// Plays the laying animation.
    /// </summary>
    public void PlayLayingAnimation(bool isLaying)
    {
        if (animator == null) return; // Safety check

        if (isLaying)
        {
            ChangeState(PlayerState.Laying);
        }
        else if (currentState == PlayerState.Laying)
        {
            ChangeState(PlayerState.Idle);
        }
    }

    /// <summary>
    /// Plays the working animation.
    /// </summary>
    public void PlayWorkingAnimation(bool isWorking)
    {
        if (animator == null) return; // Safety check

        if (isWorking)
        {
            ChangeState(PlayerState.Working);
        }
        else if (currentState == PlayerState.Working)
        {
            ChangeState(PlayerState.Idle);
        }
    }
    public void PlayCookingAnimation()
    {
        if (animator == null) return; // Safety check

        animator.SetTrigger("isCooking");
    }

    /// <summary>
    /// Triggers the pick up animation.
    /// </summary>
    public void PlayPickUpAnimation()
    {
        if (animator == null) return; // Safety check

        animator.SetTrigger(PickUp);
    }

    /// <summary>
    /// Sets the speed of the walking animation.
    /// </summary>
    /// <param name="speed">The speed of the walking animation.</param>
    public void SetWalkingSpeed(float speed)
    {
        if (animator == null) return; // Safety check

        animator.SetFloat(WalkSpeed, speed);
    }

    /// <summary>
    /// Sets whether the player is alive.
    /// </summary>
    public void SetAlive(bool isAlive)
    {
        if (animator == null) return; // Safety check

        animator.SetBool(IsAlive, isAlive);
    }

    /// <summary>
    /// Plays the stun animation.
    /// </summary>
    /// <param name="isStun">True if the player is stunned, false otherwise.</param>
    public void PlayerStunAnimation(bool isStun)
    {
        if (animator == null) return; // Safety check

        animator.SetBool("isStun", isStun);

        if (isStun)
            animator.SetTrigger("StunStart");
    }

    public void SetDie()
    {
        animator.SetBool("isDie", true);
    }

    /// <summary>
    /// Triggers the eating animation.
    /// </summary>
    public void EatingAnimation()
    {
        if (animator == null) return; // Safety check

        animator.SetTrigger("isEating");
    }

    /// <summary>
    /// Triggers the drinking animation.
    /// </summary>
    public void DrinkingAnimation()
    {
        if (animator == null) return; // Safety check

        animator.SetTrigger("isDrinking");
    }

    /// <summary>
    /// Plays the ending motion animation.
    /// </summary>
    public void PlayEndingMotion()
    {
        if (animator == null) return; // Safety check

        animator.SetBool("isEnding", true);
    }

    public void SetWorkingState(bool isWorking)
    {
        animator.SetBool(IsWalking, isWorking);
    }
}
