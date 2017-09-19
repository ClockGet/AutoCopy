using System;

namespace AutoCopyLib
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CopyIgnoreAttribute : Attribute
    {

    }
}
