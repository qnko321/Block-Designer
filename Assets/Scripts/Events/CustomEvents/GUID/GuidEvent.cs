using System;
using UnityEngine;

namespace Events
{
    [CreateAssetMenu(fileName = "New Guid Event", menuName = "Game Events/Guid Event")]
    public class GuidEvent : BaseGameEvent<Guid> { }
}