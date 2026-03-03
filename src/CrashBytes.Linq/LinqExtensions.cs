namespace CrashBytes.Linq;

/// <summary>
/// Provides a comprehensive set of LINQ extension methods for <see cref="IEnumerable{T}"/>.
/// </summary>
public static partial class LinqExtensions
{
    // ──────────────────────────────────────────────
    //  WhereNotNull
    // ──────────────────────────────────────────────

    /// <summary>
    /// Filters out null elements from a sequence of reference types.
    /// </summary>
    /// <typeparam name="T">The element type (must be a reference type).</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A sequence containing only non-null elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : class
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        return WhereNotNullIterator(source);
    }

    private static IEnumerable<T> WhereNotNullIterator<T>(IEnumerable<T?> source) where T : class
    {
        foreach (var item in source)
        {
            if (item is not null)
                yield return item;
        }
    }

    /// <summary>
    /// Filters out null elements from a sequence of nullable value types.
    /// </summary>
    /// <typeparam name="T">The underlying value type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <returns>A sequence containing only non-null values.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> source) where T : struct
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        return WhereNotNullIterator(source);
    }

    private static IEnumerable<T> WhereNotNullIterator<T>(IEnumerable<T?> source) where T : struct
    {
        foreach (var item in source)
        {
            if (item.HasValue)
                yield return item.Value;
        }
    }

    // ──────────────────────────────────────────────
    //  WhereIf
    // ──────────────────────────────────────────────

    /// <summary>
    /// Conditionally applies a Where filter only when <paramref name="condition"/> is <c>true</c>.
    /// When <paramref name="condition"/> is <c>false</c>, the sequence is returned unchanged.
    /// Useful for building conditional query pipelines.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="condition">Whether to apply the filter.</param>
    /// <param name="predicate">The filter predicate to apply when <paramref name="condition"/> is <c>true</c>.</param>
    /// <returns>The filtered or unchanged sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <c>null</c>.</exception>
    public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));

        if (!condition)
            return source;

        return WhereIfIterator(source, predicate);
    }

    private static IEnumerable<T> WhereIfIterator<T>(IEnumerable<T> source, Func<T, bool> predicate)
    {
        foreach (var item in source)
        {
            if (predicate(item))
                yield return item;
        }
    }

    // ──────────────────────────────────────────────
    //  IsNullOrEmpty
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns <c>true</c> if the sequence is <c>null</c> or contains no elements.
    /// Does not enumerate beyond the first element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence (may be <c>null</c>).</param>
    /// <returns><c>true</c> if <paramref name="source"/> is <c>null</c> or empty; otherwise <c>false</c>.</returns>
    public static bool IsNullOrEmpty<T>(this IEnumerable<T>? source)
    {
        if (source is null)
            return true;

        // Fast path for ICollection
        if (source is ICollection<T> collection)
            return collection.Count == 0;

        using var enumerator = source.GetEnumerator();
        return !enumerator.MoveNext();
    }

    // ──────────────────────────────────────────────
    //  ForEach
    // ──────────────────────────────────────────────

    /// <summary>
    /// Executes the specified <paramref name="action"/> on each element of the sequence.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="action">The action to execute on each element.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <c>null</c>.</exception>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (action is null) throw new ArgumentNullException(nameof(action));

        foreach (var item in source)
            action(item);
    }

    /// <summary>
    /// Executes the specified <paramref name="action"/> on each element of the sequence,
    /// providing the zero-based index of the element.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="action">The action to execute, receiving the element and its index.</param>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="action"/> is <c>null</c>.</exception>
    public static void ForEach<T>(this IEnumerable<T> source, Action<T, int> action)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (action is null) throw new ArgumentNullException(nameof(action));

        int index = 0;
        foreach (var item in source)
            action(item, index++);
    }

    // ──────────────────────────────────────────────
    //  Shuffle
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns elements in random order using the Fisher-Yates shuffle algorithm.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="random">Optional <see cref="Random"/> instance for deterministic results.</param>
    /// <returns>A new sequence with elements in random order.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random? random = null)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var rng = random ?? new Random();
        var buffer = source.ToList();

        for (int i = buffer.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }

        return buffer;
    }

    // ──────────────────────────────────────────────
    //  PickRandom
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns a single random element from the sequence.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="random">Optional <see cref="Random"/> instance for deterministic results.</param>
    /// <returns>A randomly selected element.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    /// <exception cref="InvalidOperationException">The sequence is empty.</exception>
    public static T PickRandom<T>(this IEnumerable<T> source, Random? random = null)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        var rng = random ?? new Random();
        var list = source.ToList();

        if (list.Count == 0)
            throw new InvalidOperationException("Sequence contains no elements.");

        return list[rng.Next(list.Count)];
    }

    /// <summary>
    /// Returns <paramref name="count"/> random elements from the sequence without repetition.
    /// If <paramref name="count"/> exceeds the number of elements, all elements are returned in random order.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="count">The number of random elements to pick.</param>
    /// <param name="random">Optional <see cref="Random"/> instance for deterministic results.</param>
    /// <returns>A sequence of randomly selected elements without repetition.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="count"/> is negative.</exception>
    public static IEnumerable<T> PickRandom<T>(this IEnumerable<T> source, int count, Random? random = null)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "Count must not be negative.");

        var rng = random ?? new Random();
        var buffer = source.ToList();

        // Fisher-Yates partial shuffle
        int n = Math.Min(count, buffer.Count);
        for (int i = 0; i < n; i++)
        {
            int j = rng.Next(i, buffer.Count);
            (buffer[i], buffer[j]) = (buffer[j], buffer[i]);
        }

        return buffer.Take(n);
    }

    // ──────────────────────────────────────────────
    //  Batch
    // ──────────────────────────────────────────────

    /// <summary>
    /// Splits the source sequence into batches of the specified <paramref name="size"/>.
    /// The last batch may contain fewer elements.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="size">The maximum number of elements per batch.</param>
    /// <returns>A sequence of batches, each containing up to <paramref name="size"/> elements.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="size"/> is less than 1.</exception>
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (size < 1) throw new ArgumentOutOfRangeException(nameof(size), "Batch size must be at least 1.");

        return BatchIterator(source, size);
    }

    private static IEnumerable<IEnumerable<T>> BatchIterator<T>(IEnumerable<T> source, int size)
    {
        var batch = new List<T>(size);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count == size)
            {
                yield return batch;
                batch = new List<T>(size);
            }
        }

        if (batch.Count > 0)
            yield return batch;
    }

    // ──────────────────────────────────────────────
    //  Pairwise
    // ──────────────────────────────────────────────

    /// <summary>
    /// Applies a selector function to each pair of consecutive elements in the sequence.
    /// For example, [1,2,3,4] with (a,b) => a+b produces [3,5,7].
    /// </summary>
    /// <typeparam name="T">The source element type.</typeparam>
    /// <typeparam name="TResult">The result element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="selector">The function to apply to each consecutive pair.</param>
    /// <returns>A sequence of results from applying the selector to consecutive pairs.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="selector"/> is <c>null</c>.</exception>
    public static IEnumerable<TResult> Pairwise<T, TResult>(this IEnumerable<T> source, Func<T, T, TResult> selector)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (selector is null) throw new ArgumentNullException(nameof(selector));

        return PairwiseIterator(source, selector);
    }

    private static IEnumerable<TResult> PairwiseIterator<T, TResult>(IEnumerable<T> source, Func<T, T, TResult> selector)
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;

        var previous = enumerator.Current;
        while (enumerator.MoveNext())
        {
            yield return selector(previous, enumerator.Current);
            previous = enumerator.Current;
        }
    }

    // ──────────────────────────────────────────────
    //  Scan
    // ──────────────────────────────────────────────

    /// <summary>
    /// Applies an accumulator function over a sequence and yields each intermediate result.
    /// For example, [1,2,3] with (a,b) => a+b produces [1,3,6].
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="accumulator">The accumulator function.</param>
    /// <returns>A sequence of intermediate accumulated values.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="accumulator"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Scan<T>(this IEnumerable<T> source, Func<T, T, T> accumulator)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (accumulator is null) throw new ArgumentNullException(nameof(accumulator));

        return ScanIterator(source, accumulator);
    }

    private static IEnumerable<T> ScanIterator<T>(IEnumerable<T> source, Func<T, T, T> accumulator)
    {
        using var enumerator = source.GetEnumerator();
        if (!enumerator.MoveNext())
            yield break;

        var state = enumerator.Current;
        yield return state;

        while (enumerator.MoveNext())
        {
            state = accumulator(state, enumerator.Current);
            yield return state;
        }
    }

    /// <summary>
    /// Applies an accumulator function with a seed value over a sequence and yields each intermediate result.
    /// </summary>
    /// <typeparam name="T">The source element type.</typeparam>
    /// <typeparam name="TAccumulate">The type of the accumulated value.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="seed">The initial accumulator value.</param>
    /// <param name="accumulator">The accumulator function.</param>
    /// <returns>A sequence of intermediate accumulated values, starting with <paramref name="seed"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="accumulator"/> is <c>null</c>.</exception>
    public static IEnumerable<TAccumulate> Scan<T, TAccumulate>(
        this IEnumerable<T> source,
        TAccumulate seed,
        Func<TAccumulate, T, TAccumulate> accumulator)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (accumulator is null) throw new ArgumentNullException(nameof(accumulator));

        return ScanIterator(source, seed, accumulator);
    }

    private static IEnumerable<TAccumulate> ScanIterator<T, TAccumulate>(
        IEnumerable<T> source,
        TAccumulate seed,
        Func<TAccumulate, T, TAccumulate> accumulator)
    {
        var state = seed;
        yield return state;

        foreach (var item in source)
        {
            state = accumulator(state, item);
            yield return state;
        }
    }

    // ──────────────────────────────────────────────
    //  Interleave
    // ──────────────────────────────────────────────

    /// <summary>
    /// Alternates elements from the source sequence and one or more other sequences.
    /// For example, [1,2], [a,b], [x,y] produces [1,a,x,2,b,y].
    /// When sequences have unequal lengths, exhausted sequences are skipped.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="others">Additional sequences to interleave.</param>
    /// <returns>A sequence with elements alternated from all input sequences.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="others"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Interleave<T>(this IEnumerable<T> source, params IEnumerable<T>[] others)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (others is null) throw new ArgumentNullException(nameof(others));

        return InterleaveIterator(source, others);
    }

    private static IEnumerable<T> InterleaveIterator<T>(IEnumerable<T> source, IEnumerable<T>[] others)
    {
        var enumerators = new List<IEnumerator<T>>();
        try
        {
            enumerators.Add(source.GetEnumerator());
            foreach (var other in others)
            {
                if (other is null)
                    throw new ArgumentNullException(nameof(others), "One of the sequences in 'others' is null.");
                enumerators.Add(other.GetEnumerator());
            }

            bool anyHasNext = true;
            while (anyHasNext)
            {
                anyHasNext = false;
                foreach (var enumerator in enumerators)
                {
                    if (enumerator.MoveNext())
                    {
                        yield return enumerator.Current;
                        anyHasNext = true;
                    }
                }
            }
        }
        finally
        {
            foreach (var enumerator in enumerators)
                enumerator.Dispose();
        }
    }

    // ──────────────────────────────────────────────
    //  Flatten
    // ──────────────────────────────────────────────

    /// <summary>
    /// Flattens one level of nesting from a sequence of sequences.
    /// Equivalent to <c>SelectMany(x => x)</c> but more readable.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence of sequences.</param>
    /// <returns>A flattened sequence.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));

        return FlattenIterator(source);
    }

    private static IEnumerable<T> FlattenIterator<T>(IEnumerable<IEnumerable<T>> source)
    {
        foreach (var inner in source)
        {
            foreach (var item in inner)
                yield return item;
        }
    }

    // ──────────────────────────────────────────────
    //  Partition
    // ──────────────────────────────────────────────

    /// <summary>
    /// Splits a sequence into two lists based on a predicate.
    /// Returns a tuple of (matching, nonMatching).
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">The predicate to test each element.</param>
    /// <returns>A tuple where the first list contains elements matching the predicate, and the second contains the rest.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <c>null</c>.</exception>
    public static (List<T> Matching, List<T> NonMatching) Partition<T>(
        this IEnumerable<T> source,
        Func<T, bool> predicate)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));

        var matching = new List<T>();
        var nonMatching = new List<T>();

        foreach (var item in source)
        {
            if (predicate(item))
                matching.Add(item);
            else
                nonMatching.Add(item);
        }

        return (matching, nonMatching);
    }

    // ──────────────────────────────────────────────
    //  DistinctBy
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns distinct elements from a sequence based on a key selector function.
    /// The first element with a given key value is kept.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <typeparam name="TKey">The key type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The function to extract the key from each element.</param>
    /// <returns>A sequence of elements with distinct keys.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="keySelector"/> is <c>null</c>.</exception>
    public static IEnumerable<T> DistinctBy<T, TKey>(this IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (keySelector is null) throw new ArgumentNullException(nameof(keySelector));

        return DistinctByIterator(source, keySelector);
    }

    private static IEnumerable<T> DistinctByIterator<T, TKey>(IEnumerable<T> source, Func<T, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();
        foreach (var item in source)
        {
            if (seen.Add(keySelector(item)))
                yield return item;
        }
    }

    // ──────────────────────────────────────────────
    //  ToDictionarySafe
    // ──────────────────────────────────────────────

    /// <summary>
    /// Creates a dictionary from a sequence, silently keeping the first element for duplicate keys
    /// instead of throwing an exception.
    /// </summary>
    /// <typeparam name="T">The source element type.</typeparam>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="keySelector">The function to extract the key from each element.</param>
    /// <param name="valueSelector">The function to extract the value from each element.</param>
    /// <returns>A dictionary with unique keys, keeping the first occurrence for duplicates.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/>, <paramref name="keySelector"/>, or <paramref name="valueSelector"/> is <c>null</c>.</exception>
    public static Dictionary<TKey, TValue> ToDictionarySafe<T, TKey, TValue>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector,
        Func<T, TValue> valueSelector) where TKey : notnull
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (keySelector is null) throw new ArgumentNullException(nameof(keySelector));
        if (valueSelector is null) throw new ArgumentNullException(nameof(valueSelector));

        var dictionary = new Dictionary<TKey, TValue>();
        foreach (var item in source)
        {
            var key = keySelector(item);
            if (!dictionary.ContainsKey(key))
                dictionary[key] = valueSelector(item);
        }

        return dictionary;
    }

    // ──────────────────────────────────────────────
    //  Page
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns a specific page of results from the sequence (0-based page index).
    /// Equivalent to <c>Skip(pageIndex * pageSize).Take(pageSize)</c>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="pageIndex">The zero-based page index.</param>
    /// <param name="pageSize">The number of elements per page.</param>
    /// <returns>The elements on the specified page.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="pageIndex"/> is negative or <paramref name="pageSize"/> is less than 1.</exception>
    public static IEnumerable<T> Page<T>(this IEnumerable<T> source, int pageIndex, int pageSize)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex), "Page index must not be negative.");
        if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");

        return PageIterator(source, pageIndex, pageSize);
    }

    private static IEnumerable<T> PageIterator<T>(IEnumerable<T> source, int pageIndex, int pageSize)
    {
        return source.Skip(pageIndex * pageSize).Take(pageSize);
    }

    // ──────────────────────────────────────────────
    //  IndexOf
    // ──────────────────────────────────────────────

    /// <summary>
    /// Returns the zero-based index of the first element matching the predicate, or -1 if not found.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The source sequence.</param>
    /// <param name="predicate">The predicate to match.</param>
    /// <returns>The index of the first matching element, or -1 if no match is found.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="predicate"/> is <c>null</c>.</exception>
    public static int IndexOf<T>(this IEnumerable<T> source, Func<T, bool> predicate)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (predicate is null) throw new ArgumentNullException(nameof(predicate));

        int index = 0;
        foreach (var item in source)
        {
            if (predicate(item))
                return index;
            index++;
        }

        return -1;
    }

    // ──────────────────────────────────────────────
    //  Traverse
    // ──────────────────────────────────────────────

    /// <summary>
    /// Recursively flattens a tree structure using depth-first traversal.
    /// Starting from each element in the source, recurses into children obtained via
    /// <paramref name="childrenSelector"/>.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    /// <param name="source">The root elements to start traversal from.</param>
    /// <param name="childrenSelector">A function that returns the children of a given element.</param>
    /// <returns>A depth-first flattened sequence of all elements in the tree.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="source"/> or <paramref name="childrenSelector"/> is <c>null</c>.</exception>
    public static IEnumerable<T> Traverse<T>(this IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        if (childrenSelector is null) throw new ArgumentNullException(nameof(childrenSelector));

        return TraverseIterator(source, childrenSelector);
    }

    private static IEnumerable<T> TraverseIterator<T>(IEnumerable<T> source, Func<T, IEnumerable<T>> childrenSelector)
    {
        var stack = new Stack<T>();

        // Push in reverse order so the first element is at the top
        var list = source.ToList();
        for (int i = list.Count - 1; i >= 0; i--)
            stack.Push(list[i]);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            yield return current;

            var children = childrenSelector(current)?.ToList();
            if (children is not null)
            {
                for (int i = children.Count - 1; i >= 0; i--)
                    stack.Push(children[i]);
            }
        }
    }
}
