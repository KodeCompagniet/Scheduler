using System;
using System.Collections.Generic;
using System.Configuration;

namespace DemiCode.Configuration
{
    /// <summary>
    /// This class simplifies implementing <see cref="ConfigurationElementCollection"/> classes that also have <see cref="ConfigurationProperty"/> properties
    /// in addition to being a collection of <see cref="ConfigurationElement"/> instances.
    /// </summary>
    public abstract class ExtendedConfigurationElementCollection : ConfigurationElementCollection
    {
        ///<summary>
        ///Compares the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to the specified object.
        /// Elements in this collection will be compared to elements in the <paramref name="compareTo"/> collection.
        /// Additionally, any properties defined on this collection will be compared.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare. </param>
        public override bool Equals(object compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            if (compareTo == null) return false;

            var collection = compareTo as ExtendedConfigurationElementCollection;
            if (collection == null) return false;

            if (Count != collection.Count) return false;

            // First, compare any properties on us since ConfigurationElementCollection removes this from its Equals implementation
            foreach (ConfigurationProperty property in Properties)
            {
                if (!collection.Properties.Contains(property.Name))
                    return false;

                var val = base[property.Name];
                var val2 = collection[property.Name];
                if (val == null && val2 == null) continue;

                //TODO: this should be refactored away!
                if (property.Type == typeof(Dictionary<string, string>))
                {
                    var left = val as Dictionary<string, string>;
                    var right = val2 as Dictionary<string, string>;
                    if (left == null && right == null)
                        throw new InvalidOperationException("Property values was not of type " + typeof(Dictionary<string, string>));

                    if ((left != null && right == null) || (left == null && right != null)) return false;
                    if (left.Keys.Count != right.Keys.Count) return false;
                    foreach (var pair in left)
                    {
                        if (!right.ContainsKey(pair.Key)) return false;
                        if (right[pair.Key] != pair.Value) return false;
                    }
                }
                else if (!Equals(val, val2)) return false;
            }

            // Next, let the base collection compare
            return base.Equals(compareTo);
        }

        ///<summary>
        ///Serves as a hash function for this type.
        ///</summary>
        ///<returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = base.GetHashCode();
                hash = (hash * 397) ^ Count;

                foreach (ConfigurationProperty property in Properties)
                {
                    hash = (hash * 397) ^ (property.Name?.GetHashCode() ?? 0);
                    var val = base[property.Name];
                    if (val != null)
                    {
                        if (property.Type == typeof(Dictionary<string, string>))
                        {
                            var dict = val as Dictionary<string, string>;
                            if (dict != null)
                            {
                                foreach (var pair in dict)
                                {
                                    hash = (hash * 397) ^ (pair.Key?.GetHashCode() ?? 0);
                                    hash = (hash * 397) ^ (pair.Value?.GetHashCode() ?? 0);
                                }
                            }
                        }
                        else
                        {
                            hash = (hash * 397) ^ val.GetHashCode();
                        }
                    }
                }

                return hash;
            }
        }
    }
}
