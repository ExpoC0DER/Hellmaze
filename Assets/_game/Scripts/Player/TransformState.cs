using System;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Player
{
    public class TransformState : INetworkSerializable, IEquatable<TransformState>
    {
        public int Tick;
        public Vector3 Position;
        public Quaternion Rotation;
        public bool HasStartedMoving;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            if (serializer.IsReader)
            {
                FastBufferReader reader = serializer.GetFastBufferReader();
                reader.ReadValueSafe(out Tick);
                reader.ReadValueSafe(out Position);
                reader.ReadValueSafe(out Rotation);
                reader.ReadValueSafe(out HasStartedMoving);
            }
            else
            {
                FastBufferWriter writer = serializer.GetFastBufferWriter();
                writer.WriteValueSafe(Tick);
                writer.WriteValueSafe(Position);
                writer.WriteValueSafe(Rotation);
                writer.WriteValueSafe(HasStartedMoving);
            }
        }
        public bool Equals(TransformState other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            return Tick == other.Tick && Position.Equals(other.Position) && Rotation.Equals(other.Rotation) && HasStartedMoving == other.HasStartedMoving;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != this.GetType())
                return false;
            return Equals((TransformState)obj);
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Tick, Position, Rotation, HasStartedMoving);
        }
    }
}
