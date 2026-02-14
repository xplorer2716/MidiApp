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
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Resources;
using System.Windows.Forms;

namespace MidiApp.UIControls
{
    /// <summary>
    /// An item for the ComboBoxValuedControl. Handles custom item names
    /// </summary>
    public class ComboBoxValuedControlItem
    {
        private readonly object _item;
        private readonly string _itemDescription;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="internalItem">the item</param>
        /// <param name="itemDescription">the item description</param>
        public ComboBoxValuedControlItem(object internalItem, string itemDescription)
        {
            _item = internalItem;
            _itemDescription = itemDescription;
        }

        /// <summary>
        /// The item
        /// </summary>
        public object Item
        {
            get { return _item; }
        }

        /// <summary>
        /// Returns a <see cref="System.String"/> that represents the item
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return _itemDescription;
        }

        /// <summary>
        /// Equals override for ComboBox.Contains()
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            //trivial
            if (this == obj) return true;
            ComboBoxValuedControlItem item = obj as ComboBoxValuedControlItem;
            if (item == null) return false;
            // Equals will not work if comparing int to enum, but will work if comparing enum to int
            else if (((int)item._item).Equals((int)this._item))
            {
                return true;
            }
            return false;
        }

        //
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    /// A ComboBox than implements IValuedControl with Enum support
    /// </summary>
    public partial class ComboBoxValuedControl : ComboBox, IValuedControl
    {
        private int _minimum = 0;
        private int _maximum = 1;
        private int _initializationValue = int.MinValue;

        private Type _enumType = null;

        // internal flag to avoid event firing when the IValuedControl.Value property is changed
        private bool _avoidSelectedItemEvent = false;

        /// <summary>
        /// Ctor
        /// </summary>
        public ComboBoxValuedControl()
        {
            BeginUpdate();
            this.DoubleBuffered = true;
            InitializeComponent();
            EndUpdate();
        }

        // cache for enumerations type value
        private static Dictionary<string, Array> _enumTpeValues = new Dictionary<string, Array>(200);

        /// <summary>
        /// Define the enum and resource manager used by the control to use/show values
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="resourceManager"></param>
        public void SetEnumType(Type enumType, ResourceManager resourceManager)
        {
            BeginUpdate();
            _minimum = 0;
            _maximum = 1;

            _enumType = enumType;

            this.Items.Clear();

            // try to get values from cache
            Array values;
            if (!_enumTpeValues.TryGetValue(enumType.Name, out values))
            {
                values = Enum.GetValues(enumType);
                _enumTpeValues[enumType.Name] = values;
            }

            foreach (int value in values)
            {
                string description = UIService.GetStringForEnumValue(enumType, value, resourceManager);
                ComboBoxValuedControlItem item = new ComboBoxValuedControlItem(value, description == null ? Enum.GetName(enumType, value) : description);
                this.Items.Add(item);
            }

            //set min/max, and default selected item
            int index = 0;
            int ValueIndex = -1;
            foreach (int value in values)
            {
                _minimum = System.Math.Min(value, _minimum);
                _maximum = System.Math.Max(value, _maximum);
                if (value == _initializationValue)
                {
                    ValueIndex = index;
                }
                index++;
            }

            // _value was not set to an enum type value, default init
            if (ValueIndex == -1)
            {
                SelectedIndex = 0;
            }
            else
            {
                // select the index of the Value
                SelectedIndex = ValueIndex;
            }
            EndUpdate();
        }

        public Type GetEnumType()
        {
            return _enumType;
        }

        #region IValuedControl Membres

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Value
        {
            get
            {
                // return the value of the current Enum Value
                if (SelectedItem != null)
                {
                    return (int)((ComboBoxValuedControlItem)SelectedItem).Item;
                }
                else return _initializationValue;
            }
            set
            {
                BeginUpdate();
                if (_enumType != null)
                {
                    _avoidSelectedItemEvent = true;
                    SelectedItem = Items.OfType<ComboBoxValuedControlItem>().First(comboItem => (int)comboItem.Item == value);
                }
                else
                {
                    _initializationValue = value;
                }
                EndUpdate();
            }
        }

        /// <summary>
        /// override to avoid event firing when the Value property is changed from outside
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectedItemChanged(EventArgs e)
        {
            if (!_avoidSelectedItemEvent)
            {
                base.OnSelectedItemChanged(e);
            }
            else
            {
                _avoidSelectedItemEvent = false;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Minimum
        {
            get
            {
                return _minimum;
            }
            set
            {
                _minimum = value;
                // the map is the reference, this will be override.
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Maximum
        {
            get
            {
                return _maximum;
            }
            set
            {
                _maximum = value;
                // the map is the reference, this will be override.
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public int Step
        {
            get
            {
                return 1;
            }
            set
            {
                // do nothing no sense here
            }
        }

        #endregion IValuedControl Membres
    }
}