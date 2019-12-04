using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public abstract class FirestoreEntity<T> where T : FirestoreEntity<T>, new()
    {
        public T GetInstance()
        {
            return new T();
        }
        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();
            var props = this.GetType().GetProperties();

            foreach (var prop in props)
            {
                if (prop.PropertyType.IsPrimitive || prop.PropertyType == typeof(string))
                {
                    result[prop.Name] = prop.GetValue(this);
                }
                else if (prop.PropertyType == typeof(IEnumerable<object>))
                {
                    var type = prop.GetValue(this).GetType().GetTypeInfo().GenericTypeArguments[0];
                    if (type.IsPrimitive || type == typeof(string))
                        result[prop.Name.ToLower()] = prop.GetValue(this);
                    else
                    {
                        var cast = prop.GetValue(this) as IEnumerable<object>;

                        result[prop.Name.ToLower()] = cast.Select(item =>
                        {
                            var castItem = item as FirestoreEntity<T>;
                            return castItem.ToDictionary();
                        }).ToList();
                    }
                }
                else if (prop.PropertyType == typeof(GeoPoint))
                {
                    var cast = prop.GetValue(this) as GeoPoint?;
                    result[prop.Name.ToLower()] = cast.Value;
                }
                else
                {
                    var cast = prop.GetValue(this) as FirestoreEntity<T>;
                    result[prop.Name.ToLower()] = cast.ToDictionary();
                }
            }

            return result;
        }
    }

    public class FirestoreEntityConverter<T> : IFirestoreConverter<T> where T : FirestoreEntity<T>, new()
    {
        public T FromFirestore(object value)
        {
            if(value is IDictionary<string, object> map)
            {
                var result = new T();
                var props = result.GetType().GetProperties();
                foreach (var prop in props)
                {
                    if(map.ContainsKey(prop.Name.ToLower()))
                        prop.SetValue(result, map[prop.Name.ToLower()]);
                }
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
