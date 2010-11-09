// <copyright file="JsonArray.cs" company="Microsoft Corporation">
//   Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>

namespace System.Json
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Xml;

    /// <summary>
    /// A JsonArray is an ordered sequence of zero or more <see cref="System.Json.JsonValue"/> objects.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710",
        Justification = "Array already conveys the meaning of collection")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033",
        Justification = "Class already shipped, cannot make it sealed")]
    public sealed class JsonArray : JsonValue, IList<JsonValue>
    {
        private List<JsonValue> values = new List<JsonValue>();
        private int changingListeners = 0;
        private int changedListeners = 0;

        /// <summary>
        /// Creates an instance of the <see cref="System.Json.JsonArray"/> class initialized by
        /// an <see cref="System.Collections.Generic.IEnumerable{T}"/> enumeration of
        /// objects of type <see cref="System.Json.JsonValue"/>.
        /// </summary>
        /// <param name="items">The <see cref="System.Collections.Generic.IEnumerable{T}"/> enumeration
        /// of objects of type <see cref="System.Json.JsonValue"/> used to initialize the JavaScript Object Notation (JSON)
        /// array.</param>
        /// <exception cref="System.ArgumentNullException">If items is null.</exception>
        /// <exception cref="System.ArgumentException">If any of the items in the collection
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public JsonArray(IEnumerable<JsonValue> items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Creates an instance of the <see cref="System.Json.JsonArray"/> class, initialized by an array of type <see cref="System.Json.JsonValue"/>.
        /// </summary>
        /// <param name="items">The array of type <see cref="System.Json.JsonValue"/> used to initialize the
        /// JavaScript Object Notation (JSON) array.</param>
        /// <exception cref="System.ArgumentException">If any of the items in the collection
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public JsonArray(params JsonValue[] items)
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
                    foreach (JsonValue child in this.values)
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
                    foreach (JsonValue child in this.values)
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
                    foreach (JsonValue child in this.values)
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
                    foreach (JsonValue child in this.values)
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
        /// Gets the JSON type of this <see cref="System.Json.JsonArray"/>. The return value
        /// is always <see cref="System.Json.JsonType">Array</see>.
        /// </summary>
        public override JsonType JsonType
        {
            get
            {
                return JsonType.Array;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="System.Json.JsonArray"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ((IList)this.values).IsReadOnly;
            }
        }

        /// <summary>
        /// Returns the number of <see cref="System.Json.JsonValue"/> elements in the array.
        /// </summary>
        public override int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        /// <summary>
        /// Gets or sets the JSON value at a specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The <see cref="System.Json.JsonValue"/> element at the specified index.</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">If index is not a valid index for this array.</exception>
        /// <exception cref="System.ArgumentException">The property is set and the value is a
        /// <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/>
        /// property of value <see cref="System.Json.JsonType">Default</see>.</exception>
        public override JsonValue this[int index]
        {
            get
            {
                return this.values[index];
            }

            set
            {
                if (value != null && value.JsonType == JsonType.Default)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString(SR.UseOfDefaultNotAllowed));
                }

                JsonValue oldValue = this.values[index];
                this.RaiseItemChanging(value, JsonValueChange.Replace, index);
                this.values[index] = value;
                this.RaiseItemChanged(oldValue, JsonValueChange.Replace, index);

                this.RemoveChildHandlers(oldValue);
                this.AddChildHandlers(value);
            }
        }

        /// <summary>
        /// Adds the elements from a collection of type <see cref="System.Json.JsonValue"/> to this instance.
        /// </summary>
        /// <param name="items">Collection of items to add.</param>
        /// <exception cref="System.ArgumentNullException">If items is null.</exception>
        /// <exception cref="System.ArgumentException">If any of the items in the collection
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public void AddRange(IEnumerable<JsonValue> items)
        {
            if (items == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("items");
            }

            if (this.changingListeners > 0)
            {
                int index = this.Count;
                foreach (JsonValue toBeAdded in items)
                {
                    this.RaiseItemChanging(toBeAdded, JsonValueChange.Add, index++);
                }
            }

            foreach (JsonValue item in items)
            {
                if (item != null && item.JsonType == JsonType.Default)
                {
                    throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString(SR.UseOfDefaultNotAllowed));
                }

                this.values.Add(item);
                this.AddChildHandlers(item);
                this.RaiseItemChanged(item, JsonValueChange.Add, this.values.Count - 1);
            }
        }

        /// <summary>
        /// Adds the elements from an array of type <see cref="System.Json.JsonValue"/> to this instance.
        /// </summary>
        /// <param name="items">The array of type JsonValue to be added to this instance.</param>
        /// <exception cref="System.ArgumentNullException">If items is null.</exception>
        /// <exception cref="System.ArgumentException">If any of the items in the array
        /// is a <see cref="System.Json.JsonValue"/> with <see cref="System.Json.JsonValue.JsonType"/> property of
        /// value <see cref="System.Json.JsonType">Default</see>.</exception>
        public void AddRange(params JsonValue[] items)
        {
            this.AddRange(items as IEnumerable<JsonValue>);
        }

        /// <summary>
        /// Searches for a specified object and returns the zero-based index of its first
        /// occurrence within this <see cref="System.Json.JsonArray"/>.
        /// </summary>
        /// <param name="item">The <see cref="System.Json.JsonValue"/> object to look up.</param>
        /// <returns>The zero-based index of the first occurrence of item within the
        /// <see cref="System.Json.JsonArray"/>, if found; otherwise, -1.</returns>
        public int IndexOf(JsonValue item)
        {
            return this.values.IndexOf(item);
        }

        /// <summary>
        /// Insert a JSON CLR type into the array at a specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the item should be inserted.</param>
        /// <param name="item">The <see cref="System.Json.JsonValue"/> object to insert.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If index is less than zero or larger than
        /// the size of the array.</exception>
        /// <exception cref="System.ArgumentException">If the object to insert has a
        /// <see cref="System.Json.JsonValue.JsonType"/> property of value
        /// <see cref="System.Json.JsonType">Default</see>.</exception>
        public void Insert(int index, JsonValue item)
        {
            if (item != null && item.JsonType == JsonType.Default)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString(SR.UseOfDefaultNotAllowed));
            }

            this.RaiseItemChanging(item, JsonValueChange.Add, index);
            this.values.Insert(index, item);
            this.RaiseItemChanged(item, JsonValueChange.Add, index);
            this.AddChildHandlers(item);
        }

        /// <summary>
        /// Remove the JSON value at a specified index of <see cref="System.Json.JsonArray"/>.
        /// </summary>
        /// <param name="index">The zero-based index at which to remove the <see cref="System.Json.JsonValue"/>.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">If index is less than zero or index
        /// is equal or larger than the size of the array.</exception>
        public void RemoveAt(int index)
        {
            JsonValue item = this.values[index];
            this.RaiseItemChanging(item, JsonValueChange.Remove, index);
            this.values.RemoveAt(index);
            this.RaiseItemChanged(item, JsonValueChange.Remove, index);
            this.RemoveChildHandlers(item);
        }

        /// <summary>
        /// Adds a <see cref="System.Json.JsonValue"/> object to the end of the array.
        /// </summary>
        /// <param name="item">The <see cref="System.Json.JsonValue"/> object to add.</param>
        /// <exception cref="System.ArgumentException">If the object to add has a
        /// <see cref="System.Json.JsonValue.JsonType"/> property of value
        /// <see cref="System.Json.JsonType">Default</see>.</exception>
        public void Add(JsonValue item)
        {
            if (item != null && item.JsonType == JsonType.Default)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgument(SR.GetString(SR.UseOfDefaultNotAllowed));
            }
            
            int index = this.Count;
            this.RaiseItemChanging(item, JsonValueChange.Add, index);
            this.values.Add(item);
            this.RaiseItemChanged(item, JsonValueChange.Add, index);
            this.AddChildHandlers(item);
        }

        /// <summary>
        /// Removes all JSON CLR types from the <see cref="System.Json.JsonArray"/>.
        /// </summary>
        public void Clear()
        {
            this.RaiseItemChanging(null, JsonValueChange.Clear, 0);
            foreach (JsonValue child in this.values)
            {
                this.RemoveChildHandlers(child);
            }

            this.values.Clear();
            this.RaiseItemChanged(null, JsonValueChange.Clear, 0);
        }

        /// <summary>
        /// Checks whether a specified JSON CLR type is in the <see cref="System.Json.JsonArray"/>.
        /// </summary>
        /// <param name="item">The <see cref="System.Json.JsonValue"/> to check for in the array.</param>
        /// <returns>true if item is found in the <see cref="System.Json.JsonArray"/>; otherwise, false.</returns>
        public bool Contains(JsonValue item)
        {
            return this.values.Contains(item);
        }

        /// <summary>
        /// Copies the contents of the current JSON CLR array instance into a specified
        /// destination array beginning at the specified index.
        /// </summary>
        /// <param name="array">The destination array to which the elements of the current
        /// <see cref="System.Json.JsonArray"/> object are copied.</param>
        /// <param name="arrayIndex">The zero-based index in the destination array at which the
        /// copying of the elements of the JSON CLR array begins.</param>
        public void CopyTo(JsonValue[] array, int arrayIndex)
        {
            this.values.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of the specified JSON value from the array.
        /// </summary>
        /// <param name="item">The <see cref="System.Json.JsonValue"/> to remove from the <see cref="System.Json.JsonArray"/>.</param>
        /// <returns>true if item is successfully removed; otherwise, false. This method
        /// also returns false if item was not found in the <see cref="System.Json.JsonArray"/>.</returns>
        public bool Remove(JsonValue item)
        {
            int index = -1;
            if (this.changingListeners > 0 || this.changedListeners > 0)
            {
                index = this.IndexOf(item);
            }

            if (index >= 0)
            {
                this.RaiseItemChanging(item, JsonValueChange.Remove, index);
            }

            bool result = this.values.Remove(item);
            if (index >= 0)
            {
                this.RaiseItemChanged(item, JsonValueChange.Remove, index);
                this.RemoveChildHandlers(item);
            }

            return result;
        }

        /// <summary>
        /// Returns an enumerator which iterates through the values in this object.
        /// </summary>
        /// <returns>An <see cref="System.Collections.Generic.IEnumerator{T}"/> which iterates through the values in this object.</returns>
        /// <remarks>The enumerator returned by this class contains one pair for each element
        /// in this array, whose key is the element index (as a string), and the value is the
        /// element itself.</remarks>
        public override IEnumerator<KeyValuePair<string, JsonValue>> GetEnumerator()
        {
            for (int i = 0; i < this.values.Count; i++)
            {
                yield return new KeyValuePair<string, JsonValue>(i.ToString(CultureInfo.InvariantCulture), this.values[i]);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="System.Json.JsonValue"/> objects in the array.
        /// </summary>
        /// <returns>Returns an <see cref="System.Collections.Generic.IEnumerator{T}"/> object that
        /// iterates through the <see cref="System.Json.JsonValue"/> elements in this <see cref="System.Json.JsonArray"/>.</returns>
        IEnumerator<JsonValue> IEnumerable<JsonValue>.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="System.Json.JsonValue"/> objects in the array.
        /// </summary>
        /// <returns>Returns an <see cref="System.Collections.IEnumerator"/> object that
        /// iterates through the <see cref="System.Json.JsonValue"/> elements in this <see cref="System.Json.JsonArray"/>.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Safe indexer for the <see cref="System.Json.JsonValue"/> type. 
        /// </summary>
        /// <param name="index">The zero-based index of the element to get.</param>
        /// <returns>If the index is within the array bounds and the value corresponding to the
        /// index is not null, then it will return that value. Otherwise it will return a
        /// <see cref="System.Json.JsonValue"/> instance with <see cref="System.Json.JsonValue.JsonType"/>
        /// equals to <see cref="System.Json.JsonType">Default</see>.</returns>
        public override JsonValue ValueOrDefault(int index)
        {
            if (index >= 0 && index < this.Count && this[index] != null)
            {
                return this[index];
            }

            return base.ValueOrDefault(index);
        }

        /// <summary>
        /// Callback method called to let an instance write the proper JXML attribute when saving this
        /// instance.
        /// </summary>
        /// <param name="jsonWriter">The JXML writer used to write JSON.</param>
        protected override void WriteAttributeString(XmlDictionaryWriter jsonWriter)
        {
            if (jsonWriter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("jsonWriter");
            }

            jsonWriter.WriteAttributeString(JXmlToJsonValueConverter.TypeAttributeName, JXmlToJsonValueConverter.ArrayAttributeValue);
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
            if (jsonWriter == null)
            {
                throw DiagnosticUtility.ExceptionUtility.ThrowHelperArgumentNull("jsonWriter");
            }

            jsonWriter.WriteStartElement(JXmlToJsonValueConverter.ItemElementName);
            JsonValue nextValue = this[currentIndex];
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

        private void RaiseItemChanging(JsonValue child, JsonValueChange change, int index)
        {
            if (this.changingListeners > 0)
            {
                this.RaiseChangingEvent(this, new JsonValueChangeEventArgs(child, change, index));
            }
        }

        private void RaiseItemChanged(JsonValue child, JsonValueChange change, int index)
        {
            if (this.changedListeners > 0)
            {
                this.RaiseChangedEvent(this, new JsonValueChangeEventArgs(child, change, index));
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
