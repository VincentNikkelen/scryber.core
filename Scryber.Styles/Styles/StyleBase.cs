﻿/*  Copyright 2012 PerceiveIT Limited
 *  This file is part of the Scryber library.
 *
 *  You can redistribute Scryber and/or modify 
 *  it under the terms of the GNU Lesser General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 * 
 *  Scryber is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU Lesser General Public License for more details.
 * 
 *  You should have received a copy of the GNU Lesser General Public License
 *  along with Scryber source code in the COPYING.txt file.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scryber;
using Scryber.Drawing;

namespace Scryber.Styles
{
    /// <summary>
    /// Abstract base class for all full styles (contains the StyleItems, AND the values associated with those items).
    /// </summary>
    public abstract class StyleBase : PDFObject
    {
        private const int DirectFillFactor = 5;
        private const int InheritedFillFactor = 5;

        private StyleValueDictionary _direct = null;
        private StyleValueDictionary _inherited = null;
        private StyleItemCollection _items = null;

        #region public bool Immutable {get;set;}

        private bool _isimmutable;

        
        /// <summary>
        /// Gets or sets the flag that marks this style as immutable
        /// </summary>
        public bool Immutable
        {
            get { return this._isimmutable; }
            set { this._isimmutable = value; }
        }

        #endregion

        #region public bool HasValues {get;}

        /// <summary>
        /// Returns true if this style has one or more values
        /// </summary>
        public bool HasValues
        {
            get
            {
                if (null != _direct && _direct.Count > 0)
                    return true;
                else if (null != _inherited && _inherited.Count > 0)
                    return true;
                else
                    return false;
            }
        }

        #endregion

        #region public PDFStyleItemCollection Items

        /// <summary>
        /// Gets the collection of StyleItems that have been instaniated 
        /// (There may be style values that are defined in this Style which do not have a corresponding StyleItem)
        /// </summary>
        public virtual StyleItemCollection StyleItems
        {
            get
            {
                if (null == _items)
                    _items = new StyleItemCollection(this);
                return _items;
            }
        }

        #endregion

        #region protected StyleValueDictionary DirectValues

        /// <summary>
        /// Gets the dictionary of values applied directly to this style
        /// </summary>
        protected StyleValueDictionary DirectValues
        {
            get
            {
                if (null == _direct)
                    _direct = new StyleValueDictionary(DirectFillFactor);
                return _direct;
            }
        }

        #endregion

        #region protected StyleValueDictionary InheritedValues

        /// <summary>
        /// Gets the dictionary of values applied directly to this style
        /// </summary>
        protected StyleValueDictionary InheritedValues
        {
            get
            {
                if (null == _inherited)
                    _inherited = new StyleValueDictionary(InheritedFillFactor);
                return _inherited;
            }
        }

        #endregion

        #region public int ValueCount {get;}

        /// <summary>
        /// Gets the number of Values in this style
        /// </summary>
        public int ValueCount
        {
            get
            {
                int valCount = 0;
                if (null != _inherited)
                    valCount += _inherited.Count;
                if (null != _direct)
                    valCount += _direct.Count;
                return valCount;
            }
        }

        #endregion

        //
        // ctor
        //

        #region protected PDFStyleBase()

        protected StyleBase(PDFObjectType type)
            : base(type)
        {
            _isimmutable = false;
        }

        #endregion

        //
        // public methods
        //

        #region public virtual void MergeInto(PDFStyleBase style) + 1 overload

        
        /// <summary>
        /// Merges all the style values in this style into the provided style.
        /// This will overwrite existing values in the provided style.
        /// </summary>
        /// <param name="style"></param>
        public virtual void MergeInto(StyleBase style, int priority)
        {
            if (null == style)
                throw new ArgumentNullException("style");

            style.BeginStyleChange();

            if (null != this._direct && this._direct.Count > 0)
            {
                foreach (KeyValuePair<StyleKey, PDFStyleValueBase> kvp in this._direct)
                {
                    style.DirectValues.SetPriorityValue(kvp.Key, kvp.Value, priority);
                }
            }

            if (null != this._inherited && this._inherited.Count > 0)
            {
                foreach (KeyValuePair<StyleKey, PDFStyleValueBase> kvp in this._inherited)
                {
                    style.InheritedValues.SetPriorityValue(kvp.Key, kvp.Value, priority);
                }
            }

            
        }

        #endregion

        #region public virtual void MergeInto(PDFStyleBase style) + 1 overload


        /// <summary>
        /// Merges all the style values in this style into the provided style.
        /// This will overwrite existing values in the provided style as long as they are higher priority
        /// </summary>
        /// <param name="style"></param>
        public virtual void MergeInto(StyleBase style)
        {
            if (null == style)
                throw new ArgumentNullException("style");

            style.BeginStyleChange();

            if (null != this._direct && this._direct.Count > 0)
            {
                foreach (KeyValuePair<StyleKey, PDFStyleValueBase> kvp in this._direct)
                {
                    style.DirectValues.SetPriorityValue(kvp.Key, kvp.Value, kvp.Value.Priority);
                }
            }

            if (null != this._inherited && this._inherited.Count > 0)
            {
                foreach (KeyValuePair<StyleKey, PDFStyleValueBase> kvp in this._inherited)
                {
                    style.InheritedValues.SetPriorityValue(kvp.Key, kvp.Value, kvp.Value.Priority);
                }
            }


        }

        #endregion

        #region public virtual void MergeInto(PDFStyle style, Scryber.IPDFComponent Component, Scryber.ComponentState state)

        /// <summary>
        /// Merges all the style values in this style into the provided style, based on the component and state.
        /// This will overwrite existing values in the provided style.
        /// </summary>
        /// <param name="style"></param>
        /// <param name="Component"></param>
        /// <param name="state"></param>
        public virtual void MergeInto(Style style, Scryber.IPDFComponent Component, Scryber.ComponentState state)
        {
            this.MergeInto(style, 0);
        }

        #endregion

        #region public void MergeInherited(PDFStyle style, Scryber.IPDFComponent Component, bool replace)

        /// <summary>
        /// Merges all the inherited values from this style into the provided style based on the component and replace flag
        /// </summary>
        /// <param name="style"></param>
        /// <param name="Component"></param>
        /// <param name="replace"></param>
        public void MergeInherited(Style style, bool replace, int priority)
        {
            if (null == style)
                throw new ArgumentNullException("style");

            style.BeginStyleChange();

            if(null != this._inherited && this._inherited.Count > 0)
            {
                if (replace)
                {
                    foreach (KeyValuePair<StyleKey, PDFStyleValueBase> kvp in this._inherited)
                    {
                        style.InheritedValues[kvp.Key] = kvp.Value.CloneWithPriority(priority);   
                    }
                }
                else
                {
                    foreach (KeyValuePair<StyleKey, PDFStyleValueBase> kvp in this._inherited)
                    {
                        if (!style.InheritedValues.ContainsKey(kvp.Key))
                            style.InheritedValues.SetPriorityValue(kvp.Key, kvp.Value, priority);
                    }
                }
            }
        }

        #endregion

        #region public virtual PDFStyle Flatten()

        /// <summary>
        /// Returns a flat version of the PDFStyle. (In this case it is the same instance).
        /// </summary>
        /// <returns></returns>
        public virtual Style Flatten()
        {
            //Does nothing in the new implementation
            //As we are always flat.
            return (Style)this;
        }

        #endregion

        #region public void Clear()

        /// <summary>
        /// Removes all items from this Style
        /// </summary>
        public void Clear()
        {
            this.BeginStyleChange();
            this.DoClear();
        }

        #endregion

        //
        // protected methods
        //

        #region protected virtual void BeginStyleChange()

        /// <summary>
        /// Makes sure this style can be modified. Throws an invalid operation excpetion if it cannot.
        /// </summary>
        protected virtual void BeginStyleChange()
        {
            if (this.Immutable)
                throw new InvalidOperationException("Cannot modify this style as it is immutable");
        }

        #endregion

        #region protected virtual void DoClear()

        /// <summary>
        /// Removes all items and values from this style
        /// </summary>
        protected virtual void DoClear()
        {
            if (null != _direct)
                this._direct.Clear();
            if (null != _inherited)
                this._inherited.Clear();
            if (null != _items)
                this._items.Clear();
        }

        #endregion


        //TODO: Improve the implementation so that Items do not have to retain event fields

        #region protected virtual void DoDataBind(PDFDataContext context, bool includechildren)

        protected virtual void DoDataBind(PDFDataContext context, bool includechildren)
        {
            if (includechildren && this.StyleItems.Count > 0)
            {
                foreach (StyleItemBase item in this.StyleItems)
                {
                    item.DataBind(context);
                }
            }
        }

        #endregion

        //
        // impementation
        //

        #region public bool IsValueDefined(StyleKey key)

        /// <summary>
        /// Returns true if this style contains the provided key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsValueDefined(StyleKey key)
        {
            if (null == key)
                throw new ArgumentNullException("key");
            else if (key.Inherited)
                return (null != this._inherited && this._inherited.ContainsKey(key));
            else
                return (null != this._direct && this._direct.ContainsKey(key));
        }

        #endregion

        #region internal void AddItem(StyleItemBase baseitem)

        /// <summary>
        /// Adds the style item to this styles collection.
        /// NOTE: If the collection already contains an item with the same key then an exception is raised.
        /// </summary>
        /// <param name="baseitem"></param>
        internal void AddItem(StyleItemBase baseitem)
        {
            if (null == baseitem)
                throw new ArgumentNullException("baseitem");

            this.StyleItems.Add(baseitem);
        }

        #endregion

        #region internal void AddValue(StyleValueBase basevalue)

        /// <summary>
        /// Adds the value to this style
        /// </summary>
        /// <param name="basevalue"></param>
        internal void AddValue(PDFStyleValueBase basevalue)
        {
            BeginStyleChange();

            if (null == basevalue)
                throw new ArgumentNullException("basevalue");
            else if (null == basevalue.Key)
                throw new ArgumentNullException("basevalue.Key");

            else if (basevalue.Key.Inherited)
                this.InheritedValues.Add(basevalue.Key, basevalue);
            else
                this.DirectValues.Add(basevalue.Key, basevalue);
        }

        #endregion

        #region internal void AddValueRange(IEnumerable<StyleValueBase> all)

        /// <summary>
        /// Adds a range of values to this style
        /// </summary>
        /// <param name="all"></param>
        internal void AddValueRange(IEnumerable<PDFStyleValueBase> all)
        {
            if (null == all)
                return;

            BeginStyleChange();

            foreach (PDFStyleValueBase item in all)
            {
                if (item.Key.Inherited)
                    this.InheritedValues.Add(item.Key, item);
                else
                    this.DirectValues.Add(item.Key, item);
            }
        }

        #endregion

        #region public bool RemoveValue(StyleKey key)

        /// <summary>
        /// Removes any value form this style associated with the specified key. 
        /// Returns true if a value was removed.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool RemoveValue(StyleKey key)
        {
            BeginStyleChange();

            if (null == key)
                throw new ArgumentNullException("key");

            else if (key.Inherited)
            {
                if (null == _inherited)
                    return false;
                else
                    return _inherited.Remove(key);
            }
            else
            {
                if (null == _direct)
                    return false;
                else
                    return _direct.Remove(key);
            }
        }

        #endregion

        #region public bool RemoveItemStyleValues(PDFStyleKey itemkey)

        /// <summary>
        /// Removes all the style values assocated with the provided item style in this style
        /// </summary>
        /// <param name="itemkey"></param>
        /// <returns></returns>
        public bool RemoveItemStyleValues(StyleKey itemkey)
        {
            bool removed = false;
            if (itemkey.IsItemKey == false)
                throw new InvalidOperationException("itemkey is not actually an item key - use RemoveValue for value keys");

            if (itemkey.Inherited)
            {
                if (null != _inherited && _inherited.Count > 0)
                {
                    List<StyleKey> found = new List<StyleKey>();
                    foreach (KeyValuePair<StyleKey,PDFStyleValueBase> exist in this._inherited)
                    {
                        if (exist.Key.StyleItemKey == itemkey.StyleItemKey)
                            found.Add(exist.Key);
                    }

                    foreach (StyleKey toremove in found)
                    {
                        this._inherited.Remove(toremove);
                        
                    }

                    removed = found.Count > 0;
                }
            }
            else
            {
                if (null != _direct && _direct.Count > 0)
                {
                    List<StyleKey> found = new List<StyleKey>();
                    foreach (KeyValuePair<StyleKey, PDFStyleValueBase> exist in this._direct)
                    {
                        if (exist.Key.StyleItemKey == itemkey.StyleItemKey)
                            found.Add(exist.Key);
                    }

                    foreach (StyleKey toremove in found)
                    {
                        this._direct.Remove(toremove);
                    }

                    removed = found.Count > 0;
                }
            }

            return removed;
        }

        #endregion

        #region internal bool TryGetBaseItem(StyleKey key, out StyleItemBase found)

        /// <summary>
        /// Looks for a StyleItem in this Style, and returns true if one was found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="found"></param>
        /// <returns></returns>
        internal bool TryGetBaseItem(StyleKey key, out StyleItemBase found)
        {
            if(null == _items)
            {
                found = null;
                return false;
            }
            else
                return _items.TryGetItem(key, out found);
        }

        #endregion

        #region public bool TryGetBaseValue(StyleKey key, out StyleValueBase found)

        /// <summary>
        /// Looks up a StyleValue in this Style, and returns true if one was found.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="found"></param>
        /// <returns></returns>
        public bool TryGetBaseValue(StyleKey key, out PDFStyleValueBase found)
        {
            if (key.Inherited)
            {
                if(null == _inherited || _inherited.Count == 0)
                {
                    found = null;
                    return false;
                }
                else
                    return _inherited.TryGetValue(key, out found);
            }
            else
            {
                if(null == _direct || _direct.Count == 0)
                {
                    found = null;
                    return false;
                }
                else
                    return _direct.TryGetValue(key, out found);
            }
        }

        #endregion

        #region internal StyleValueBase[] RemoveItemStyleValues(StyleItemBase style)

        /// <summary>
        /// Removes all the style values from this style that belong to the specified style item
        /// </summary>
        /// <param name="styleKey"></param>
        /// <returns></returns>
        internal PDFStyleValueBase[] RemoveAndReturnItemStyleValues(StyleItemBase style)
        {
            List<StyleKey> matching = new List<StyleKey>();
            List<PDFStyleValueBase> values = new List<PDFStyleValueBase>();
            int count = 0;
            if(null == style)
                throw new ArgumentNullException("style");

            Scryber.PDFObjectType itemkey = style.ItemKey.StyleItemKey;

            if (null != this._direct && this._direct.Count > 0)
            {
                //remove all the style values from the direct dictionary
                //that have the same item key as the provided style item key.
                foreach (KeyValuePair<StyleKey,PDFStyleValueBase> exist in this._direct)
                {
                    if (exist.Key.StyleItemKey == itemkey)
                    {
                        matching.Add(exist.Key);
                        values.Add(exist.Value);
                    }
                }
                if (matching.Count > 0)
                {
                    count += matching.Count;
                    foreach (StyleKey exist in matching)
                    {
                        _direct.Remove(exist);
                    }
                }
            }

            if(null != this._inherited && this._inherited.Count > 0)
            {
                //remove all the style values from the inherited dictionary
                //that have the same item key as the provided style item key.
                
                matching.Clear();

                foreach (KeyValuePair<StyleKey, PDFStyleValueBase> exist in this._inherited)
                {
                    if (exist.Key.StyleItemKey == itemkey)
                    {
                        matching.Add(exist.Key);
                        values.Add(exist.Value);
                    }
                }
                if (matching.Count > 0)
                {
                    count += matching.Count;
                    foreach (StyleKey exist in matching)
                    {
                        _inherited.Remove(exist);
                    }
                }
            }
            if (count > 0)
                return values.ToArray();
            else
                return EmptyStyleValueArray;
        }

        private static readonly PDFStyleValueBase[] EmptyStyleValueArray = new PDFStyleValueBase[] { };

        #endregion

        #region internal Scryber.Drawing.PDFThickness GetThickness(bool inherited, StyleKey all, StyleKey top, StyleKey left, StyleKey bottom, StyleKey right)

        /// <summary>
        /// Based on the provided style keys, builds a new PDFThickness with all values set and returns it.
        /// </summary>
        /// <param name="inherited"></param>
        /// <param name="all"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        internal bool TryGetThickness(bool inherited, PDFStyleKey<PDFUnit> all, PDFStyleKey<PDFUnit> top, PDFStyleKey<PDFUnit> left, PDFStyleKey<PDFUnit> bottom, PDFStyleKey<PDFUnit> right, out PDFThickness thickness)
        {
            Dictionary<StyleKey, PDFStyleValueBase> lookup = inherited ? _inherited : _direct;

            thickness = new Scryber.Drawing.PDFThickness();
            bool hasvalues = false;
            PDFStyleValueBase found;

            if(null == lookup || lookup.Count == 0)
            {
                thickness = PDFThickness.Empty();
                return false;
            }

            if (lookup.TryGetValue(all, out found))
            {
                thickness.SetAll(((StyleValue<PDFUnit>)found).Value);
                hasvalues = true;
            }

            if (lookup.TryGetValue(top, out found))
            {
                thickness.Top = ((StyleValue<PDFUnit>)found).Value;
                hasvalues = true;
            }

            if (lookup.TryGetValue(left, out found))
            {
                thickness.Left = ((StyleValue<PDFUnit>)found).Value;
                hasvalues = true;
            }

            if (lookup.TryGetValue(bottom, out found))
            {
                thickness.Bottom = ((StyleValue<PDFUnit>)found).Value;
                hasvalues = true;
            }

            if (lookup.TryGetValue(right, out found))
            {
                thickness.Right = ((StyleValue<PDFUnit>)found).Value;
                hasvalues = true;
            }

            return hasvalues;
        }

        #endregion

        #region internal void SetThickness(bool inherited, Scryber.Drawing.PDFThickness thickness, StyleKey top, StyleKey left, StyleKey bottom, StyleKey right)

        /// <summary>
        /// Based on the specified thickness, sets all the style values in this style to the correct value. 
        /// Replaces any that may already exist
        /// </summary>
        /// <param name="inherited"></param>
        /// <param name="thickness"></param>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        internal void SetThickness(bool inherited, Scryber.Drawing.PDFThickness thickness, PDFStyleKey<PDFUnit> top, PDFStyleKey<PDFUnit> left, PDFStyleKey<PDFUnit> bottom, PDFStyleKey<PDFUnit> right)
        {
            Dictionary<StyleKey, PDFStyleValueBase> lookup = inherited ? this.InheritedValues : this.DirectValues;

            lookup[top] = new StyleValue<PDFUnit>(top, thickness.Top);
            lookup[left] = new StyleValue<Scryber.Drawing.PDFUnit>(left, thickness.Left);
            lookup[bottom] = new StyleValue<Scryber.Drawing.PDFUnit>(bottom, thickness.Bottom);
            lookup[right] = new StyleValue<Scryber.Drawing.PDFUnit>(right, thickness.Right);
        }

        #endregion


        //
        // DoCreateXXX methods
        //

        #region internal protected virtual PDFPageSize DoCreatePageSize()

        /// <summary>
        /// Implementation of the PDFPageSize creation. Inheritors can override
        /// </summary>
        /// <returns></returns>
        internal protected virtual PageSize DoCreatePageSize()
        {
            StyleValue<PDFUnit> w;
            StyleValue<PDFUnit> h;

            //We use the explicit Position width and height
            bool hasw = this.TryGetValue(StyleKeys.SizeWidthKey, out w);
            bool hash = this.TryGetValue(StyleKeys.SizeHeightKey, out h);

            //If they are not set then we fall back to the Page width and height
            if (!hasw)
                hasw = this.TryGetValue(StyleKeys.PageWidthKey, out w);
            if (!hash)
                hash = this.TryGetValue(StyleKeys.PageHeightKey, out h);

            if (hasw && hash)
                return new PageSize(new PDFSize(w.Value, h.Value));
            else
            {
                //if we don't have any explicit sizes - and we need both we use the paper size
                PaperSize sz;
                PaperOrientation orient;
                StyleValue<PaperSize> szVal;
                StyleValue<PaperOrientation> orientVal;
                if (this.TryGetValue(StyleKeys.PagePaperSizeKey, out szVal))
                    sz = szVal.Value;
                else
                    sz = Scryber.Const.DefaultPaperSize;

                if (this.TryGetValue(StyleKeys.PageOrientationKey, out orientVal))
                    orient = orientVal.Value;
                else
                    orient = Scryber.Const.DefaultPaperOrientation;

                return new PageSize(sz, orient);
            }
        }

        #endregion

        #region internal protected virtual PDFPositionOptions DoCreatePositionOptions()

        /// <summary>
        /// Implementation of the PDFPositionOptions creation
        /// </summary>
        /// <returns></returns>
        internal protected virtual PDFPositionOptions DoCreatePositionOptions()
        {
            PDFPositionOptions options = new PDFPositionOptions();
            StyleValue<PositionMode> posmode;

            if (this.TryGetValue(StyleKeys.PositionModeKey, out posmode))
                options.PositionMode = posmode.Value;
            else
                options.PositionMode = PositionMode.Block;

            StyleValue<PDFUnit> unit;

            // X
            if (this.TryGetValue(StyleKeys.PositionXKey, out unit))
            {
                options.X = unit.Value;

                if (options.PositionMode != PositionMode.Absolute)
                    options.PositionMode = PositionMode.Relative;
            }
            else
                options.X = null;

            // Y
            if (this.TryGetValue(StyleKeys.PositionYKey, out unit))
            {
                options.Y = unit.Value;

                if (options.PositionMode != PositionMode.Absolute)
                    options.PositionMode = PositionMode.Relative;
            }
            else
                options.Y = null;

            // Width
            if (this.TryGetValue(StyleKeys.SizeWidthKey, out unit))
            {
                options.Width = unit.Value;
            }
            else
                options.Width = null;

            // Height
            if (this.TryGetValue(StyleKeys.SizeHeightKey, out unit))
            {
                options.Height = unit.Value;
            }
            else
                options.Height = null;

            // MinimumWidth
            if (this.TryGetValue(StyleKeys.SizeMinimumWidthKey, out unit))
            {
                options.MinimumWidth = unit.Value;
            }
            else
                options.MinimumWidth = null;

            // MinimumHeight
            if (this.TryGetValue(StyleKeys.SizeMinimumHeightKey, out unit))
            {
                options.MinimumHeight = unit.Value;
            }
            else
                options.MinimumHeight = null;

            // MaximumWidth
            if (this.TryGetValue(StyleKeys.SizeMaximumWidthKey, out unit))
            {
                options.MaximumWidth = unit.Value;
            }
            else
                options.MaximumWidth = null;

            // MaximumHeight
            if (this.TryGetValue(StyleKeys.SizeMaximumHeightKey, out unit))
            {
                options.MaximumHeight = unit.Value;
            }
            else
                options.MaximumHeight = null;

            StyleValue<bool> b;
            if (this.TryGetValue(StyleKeys.SizeFullWidthKey, out b))
                options.FillWidth = b.Value;


            StyleValue<VerticalAlignment> valign;
            if (this.TryGetValue(StyleKeys.PositionVAlignKey, out valign))
                options.VAlign = valign.Value;
            else
                options.VAlign = Const.DefaultVerticalAlign;

            StyleValue<HorizontalAlignment> halign;
            StyleValue<TextDirection> direction;

            if (this.TryGetValue(StyleKeys.PositionHAlignKey, out halign))
                options.HAlign = halign.Value;
            else if (this.TryGetValue(StyleKeys.TextDirectionKey, out direction) && direction.Value == TextDirection.RTL)
                options.HAlign = HorizontalAlignment.Right;
            else
                options.HAlign = Const.DefaultHorizontalAlign;

            StyleValue<OverflowAction> action;
            bool hasaction;

            if (this.TryGetValue(StyleKeys.OverflowActionKey, out action))
            {
                options.OverflowAction = action.Value;
                hasaction = true;
            }
            else
            {
                hasaction = false;
            }

            StyleValue<OverflowSplit> split;
            if (this.TryGetValue(StyleKeys.OverflowSplitKey, out split))
                options.OverflowSplit = split.Value;
            else
                options.OverflowSplit = OverflowSplit.Any;

            PDFThickness thickness;

            if (hasaction == false || options.OverflowAction == OverflowAction.Clip)
            {
                //If there is no explicit overflow action or the overflow action is clip
                //Then let's check the clipping rect. If set then we should store the thichness (and set the action to Clip if not already done so).

                if (this.TryGetThickness(StyleKeys.ClipItemKey.Inherited, StyleKeys.ClipAllKey, StyleKeys.ClipTopKey, StyleKeys.ClipLeftKey, StyleKeys.ClipBottomKey, StyleKeys.ClipRightKey, out thickness))
                {
                    options.ClipInset = thickness;

                    //If the overflow action has not been set, but we have a clipping value, 
                    //then we need to set the action to Clip.
                    if (!hasaction)
                        options.OverflowAction = OverflowAction.Clip;
                }
                else
                    options.ClipInset = PDFThickness.Empty();
            }

            if (this.TryGetThickness(StyleKeys.MarginsItemKey.Inherited, StyleKeys.MarginsAllKey, StyleKeys.MarginsTopKey, StyleKeys.MarginsLeftKey, StyleKeys.MarginsBottomKey, StyleKeys.MarginsRightKey, out thickness))
                options.Margins = thickness;
            else
                options.Margins = PDFThickness.Empty();

            if (this.TryGetThickness(StyleKeys.PaddingItemKey.Inherited, StyleKeys.PaddingAllKey, StyleKeys.PaddingTopKey, StyleKeys.PaddingLeftKey, StyleKeys.PaddingBottomKey, StyleKeys.PaddingRightKey, out thickness))
                options.Padding = thickness;
            else
                options.Padding = PDFThickness.Empty();

            StyleValue<int> colcount;
            if (this.TryGetValue(StyleKeys.ColumnCountKey, out colcount) && colcount.Value > 0)
                options.ColumnCount = colcount.Value;

            if (this.TryGetValue(StyleKeys.ColumnAlleyKey, out unit))
                options.AlleyWidth = unit.Value;


            // get transformations

            PDFTransformationMatrix transform = null;

            if (this.IsValueDefined(StyleKeys.TransformXOffsetKey) || this.IsValueDefined(StyleKeys.TransformYOffsetKey))
            {
                if (null == transform)
                    transform = new PDFTransformationMatrix();
                transform.SetTranslation(this.GetValue(StyleKeys.TransformXOffsetKey, 0.0F), this.GetValue(StyleKeys.TransformYOffsetKey, 0.0F));

                if (options.PositionMode != PositionMode.Absolute)
                    options.PositionMode = PositionMode.Relative;
            }

            if (this.IsValueDefined(StyleKeys.TransformRotateKey))
            {
                if (null == transform)
                    transform = new PDFTransformationMatrix();
                transform.SetRotation(this.GetValue(StyleKeys.TransformRotateKey, 0.0F));

                if (options.PositionMode != PositionMode.Absolute)
                    options.PositionMode = PositionMode.Relative;
            }

            if (this.IsValueDefined(StyleKeys.TransformXScaleKey) || this.IsValueDefined(StyleKeys.TransformYScaleKey))
            {
                if (null == transform)
                    transform = new PDFTransformationMatrix();
                transform.SetScale(this.GetValue(StyleKeys.TransformXScaleKey, 0.0F), this.GetValue(StyleKeys.TransformYScaleKey, 0.0F));

                if (options.PositionMode != PositionMode.Absolute)
                    options.PositionMode = PositionMode.Relative;
            }

            if (this.IsValueDefined(StyleKeys.TransformXSkewKey) || this.IsValueDefined(StyleKeys.TransformYSkewKey))
            {
                if (null == transform)
                    transform = new PDFTransformationMatrix();

                transform.SetSkew(this.GetValue(StyleKeys.TransformXSkewKey, 0.0F), this.GetValue(StyleKeys.TransformYSkewKey, 0.0F));

                if (options.PositionMode != PositionMode.Absolute)
                    options.PositionMode = PositionMode.Relative;
            }

            

            options.TransformMatrix = transform;
            
            //if(null != transform && this.IsValueDefined(PDFStyleKeys.TransformOriginKey))
            //{
            //    options.TransformationOrigin = this.GetValue(PDFStyleKeys.TransformOriginKey, TransformationOrigin.CenterMiddle);
            //}
            

            //if (this.TryGetValue(PDFColumnsStyle.ColumnFlowKey, out b))
            //    options.FlowColumns = b.Value;

            return options;
        }

        #endregion

        #region internal protected virtual PDFFont DoCreateFont()

        /// <summary>
        /// Creates a PDFFont from this styles value
        /// </summary>
        /// <param name="force">If true, then even if there are no values in the style, a default font will be returned.</param>
        /// <returns></returns>
        internal protected virtual PDFFont DoCreateFont(bool force)
        {
            bool hasvalues = false;
            StyleValue<string> familyVal;
            StyleValue<PDFUnit> sizeVal;
            StyleValue<bool> boldVal, italicVal;

            string family;
            PDFUnit size;
            Drawing.FontStyle style = Drawing.FontStyle.Regular;

            if (this.TryGetValue(StyleKeys.FontFamilyKey, out familyVal) && !string.IsNullOrEmpty(familyVal.Value))
            {
                family = familyVal.Value;
                hasvalues = true;
            }
            else
                family = Const.DefaultFontFamily;

            if (this.TryGetValue(StyleKeys.FontSizeKey, out sizeVal))
            {
                size = sizeVal.Value;
                hasvalues = true;
            }
            else
                size = new PDFUnit(Const.DefaultFontSize, PageUnits.Points);

            if (this.TryGetValue(StyleKeys.FontBoldKey, out boldVal) && boldVal.Value)
            {
                hasvalues = true;
                style |= Drawing.FontStyle.Bold;
            }

            if (this.TryGetValue(StyleKeys.FontItalicKey, out italicVal) && italicVal.Value)
            {
                hasvalues = true;
                style |= Drawing.FontStyle.Italic;
            }

            if (force || hasvalues)
            {
                PDFFont font = new PDFFont(family, size, style);
                return font;
            }
            else
                return null;
            

        }

        #endregion

        #region internal protected virtual PDFPageNumberOptions DoCreatePageNumberOptions()

        /// <summary>
        /// Creates the PDFPageNumberOptions from this styles values
        /// </summary>
        /// <returns></returns>
        internal protected virtual PDFPageNumberOptions DoCreatePageNumberOptions()
        {
            PDFPageNumberOptions opts = new PDFPageNumberOptions();
            StyleValue<PageNumberStyle> style;
            StyleValue<string> grp;
            StyleValue<int> start;
            StyleValue<string> format;
            StyleValue<int> grphint;
            StyleValue<int> totalhint;

            bool hasvalues = false;

            if(this.TryGetValue(StyleKeys.PageNumberStyleKey,out style))
            {
                opts.NumberStyle = style.Value;
                hasvalues = true;
            }
            if (this.TryGetValue(StyleKeys.PageNumberGroupKey, out grp))
            {
                opts.NumberGroup = grp.Value;
                hasvalues = true;
            }
            if (this.TryGetValue(StyleKeys.PageNumberStartKey, out start))
            {
                opts.StartIndex = start.Value;
                hasvalues = true;
            }
            if (this.TryGetValue(StyleKeys.PageNumberFormatKey, out format))
            {
                opts.Format = format.Value;
                hasvalues = true;
            }
            if (this.TryGetValue(StyleKeys.PageNumberGroupHintKey, out grphint))
            {
                opts.GroupCountHint = grphint.Value;
                hasvalues = true;
            }
            if (this.TryGetValue(StyleKeys.PageNumberTotalHintKey, out totalhint))
            {
                opts.TotalCountHint = totalhint.Value;
                hasvalues = true;
            }
            opts.HasPageNumbering = hasvalues;
            return opts;
        }

        #endregion

        #region internal protected virtual PDFTextRenderOptions DoCreateTextOptions()

        /// <summary>
        /// Implementation of the PDFTextRenderOptions creation. Inheritors can override
        /// </summary>
        /// <returns></returns>
        internal protected virtual PDFTextRenderOptions DoCreateTextOptions()
        {
            PDFTextRenderOptions options = new PDFTextRenderOptions();

            options.Font = this.DoCreateFont(false);
            options.FillBrush = this.DoCreateFillBrush();
            options.Background = this.DoCreateBackgroundBrush();
            options.Stroke = this.DoCreateStrokePen();

            StyleValue<PDFUnit> flindent;
            if (this.TryGetValue(StyleKeys.TextFirstLineIndentKey,out flindent))
                options.FirstLineInset = flindent.Value;

            StyleValue<PDFUnit> space;
            if (this.TryGetValue(StyleKeys.TextWordSpacingKey, out space))
                options.WordSpacing = space.Value;

            if (this.TryGetValue(StyleKeys.TextCharSpacingKey, out space))
                options.CharacterSpacing = space.Value;

            StyleValue<Text.WordWrap> wrap;
            if (this.TryGetValue(StyleKeys.TextWordWrapKey, out wrap))
                options.WrapText = wrap.Value;

            StyleValue<double> hscale;
            if (this.TryGetValue(StyleKeys.TextHorizontalScaling, out hscale))
                options.CharacterHScale = hscale.Value;

            StyleValue<Scryber.TextDirection> dir;
            if (this.TryGetValue(StyleKeys.TextDirectionKey, out dir))
                options.TextDirection = dir.Value;

            StyleValue<PDFUnit> lead;
            if (this.TryGetValue(StyleKeys.TextLeadingKey, out lead))
                options.Leading = lead.Value;

            StyleValue<Text.TextDecoration> decor;
            if (this.TryGetValue(StyleKeys.TextDecorationKey, out decor))
                options.TextDecoration = decor.Value;

            return options;
        }

        #endregion

        internal protected virtual PDFColumnOptions DoCreateColumnOptions()
        {
            return new PDFColumnOptions()
            {
                AlleyWidth = this.GetValue(StyleKeys.ColumnAlleyKey, ColumnsStyle.DefaultAlleyWidth),
                ColumnCount = this.GetValue(StyleKeys.ColumnCountKey, 1),
                ColumnWidths = this.GetValue(StyleKeys.ColumnWidthKey,PDFColumnWidths.Empty),
                AutoFlow = this.GetValue(StyleKeys.ColumnFlowKey, ColumnsStyle.DefaultAutoFlow)
            };
        }
        private static PDFDash DefaultDash = new PDFDash(new int[] { 4 }, 0);
        private static PDFUnit DefaultWidth = new PDFUnit(1, PageUnits.Points);

        /// <summary>
        /// The size of the x step if it is not repeated in the x direction
        /// </summary>
        public static readonly PDFUnit NoXRepeatStepSize = int.MaxValue;

        /// <summary>
        /// The size of the y step if it is not repeated in the y direction
        /// </summary>
        public static readonly PDFUnit NoYRepeatStepSize = int.MaxValue;

        /// <summary>
        /// The size value if repeating should use the natural zise of the pattern or image
        /// </summary>
        public static readonly PDFUnit RepeatNaturalSize = 0;

        #region internal protected virtual PDFPen DoCreateBorderPen()

        internal protected virtual PDFPen DoCreateBorderPen()
        {
            
            PDFPen pen;

            StyleValue<LineType> penstyle;
            StyleValue<PDFDash> dash;
            StyleValue<PDFColor> c;
            StyleValue<PDFUnit> width;

            if (this.TryGetValue(StyleKeys.BorderStyleKey, out penstyle))
            {
                //We have an explict style

                if (penstyle.Value == LineType.None)
                    return null;

                //Check the color and width

                if (this.TryGetValue(StyleKeys.BorderColorKey, out c) && c.Value == PDFColors.Transparent)
                    return null;

                if (this.TryGetValue(StyleKeys.BorderWidthKey, out width) && width.Value <= 0)
                    return null;

                //create a pen based on the explicit style

                if (penstyle.Value == LineType.Solid)
                {
                    pen = new PDFSolidPen() { Color = (null == c)? PDFColors.Black : c.Value, Width = (null == width) ? 1.0 : width.Value  };
                }
                else if (penstyle.Value == LineType.Dash)
                {
                    if (this.TryGetValue(StyleKeys.BorderDashKey, out dash))
                        pen = new PDFDashPen(dash.Value) { 
                            Color = (null == c) ? PDFColors.Black : c.Value,
                            Width = (null == width) ? 1.0 : width.Value };
                    else // set as Dash, but there is none so use default
                        pen = new PDFDashPen(DefaultDash) { 
                            Color = (null == c) ? PDFColors.Black : c.Value,
                            Width = (null == width) ? 1.0 : width.Value };
                }
                else
                    throw new IndexOutOfRangeException(StyleKeys.BorderStyleKey.ToString());

            }
            else if (this.TryGetValue(StyleKeys.BorderDashKey, out dash))
            {
                if (this.TryGetValue(StyleKeys.BorderColorKey, out c) && c.Value == PDFColors.Transparent)
                    return null;
                if (this.TryGetValue(StyleKeys.BorderWidthKey, out width) && width.Value <= 0)
                    return null;

                pen = new PDFDashPen(dash.Value)
                {
                    Color = (null == c) ? PDFColors.Black : c.Value,
                    Width = (null == width) ? 1.0 : width.Value
                };
            
            }
            else if (this.TryGetValue(StyleKeys.BorderColorKey, out c))
            {
                if (c.Value == PDFColors.Transparent)
                    return null;

                if (this.TryGetValue(StyleKeys.BorderWidthKey, out width) && width.Value <= 0)
                    return null;


                pen = new PDFSolidPen()
                {
                    Color = (null == c) ? PDFColors.Black : c.Value,
                    Width = (null == width) ? DefaultWidth : width.Value
                };
            }
            else if (this.TryGetValue(StyleKeys.BorderWidthKey, out width)) //just width set
            {
                if (width.Value <= 0)
                    return null;

                pen = new PDFSolidPen() { Color = PDFColors.Black, Width = width.Value };
            }
            else //no values set
                return null;

            StyleValue<LineJoin> join;
            StyleValue<LineCaps> caps;
            StyleValue<float> mitre;
            StyleValue<double> opacity;

            if (this.TryGetValue(StyleKeys.BorderJoinKey, out join))
                pen.LineJoin = join.Value;

            if (this.TryGetValue(StyleKeys.BorderEndingKey, out caps))
                pen.LineCaps = caps.Value;

            if (this.TryGetValue(StyleKeys.BorderMitreKey, out mitre))
                pen.MitreLimit = mitre.Value;

            if (this.TryGetValue(StyleKeys.BorderOpacityKey, out opacity))
                pen.Opacity = (Scryber.Native.PDFReal)opacity.Value;

            return pen;
        }

        #endregion

        #region internal protected virtual PDFPen DoCreateStrokePen()

        internal protected virtual PDFPen DoCreateStrokePen()
        {
            PDFPen pen;

            StyleValue<LineType> penstyle;
            StyleValue<PDFDash> dash;
            StyleValue<PDFColor> c;
            StyleValue<PDFUnit> width;

            if (this.TryGetValue(StyleKeys.StrokeStyleKey, out penstyle))
            {
                //We have an explict style

                if (penstyle.Value == LineType.None)
                    return null;

                //Check the color and width

                if (this.TryGetValue(StyleKeys.StrokeColorKey, out c) && c.Value == PDFColors.Transparent)
                    return null;

                if (this.TryGetValue(StyleKeys.StrokeWidthKey, out width) && width.Value <= 0)
                    return null;

                //create a pen based on the explicit style

                if (penstyle.Value == LineType.Solid)
                {
                    pen = new PDFSolidPen() { Color = (null == c) ? PDFColors.Black : c.Value, Width = (null == width) ? 1.0 : width.Value };
                }
                else if (penstyle.Value == LineType.Dash)
                {
                    if (this.TryGetValue(StyleKeys.StrokeDashKey, out dash))
                        pen = new PDFDashPen(dash.Value)
                        {
                            Color = (null == c) ? PDFColors.Black : c.Value,
                            Width = (null == width) ? 1.0 : width.Value
                        };
                    else // set as Dash, but there is none so use default
                        pen = new PDFDashPen(DefaultDash)
                        {
                            Color = (null == c) ? PDFColors.Black : c.Value,
                            Width = (null == width) ? 1.0 : width.Value
                        };
                }
                else
                    throw new IndexOutOfRangeException(StyleKeys.StrokeStyleKey.ToString());

            }
            else if (this.TryGetValue(StyleKeys.StrokeDashKey, out dash))
            {
                if (this.TryGetValue(StyleKeys.StrokeColorKey, out c) && c.Value == PDFColors.Transparent)
                    return null;
                if (this.TryGetValue(StyleKeys.StrokeWidthKey, out width) && width.Value <= 0)
                    return null;

                pen = new PDFDashPen(dash.Value)
                {
                    Color = (null == c) ? PDFColors.Black : c.Value,
                    Width = (null == width) ? 1.0 : width.Value
                };
            }
            else if (this.TryGetValue(StyleKeys.StrokeColorKey, out c))
            {
                if (c.Value == PDFColors.Transparent)
                    return null;
                if (this.TryGetValue(StyleKeys.StrokeWidthKey, out width) && width.Value <= 0)
                    return null;

                pen = new PDFSolidPen()
                {
                    Color = c.Value,
                    Width = (null == width) ? DefaultWidth : width.Value
                };
            }
            else if (this.TryGetValue(StyleKeys.StrokeWidthKey, out width)) //just width set
            {
                if (width.Value <= 0)
                    return null;

                pen = new PDFSolidPen()
                {
                    Color = (null == c) ? PDFColors.Black : c.Value,
                    Width = (null == width) ? DefaultWidth : width.Value
                };
            }
            else //no values set
                return null;

            StyleValue<LineJoin> join;
            StyleValue<LineCaps> caps;
            StyleValue<float> mitre;
            StyleValue<double> opacity;

            if (this.TryGetValue(StyleKeys.StrokeJoinKey, out join))
                pen.LineJoin = join.Value;

            if (this.TryGetValue(StyleKeys.StrokeEndingKey, out caps))
                pen.LineCaps = caps.Value;

            if (this.TryGetValue(StyleKeys.StrokeMitreKey, out mitre))
                pen.MitreLimit = mitre.Value;

            if (this.TryGetValue(StyleKeys.StrokeOpacityKey, out opacity))
                pen.Opacity = (Scryber.Native.PDFReal)opacity.Value;

            return pen;
        }

        #endregion

        #region internal protected virtual PDFBrush DoCreateBackgroundBrush()

        /// <summary>
        /// Creates a background brush based on this styles values
        /// </summary>
        /// <returns></returns>
        internal protected virtual PDFBrush DoCreateBackgroundBrush()
        {

            StyleValue<Drawing.FillType> fillstyle;
            StyleValue<string> imgsrc;
            StyleValue<double> opacity;
            StyleValue<PDFColor> color;
            if (this.TryGetValue(StyleKeys.BgStyleKey, out fillstyle))
            {
                if (fillstyle.Value == Drawing.FillType.None)
                    return null;
            }

            PDFBrush brush = null;

            if ((this).TryGetValue(StyleKeys.BgColorKey, out color) && (fillstyle == null || fillstyle.Value == Drawing.FillType.Solid))
            {
                PDFSolidBrush solid = new PDFSolidBrush(color.Value);

                //if we have an opacity set it.
                if ((this).TryGetValue(StyleKeys.BgOpacityKey, out opacity))
                    solid.Opacity = opacity.Value;

                brush = solid;
            }

            //If we have an image source and we are set to use an image fill style (or it has not been specified)
            if ((this).TryGetValue(StyleKeys.BgImgSrcKey, out imgsrc) && !string.IsNullOrEmpty(imgsrc.Value) && (fillstyle == null || fillstyle.Value == Drawing.FillType.Image))
            {
                PatternRepeat repeat = PatternRepeat.RepeatBoth;
                StyleValue<PatternRepeat> repeatValue;

                if ((this).TryGetValue(StyleKeys.BgRepeatKey, out repeatValue))
                    repeat = repeatValue.Value;

                if (repeat == PatternRepeat.Fill)
                {
                    PDFFullImageBrush full = new PDFFullImageBrush(imgsrc.Value);
                    if ((this).TryGetValue(StyleKeys.BgOpacityKey, out opacity))
                        full.Opacity = opacity.Value;

                    if (null != brush)
                        full.UnderBrush = brush;

                    brush = full;

                }
                else
                {
                    PDFImageBrush img = new PDFImageBrush(imgsrc.Value);
                    StyleValue<PDFUnit> unitValue;

                    if (repeat == PatternRepeat.RepeatX || repeat == PatternRepeat.RepeatBoth)
                        img.XStep = (this).TryGetValue(StyleKeys.BgXStepKey, out unitValue) ? unitValue.Value : RepeatNaturalSize;
                    else
                        img.XStep = NoXRepeatStepSize;

                    if (repeat == PatternRepeat.RepeatY || repeat == PatternRepeat.RepeatBoth)
                        img.YStep = (this).TryGetValue(StyleKeys.BgYStepKey, out unitValue) ? unitValue.Value : RepeatNaturalSize;
                    else
                        img.YStep = NoYRepeatStepSize;

                    img.XPostion = (this).TryGetValue(StyleKeys.BgXPosKey, out unitValue) ? unitValue.Value : PDFUnit.Zero;
                    img.YPostion = (this).TryGetValue(StyleKeys.BgYPosKey, out unitValue) ? unitValue.Value : PDFUnit.Zero;

                    if ((this).TryGetValue(StyleKeys.BgXSizeKey, out unitValue))
                        img.XSize = unitValue.Value;

                    if ((this).TryGetValue(StyleKeys.BgYSizeKey, out unitValue))
                        img.YSize = unitValue.Value;

                    if ((this).TryGetValue(StyleKeys.BgOpacityKey, out opacity))
                        img.Opacity = opacity.Value;

                    if (null != brush)
                        img.UnderBrush = brush;

                    brush = img;
                }
                
            }

            return brush;
        }

        #endregion

        #region internal protected virtual PDFBrush DoCreateFillBrush()

        /// <summary>
        /// Creates a fill brush
        /// </summary>
        /// <returns></returns>
        internal protected virtual PDFBrush DoCreateFillBrush()
        {
            StyleValue<Drawing.FillType> fillstyle;

            StyleValue<string> imgsrc;
            StyleValue<double> opacity;
            StyleValue<PDFColor> color;

            if (this.TryGetValue(StyleKeys.FillStyleKey, out fillstyle))
            {
                if (fillstyle.Value == Drawing.FillType.None)
                    return null;
            }

            //If we have an image source and we are set to use an image fill style (or it has not been specified)
            if ((this).TryGetValue(StyleKeys.FillImgSrcKey, out imgsrc) && !string.IsNullOrEmpty(imgsrc.Value) && (fillstyle == null || fillstyle.Value == Drawing.FillType.Image))
            {
                PatternRepeat repeat = PatternRepeat.RepeatBoth;
                StyleValue<PatternRepeat> repeatValue;

                if ((this).TryGetValue(StyleKeys.FillRepeatKey, out repeatValue))
                    repeat = repeatValue.Value;

                PDFBrush brush;
                if (repeat == PatternRepeat.Fill)
                {
                    PDFFullImageBrush full = new PDFFullImageBrush(imgsrc.Value);
                    if ((this).TryGetValue(StyleKeys.FillOpacityKey, out opacity))
                        full.Opacity = opacity.Value;

                    brush = full;
                }
                else
                {
                    PDFImageBrush img = new PDFImageBrush(imgsrc.Value);
                    StyleValue<PDFUnit> unitValue;

                    if (repeat == PatternRepeat.RepeatX || repeat == PatternRepeat.RepeatBoth)
                        img.XStep = (this).TryGetValue(StyleKeys.FillXStepKey, out unitValue) ? unitValue.Value : RepeatNaturalSize;
                    else
                        img.XStep = NoXRepeatStepSize;

                    if (repeat == PatternRepeat.RepeatY || repeat == PatternRepeat.RepeatBoth)
                        img.YStep = (this).TryGetValue(StyleKeys.FillYStepKey, out unitValue) ? unitValue.Value : RepeatNaturalSize;
                    else
                        img.YStep = NoYRepeatStepSize;

                    img.XPostion = (this).TryGetValue(StyleKeys.FillXPosKey, out unitValue) ? unitValue.Value : PDFUnit.Zero;
                    img.YPostion = (this).TryGetValue(StyleKeys.FillYPosKey, out unitValue) ? unitValue.Value : PDFUnit.Zero;

                    if ((this).TryGetValue(StyleKeys.FillXSizeKey, out unitValue))
                        img.XSize = unitValue.Value;

                    if ((this).TryGetValue(StyleKeys.FillYSizeKey, out unitValue))
                        img.YSize = unitValue.Value;

                    if ((this).TryGetValue(StyleKeys.FillOpacityKey, out opacity))
                        img.Opacity = opacity.Value;

                    brush = img;
                }
                return brush;
            }

            //if we have a colour and a solid style (or no style)
            else if ((this).TryGetValue(StyleKeys.FillColorKey, out color) && (fillstyle == null || fillstyle.Value == Drawing.FillType.Solid))
            {
                PDFSolidBrush solid = new PDFSolidBrush(color.Value);

                //if we have an opacity set it.
                if ((this).TryGetValue(StyleKeys.FillOpacityKey, out opacity))
                    solid.Opacity = opacity.Value;

                return solid;
            }
            else
                return null;
        }

        #endregion

        #region internal protected virtual PDFThickness DoCreateMarginsThickness()

        internal protected virtual PDFThickness DoCreateMarginsThickness()
        {
            PDFThickness thickness;
            if (this.TryGetThickness(StyleKeys.MarginsItemKey.Inherited, StyleKeys.MarginsAllKey, StyleKeys.MarginsTopKey, StyleKeys.MarginsLeftKey, StyleKeys.MarginsBottomKey, StyleKeys.MarginsRightKey, out thickness))
                return thickness;
            else
                return PDFThickness.Empty();
        }

        #endregion

        #region internal protected virtual PDFThickness DoCreatePaddingThickness()

        internal protected virtual PDFThickness DoCreatePaddingThickness()
        {
            PDFThickness thickness;
            if (this.TryGetThickness(StyleKeys.PaddingItemKey.Inherited, StyleKeys.PaddingAllKey, StyleKeys.PaddingTopKey, StyleKeys.PaddingLeftKey, StyleKeys.PaddingBottomKey, StyleKeys.PaddingRightKey, out thickness))
                return thickness;
            else
                return PDFThickness.Empty();
        }

        #endregion

        #region internal protected virtual PDFThickness DoCreateClippingThickness()

        internal protected virtual PDFThickness DoCreateClippingThickness()
        {
            PDFThickness thickness;
            if(this.TryGetThickness(StyleKeys.ClipItemKey.Inherited, StyleKeys.ClipAllKey, StyleKeys.ClipTopKey, StyleKeys.ClipLeftKey, StyleKeys.ClipBottomKey, StyleKeys.ClipRightKey, out thickness))
                return thickness;
            else
                return PDFThickness.Empty();
        }

        #endregion

        #region internal protected virtual PDFPen DoCreateOverlayGridPen()

        internal protected virtual PDFPen DoCreateOverlayGridPen()
        {
            StyleValue<bool> show;
            StyleValue<PDFColor> color;
            StyleValue<double> opacity;
            if(this.TryGetValue(StyleKeys.OverlayShowGridKey,out show) && show.Value)
            {
                if (this.TryGetValue(StyleKeys.OverlayColorKey, out color) == false)
                    color = new StyleValue<PDFColor>(StyleKeys.OverlayColorKey, OverlayGridStyle.DefaultGridColor);
                if (this.TryGetValue(StyleKeys.OverlayOpacityKey, out opacity) == false)
                    opacity = new StyleValue<double>(StyleKeys.OverlayOpacityKey, OverlayGridStyle.DefaultGridOpacity);

                PDFPen solid = new PDFSolidPen(color.Value, OverlayGridStyle.DefaultGridPenWidth) { Opacity = opacity.Value };
                return solid;
            }
            return null;
        }

        #endregion
    }

    public static class PDFStyleBaseExtensions
    {
        public static T GetValue<T>(this StyleBase stylebase, PDFStyleKey<T> key, T defaultValue)
        {
            PDFStyleValueBase exist;
            StyleValue<T> found;
            if (stylebase.TryGetBaseValue(key, out exist))
            {
                found = (StyleValue<T>)exist;
                return found.Value;
            }
            else
                return defaultValue;
        }

        public static void SetValue<T>(this StyleBase stylebase, PDFStyleKey<T> key, T value)
        {
            StyleValue<T> match;
            if(stylebase.TryGetValue(key,out match))
            {
                match.SetValue(value);
            }
            else
            {
                match = new StyleValue<T>(key, value);
                stylebase.AddValue(match);
            }
        }

        public static void SetMargins(this StyleBase stylebase, PDFThickness thickness)
        {
            stylebase.SetThickness(StyleKeys.MarginsItemKey.Inherited, thickness, StyleKeys.MarginsTopKey, StyleKeys.MarginsLeftKey, StyleKeys.MarginsBottomKey, StyleKeys.MarginsRightKey);
        }


        public static void SetPadding(this StyleBase stylebase, PDFThickness thickness)
        {
            stylebase.SetThickness(StyleKeys.PaddingItemKey.Inherited, thickness, StyleKeys.PaddingTopKey, StyleKeys.PaddingLeftKey, StyleKeys.PaddingBottomKey, StyleKeys.PaddingRightKey);
        }


        public static bool TryGetItem<T>(this StyleBase stylebase, StyleKey itemkey, out T found) where T : StyleItemBase
        {
            StyleItemBase exist;
            if (stylebase.TryGetBaseItem(itemkey, out exist))
            {
                found = (T)exist;
                return true;
            }
            else
            {
                found = null;
                return false;
            }
        }

        public static bool TryGetValue<T>(this StyleBase stylebase, PDFStyleKey<T> key, out StyleValue<T> found)
        {
            PDFStyleValueBase exist;
            if (stylebase.TryGetBaseValue(key, out exist))
            {
                found = (StyleValue<T>)exist;
                return true;
            }
            else
            {
                found = null;
                return false;
            }
        }

        public static T GetOrCreateItem<T>(this StyleBase stylebase, StyleKey key) where T : StyleItemBase, new()
        {
            T found;
            if (stylebase.TryGetItem(key, out found) == false)
            {
                found = new T();
                stylebase.AddItem(found);
            }
            return found;
        }
    }
}