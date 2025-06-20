﻿using UnityEngine;

namespace Assets.Scripts.Movement.Interface
{
    public interface IMovementInput
    {
        Vector2 MovementVector { get; }
        bool IsRunning { get; }
        bool IsDodging { get; }
    }
}