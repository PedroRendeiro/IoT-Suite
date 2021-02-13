using System;
using System.Collections;
using System.Linq;

namespace IoTSuite.Shared.Filters
{
    public class PaginationFilter
    {
        private int _PageNumber = 1;
        public int PageNumber
        {
            get => _PageNumber;
            
            set
            {
                _PageNumber = value < 1 ? 1 : value;
            }
        }

        private int _PageSize = 100;
        public int PageSize
        {
            get => _PageSize;

            set
            {
                _PageSize = value < 10 ? 10 : value;
            }
        }

        public Order Order { get; set; }

        public PaginationFilter() { }
        
        public PaginationFilter(int pageNumber = 1, int pageSize = 100, Order order = Order.Asc)
        {
            this.PageNumber = pageNumber;
            this.PageSize = pageSize;
            this.Order = order;
        }
    }

    public static class UrlHelpers
    {
        public static string ToQueryString(this object request, string separator = ",")
        {
            if (request == null)
                throw new ArgumentNullException("request");

            // Get all properties on the object
            var properties = request.GetType().GetProperties()
                .Where(x => x.CanRead)
                .Where(x => x.GetValue(request, null) != null)
                .ToDictionary(x => x.Name, x => x.GetValue(request, null));

            // Get names for all IEnumerable properties (excl. string)
            var propertyNames = properties
                .Where(x => !(x.Value is string) && x.Value is IEnumerable)
                .Select(x => x.Key)
                .ToList();

            // Concat all IEnumerable properties into a comma separated string
            foreach (var key in propertyNames)
            {
                var valueType = properties[key].GetType();
                var valueElemType = valueType.IsGenericType
                                        ? valueType.GetGenericArguments()[0]
                                        : valueType.GetElementType();
                if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                {
                    var enumerable = properties[key] as IEnumerable;
                    properties[key] = string.Join(separator, enumerable.Cast<object>());
                }
            }

            // Concat all key/value pairs into a string separated by ampersand
            return string.Join("&", properties
                .Select(x => string.Concat(
                    Uri.EscapeDataString(x.Key), "=",
                    Uri.EscapeDataString(x.Value.ToString()))));
        }
    }

    public enum Order
    {
        Asc,
        Desc
    }
}
