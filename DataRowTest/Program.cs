using AgileObjects.ReadableExpressions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRowTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var autoCopy = AutoCopyLib.AutoCopy.CreateMap<DataRowView,SimpleModel>();
            autoCopy.Provider = new DataRowExpressionProvider(typeof(DataRowView));
            autoCopy.Register();
            Console.WriteLine(autoCopy.Lambda.ToReadableString());
        }
    }
}
