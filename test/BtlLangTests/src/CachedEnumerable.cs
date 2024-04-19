using BtlLang;

namespace BtlLangTests;

public class CachedEnumerableTest
{
    [Fact]
    public void TestSeek()
    {
        var enumerable = new List<int> { 1, 2, 3, 4, 5 };
        var enumerator = new CachedEnumerable<int>(enumerable);
        enumerator.MoveNext();
        Assert.True(enumerator.Seek(2));
        Assert.Equal(3, enumerator.Current);
        Assert.True(enumerator.Seek(-1));
        Assert.Equal(2, enumerator.Current);
        Assert.True(enumerator.Seek(-1));
        Assert.Equal(1, enumerator.Current);
        Assert.False(enumerator.Seek(-1));
        Assert.Equal(1, enumerator.Current);
        Assert.True(enumerator.Seek(4));
        Assert.Equal(5, enumerator.Current);
        Assert.False(enumerator.Seek(1));
        Assert.Equal(5, enumerator.Current);
    }

    [Fact]
    public void TestNext()
    {
        var enumerable = new List<int> { 1, 2, 3, 4, 5 };
        var enumerator = new CachedEnumerable<int>(enumerable);
        enumerator.MoveNext();
        Assert.Equal(1, enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(2, enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(3, enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(4, enumerator.Current);
        Assert.True(enumerator.MoveNext());
        Assert.Equal(5, enumerator.Current);
        Assert.False(enumerator.MoveNext());
        Assert.Equal(5, enumerator.Current);
    }

    [Fact]
    public void TestMovePrevious()
    {
        var enumerable = new List<int> { 1, 2, 3, 4, 5 };
        var enumerator = new CachedEnumerable<int>(enumerable);
        enumerator.MoveNext();
        Assert.Equal(1, enumerator.Current);
        Assert.False(enumerator.MovePrevious());
        Assert.Equal(1, enumerator.Current);
        Assert.True(enumerator.Seek(4));
        Assert.Equal(5, enumerator.Current);
        Assert.True(enumerator.MovePrevious());
        Assert.Equal(4, enumerator.Current);
    }

    [Fact]
    public void TestReset()
    {
        var enumerable = new List<int> { 1, 2, 3, 4, 5 };
        var enumerator = new CachedEnumerable<int>(enumerable);
        enumerator.MoveNext();
        Assert.Equal(1, enumerator.Current);
        Assert.True(enumerator.Seek(4));
        Assert.Equal(5, enumerator.Current);
        enumerator.Reset();
        Assert.Equal(1, enumerator.Current);
    }
}