using Microsoft.FSharp.Core;
using Foo;

namespace csharp;
class Program
{
    static void Main(string[] args)
    {
        var m = Bar.construct();
        var func = m.Increment;
        func.Invoke("a").Invoke("b"); // returns a function because currying, so nothing printed.
        func.Invoke("a").Invoke("b").Invoke("c") // currying has finished, print function will be invoked;
    }
}
