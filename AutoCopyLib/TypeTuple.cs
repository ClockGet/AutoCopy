using System;

namespace AutoCopyLib
{
    public class TypeTuple : Tuple<Type, Type>
    {
        public TypeTuple(Type item1, Type item2) : base(item1, item2)
        {
        }
        public override int GetHashCode()
        {
            return Item1.GetHashCode() + Item2.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            TypeTuple type = obj as TypeTuple;
            if (type == null)
                return false;
            return Item1 == type.Item1 && Item2 == type.Item2;
        }
    }
}
