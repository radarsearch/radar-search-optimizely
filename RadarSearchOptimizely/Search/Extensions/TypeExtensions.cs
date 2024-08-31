namespace RadarSearchOptimizely.Search.Extensions
{
    public static class TypeExtensions
    {
        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            if ((type == null) || (type.BaseType == null))
            {
                yield break;
            }

            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }
    }
}
