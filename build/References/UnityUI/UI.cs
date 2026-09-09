using System;
using UnityEngine;

// Compile-time declarations only. This assembly is never shipped or executed.
namespace UnityEngine.EventSystems
{
    public abstract class UIBehaviour : MonoBehaviour { }
    public abstract class AbstractEventData
    {
        public virtual void Use() => throw new NotSupportedException();
    }
    public class BaseEventData : AbstractEventData { }
    public class PointerEventData : BaseEventData
    {
        public enum InputButton { Left, Right, Middle }
        public InputButton button { get => throw new NotSupportedException(); }
    }
}

namespace UnityEngine.UI
{
    public abstract class Graphic : EventSystems.UIBehaviour
    {
        public virtual Color color { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    }
    public abstract class MaskableGraphic : Graphic { }
    public class Text : MaskableGraphic
    {
        public virtual string text { get => throw new NotSupportedException(); set => throw new NotSupportedException(); }
    }
}
