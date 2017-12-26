using AutoCopyLib.Utilities;
using System;

namespace AutoCopyLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CopyMapAttribute : Attribute
    {
        private string _mapName;
        public CopyMapAttribute(string mapName)
        {
            Guard.ArgumentNotNullOrEmpty(mapName, mapName);
            _mapName = mapName;
        }
        public string MapName
        {
            get
            {
                return _mapName;
            }
        }
    }
}
