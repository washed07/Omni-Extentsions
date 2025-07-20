using System.Collections.Concurrent;

namespace OX.Collections
{
    /// <summary>
    /// A thread-safe registry that holds data of type T and allows storing additional properties as key-value pairs.
    /// This class provides a flexible way to associate metadata with any object while maintaining thread safety.
    /// </summary>
    /// <typeparam name="T">The type of data stored in this registry</typeparam>
    /// <param name="data">The initial data to store in this registry</param>
    public class Registry<T>(T data)
    {
        /// <summary>
        /// Gets or sets the primary data stored in this registry.
        /// </summary>
        /// <value>The data of type T associated with this registry</value>
        public T Data { get; set; } = data;
        
        /// <summary>
        /// Thread-safe dictionary to store additional properties as key-value pairs.
        /// </summary>
        private readonly ConcurrentDictionary<string, object> properties = new();

        /// <summary>
        /// Gets a read-only view of all properties stored in this registry.
        /// </summary>
        /// <value>A read-only dictionary containing all property key-value pairs</value>
        public IReadOnlyDictionary<string, object> Properties => properties;

        /// <summary>
        /// Sets a property value for the specified key. If the key already exists, updates the value.
        /// This operation is thread-safe.
        /// </summary>
        /// <param name="key">The property key to set or update</param>
        /// <param name="value">The value to associate with the key</param>
        /// <returns>
        /// True if the property was successfully set or updated; false if the operation failed.
        /// Note: This should typically always return true unless there are exceptional circumstances.
        /// </returns>
        public bool SetProperty(string key, object value) =>
            properties.TryAdd(key, value) || properties.TryUpdate(key, value, properties[key]);

        /// <summary>
        /// Attempts to retrieve a property value for the specified key.
        /// This operation is thread-safe.
        /// </summary>
        /// <param name="key">The property key to retrieve</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key, 
        /// if the key is found; otherwise, null. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// True if the registry contains a property with the specified key; otherwise, false.
        /// </returns>
        public bool GetProperty(string key, out object? value) =>
            properties.TryGetValue(key, out value);
    }
}
