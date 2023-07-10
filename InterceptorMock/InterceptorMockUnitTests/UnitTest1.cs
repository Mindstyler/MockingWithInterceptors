using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using InterceptorMock;

namespace InterceptorMockUnitTests;

/// UNIT TEST NOT CURRENTLY WRITTEN, stub from different project

//[TestClass]
//public class UnitTest1
//{
//    [TestMethod]
//    public void TestMethod1()
//    {
//        CSharpCompilation inputCompilation = CreateCompilation("""
//            public sealed class MyTestClass
//            {
//                public void MyUnitTest()
//                {
//                    // any arguments you want for your original method, you can pass to it. The substitute method must also accept all of these.
//                    int argument1 = 0;

//                    // first argument has to be a nameof() with the substitute method.
//                    // second argument is required to be a lambda with the sole purpose of invoking the original method.
//                    string result = MockWrapper.MockThis(nameof(Substitutes.MySubstituteMethod), () => ToIntercept(argument1));

//                    Console.WriteLine(result);
//                }

//                public string ToIntercept(int parameter) // method to intercept must not be static
//                {
//                    return "This was supposed to be intercepted!";
//                }
//            }



//            // manually all write this
//            public static partial class Substitutes
//            {
//                public static partial string MySubstituteMethod(this MyTestClass classOfMethodToIntercept, int parameter1)
//                {
//                    return "Hello, world!";
//                }
//            }
//            """);

//        InterceptorGenerator generator = new();

//        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);

//        driver = driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out Compilation outputCompilation, out ImmutableArray<Diagnostic> diagnostics);

//        GeneratorDriverRunResult runResult = driver.GetRunResult();

//        string s = string.Empty;

//        foreach (SyntaxTree tree in outputCompilation.SyntaxTrees)
//        {
//            s += tree.GetText().ToString();
//        }

//        //Debug.Assert(diagnostics.IsEmpty);
//        Debug.Assert(outputCompilation.SyntaxTrees.Count() == 2);
//    }

//    private static CSharpCompilation CreateCompilation(string source)
//    {
//        return CSharpCompilation.Create("compilation",
//            new[] { CSharpSyntaxTree.ParseText(source) },
//            new[] { MetadataReference.CreateFromFile(typeof(Binder).GetTypeInfo().Assembly.Location) },
//            new CSharpCompilationOptions(OutputKind.ConsoleApplication));
//    }
//}