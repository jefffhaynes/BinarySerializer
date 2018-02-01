using System;

namespace BinarySerialization.Test.Issues.Issue90
{
    public static class EnumExtension
    {
        public static T CastTo<T>(this Enum oldEnum) => (T)Enum.Parse(typeof(T).IsEnum ? typeof(T) : throw new InvalidOperationException("Tried to cast enum to invalid type."), oldEnum.ToString());
    }
}