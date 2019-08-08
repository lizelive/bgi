using System.Collections.Generic;
/// world's worst 
public class PriorityQueue<T>
{
    Dictionary<T, float> backing = new Dictionary<T, float>();


    public bool Any()
    {
        return backing.Count != 0;
    }

    public T PopSmallest()
    {
        var remove = backing.MinBy(x => x.Value);
        backing.Remove(remove.Key);
        return remove.Key;
    }

    public void Insert(float priority, T data)
    {
        backing[data] = priority;
    }
}
