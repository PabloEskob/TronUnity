namespace Core.Scripts.Logic
{
  public interface IAnimationStateReader
  {
    /*void EnteredState(int stateHash);
    void ExitedState(int stateHash);*/
    AnimatorState State { get; }
  }
}