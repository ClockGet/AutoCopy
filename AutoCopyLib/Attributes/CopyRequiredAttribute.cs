using System;

namespace AutoCopyLib.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class CopyRequiredAttribute:Attribute
    {
        private string errMsgFormat;
        public CopyRequiredAttribute(string errMsg=null)
        {
            if (string.IsNullOrEmpty(errMsg))
                errMsgFormat = "not found {0}";
            else
                errMsgFormat = errMsg;
        }
        public string MsgFormat
        {
            get
            {
                return errMsgFormat;
            }
        }
    }
}
