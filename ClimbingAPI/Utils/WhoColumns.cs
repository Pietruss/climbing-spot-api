using System;
using System.Reflection;

namespace ClimbingAPI.Utils
{
    public static class WhoColumns
    {
        public static void CreationFiller(object obj, int? userId, DateTime dateTime)
        {
            var properties = GetObjectProperties(obj);

            foreach (var property in properties)
            {
                SetCreationValues(property, obj, userId, dateTime);
            }
        }

        public static void ModificationFiller(object obj, int? userId, DateTime dateTime)
        {
            var properties = GetObjectProperties(obj);

            foreach (var property in properties)
            {
                SetModificationValues(property, obj, userId, dateTime);
            }
        }

        public static PropertyInfo[] GetObjectProperties(object obj)
        {
            Type objType = obj.GetType();
            return objType.GetProperties();
        }

        public static void SetModificationValues(PropertyInfo property, object obj, int? userId, DateTime dateTime)
        {
            if (property.Name.Equals("ModifiedByUserId"))
            {
                property.SetValue(obj, userId.ToString());
            }
            if (property.Name.Equals("ModificationDateTime"))
            {
                property.SetValue(obj, dateTime);
            }
        }

        public static void SetCreationValues(PropertyInfo property, object obj, int? userId, DateTime dateTime)
        {
            if (property.Name.Equals("CreatedByUserId"))
            {
                property.SetValue(obj, userId.ToString());
            }
            if (property.Name.Equals("CreationDateTime"))
            {
                property.SetValue(obj, dateTime);
            }
        }
    }
}
