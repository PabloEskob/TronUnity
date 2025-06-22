using UnityEngine;

namespace Core.Services
{
    public static class ComponentFinder
    {
        /// <summary>
        /// Ищет все компоненты типа T в дочерних объектах и пишет лог, если ничего не найдено.
        /// </summary>
        /// <typeparam name="T">Тип компонента</typeparam>
        /// <param name="parent">Родительский объект</param>
        /// <param name="logIfMissing">Писать ли лог, если ничего не найдено</param>
        /// <returns>Массив компонентов T (может быть пустым)</returns>
        public static T[] FindAllInChildren<T>(GameObject parent, bool logIfMissing = true) where T : Component
        {
            T[] components = parent.GetComponentsInChildren<T>(includeInactive: true);
            if (components.Length == 0 && logIfMissing)
            {
                UnityEngine.Debug.LogWarning($"[ComponentFinder] Не найдено компонентов типа {typeof(T).Name} в дочерних объектах {parent.name}");
            }

            return components;
        }

        /// <summary>
        /// Ищет первый компонент типа T в дочерних объектах и пишет лог, если он не найден.
        /// </summary>
        public static T FindFirstInChildren<T>(GameObject parent, bool logIfMissing = true) where T : Component
        {
            T component = parent.GetComponentInChildren<T>(includeInactive: true);
            if (component == null && logIfMissing)
            {
                UnityEngine.Debug.LogWarning($"[ComponentFinder] Компонент типа {typeof(T).Name} не найден в дочерних объектах {parent.name}");
            }

            return component;
        }
    }
}