using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Security.Principal;

namespace Lifetrons.Erp.Data
{
    public static class ExtensionMethods
    {
        public static int WordCount(this String str)
        {
            return str.Split(new char[] { ' ', '.', '?' },
                             StringSplitOptions.RemoveEmptyEntries).Length;
        }

        public static int Factorial(this int x)
        {
            if (x <= 1) return 1;
            if (x == 2) return 2;
            else
                return x * Factorial(x - 1);
        }

        public static Guid ToSysGuid(this string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException("id", "NULL or empty string passed for Guid conversion.");
            }

            Guid retVal = new Guid(id);

            if (retVal == Guid.Empty)
            {
                throw new ArgumentNullException("id", "Empty Guid passed.");
            }

            return retVal;
        }

        public static Guid? ToGuid(this string id)
        {
            Guid? retVal = new Guid(id);
            if (retVal == Guid.Empty)
            {
                retVal = null;
            }

            return retVal;
        }
        public static Guid? ToGuid(this Guid? id)
        {
            Guid? retVal = id;
            if (retVal == Guid.Empty)
            {
                retVal = null;
            }
            return retVal;
        }
        public static Guid ToGuid(this Guid id)
        {
            Guid retVal = id;
            if (retVal == Guid.Empty)
            {
                throw new ArgumentNullException("id", "Empty Guid passed.");
            }
            return retVal;
        }
        public static Guid ToSysGuid(this Guid? id)
        {
            if (id == null)
            {
                throw new ArgumentNullException("id", "NULL Guid passed.");
            }

            Guid retVal = (System.Guid)id;

            if (retVal == Guid.Empty)
            {
                throw new ArgumentNullException("id", "Empty Guid passed.");
            }
            return retVal;
        }

        public static decimal ToJobNumber(this string jobNumber)
        {
            if (string.IsNullOrEmpty(jobNumber) || string.IsNullOrWhiteSpace(jobNumber))
            {
                throw (new ApplicationException("Job Number should be a number. You entered " + jobNumber));
            }

            decimal jobNoDecimal = -1;
            try
            {
                jobNoDecimal = Convert.ToDecimal(jobNumber);
            }
            catch (Exception ex)
            {
                throw (new ApplicationException("Job Number should be a number. You entered " + jobNumber));
            }

            return jobNoDecimal;
        }

        public static DataTable ToDataTableStructure<T>(IEnumerable<T> l_oItems)
        {
            DataTable oReturn = new DataTable(typeof(T).Name);
            object[] a_oValues;
            int i;

            //#### Collect the a_oProperties for the passed T
            PropertyInfo[] a_oProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //#### Traverse each oProperty, .Add'ing each .Name/.BaseType into our oReturn value
            //####     NOTE: The call to .BaseType is required as DataTables/DataSets do not support nullable types, so it's non-nullable counterpart Type is required in the .Column definition
            foreach (PropertyInfo oProperty in a_oProperties)
            {
                oReturn.Columns.Add(oProperty.Name, BaseType(oProperty.PropertyType));
            }

            //#### Return the above determined oReturn value to the caller
            return oReturn;
        }

        /// <summary>
        /// Convert a List to a DataTable.
        /// </summary>
        /// <typeparam name="T">Type representing the type to convert.</typeparam>
        /// <param name="l_oItems">List of requested type representing the values to convert.</param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(IEnumerable<T> l_oItems)
        {
            var oReturn = new DataTable(typeof(T).Name);
            int i;

            //#### Collect the a_oProperties for the passed T
            PropertyInfo[] aOProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            //#### Traverse each oProperty, .Add'ing each .Name/.BaseType into our oReturn value
            //####     NOTE: The call to .BaseType is required as DataTables/DataSets do not support nullable types, so it's non-nullable counterpart Type is required in the .Column definition
            foreach (PropertyInfo oProperty in aOProperties)
            {
                oReturn.Columns.Add(oProperty.Name, BaseType(oProperty.PropertyType));
            }

            //#### Traverse the l_oItems
            foreach (T oItem in l_oItems)
            {
                //#### Collect the a_oValues for this loop
                var aOValues = new object[aOProperties.Length];

                //#### Traverse the a_oProperties, populating each a_oValues as we go
                for (i = 0; i < aOProperties.Length; i++)
                {
                    aOValues[i] = aOProperties[i].GetValue(oItem, null);

                }

                //#### .Add the .Row that represents the current a_oValues into our oReturn value
                oReturn.Rows.Add(aOValues);
            }

            //#### Return the above determined oReturn value to the caller
            return oReturn;
        }


        /// <summary>
        /// Returns the underlying/base type of nullable types.
        /// </summary>
        /// <param name="oType">Type representing the type to query.</param>
        /// <returns>Type representing the underlying/base type.</returns>
        private static Type BaseType(Type oType)
        {
            //#### If the passed oType is valid, .IsValueType and is logicially nullable, .Get(its)UnderlyingType
            if (oType != null && oType.IsValueType &&
                oType.IsGenericType && oType.GetGenericTypeDefinition() == typeof(Nullable<>)
            )
            {
                return Nullable.GetUnderlyingType(oType);
            }
            //#### Else the passed oType was null or was not logicially nullable, so simply return the passed oType
            else
            {
                return oType;
            }
        }

        public static ObservableCollection<T> DeepCopy<T>(IEnumerable<T> list) where T : ICloneable
        {
            return new ObservableCollection<T>(list.Select(x => x.Clone()).Cast<T>());
        }
      

    }
}
