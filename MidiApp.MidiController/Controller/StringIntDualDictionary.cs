/*
Xplorer - A real-time editor for the Oberheim Xpander and Matrix-12 synths
Copyright (C) 2012-2024 Pascal Schmitt

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;

namespace MidiApp.MidiController.Controller
{
    /// <summary>
    /// a class to associate 1 string to 1 int
    /// </summary>
    public class StringIntDualEntry
    {
        public string StringValue { get; set; }
        public int IntValue { get; set; }

        public StringIntDualEntry(string stringValue, int intValue)
        {
            StringValue = stringValue;
            IntValue = intValue;
        }
    }

    /// <summary>
    /// a dual entry dictionary to
    /// - find a CC number by parameter name
    /// - find a parameter name by associated CC number
    /// </summary>
    public class StringIntDualDictionary : ICollection<StringIntDualEntry>, IEnumerator<StringIntDualEntry>
    {
        private const int INITIAL_CAPACITY = 128;

        // make this class thread safe
        private readonly object _lockObject;

        // for IEnumerator<> impl.
        private StringIntDualEntry _iteratorCurrentEntry;

        private Dictionary<string, int>.Enumerator _stringIntListDictionaryEnumerator;

        private readonly Dictionary<int, List<string>> _intStringListDictionary;
        private readonly Dictionary<string, int> _stringIntListDictionary;

        public StringIntDualDictionary()
        {
            // instanciate
            _intStringListDictionary = new Dictionary<int, List<string>>(INITIAL_CAPACITY);
            _stringIntListDictionary = new Dictionary<string, int>(INITIAL_CAPACITY);
            _iteratorCurrentEntry = null;
            _lockObject = new object();
        }

        /// <summary>
        /// indexer to get int value by string value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>the value or int.MinValue if not found</returns>
        public int this[string value]
        {
            get
            {
                lock (_lockObject)
                {
                    int Value;
                    if (_stringIntListDictionary.TryGetValue(value, out Value))
                    {
                        return Value;
                    }
                    return int.MinValue;
                }
            }
        }

        /// <summary>
        /// indexer to get string value by int value
        /// </summary>
        /// <param name="value"></param>
        /// <returns>the value or null if not found</returns>
        public List<string> this[int value]
        {
            get
            {
                lock (_lockObject)
                {
                    List<string> stringList;
                    if (_intStringListDictionary.TryGetValue(value, out stringList))
                    {
                        return stringList;
                    }
                    return null;
                }
            }
        }

        #region ICollection<CCParameterEntry> Membres

        public void Add(StringIntDualEntry item)
        {
            lock (_lockObject)
            {
                // remove item if exists, don't care if not
                Remove(item);

                // add in both dictionnaries
                _stringIntListDictionary[item.StringValue] = item.IntValue;

                //same for int/string dictionary
                List<string> stringList;
                if (_intStringListDictionary.TryGetValue(item.IntValue, out stringList))
                {
                    stringList.Add(item.StringValue);
                }
                else
                {
                    List<string> newList = new List<string>
                    {
                        item.StringValue
                    };
                    _intStringListDictionary.Add(item.IntValue, newList);
                }
            }
        }

        public void Clear()
        {
            lock (_lockObject)
            {
                _stringIntListDictionary.Clear();
                _intStringListDictionary.Clear();
            }
        }

        public bool Contains(StringIntDualEntry item)
        {
            lock (_lockObject)
            {
                throw new NotImplementedException();
            }
        }

        public void CopyTo(StringIntDualEntry[] array, int arrayIndex)
        {
            lock (_lockObject)
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// returns the <b><u>string</u></b> Count
        /// </summary>
        public int Count
        {
            get
            {
                lock (_lockObject)
                {
                    // both dictionary should have same count
                    int iCount = _stringIntListDictionary.Count;
                    System.Diagnostics.Debug.Assert(iCount == _intStringListDictionary.Count);
                    return iCount;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                lock (_lockObject)
                {
                    throw new NotImplementedException();
                }
            }
        }

        public bool Remove(StringIntDualEntry item)
        {
            lock (_lockObject)
            {
                // remove in both dictionnaries
                this._stringIntListDictionary.Remove(item.StringValue);

                //same for int/string dictionary
                List<string> stringList;
                if (_intStringListDictionary.TryGetValue(item.IntValue, out stringList))
                {
                    stringList.Remove(item.StringValue);
                    //do not remove the list if empty to avoid reallocation
                }
                return true;
            }
        }

        #endregion ICollection<CCParameterEntry> Membres

        #region IEnumerable<CCParameterEntry> Membres

        public IEnumerator<StringIntDualEntry> GetEnumerator()
        {
            lock (_lockObject)
            {
                // get a new  iterator
                _stringIntListDictionaryEnumerator = _stringIntListDictionary.GetEnumerator();
                return (IEnumerator<StringIntDualEntry>)this;
            }
        }

        #endregion IEnumerable<CCParameterEntry> Membres

        #region IEnumerable Membres

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            lock (_lockObject)
            {
                return ((IEnumerable<StringIntDualEntry>)this).GetEnumerator();
            }
        }

        #endregion IEnumerable Membres

        #region IEnumerator<CCParameterEntry> Membres

        public StringIntDualEntry Current
        {
            get
            {
                lock (_lockObject)
                {
                    return _iteratorCurrentEntry;
                }
            }
        }

        #endregion IEnumerator<CCParameterEntry> Membres

        #region IDisposable Membres

        public void Dispose()
        {
            //nop
        }

        #endregion IDisposable Membres

        #region IEnumerator Membres

        object System.Collections.IEnumerator.Current
        {
            get
            {
                lock (_lockObject)
                {
                    return ((IEnumerator<StringIntDualEntry>)this).Current;
                }
            }
        }

        public bool MoveNext()
        {
            lock (_lockObject)
            {
                bool bStatus = false;

                bool bMoveNext = _stringIntListDictionaryEnumerator.MoveNext();
                if (bMoveNext)
                {
                    //update the current entry
                    if (_iteratorCurrentEntry == null)
                    {
                        _iteratorCurrentEntry = new StringIntDualEntry(null, int.MinValue);
                    }
                    _iteratorCurrentEntry.StringValue = _stringIntListDictionaryEnumerator.Current.Key;
                    _iteratorCurrentEntry.IntValue = _stringIntListDictionaryEnumerator.Current.Value;
                }
                else
                {
                    _iteratorCurrentEntry = null;
                }
                bStatus = bMoveNext;

                return bStatus;
            }
        }

        public void Reset()
        {
            lock (_lockObject)
            {
                // no Reset for Generic Iterator ?
                throw new NotImplementedException();
            }
        }

        #endregion IEnumerator Membres
    }
}