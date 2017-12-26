using System;

namespace AutoCopyLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CopyIgnoreAttribute : Attribute
    {

    }
}
