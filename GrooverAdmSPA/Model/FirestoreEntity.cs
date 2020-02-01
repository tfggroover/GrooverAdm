using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{

    public abstract class FirestoreEntity
    {

    }
    public abstract class FirestoreEntity<T> : FirestoreEntity where T : FirestoreEntity<T>, new()
    {
        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();
            var props = this.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    result[prop.Name.ToLower()] = prop.GetValue(this);
                }
                else if (typeof(IEnumerable<object>).IsAssignableFrom(prop.PropertyType))
                {
                    var type = prop.GetValue(this).GetType().GetTypeInfo().GenericTypeArguments[0];
                    if (type.IsPrimitive || type == typeof(string))
                        result[prop.Name.ToLower()] = prop.GetValue(this);
                    else
                    {
                        var cast = prop.GetValue(this) as IEnumerable<object>;
                        var method = type.GetMethod("ToDictionary");
                        result[prop.Name.ToLower()] = cast.Select(item =>
                        {
                            return method.Invoke(item, new object[] { });
                        }).ToList();
                    }
                }
                else if (prop.PropertyType == typeof(GeoPoint))
                {
                    var cast = prop.GetValue(this) as GeoPoint?;
                    result[prop.Name.ToLower()] = cast.Value;
                }
                else if (typeof(Dictionary<, >).IsAssignableFrom(prop.PropertyType))
                {
                    result[prop.Name.ToLower()] = prop.GetValue(this);
                }
                else
                {
                    var type = prop.PropertyType;
                    var cast = prop.GetValue(this);
                    if (cast != null && type.GetMethod("ToDictionary") != null)
                        result[prop.Name.ToLower()] = type.GetMethod("ToDictionary").Invoke(cast, new object[] { });
                }
            }

            return result;
        }

        public static T FromDictionary(IDictionary<string, object> map)
        {

            var result = new T();
            var props = result.GetType().GetProperties();
            foreach (var prop in props)
            {
                if (map.ContainsKey(prop.Name.ToLower()))
                    if (map[prop.Name.ToLower()]?.GetType() == prop.PropertyType)
                        prop.SetValue(result, map[prop.Name.ToLower()]);
                    else if (map[prop.Name.ToLower()]?.GetType() == typeof(Dictionary<string, object>)) //We have an object inside
                    {
                        if (prop.PropertyType.GetMethod("FromDictionary", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy) != null)
                        {
                            var obj = prop.PropertyType.GetMethod("FromDictionary", BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy).Invoke(null, new object[] { map[prop.Name.ToLower()] });
                            prop.SetValue(result, obj);
                        }
                    }
                    else if (map[prop.Name.ToLower()]?.GetType() == typeof(List<object>))
                    {
                        if(map[prop.Name.ToLower()] != null)
                        {

                        }
                    }
            }

            return result;
        }

    }

    public class FirestoreEntityConverter<T> : IFirestoreConverter<T> where T : FirestoreEntity<T>, new()
    {
        public T FromFirestore(object value)
        {
            if (value is IDictionary<string, object> map)
            {
                var result = FirestoreEntity<T>.FromDictionary(map);
                return result;

            }
            throw new ArgumentException($"Unexpected data: {value.GetType()}");
        }

        public object ToFirestore(T value) => value.ToDictionary();

    }

    public class FirestoreEntityListConverter<T> : IFirestoreConverter<List<T>> where T : FirestoreEntity<T>, new()
    {
        public List<T> FromFirestore(object value)
        {
            if (value is List<IDictionary<string, object>> list)
            {
                var result = new List<T>();
                var props = typeof(T).GetProperties();

                foreach (var map in list)
                {
                    var item = new T();
                    foreach (var prop in props)
                    {
                        if (map.ContainsKey(prop.Name.ToLower()))
                            prop.SetValue(item, map[prop.Name.ToLower()]);
                    }
                    result.Add(item);
                }
                return result;
            }
            throw new ArgumentException($"Unexpected data: {value.GetType()}");
        }

        public object ToFirestore(List<T> value) => value.Select(t => t.ToDictionary());
    }
}
