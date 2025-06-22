namespace Core.Input.Interfaces
{
    public interface ICombatInput
    {
        bool IsAttacking         { get; }
        bool IsBlocking          { get; }
        bool IsSpecialAttack     { get; }
        int  WeaponSwitchDirection{ get; } // –1 0 1
    }
}