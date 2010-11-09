// <copyright file="JsonObject.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Xml;

    /// <summary>
    /// A JsonObject is an unordered collection of zero or more key/value pairs.
    /// </summary>
    /// <remarks>A JsonObject is an unordered collection of zero or more key/value pairs,
    /// where each key is a String and each value is a <see cref="System.Json.JsonValue"/>, which can be a
    /// <see cref="System.Json.JsonPrimitive"/>, a <see cref="System.Json.JsonArray"/>, or a JsonObject.</remarks>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710",
        Justification = "Object in the context of JSON already conveys the meaning of dictionary")]
    public sealed class JsonObject : JsonValue, IDictionary<string, JsonValue>
    {
        private Dictionary<string, JsonValue> values = new Dictionary<string, JsonValue>();
        private List<string> indexedKeys;
        private int instancesBeingSaved = 0;
        private object saveLock = new object();
        private int changingListeners = 0;
        private int changedListeners = 0;

        /// <summary>
        /// Creates an instance of the <see cref="System.Json.JsonObject"/> class initialized with an
        /// <see cref="System.Collections.Generic.IEnumerable{T}"/> collection of key/value pairs.
        /// </summary>
        /// <param name="items">The <see cref="System.Collections.Generic.IEnumerable{T}"/> collection of
        /// <see cref="System.Collections.Generic.KeyValuePair{K, V}"/> used to initialize the
        /// key/value pairs</param>
        /// <exception cref="System.ArgumentNullException">If items is null.</exception>
        /// <exception cref="System.ArgumentException">If any of the values in the collection
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public JsonObject(IEnumerable<KeyValuePair<string, JsonValue>> items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Creates an instance of the <see cref="System.Json.JsonObject"/> class initialized with a collection of key/value pairs.
        /// </summary>
        /// <param name="items">The <see cref="System.Collections.Generic.KeyValuePair{K, V}"/> objects used to initialize the key/value pairs.</param>
        /// <exception cref="System.ArgumentException">If any of the values in the collection
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public JsonObject(params KeyValuePair<string, JsonValue>[] items)
        {
            if (items != null)
            {
                this.AddRange(items);
            }
        }

        /// <summary>
        /// Raised when this <see cref="System.Json.JsonValue"/> or any of its members are about to be changed.
        /// </summary>
        /// <remarks><p>Events are raised when elements are added or removed to <see cref="System.Json.JsonValue"/>
        /// instances. It applies to both complex descendants of <see cref="System.Json.JsonValue"/>: <see cref="System.Json.JsonArray"/>
        /// and <see cref="System.Json.JsonObject"/>.</p>
        /// <p>You should be careful when modifying a <see cref="System.Json.JsonValue"/> tree within one of these events,
        /// because doing this might lead to unexpected results. For example, if you receive a Changing event, and while
        /// the event is being processed you remove the node from the tree, you might not receive the Changed event. When
        /// an event is being processed, it is valid to modify a tree other than the one that contains the node that is
        /// receiving the event; it is even valid to modify the same tree provided the modifications do not affect the
        /// specific nodes on which the event was raised. However, if you modify the area of the tree that contains the
        /// node receiving the event, the events that you receive and the impact to the tree are undefined.</p></remarks>
        public override event EventHandler<JsonValueChangeEventArgs> Changing
        {
            add
            {
                bool needToAddChildren = this.changingListeners == 0;
                this.changingListeners++;
                base.Changing += value;
                if (needToAddChildren)
                {
                    foreach (JsonValue child in this.values.Values)
                    {
                        if (child != null)
                        {
                            child.Changing += new EventHandler<JsonValueChangeEventArgs>(this.ChildChanging);
                        }
                    }
                }
            }

            remove
            {
                this.changingListeners--;
                bool needToRemoveChildren = this.changingListeners == 0;
                base.Changing -= value;
                if (needToRemoveChildren)
                {
                    foreach (JsonValue child in this.values.Values)
                    {
                        if (child != null)
                        {
                            child.Changing -= new EventHandler<JsonValueChangeEventArgs>(this.ChildChanging);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Raised when this <see cref="System.Json.JsonValue"/> or any of its members have changed.
        /// </summary>
        /// <remarks><p>Events are raised when elements are added or removed to <see cref="System.Json.JsonValue"/>
        /// instances. It applies to both complex descendants of <see cref="System.Json.JsonValue"/>: <see cref="System.Json.JsonArray"/>
        /// and <see cref="System.Json.JsonObject"/>.</p>
        /// <p>You should be careful when modifying a <see cref="System.Json.JsonValue"/> tree within one of these events,
        /// because doing this might lead to unexpected results. For example, if you receive a Changing event, and while
        /// the event is being processed you remove the node from the tree, you might not receive the Changed event. When
        /// an event is being processed, it is valid to modify a tree other than the one that contains the node that is
        /// receiving the event; it is even valid to modify the same tree provided the modifications do not affect the
        /// specific nodes on which the event was raised. However, if you modify the area of the tree that contains the
        /// node receiving the event, the events that you receive and the impact to the tree are undefined.</p></remarks>
        public override event EventHandler<JsonValueChangeEventArgs> Changed
        {
            add
            {
                bool needToAddChildren = this.changedListeners == 0;
                this.changedListeners++;
                base.Changed += value;
                if (needToAddChildren)
                {
                    foreach (JsonValue child in this.values.Values)
                    {
                        if (child != null)
                        {
                            child.Changed += new EventHandler<JsonValueChangeEventArgs>(this.ChildChanged);
                        }
                    }
                }
            }

            remove
            {
                this.changedListeners--;
                bool needToRemoveChildren = this.changedListeners == 0;
                base.Changed -= value;
                if (needToRemoveChildren)
                {
                    foreach (JsonValue child in this.values.Values)
                    {
                        if (child != null)
                        {
                            child.Changed -= new EventHandler<JsonValueChangeEventArgs>(this.ChildChanged);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the JSON type of this <see cref="System.Json.JsonObject"/>. The return value
        /// is always <see cref="System.Json.JsonType">Object</see>.
        /// </summary>
        public override JsonType JsonType
        {
            get
            {
                return JsonType.Object;
            }
        }

        /// <summary>
        /// Gets a collection that contains the keys in this <see cref="System.Json.JsonObject"/>.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.values.Keys;
            }
        }

        /// <summary>
        /// Gets a collection that contains the values in this <see cref="System.Json.JsonObject"/>.
        /// </summary>
        public ICollection<JsonValue> Values
        {
            get
            {
                return this.values.Values;
            }
        }

        /// <summary>
        /// Returns the number of key/value pairs in this <see cref="System.Json.JsonObject"/>.
        /// </summary>
        public override int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this JSON CLR object is read-only.
        /// </summary>
        bool ICollection<KeyValuePair<string, JsonValue>>.IsReadOnly
        {
            get
            {
                return ((ICollection<KeyValuePair<string, JsonValue>>)this.values).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> associated to the specified key.</returns>
        /// <exception cref="System.ArgumentNullException">If key is null.</exception>
        /// <exception cref="System.ArgumentException">The property is set and the value is a
        /// <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/>
        /// property of value <see cref="System.Json.JsonType">Default</see>.</exception>
        public override JsonValue this[string key]
        {
            get
            {
                if (key == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
                }

                return this.values[key];
            }

            set
            {
                if (value != null && value.JsonType == JsonType.Default)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SG.GetString(SR.UseOfDefaultNotAllowed));
                }

                if (key == null)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("key");
                }

                bool replacement = this.values.ContainsKey(key);
                JsonValue oldValue = null;
                if (replacement)
                {
                    oldValue = this.values[key];
                    this.RaiseItemChanging(value, JsonValueChange.Replace, key);
                }
                else
                {
                    this.RaiseItemChanging(value, JsonValueChange.Add, key);
                }

                this.values[key] = value;
                if (replacement)
                {
                    this.RaiseItemChanged(oldValue, JsonValueChange.Replace, key);
                    this.RemoveChildHandlers(oldValue);
                }
                else
                {
                    this.RaiseItemChanged(value, JsonValueChange.Add, key);
                }

                this.AddChildHandlers(value);
            }
        }

        /// <summary>
        /// Safe string indexer for the <see cref="System.Json.JsonValue"/> type. 
        /// </summary>
        /// <param name="key">The key of the element to get.</param>
        /// <returns>If this instance contains the given key and the value corresponding to
        /// the key is not null, then it will return that value. Otherwise it will return a
        /// <see cref="System.Json.JsonValue"/> instance with <see cref="System.Json.JsonValue.JsonType"/>
        /// equals to <see cref="System.Json.JsonType">Default</see>.</returns>
        public override JsonValue ValueOrDefault(string key)
        {
            if (key != null && this.ContainsKey(key) && this[key] != null)
            {
                return this[key];
            }

            return base.ValueOrDefault(key);
        }

        /// <summary>
        /// Adds a specified collection of key/value pairs to this instance.
        /// </summary>
        /// <param name="items">The collection of key/value pairs to add.</param>
        /// <exception cref="System.ArgumentNullException">If items is null.</exception>
        /// <exception cref="System.ArgumentException">If the value of any of the items in the collection
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public void AddRange(IEnumerable<KeyValuePair<string, JsonValue>> items)
        {
            if (items == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("items");
            }

            if (this.changingListeners > 0)
            {
                foreach (KeyValuePair<string, JsonValue> item in items)
                {
                    this.RaiseItemChanging(item.Value, JsonValueChange.Add, item.Key);
                }
            }

            foreach (KeyValuePair<string, JsonValue> item in items)
            {
                if (item.Value != null && item.Value.JsonType == JsonType.Default)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SG.GetString(SR.UseOfDefaultNotAllowed));
                }
                
                this.values.Add(item.Key, item.Value);
                this.AddChildHandlers(item.Value);
                this.RaiseItemChanged(item.Value, JsonValueChange.Add, item.Key);
            }
        }

        /// <summary>
        /// Adds the elements from an array of type <see cref="System.Json.JsonValue"/> to this instance.
        /// </summary>
        /// <param name="items">The array of key/value paris to be added to this instance.</param>
        /// <exception cref="System.ArgumentException">If the value of any of the items in the array
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public void AddRange(params KeyValuePair<string, JsonValue>[] items)
        {
            this.AddRange(items as IEnumerable<KeyValuePair<string, JsonValue>>);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)this.values).GetEnumerator();
        }

        /// <summary>
        /// Adds a key/value pair to this <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        /// <param name="key">The key for the element added.</param>
        /// <param name="value">The <see cref="System.Json.JsonValue"/> for the element added.</param>
        /// <exception cref="System.ArgumentException">If the value is a <see cref="System.Json.JsonValue"/>
        /// with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public void Add(string key, JsonValue value)
        {
            if (value != null && value.JsonType == JsonType.Default)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SG.GetString(SR.UseOfDefaultNotAllowed));
            }
            
            this.RaiseItemChanging(value, JsonValueChange.Add, key);
            this.values.Add(key, value);
            this.RaiseItemChanged(value, JsonValueChange.Add, key);
            this.AddChildHandlers(value);
        }

        /// <summary>
        /// Adds a key/value pair to this <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        /// <param name="item">The key/value pair to be added.</param>
        /// <exception cref="System.ArgumentException">If the value of the pair is a
        /// <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/>
        /// property of value <see cref="System.Json.JsonType">Default</see>.</exception>
        public void Add(KeyValuePair<string, JsonValue> item)
        {
            this.Add(item.Key, item.Value);
        }

        /// <summary>
        /// Checks whether a key/value pair with a specified key exists in this <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        /// <param name="key">The key to check for.</param>
        /// <returns>true if this instance contains the key; otherwise, false.</returns>
        public override bool ContainsKey(string key)
        {
            return this.values.ContainsKey(key);
        }

        /// <summary>
        /// Removes the key/value pair with a specified key from this <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        /// <returns>true if the element is successfully found and removed; otherwise, false.
        /// This method returns false if key is not found in this <see cref="System.Json.JsonObject"/> instance.</returns>
        public bool Remove(string key)
        {
            JsonValue original = null;
            bool containsKey = false;
            if (this.changingListeners > 0 || this.changedListeners > 0)
            {
                containsKey = this.TryGetValue(key, out original);
            }

            if (containsKey && this.changingListeners > 0)
            {
                this.RaiseItemChanging(original, JsonValueChange.Remove, key);
            }

            bool result = this.values.Remove(key);

            if (containsKey && this.changedListeners > 0)
            {
                this.RaiseItemChanged(original, JsonValueChange.Remove, key);
            }

            this.RemoveChildHandlers(original);
            return result;
        }

        /// <summary>
        /// Attempts to get the value that corresponds to the specified key.
        /// </summary>
        /// <param name="key">The key of the value to retrieve.</param>
        /// <param name="value">The primitive or structured <see cref="System.Json.JsonValue"/> object that has the key
        /// specified. If this object does not contain a key/value pair with the given key,
        /// this parameter is set to null.</param>
        /// <returns>true if the instance of the <see cref="System.Json.JsonObject"/> contains an element with the
        /// specified key; otherwise, false.</returns>
        public bool TryGetValue(string key, out JsonValue value)
        {
            return this.values.TryGetValue(key, out value);
        }

        /// <summary>
        /// Removes all key/value pairs from this <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        public void Clear()
        {
            this.RaiseItemChanging(null, JsonValueChange.Clear, null);
            foreach (KeyValuePair<string, JsonValue> item in this.values)
            {
                this.RemoveChildHandlers(item.Value);
            }

            this.values.Clear();
            this.RaiseItemChanged(null, JsonValueChange.Clear, null);
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Contains(KeyValuePair<string, JsonValue> item)
        {
            return ((ICollection<KeyValuePair<string, JsonValue>>)this.values).Contains(item);
        }

        /// <summary>
        /// Copies the contents of this <see cref="System.Json.JsonObject"/> instance into a specified
        /// key/value destination array beginning at a specified index.
        /// </summary>
        /// <param name="array">The destination array of type <see cref="System.Collections.Generic.KeyValuePair{K, V}"/>
        /// to which the elements of this <see cref="System.Json.JsonObject"/> are copied.</param>
        /// <param name="arrayIndex">The zero-based index at which to begin the insertion of the
        /// contents from this <see cref="System.Json.JsonObject"/> instance.</param>
        public void CopyTo(KeyValuePair<string, JsonValue>[] array, int arrayIndex)
        {
            ((ICollection<KeyValuePair<string, JsonValue>>)this.values).CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, JsonValue>>.Remove(KeyValuePair<string, JsonValue> item)
        {
            if (this.changingListeners > 0)
            {
                if (this.ContainsKey(item.Key) && EqualityComparer<JsonValue>.Default.Equals(item.Value, this.values[item.Key]))
                {
                    this.RaiseItemChanging(item.Value, JsonValueChange.Remove, item.Key);
                }
            }

            bool result = ((ICollection<KeyValuePair<string, JsonValue>>)this.values).Remove(item);
            if (result)
            {
                this.RaiseItemChanged(item.Value, JsonValueChange.Remove, item.Key);
                this.RemoveChildHandlers(item.Value);
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator over the key/value pairs contained in this <see cref="System.Json.JsonObject"/> instance.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerator{T}"/> which iterates
        /// through the members of this instance.</returns>
        public override IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Callback method called when a Save operation is starting for this instance.
        /// </summary>
        protected override void OnSaveStarted()
        {
            lock (this.saveLock)
            {
                this.instancesBeingSaved++;
                if (this.indexedKeys == null)
                {
                    this.indexedKeys = new List<string>(this.Keys);
                }
            }
        }

        /// <summary>
        /// Callback method called when a Save operation is finished for this instance.
        /// </summary>
        protected override void OnSaveEnded()
        {
            lock (this.saveLock)
            {
                this.instancesBeingSaved--;
                if (this.instancesBeingSaved == 0)
                {
                    this.indexedKeys = null;
                }
            }
        }

        /// <summary>
        /// Callback method called to let an instance write the proper JXML attribute when saving this
        /// instance.
        /// </summary>
        /// <param name="jsonWriter">The JXML writer used to write JSON.</param>
        protected override void WriteAttributeString(XmlDictionaryWriter jsonWriter)
        {
            jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.TypeAttributeName, JXmlToJsonValueConverter.ObjectAttributeValue);
        }

        /// <summary>
        /// Callback method called during Save operations to let the instance write the start element
        /// and return the next element in the collection.
        /// </summary>
        /// <param name="jsonWriter">The JXML writer used to write JSON.</param>
        /// <param name="currentIndex">The index within this collection.</param>
        /// <returns>The next item in the collection, or null of there are no more items.</returns>
        protected override JsonValue WriteStartElementAndGetNext(XmlDictionaryWriter jsonWriter, int currentIndex)
        {
            string currentKey = this.indexedKeys[currentIndex];

            if (currentKey.Length == 0)
            {
                // special case in JXML world
                jsonWriter.WriteStartElement(JXmlToJsonValueConverter.ItemElementName, JXmlToJsonValueConverter.ItemElementName);
                jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.ItemElementName, string.Empty);
            }
            else
            {
                jsonWriter.WriteStartElement(currentKey);
            }

            JsonValue nextValue = this[currentKey];

            return nextValue;
        }

        private void ChildChanging(object sender, JsonValueChangeEventArgs e)
        {
            this.RaiseChangingEvent(sender, e);
        }

        private void ChildChanged(object sender, JsonValueChangeEventArgs e)
        {
            this.RaiseChangedEvent(sender, e);
        }

        private void RaiseItemChanging(JsonValue child, JsonValueChange change, string key)
        {
            if (this.changingListeners > 0)
            {
                this.RaiseChangingEvent(this, new JsonValueChangeEventArgs(child, change, key));
            }
        }

        private void RaiseItemChanged(JsonValue child, JsonValueChange change, string key)
        {
            if (this.changedListeners > 0)
            {
                this.RaiseChangedEvent(this, new JsonValueChangeEventArgs(child, change, key));
            }
        }

        private void AddChildHandlers(JsonValue child)
        {
            if (child != null)
            {
                if (this.changingListeners > 0)
                {
                    child.Changing += new EventHandler<JsonValueChangeEventArgs>(this.ChildChanging);
                }

                if (this.changedListeners > 0)
                {
                    child.Changed += new EventHandler<JsonValueChangeEventArgs>(this.ChildChanged);
                }
            }
        }

        private void RemoveChildHandlers(JsonValue child)
        {
            if (child != null)
            {
                if (this.changingListeners > 0)
                {
                    child.Changing -= new EventHandler<JsonValueChangeEventArgs>(this.ChildChanging);
                }

                if (this.changedListeners > 0)
                {
                    child.Changed -= new EventHandler<JsonValueChangeEventArgs>(this.ChildChanged);
                }
            }
        }
    }
}

