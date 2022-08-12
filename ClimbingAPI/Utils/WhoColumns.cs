using System;
using System.Reflection;

namespace ClimbingAPI.Utils
{
    public class WhoColumns
    {
        public static void CreationFiller(object obj, int? userId)
        {
            var properties = GetObjectProperties(obj);

            foreach (var property in properties)
            {
                SetCreationValues(property, obj, userId);
            }
        }

        public static void ModificationFiller(object obj, int? userId)
        {
            var properties = GetObjectProperties(obj);

            foreach (var property in properties)
            {
                SetModificationValues(property, obj, userId);
            }
        }

        private static PropertyInfo[] GetObjectProperties(object obj)
        {
            Type objType = obj.GetType();
            return objType.GetProperties();
        }

        private static void SetModificationValues(PropertyInfo property, object obj, int? userId)
        {
            if (property.Name.Equals("ModifiedByUserId"))
            {
                property.SetValue(obj, userId.ToString());
            }
            if (property.Name.Equals("ModificationDateTime"))
            {
                property.SetValue(obj, DateTime.Now);
            }
        }

        private static void SetCreationValues(PropertyInfo property, object obj, int? userId)
        {
            if (property.Name.Equals("CreatedByUserId"))
            {
                property.SetValue(obj, userId.ToString());
            }
            if (property.Name.Equals("CreationDateTime"))
            {
                property.SetValue(obj, DateTime.Now);
            }
        }
    }
}
