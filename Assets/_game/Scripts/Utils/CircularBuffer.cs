using System;
using Unity.Netcode;
using UnityEngine;

namespace _game.Scripts.Utils
{
    public class CircularBuffer<T>
    {
        private T[] _buffer;
        private int _bufferSize;

        public CircularBuffer(int bufferSize)
        {
            _bufferSize = bufferSize;
            _buffer = new T[bufferSize];
        }

        public void Add(T item, int index) => _buffer[index % _bufferSize] = item;
        public T Get(int index) => _buffer[index % _bufferSize];
        public void Clear() => _buffer = new T[_bufferSize];
    }

    public struct InputPayload : INetworkSerializable
    {
        public int Tick;
        public DateTime TimeStamp;
        public ulong NetworkObjectId;
        public Vector2 InputVector;
        public Vector2 LookVector;
        public Vector3 Position;

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref TimeStamp);
            serializer.SerializeValue(ref NetworkObjectId);
            serializer.SerializeValue(ref InputVector);
            serializer.SerializeValue(ref LookVector);
            serializer.SerializeValue(ref Position);
        }
    }

    public struct StatePayload : INetworkSerializable
    {
        public int Tick;
        public ulong NetworkObjectId;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Velocity;


        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Tick);
            serializer.SerializeValue(ref NetworkObjectId);
            serializer.SerializeValue(ref Position);
            serializer.SerializeValue(ref Rotation);
            serializer.SerializeValue(ref Velocity);
        }
    }
}
