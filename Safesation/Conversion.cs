using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Safesation
{
    [DebuggerDisplay("{DebuggerDisplay}")]
    public struct Conversion
    {
        #region CONSTRUCTORS

        [DebuggerStepThrough]
        public Conversion(Type source, Type target)
        {
            _hash = 0;
            _category = null;
            _features = ConversionFeatures.None;
            _source = source;
            _target = target;
            unchecked
            {
                if (source != null)
                {
                    _hash = _hash * 17 + source.GetHashCode();
                }
                if (target != null)
                {
                    _hash = _hash * 19 + target.GetHashCode();
                }
            }
        }

        [DebuggerStepThrough]
        public Conversion(ConversionFeatures features, Type source, Type target)
        {
            _hash = 0;
            _category = null;
            _features = features;
            _source = source;
            _target = target;
            unchecked
            {
                if (source != null)
                {
                    _hash = _hash * 17 + source.GetHashCode();
                }
                if (target != null)
                {
                    _hash = _hash * 19 + target.GetHashCode();
                }
                if (features != ConversionFeatures.None)
                {
                    _hash = _hash * 29 + (int)features;
                }
            }
        }

        [DebuggerStepThrough]
        public Conversion(string category, Type source, Type target)
        {
            _hash = 0;
            _category = category;
            _features = ConversionFeatures.None;
            _source = source;
            _target = target;
            unchecked
            {
                if (source != null)
                {
                    _hash = _hash * 17 + source.GetHashCode();
                }
                if (target != null)
                {
                    _hash = _hash * 19 + target.GetHashCode();
                }
                if (category != null)
                {
                    _hash = _hash * 23 + category.GetHashCode();
                }
            }
        }

        [DebuggerStepThrough]
        public Conversion(string category, ConversionFeatures features, Type source, Type target)
        {
            _hash = 0;
            _category = category;
            _features = features;
            _source = source;
            _target = target;
            unchecked
            {
                if (source != null)
                {
                    _hash = _hash*17 + source.GetHashCode();
                }
                if (target != null)
                {
                    _hash = _hash*19 + target.GetHashCode();
                }
                if (category != null)
                {
                    _hash = _hash*23 + category.GetHashCode();
                }
                if (features != ConversionFeatures.None)
                {
                    _hash = _hash*29 + (int) features;
                }
            }
        }

        #endregion

        #region FIELDS

        private readonly string _category;

        private readonly ConversionFeatures _features;

        private readonly int _hash;

        private readonly Type _source;

        private readonly Type _target;

        #endregion

        #region PROPERTIES

        public string Category
        {
            get { return _category; }
        }

        internal string DebuggerDisplay
        {
            get
            {
                var builder = new StringBuilder("Conversion from '")
                    .Append(Source.Namespace).Append(".").Append(Source.Name).Append("' to '")
                    .Append(Target.Namespace).Append(".").Append(Target.Name);
                if (Category != null)
                {
                    builder.Append(", ").Append(Category);
                }
                if (Features != ConversionFeatures.None)
                {
                    builder.Append("' featured with '").Append(Features);
                }
                return builder.Append("'").ToString();
            }
        }

        public ConversionFeatures Features
        {
            get { return _features; }
        }

        public int Hash
        {
            get { return _hash; }
        }

        public Type Source
        {
            get { return _source;}
        }

        public Type Target
        {
            get { return _target; }
        }

        #endregion

        #region METHODS

        public override int GetHashCode()
        {
            return _hash;
        }

        public bool Equals(Conversion other)
        {
            return _source == other._source
                   && _target == other._target
                   && _features == other._features
                   && string.Equals(_category, other._category, StringComparison.OrdinalIgnoreCase);
        }

        #endregion
    }
}
