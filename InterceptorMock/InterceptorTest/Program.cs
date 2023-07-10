new MyTestClass().MyUnitTest();
Console.ReadKey();

// any of your classes
public sealed class MyTestClass
{
    public void MyUnitTest()
    {
        // any arguments you want for your original method, you can pass to it. The substitute method must also accept all of these.
        int argument1 = 0;

        // first argument has to be a nameof() with the substitute method.
        // second argument is required to be a lambda with the sole purpose of invoking the original method.
        string result = MockWrapper.MockThis(nameof(Substitutes.MySubstituteMethod), () => ToIntercept(argument1));

        Console.WriteLine(result);
    }

    public string ToIntercept(int parameter) // method to intercept must not be static
    {
        return "This was supposed to be intercepted!";
    }
}



// manually all write this
public static partial class Substitutes
{
    public static partial string MySubstituteMethod(this MyTestClass classOfMethodToIntercept, int parameter1)
    {
        return "Hello, world!";
    }
}