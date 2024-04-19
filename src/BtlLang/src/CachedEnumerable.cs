namespace BtlLang;

public class CachedEnumerable<T>
{
    private readonly List<T> _cache = [];
    private int _currentIndex = -1;
    
    public T Current { get; private set; }
    
    public CachedEnumerable(IEnumerable<T> enumerable)
    {
        foreach (var item in enumerable)
        {
            _cache.Add(item);
        }
    }

    public bool Seek(int amount)
    {
        if(_currentIndex + amount < 0 || _currentIndex + amount >= _cache.Count) return false;
        _currentIndex += amount;
        Current = _cache[_currentIndex];
        return true;
    }
    
    public bool MoveNext()
    {
        if (_currentIndex + 1 >= _cache.Count) return false;
        _currentIndex++;
        Current = _cache[_currentIndex];
        return true;
    }

    public bool MovePrevious()
    {
        if (_currentIndex - 1 < 0) return false;
        _currentIndex--;
        Current = _cache[_currentIndex];
        return true;
    }
    
    public void Reset()
    {
        _currentIndex = 0;
        Current = _cache[_currentIndex];
    }
    
    
    
}