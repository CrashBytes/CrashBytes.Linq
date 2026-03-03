namespace CrashBytes.Linq.Tests;

public class WhereNotNullTests
{
    [Fact]
    public void WhereNotNull_ReferenceTypes_FiltersNulls()
    {
        var source = new string?[] { "a", null, "b", null, "c" };
        var result = source.WhereNotNull().ToList();
        Assert.Equal(new[] { "a", "b", "c" }, result);
    }

    [Fact]
    public void WhereNotNull_ReferenceTypes_EmptySequence_ReturnsEmpty()
    {
        var source = Array.Empty<string?>();
        Assert.Empty(source.WhereNotNull());
    }

    [Fact]
    public void WhereNotNull_ReferenceTypes_AllNulls_ReturnsEmpty()
    {
        var source = new string?[] { null, null, null };
        Assert.Empty(source.WhereNotNull());
    }

    [Fact]
    public void WhereNotNull_ReferenceTypes_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<string?> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.WhereNotNull().ToList());
    }

    [Fact]
    public void WhereNotNull_NullableValueTypes_FiltersNulls()
    {
        var source = new int?[] { 1, null, 2, null, 3 };
        var result = source.WhereNotNull().ToList();
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void WhereNotNull_NullableValueTypes_AllNulls_ReturnsEmpty()
    {
        var source = new int?[] { null, null };
        Assert.Empty(source.WhereNotNull());
    }

    [Fact]
    public void WhereNotNull_NullableValueTypes_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int?> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.WhereNotNull().ToList());
    }
}

public class WhereIfTests
{
    [Fact]
    public void WhereIf_ConditionTrue_AppliesFilter()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.WhereIf(true, x => x > 3).ToList();
        Assert.Equal(new[] { 4, 5 }, result);
    }

    [Fact]
    public void WhereIf_ConditionFalse_ReturnsUnchanged()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.WhereIf(false, x => x > 3).ToList();
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void WhereIf_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.WhereIf(true, x => x > 0));
    }

    [Fact]
    public void WhereIf_NullPredicate_ThrowsArgumentNullException()
    {
        var source = new[] { 1, 2, 3 };
        Assert.Throws<ArgumentNullException>(() => source.WhereIf(true, null!));
    }
}

public class IsNullOrEmptyLinqTests
{
    [Fact]
    public void IsNullOrEmpty_NullSource_ReturnsTrue()
    {
        IEnumerable<int>? source = null;
        Assert.True(source.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_EmptySequence_ReturnsTrue()
    {
        Assert.True(Array.Empty<int>().IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_NonEmpty_ReturnsFalse()
    {
        Assert.False(new[] { 1 }.IsNullOrEmpty());
    }

    [Fact]
    public void IsNullOrEmpty_EmptyList_ReturnsTrue()
    {
        Assert.True(new List<int>().IsNullOrEmpty());
    }
}

public class ForEachTests
{
    [Fact]
    public void ForEach_ExecutesActionOnEachElement()
    {
        var source = new[] { 1, 2, 3 };
        var collected = new List<int>();
        source.ForEach(x => collected.Add(x));
        Assert.Equal(new[] { 1, 2, 3 }, collected);
    }

    [Fact]
    public void ForEach_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.ForEach(_ => { }));
    }

    [Fact]
    public void ForEach_NullAction_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.ForEach((Action<int>)null!));
    }

    [Fact]
    public void ForEach_WithIndex_ProvidesCorrectIndex()
    {
        var source = new[] { "a", "b", "c" };
        var indices = new List<int>();
        source.ForEach((_, i) => indices.Add(i));
        Assert.Equal(new[] { 0, 1, 2 }, indices);
    }

    [Fact]
    public void ForEach_WithIndex_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.ForEach((_, _) => { }));
    }

    [Fact]
    public void ForEach_WithIndex_NullAction_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.ForEach((Action<int, int>)null!));
    }
}

public class ShuffleTests
{
    [Fact]
    public void Shuffle_ContainsSameElements()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.Shuffle().ToList();
        Assert.Equal(source.Length, result.Count);
        Assert.Equal(source.OrderBy(x => x), result.OrderBy(x => x));
    }

    [Fact]
    public void Shuffle_WithSeededRandom_IsDeterministic()
    {
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var result1 = source.Shuffle(new Random(42)).ToList();
        var result2 = source.Shuffle(new Random(42)).ToList();
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void Shuffle_EmptySequence_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<int>().Shuffle());
    }

    [Fact]
    public void Shuffle_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Shuffle());
    }
}

public class PickRandomTests
{
    [Fact]
    public void PickRandom_ReturnsElementFromSequence()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.PickRandom();
        Assert.Contains(result, source);
    }

    [Fact]
    public void PickRandom_WithSeededRandom_IsDeterministic()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result1 = source.PickRandom(new Random(42));
        var result2 = source.PickRandom(new Random(42));
        Assert.Equal(result1, result2);
    }

    [Fact]
    public void PickRandom_EmptySequence_ThrowsInvalidOperationException()
    {
        Assert.Throws<InvalidOperationException>(() => Array.Empty<int>().PickRandom());
    }

    [Fact]
    public void PickRandom_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.PickRandom());
    }

    [Fact]
    public void PickRandom_Count_ReturnsCorrectNumber()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.PickRandom(3).ToList();
        Assert.Equal(3, result.Count);
        Assert.All(result, item => Assert.Contains(item, source));
    }

    [Fact]
    public void PickRandom_Count_NoDuplicates()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.PickRandom(5, new Random(42)).ToList();
        Assert.Equal(5, result.Distinct().Count());
    }

    [Fact]
    public void PickRandom_CountExceedsLength_ReturnsAll()
    {
        var source = new[] { 1, 2, 3 };
        var result = source.PickRandom(10).ToList();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void PickRandom_NegativeCount_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.PickRandom(-1).ToList());
    }

    [Fact]
    public void PickRandom_Count_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.PickRandom(3));
    }
}

public class BatchTests
{
    [Fact]
    public void Batch_SplitsEvenly()
    {
        var source = new[] { 1, 2, 3, 4, 5, 6 };
        var result = source.Batch(3).Select(b => b.ToList()).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { 1, 2, 3 }, result[0]);
        Assert.Equal(new[] { 4, 5, 6 }, result[1]);
    }

    [Fact]
    public void Batch_LastBatchSmaller()
    {
        var source = new[] { 1, 2, 3, 4, 5 };
        var result = source.Batch(3).Select(b => b.ToList()).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(new[] { 1, 2, 3 }, result[0]);
        Assert.Equal(new[] { 4, 5 }, result[1]);
    }

    [Fact]
    public void Batch_SizeOfOne_EachElementSeparate()
    {
        var source = new[] { 1, 2, 3 };
        var result = source.Batch(1).Select(b => b.ToList()).ToList();
        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Batch_SizeLargerThanSource_ReturnsSingleBatch()
    {
        var source = new[] { 1, 2, 3 };
        var result = source.Batch(10).Select(b => b.ToList()).ToList();
        Assert.Single(result);
        Assert.Equal(new[] { 1, 2, 3 }, result[0]);
    }

    [Fact]
    public void Batch_EmptySource_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<int>().Batch(5));
    }

    [Fact]
    public void Batch_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Batch(5));
    }

    [Fact]
    public void Batch_ZeroSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.Batch(0).ToList());
    }

    [Fact]
    public void Batch_NegativeSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.Batch(-1).ToList());
    }
}

public class PairwiseTests
{
    [Fact]
    public void Pairwise_AppliesSelectorToConsecutivePairs()
    {
        var source = new[] { 1, 2, 3, 4 };
        var result = source.Pairwise((a, b) => a + b).ToList();
        Assert.Equal(new[] { 3, 5, 7 }, result);
    }

    [Fact]
    public void Pairwise_SingleElement_ReturnsEmpty()
    {
        var source = new[] { 1 };
        Assert.Empty(source.Pairwise((a, b) => a + b));
    }

    [Fact]
    public void Pairwise_EmptySource_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<int>().Pairwise((a, b) => a + b));
    }

    [Fact]
    public void Pairwise_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Pairwise((a, b) => a + b));
    }

    [Fact]
    public void Pairwise_NullSelector_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1, 2 }.Pairwise<int, int>(null!));
    }

    [Fact]
    public void Pairwise_StringConcatenation()
    {
        var source = new[] { "a", "b", "c" };
        var result = source.Pairwise((a, b) => a + b).ToList();
        Assert.Equal(new[] { "ab", "bc" }, result);
    }
}

public class ScanTests
{
    [Fact]
    public void Scan_RunningSum()
    {
        var source = new[] { 1, 2, 3 };
        var result = source.Scan((a, b) => a + b).ToList();
        Assert.Equal(new[] { 1, 3, 6 }, result);
    }

    [Fact]
    public void Scan_EmptySource_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<int>().Scan((a, b) => a + b));
    }

    [Fact]
    public void Scan_SingleElement_ReturnsSingleElement()
    {
        var result = new[] { 42 }.Scan((a, b) => a + b).ToList();
        Assert.Equal(new[] { 42 }, result);
    }

    [Fact]
    public void Scan_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Scan((a, b) => a + b));
    }

    [Fact]
    public void Scan_NullAccumulator_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Scan((Func<int, int, int>)null!));
    }

    [Fact]
    public void Scan_WithSeed_IncludesSeedAsFirstElement()
    {
        var source = new[] { 1, 2, 3 };
        var result = source.Scan(10, (a, b) => a + b).ToList();
        Assert.Equal(new[] { 10, 11, 13, 16 }, result);
    }

    [Fact]
    public void Scan_WithSeed_EmptySource_ReturnsSeedOnly()
    {
        var result = Array.Empty<int>().Scan(10, (a, b) => a + b).ToList();
        Assert.Equal(new[] { 10 }, result);
    }

    [Fact]
    public void Scan_WithSeed_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Scan(0, (a, b) => a + b));
    }

    [Fact]
    public void Scan_WithSeed_NullAccumulator_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Scan(0, (Func<int, int, int>)null!));
    }
}

public class InterleaveTests
{
    [Fact]
    public void Interleave_TwoSequences_AlternatesElements()
    {
        var a = new[] { 1, 2, 3 };
        var b = new[] { 10, 20, 30 };
        var result = a.Interleave(b).ToList();
        Assert.Equal(new[] { 1, 10, 2, 20, 3, 30 }, result);
    }

    [Fact]
    public void Interleave_ThreeSequences()
    {
        var a = new[] { 1, 2 };
        var b = new[] { 10, 20 };
        var c = new[] { 100, 200 };
        var result = a.Interleave(b, c).ToList();
        Assert.Equal(new[] { 1, 10, 100, 2, 20, 200 }, result);
    }

    [Fact]
    public void Interleave_UnequalLengths_SkipsExhausted()
    {
        var a = new[] { 1, 2, 3 };
        var b = new[] { 10 };
        var result = a.Interleave(b).ToList();
        Assert.Equal(new[] { 1, 10, 2, 3 }, result);
    }

    [Fact]
    public void Interleave_EmptyOther_ReturnsSource()
    {
        var a = new[] { 1, 2, 3 };
        var result = a.Interleave(Array.Empty<int>()).ToList();
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void Interleave_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Interleave(new[] { 1 }));
    }

    [Fact]
    public void Interleave_NullOthers_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Interleave(null!));
    }
}

public class FlattenTests
{
    [Fact]
    public void Flatten_NestedLists_ReturnsFlattened()
    {
        var source = new List<IEnumerable<int>>
        {
            new[] { 1, 2 },
            new[] { 3, 4 },
            new[] { 5 }
        };
        var result = source.Flatten().ToList();
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void Flatten_EmptyInnerSequence_Skipped()
    {
        var source = new List<IEnumerable<int>>
        {
            new[] { 1 },
            Array.Empty<int>(),
            new[] { 2 }
        };
        var result = source.Flatten().ToList();
        Assert.Equal(new[] { 1, 2 }, result);
    }

    [Fact]
    public void Flatten_EmptySource_ReturnsEmpty()
    {
        var source = new List<IEnumerable<int>>();
        Assert.Empty(source.Flatten());
    }

    [Fact]
    public void Flatten_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<IEnumerable<int>> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Flatten().ToList());
    }
}

public class PartitionTests
{
    [Fact]
    public void Partition_SplitsByPredicate()
    {
        var source = new[] { 1, 2, 3, 4, 5, 6 };
        var (even, odd) = source.Partition(x => x % 2 == 0);
        Assert.Equal(new[] { 2, 4, 6 }, even);
        Assert.Equal(new[] { 1, 3, 5 }, odd);
    }

    [Fact]
    public void Partition_AllMatch_NonMatchingEmpty()
    {
        var source = new[] { 2, 4, 6 };
        var (matching, nonMatching) = source.Partition(x => x % 2 == 0);
        Assert.Equal(new[] { 2, 4, 6 }, matching);
        Assert.Empty(nonMatching);
    }

    [Fact]
    public void Partition_NoneMatch_MatchingEmpty()
    {
        var source = new[] { 1, 3, 5 };
        var (matching, nonMatching) = source.Partition(x => x % 2 == 0);
        Assert.Empty(matching);
        Assert.Equal(new[] { 1, 3, 5 }, nonMatching);
    }

    [Fact]
    public void Partition_EmptySource_ReturnsTwoEmptyLists()
    {
        var (matching, nonMatching) = Array.Empty<int>().Partition(x => x > 0);
        Assert.Empty(matching);
        Assert.Empty(nonMatching);
    }

    [Fact]
    public void Partition_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Partition(x => x > 0));
    }

    [Fact]
    public void Partition_NullPredicate_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.Partition(null!));
    }
}

public class DistinctByTests
{
    [Fact]
    public void DistinctBy_KeepsFirstOfEachKey()
    {
        var source = new[] { ("a", 1), ("b", 2), ("a", 3), ("b", 4) };
        var result = source.DistinctBy(x => x.Item1).ToList();
        Assert.Equal(2, result.Count);
        Assert.Equal(("a", 1), result[0]);
        Assert.Equal(("b", 2), result[1]);
    }

    [Fact]
    public void DistinctBy_AllUnique_ReturnsAll()
    {
        var source = new[] { 1, 2, 3 };
        var result = source.DistinctBy(x => x).ToList();
        Assert.Equal(new[] { 1, 2, 3 }, result);
    }

    [Fact]
    public void DistinctBy_EmptySource_ReturnsEmpty()
    {
        Assert.Empty(Array.Empty<int>().DistinctBy(x => x));
    }

    [Fact]
    public void DistinctBy_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.DistinctBy(x => x));
    }

    [Fact]
    public void DistinctBy_NullKeySelector_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.DistinctBy<int, int>(null!));
    }
}

public class ToDictionarySafeTests
{
    [Fact]
    public void ToDictionarySafe_NoDuplicates_ReturnsAll()
    {
        var source = new[] { ("a", 1), ("b", 2), ("c", 3) };
        var result = source.ToDictionarySafe(x => x.Item1, x => x.Item2);
        Assert.Equal(3, result.Count);
        Assert.Equal(1, result["a"]);
    }

    [Fact]
    public void ToDictionarySafe_WithDuplicates_KeepsFirst()
    {
        var source = new[] { ("a", 1), ("b", 2), ("a", 99) };
        var result = source.ToDictionarySafe(x => x.Item1, x => x.Item2);
        Assert.Equal(2, result.Count);
        Assert.Equal(1, result["a"]);
    }

    [Fact]
    public void ToDictionarySafe_EmptySource_ReturnsEmptyDictionary()
    {
        var result = Array.Empty<(string, int)>().ToDictionarySafe(x => x.Item1, x => x.Item2);
        Assert.Empty(result);
    }

    [Fact]
    public void ToDictionarySafe_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<(string, int)> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.ToDictionarySafe(x => x.Item1, x => x.Item2));
    }

    [Fact]
    public void ToDictionarySafe_NullKeySelector_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new[] { ("a", 1) }.ToDictionarySafe<(string, int), string, int>(null!, x => x.Item2));
    }

    [Fact]
    public void ToDictionarySafe_NullValueSelector_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new[] { ("a", 1) }.ToDictionarySafe<(string, int), string, int>(x => x.Item1, null!));
    }
}

public class PageTests
{
    [Fact]
    public void Page_FirstPage_ReturnsFirstElements()
    {
        var source = Enumerable.Range(1, 20);
        var result = source.Page(0, 5).ToList();
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result);
    }

    [Fact]
    public void Page_SecondPage_ReturnsCorrectElements()
    {
        var source = Enumerable.Range(1, 20);
        var result = source.Page(1, 5).ToList();
        Assert.Equal(new[] { 6, 7, 8, 9, 10 }, result);
    }

    [Fact]
    public void Page_BeyondEnd_ReturnsEmpty()
    {
        var source = Enumerable.Range(1, 5);
        var result = source.Page(10, 5).ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void Page_LastPartialPage_ReturnsRemainingElements()
    {
        var source = Enumerable.Range(1, 7);
        var result = source.Page(1, 5).ToList();
        Assert.Equal(new[] { 6, 7 }, result);
    }

    [Fact]
    public void Page_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Page(0, 5));
    }

    [Fact]
    public void Page_NegativePageIndex_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.Page(-1, 5));
    }

    [Fact]
    public void Page_ZeroPageSize_ThrowsArgumentOutOfRangeException()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new[] { 1 }.Page(0, 0));
    }
}

public class IndexOfTests
{
    [Fact]
    public void IndexOf_Found_ReturnsCorrectIndex()
    {
        var source = new[] { 10, 20, 30, 40, 50 };
        Assert.Equal(2, source.IndexOf(x => x == 30));
    }

    [Fact]
    public void IndexOf_NotFound_ReturnsNegativeOne()
    {
        var source = new[] { 10, 20, 30 };
        Assert.Equal(-1, source.IndexOf(x => x == 99));
    }

    [Fact]
    public void IndexOf_FirstMatch_ReturnsFirstIndex()
    {
        var source = new[] { 1, 2, 3, 2, 1 };
        Assert.Equal(1, source.IndexOf(x => x == 2));
    }

    [Fact]
    public void IndexOf_EmptySource_ReturnsNegativeOne()
    {
        Assert.Equal(-1, Array.Empty<int>().IndexOf(x => x == 1));
    }

    [Fact]
    public void IndexOf_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<int> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.IndexOf(x => x == 1));
    }

    [Fact]
    public void IndexOf_NullPredicate_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { 1 }.IndexOf(null!));
    }
}

public class TraverseTests
{
    private class TreeNode
    {
        public string Name { get; set; } = "";
        public List<TreeNode> Children { get; set; } = new();
    }

    [Fact]
    public void Traverse_FlatList_ReturnsAllNodes()
    {
        var nodes = new[]
        {
            new TreeNode { Name = "A" },
            new TreeNode { Name = "B" },
            new TreeNode { Name = "C" }
        };
        var result = nodes.Traverse(n => n.Children).Select(n => n.Name).ToList();
        Assert.Equal(new[] { "A", "B", "C" }, result);
    }

    [Fact]
    public void Traverse_DepthFirst_ReturnsCorrectOrder()
    {
        var tree = new[]
        {
            new TreeNode
            {
                Name = "Root",
                Children = new List<TreeNode>
                {
                    new TreeNode
                    {
                        Name = "Child1",
                        Children = new List<TreeNode>
                        {
                            new TreeNode { Name = "Grandchild1" }
                        }
                    },
                    new TreeNode { Name = "Child2" }
                }
            }
        };
        var result = tree.Traverse(n => n.Children).Select(n => n.Name).ToList();
        Assert.Equal(new[] { "Root", "Child1", "Grandchild1", "Child2" }, result);
    }

    [Fact]
    public void Traverse_EmptySource_ReturnsEmpty()
    {
        var source = Array.Empty<TreeNode>();
        Assert.Empty(source.Traverse(n => n.Children));
    }

    [Fact]
    public void Traverse_NullSource_ThrowsArgumentNullException()
    {
        IEnumerable<TreeNode> source = null!;
        Assert.Throws<ArgumentNullException>(() => source.Traverse(n => n.Children));
    }

    [Fact]
    public void Traverse_NullChildrenSelector_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new[] { new TreeNode() }.Traverse(null!));
    }
}
