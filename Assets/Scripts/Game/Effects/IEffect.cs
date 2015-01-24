/*
 * I had a great system with an IEffect interface and all
 * effects implementing that interface, but Unity's serialization system sucks and
 * does not support that kind of sensible architecture.
 * Unity cannot serialize a list of derived classes properly - it will revert them all to their base class
 * when deserializing. Unity _will_ handle these derived classes properly if the base class inherits from
 * ScriptableObject. So IEffect inherits from ScriptableObject. Hopefully it will not add too much overhead.
 */

using UnityEngine;

[System.Serializable]
public class IEffect : ScriptableObject {
    public virtual void Apply(Company company) {}
    public virtual void Remove(Company company) {}
}
