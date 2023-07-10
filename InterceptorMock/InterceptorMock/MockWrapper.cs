namespace InterceptorMock;

public static class MockWrapper
{
    public static T MockThis<T>(string substituteMethod, Func<T> nonMockedImplementation) => nonMockedImplementation();
}
