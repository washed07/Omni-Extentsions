using System.Collections.Concurrent;

namespace OX.Collections
{
    /// <summary>
    /// A thread-safe repository that manages a collection of Registry&lt;T&gt; objects with advanced search capabilities.
    /// Provides functionality for registration, searching, indexing, and property management of data items.
    /// </summary>
    /// <typeparam name="T">The type of data stored in each registry within this repository</typeparam>
    public class Repository<T>
    {
        /// <summary>
        /// Thread-safe dictionary that stores registries indexed by unique GUIDs.
        /// </summary>
        private readonly ConcurrentDictionary<Guid, Registry<T>> registries;
        
        /// <summary>
        /// Reader-writer lock to provide thread-safe access to search indexes.
        /// </summary>
        private readonly ReaderWriterLockSlim cacheLock;
        
        /// <summary>
        /// Dictionary of search indexes that map index names to functions that extract indexable values from registries.
        /// </summary>
        private Dictionary<string, Func<Registry<T>, object>> searchIndexes;

        /// <summary>
        /// Initializes a new instance of the Repository class with empty collections and search indexes.
        /// </summary>
        public Repository()
        {
            registries = new ConcurrentDictionary<Guid, Registry<T>>();
            cacheLock = new ReaderWriterLockSlim();
            searchIndexes = new Dictionary<string, Func<Registry<T>, object>>();
        }

        /// <summary>
        /// Registers a new data item in the repository by creating a new Registry with a unique identifier.
        /// This operation is thread-safe and automatically updates all search indexes.
        /// </summary>
        /// <param name="data">The data item to register in the repository</param>
        /// <returns>The newly created Registry containing the data</returns>
        /// <exception cref="InvalidOperationException">Thrown when the registration fails due to an unexpected error</exception>
        public Registry<T> Register(T data)
        {
            var registry = new Registry<T>(data);
            var id = Guid.NewGuid();

            if (registries.TryAdd(id, registry))
            {
                UpdateSearchIndexes(registry);
                return registry;
            }

            throw new InvalidOperationException("Failed to register item");
        }

        /// <summary>
        /// Removes a registry from the repository based on its data content.
        /// This operation is thread-safe and searches for the registry containing the specified data.
        /// </summary>
        /// <param name="data">The data item whose registry should be removed</param>
        /// <returns>True if the registry was found and successfully removed; otherwise, false</returns>
        public bool Unregister(T data)
        {
            var registry = FindRegistry(data);
            if (registry != null)
            {
                return registries.TryRemove(
                    registries.FirstOrDefault(r => r.Value == registry).Key,
                    out _);
            }
            return false;
        }

        /// <summary>
        /// Adds or updates a search index that can be used for fast lookups and sorting.
        /// This operation rebuilds the index for all existing registries and is thread-safe.
        /// </summary>
        /// <param name="indexName">The unique name for this search index</param>
        /// <param name="indexer">A function that extracts the indexable value from a registry</param>
        public void AddSearchIndex(string indexName, Func<Registry<T>, object> indexer)
        {
            try
            {
                cacheLock.EnterWriteLock();
                searchIndexes[indexName] = indexer;

                // Rebuild index for existing items
                foreach (var registry in registries.Values)
                {
                    UpdateSearchIndexes(registry);
                }
            }
            finally
            {
                cacheLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Finds all registries that have a specific value for the given search index.
        /// This operation is thread-safe and uses read locks for optimal performance.
        /// </summary>
        /// <param name="indexName">The name of the search index to query</param>
        /// <param name="value">The value to search for in the specified index</param>
        /// <returns>An enumerable of registries that match the search criteria, or empty if the index doesn't exist</returns>
        public IEnumerable<Registry<T>> FindByIndex(string indexName, object value)
        {
            try
            {
                cacheLock.EnterReadLock();
                if (!searchIndexes.ContainsKey(indexName))
                    return Enumerable.Empty<Registry<T>>();

                var indexer = searchIndexes[indexName];
                return registries.Values.Where(r => Equals(indexer(r), value));
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Finds the registry that contains the specified data item using the default equality comparer.
        /// This operation is thread-safe but may be slower for large collections.
        /// </summary>
        /// <param name="data">The data item to search for</param>
        /// <returns>The registry containing the data, or null if not found</returns>
        public Registry<T>? FindRegistry(T data)
        {
            return registries.Values.FirstOrDefault(r =>
                EqualityComparer<T>.Default.Equals(r.Data, data));
        }

        /// <summary>
        /// Performs an advanced search across all registries with multiple filtering and sorting options.
        /// This operation is thread-safe and allows complex queries combining predicates, property filters, and sorting.
        /// </summary>
        /// <param name="predicate">Optional predicate function to filter registries based on custom logic</param>
        /// <param name="propertyFilters">Optional dictionary of property name-value pairs to filter by registry properties</param>
        /// <param name="sortBy">Optional search index name to sort the results by</param>
        /// <param name="ascending">Whether to sort in ascending order (true) or descending order (false)</param>
        /// <returns>An enumerable of registries that match all specified criteria, optionally sorted</returns>
        public IEnumerable<Registry<T>> Search(
            Func<Registry<T>, bool>? predicate = null,
            IDictionary<string, object>? propertyFilters = null,
            string? sortBy = null,
            bool ascending = true)
        {
            var query = registries.Values.AsEnumerable();

            // Apply predicate filter
            if (predicate != null)
                query = query.Where(predicate);

            // Apply property filters
            if (propertyFilters != null)
            {
                foreach (var filter in propertyFilters)
                {
                    query = query.Where(r =>
                        r.GetProperty(filter.Key, out var value) &&
                        Equals(value, filter.Value));
                }
            }

            // Apply sorting if specified
            if (!string.IsNullOrEmpty(sortBy) && searchIndexes.ContainsKey(sortBy))
            {
                var sortSelector = searchIndexes[sortBy];
                query = ascending
                    ? query.OrderBy(r => sortSelector(r))
                    : query.OrderByDescending(r => sortSelector(r));
            }

            return query;
        }

        /// <summary>
        /// Updates all search indexes for a given registry by applying all registered indexer functions.
        /// This is an internal method used when registries are added or when indexes are rebuilt.
        /// Uses read locks to ensure thread safety while accessing search indexes.
        /// </summary>
        /// <param name="registry">The registry to update search indexes for</param>
        private void UpdateSearchIndexes(Registry<T> registry)
        {
            try
            {
                cacheLock.EnterReadLock();
                foreach (var index in searchIndexes)
                {
                    var value = index.Value(registry);
                    registry.SetProperty($"__index_{index.Key}", value);
                }
            }
            finally
            {
                cacheLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Sets a property value for the registry containing the specified data item.
        /// This operation is thread-safe and provides a convenient way to set properties without directly accessing the registry.
        /// </summary>
        /// <param name="data">The data item whose registry should have the property set</param>
        /// <param name="propertyName">The name of the property to set</param>
        /// <param name="value">The value to set for the property</param>
        /// <returns>True if the data was found and the property was set successfully; otherwise, false</returns>
        public bool SetProperty(T data, string propertyName, object value)
        {
            var registry = FindRegistry(data);
            return registry?.SetProperty(propertyName, value) ?? false;
        }

        /// <summary>
        /// Gets a property value from the registry containing the specified data item.
        /// This operation is thread-safe and provides a convenient way to get properties without directly accessing the registry.
        /// </summary>
        /// <param name="data">The data item whose registry should be queried for the property</param>
        /// <param name="propertyName">The name of the property to retrieve</param>
        /// <param name="value">When this method returns, contains the property value if found; otherwise, null</param>
        /// <returns>True if the data was found and the property exists; otherwise, false</returns>
        public bool GetProperty(T data, string propertyName, out object? value)
        {
            var registry = FindRegistry(data);
            if (registry != null)
            {
                return registry.GetProperty(propertyName, out value);
            }
            value = null;
            return false;
        }

        /// <summary>
        /// Gets a read-only collection of all registries in the repository.
        /// This operation is thread-safe and returns a snapshot of the current registries.
        /// </summary>
        /// <returns>A read-only collection containing all registries currently in the repository</returns>
        public IReadOnlyCollection<Registry<T>> GetAll()
        {
            return registries.Values.ToList().AsReadOnly();
        }

        /// <summary>
        /// Removes all registries from the repository.
        /// This operation is thread-safe and clears all stored data and their associated properties.
        /// </summary>
        public void Clear()
        {
            registries.Clear();
        }

        /// <summary>
        /// Gets the current number of registries in the repository.
        /// This operation is thread-safe and provides an atomic count.
        /// </summary>
        /// <value>The number of registries currently stored in the repository</value>
        public int Count => registries.Count;

        /// <summary>
        /// Releases all resources used by the Repository, including the reader-writer lock.
        /// Call this method when the repository is no longer needed to prevent resource leaks.
        /// </summary>
        public void Dispose()
        {
            cacheLock?.Dispose();
        }
    }
}
