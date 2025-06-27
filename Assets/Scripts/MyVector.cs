
using System.Collections;

public class MyVector
{
    private readonly ArrayList a;

    public MyVector()
    {
        a = new ArrayList();
    }

    public MyVector(string s)
    {
        a = new ArrayList();
    }

    public MyVector(ArrayList a)
    {
        this.a = a;
    }

    public void addElement(object o)
    {
        _ = a.Add(o);
    }

    public bool contains(object o)
    {
        return a.Contains(o);
    }

    public int size()
    {
        return a == null ? 0 : a.Count;
    }

    public object elementAt(int index)
    {
        return index > -1 && index < a.Count ? a[index] : null;
    }

    public void set(int index, object obj)
    {
        if (index > -1 && index < a.Count)
        {
            a[index] = obj;
        }
    }

    public void setElementAt(object obj, int index)
    {
        if (index > -1 && index < a.Count)
        {
            a[index] = obj;
        }
    }

    public int indexOf(object o)
    {
        return a.IndexOf(o);
    }

    public void removeElementAt(int index)
    {
        if (index > -1 && index < a.Count)
        {
            a.RemoveAt(index);
        }
    }

    public void removeElement(object o)
    {
        a.Remove(o);
    }

    public void removeAllElements()
    {
        a.Clear();
    }

    public void insertElementAt(object o, int i)
    {
        a.Insert(i, o);
    }

    public object firstElement()
    {
        return a[0];
    }

    public object lastElement()
    {
        return a[^1];
    }
}
