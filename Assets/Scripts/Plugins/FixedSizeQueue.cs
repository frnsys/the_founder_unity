using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

// First-in, first-out serializable queue with a fixed size.
// https://stackoverflow.com/a/5852926/1097920
[System.Serializable]
public class FixedSizeQueue<T> : ISerializationCallbackReceiver {
    public int Size;
    public int Count {
        get { return q.Count; }
    }
    protected Queue<T> q = new Queue<T>();

    public FixedSizeQueue(int size) {
        Size = size;
    }

    [SerializeField]
    private List<T> values = new List<T>();

    public void OnBeforeSerialize() {
        values.Clear();
        foreach(T value in q) {
            values.Add(value);
        }
    }

    public void OnAfterDeserialize() {
        q = new Queue<T>();
        for(int i = 0; i < values.Count; i++)
            q.Enqueue(values[i]);
    }

     public void Enqueue(T obj) {
         q.Enqueue(obj);
         while (Count > Size)
            q.Dequeue();
     }

     public IEnumerable<T> Skip(int count) {
         return q.Skip(count);
     }

     public IEnumerator GetEnumerator() {
         return q.GetEnumerator();
     }
 }
