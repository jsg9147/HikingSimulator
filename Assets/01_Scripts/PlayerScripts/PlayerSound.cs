using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DarkTonic.MasterAudio;

public class PlayerSound : MonoBehaviour
{
    public void FootStepSound()
    {
        MasterAudio.PlaySound3DAtTransform("FootStep", transform);
    }

    public void TentSound()
    {
        MasterAudio.PlaySound3DAtTransform("TentWorking", transform);
    }

    public void PickupSound()
    {
        MasterAudio.PlaySound3DAtTransform("PickUp", transform);
    }

    public void CookingSound()
    {
        MasterAudio.PlaySound3DAtTransform("Cooking", transform);
    }

    public void StunRecovery()
    {
        GameManager.instance.playerStateController.StunRecovery();
    }
    public void UnBlockedMovement()
    {
        GameManager.instance.playerStateController.SetState(PlayerState.Idle);
    }

    public void InteractionSounc()
    {
        MasterAudio.PlaySound3DAtTransform("Interaction", transform);
    }

    public void ReturnToIdle()
    {
        GameManager.instance.playerStateController.SetState(PlayerState.Idle);
    }

    public void IncrementPedometer()
    {
        GameManager.instance.survivalStatsManager.IncrementStepCount();
    }

    public void EndingCameraEffect()
    {
        CameraController.instance.EndingCameraEffect();
    }
}
