using System;
using Core.Scripts.Logic;
using UnityEngine;

namespace Core.Scripts.Character
{
  public class HeroAnimator : MonoBehaviour, IAnimationStateReader
  {
    private static readonly int MoveHash = UnityEngine.Animator.StringToHash("Walking");
    private static readonly int AttackHash = UnityEngine.Animator.StringToHash("AttackNormal");
    private static readonly int HitHash = UnityEngine.Animator.StringToHash("Hit");
    private static readonly int DieHash = UnityEngine.Animator.StringToHash("Die");

    private readonly int _idleStateHash = UnityEngine.Animator.StringToHash("Idle");
    private readonly int _idleStateFullHash = UnityEngine.Animator.StringToHash("Base Layer.Idle");
    private readonly int _attackStateHash = UnityEngine.Animator.StringToHash("Attack Normal");
    private readonly int _walkingStateHash = UnityEngine.Animator.StringToHash("Run");
    private readonly int _deathStateHash = UnityEngine.Animator.StringToHash("Die");
    
    public event Action<AnimatorState> StateEntered;
    public event Action<AnimatorState> StateExited;
   
    public AnimatorState State { get; private set; }
    
    public UnityEngine.Animator Animator;
    public CharacterController CharacterController;

    private void Update()
    {
      Animator.SetFloat(MoveHash, CharacterController.velocity.magnitude, 0.1f, Time.deltaTime);
    }

    public bool IsAttacking => State == AnimatorState.Attack;
    

    public void PlayHit() => Animator.SetTrigger(HitHash);
    
    public void PlayAttack() => Animator.SetTrigger(AttackHash);

    public void PlayDeath() =>  Animator.SetTrigger(DieHash);

    public void ResetToIdle() => Animator.Play(_idleStateHash, -1);
    
    public void EnteredState(int stateHash)
    {
      State = StateFor(stateHash);
      StateEntered?.Invoke(State);
    }

    public void ExitedState(int stateHash) =>
      StateExited?.Invoke(StateFor(stateHash));
    
    private AnimatorState StateFor(int stateHash)
    {
      AnimatorState state;
      if (stateHash == _idleStateHash)
        state = AnimatorState.Idle;
      else if (stateHash == _attackStateHash)
        state = AnimatorState.Attack;
      else if (stateHash == _walkingStateHash)
        state = AnimatorState.Walking;
      else if (stateHash == _deathStateHash)
        state = AnimatorState.Died;
      else
        state = AnimatorState.Unknown;
      
      return state;
    }
  }
}