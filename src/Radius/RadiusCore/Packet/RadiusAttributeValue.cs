using System.Collections;

namespace RadiusCore.Packet;

public class RadiusAttributeValue : IList<byte[]>
{
    List<byte[]> value = new List<byte[]>();

    internal RadiusAttributeValue(byte[] data)
    {
        value.Add(data);
    }

    public IEnumerator<byte[]> GetEnumerator()
    {
        return value.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)value).GetEnumerator();
    }

    public void Add(byte[] item)
    {
        value.Add(item);
    }

    public void Clear()
    {
        value.Clear();
    }

    public bool Contains(byte[] item)
    {
        return value.Contains(item);
    }

    public void CopyTo(byte[][] array, int arrayIndex)
    {
        value.CopyTo(array, arrayIndex);
    }

    public bool Remove(byte[] item)
    {
        return value.Remove(item);
    }

    public int Count => value.Count;
    public bool IsReadOnly => false;
    public int IndexOf(byte[] item)
    {
        return value.IndexOf(item);
    }

    public void Insert(int index, byte[] item)
    {
        value.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        value.RemoveAt(index);
    }

    public byte[] this[int index]
    {
        get => value[index];
        set
        {
            this.value[index] = value;
        }
    }

    public virtual byte[]? Value
    {
        get { return value.Count > 0 ? value[0] : null; }
    }
}
