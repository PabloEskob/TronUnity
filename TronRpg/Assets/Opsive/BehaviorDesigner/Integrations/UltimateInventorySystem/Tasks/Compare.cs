/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateInventorySystem
{
    using System;

    /// <summary>
    /// Specifies the comparison type.
    /// </summary>
    [Serializable]
    public enum Compare
    {
        EqualsTo,           // Specifies an equals comparison (==)
        GreaterThan,        // Specifies a greater than comparison (>).
        SmallerThan,        // Specifies a smaller than comparison (<).
        SmallerOrEqualsTo,  // Specifies a smaller than or equal to comparison (<=).
        GreaterOrEqualsTo,  // Specifies a greater than or equal to comparison (>=).
    }
}