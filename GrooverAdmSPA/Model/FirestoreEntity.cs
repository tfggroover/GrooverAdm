using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace GrooverAdmSPA.Model
{
    public abstract class FirestoreEntity
    {

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
                        result[prop.Name] = prop.GetValue(this);
                    else
                    {
                        var cast = prop.GetValue(this) as IEnumerable<object>;

                        result[prop.Name] = cast.Select(item =>
                        {
                            var castItem = item as FirestoreEntity;
                            return castItem.ToDictionary();
                        }).ToList();
                    }
                }
                else
                {
                    var cast = prop.GetValue(this) as FirestoreEntity;
                    result[prop.Name] = cast.ToDictionary();
                }
            }

            return result;
        }
    }
}
