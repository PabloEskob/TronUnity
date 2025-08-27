/// ---------------------------------------------
/// Behavior Designer
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------
namespace Opsive.BehaviorDesigner.Integrations.UltimateInventorySystem
{
    using Opsive.BehaviorDesigner.Runtime.Tasks;
    using Opsive.BehaviorDesigner.Runtime.Tasks.Conditionals;
    using Opsive.GraphDesigner.Runtime;
    using Opsive.GraphDesigner.Runtime.Variables;
    using Opsive.UltimateInventorySystem.Exchange;
    using UnityEngine;

    [NodeDescription("Determines if an currencyOwner has at least the amount of currency specified.")]
    [NodeIcon("27b9242e1ae448e4596f10d7892c76b9")]
    public class CompareCurrency : Conditional
    {
        [Tooltip("Choose how to compare the currency value.")]
        public SharedVariable<Compare> m_Compare;
        [Tooltip("The currency amounts to check within the currency owner.")]
        public SharedVariable<CurrencyAmounts> m_CurrencyAmounts;
        
        // Cache the inventory component
        private CurrencyOwner m_CurrencyOwner;
        private GameObject m_PrevGameObject;

        /// <summary>
        /// Get the inventory on start.
        /// </summary>
        public override void OnStart()
        {
            if (gameObject != m_PrevGameObject) {
                m_CurrencyOwner = gameObject.GetComponent<CurrencyOwner>();
                m_PrevGameObject = gameObject;
            }
        }
        
        /// <summary>
        /// Returns success if the inventory has the item amount specified, otherwise it fails.
        /// </summary>
        /// <returns>The task status.</returns>
        public override TaskStatus OnUpdate()
        {
            if (m_CurrencyOwner == null) { return TaskStatus.Failure; }

            var equalTo = m_CurrencyOwner.CurrencyAmount.EquivalentTo(m_CurrencyAmounts.Value);
            if (equalTo && (m_Compare.Value == Compare.SmallerOrEqualsTo || m_Compare.Value == Compare.GreaterOrEqualsTo || m_Compare.Value == Compare.EqualsTo)) {
                return TaskStatus.Success;
            }
            
            var greaterOrEqualsTo = m_CurrencyOwner.CurrencyAmount.GreaterThanOrEqualTo(m_CurrencyAmounts.Value);

            if (greaterOrEqualsTo && (m_Compare.Value == Compare.GreaterThan || m_Compare.Value == Compare.GreaterOrEqualsTo) ) {
                return TaskStatus.Success;
            }else if ( m_Compare.Value ==Compare.SmallerThan || m_Compare.Value == Compare.SmallerOrEqualsTo) {
                return TaskStatus.Success;
            }
            
            return TaskStatus.Failure;
        }

        /// <summary>
        /// Reset the public variables.
        /// </summary>
        public override void Reset()
        {
            m_CurrencyAmounts = null;
        }
    }
}