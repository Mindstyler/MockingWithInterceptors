using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using InterceptorData = (string filePath, Microsoft.CodeAnalysis.Text.LinePosition line, Microsoft.CodeAnalysis.IMethodSymbol methodSignature);

namespace InterceptorMock;

[Generator(LanguageNames.CSharp)]
internal sealed class InterceptorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // local attributes or markers are not yet supported / provided by C#, although there is a proposal for it with the Interceptors proposal

        //context.SyntaxProvider.ForAttributeWithMetadataName(typeof(MockAttribute).FullName,
        //    (_, _) => true,
        //    (generatorAttributeSyntaxContext, cancellationToken) => { return generatorAttributeSyntaxContext; });

        context.RegisterPostInitializationOutput(static context =>
        {
            context.AddSource("InterceptsLocationAttribute.g.cs", """
                namespace System.Runtime.CompilerServices
                {
                    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
                    public sealed class InterceptsLocationAttribute : Attribute
                    {
                        public InterceptsLocationAttribute(string filePath, int line, int character)
                        {
                        }
                    }
                }
                """);

            context.AddSource("MockWrapper.g.cs", """
                public static class MockWrapper
                {
                    public static T MockThis<T>(string substituteMethod, Func<T> nonMockedImplementation) => nonMockedImplementation();
                }
                """);
            });

        IncrementalValuesProvider<InterceptorData> interceptorData = context.SyntaxProvider.CreateSyntaxProvider(
            static (syntaxNode, cancellationToken) =>
                syntaxNode is IdentifierNameSyntax identifierSyntax
                && identifierSyntax.Identifier.Text == nameof(MockWrapper)
                && identifierSyntax.Parent is MemberAccessExpressionSyntax,
            static (generatorSyntaxContext, cancellationToken) =>
            {
                SimpleNameSyntax typeNameSyntax = generatorSyntaxContext.Node.Parent!.Parent!
                .ChildNodes()
                .OfType<ArgumentListSyntax>()
                .First()
                .DescendantNodes()
                .OfType<MemberAccessExpressionSyntax>()
                .First().Name;

                SimpleNameSyntax methodToIntercept = generatorSyntaxContext.Node.Parent.Parent
                .ChildNodes()
                .OfType<ArgumentListSyntax>()
                .Single()
                .ChildNodes()
                .Last()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>()
                .Last()
                .ChildNodes()
                .OfType<IdentifierNameSyntax>()
                .Single();

                string filePath = typeNameSyntax.SyntaxTree.FilePath;
                LinePosition line = methodToIntercept.GetLocation().GetLineSpan().Span.Start;
                IMethodSymbol substituteMethodSymbol = (IMethodSymbol)generatorSyntaxContext.SemanticModel.GetMemberGroup(typeNameSyntax).Single();

                return (filePath, line, substituteMethodSymbol);
            });

        context.RegisterSourceOutput(interceptorData, static (sourceProductionContext, interceptorData) =>
        {
            string methodAccessibility = interceptorData.methodSignature.DeclaredAccessibility.ToString().ToLower();

            string? source = interceptorData.methodSignature.ContainingNamespace.Name != string.Empty
                ? $"namespace {interceptorData.methodSignature.ContainingNamespace.Name};"
                : null;

            source += $$"""
                public static partial class {{interceptorData.methodSignature.ContainingType.Name}}
                {
                    [System.Runtime.CompilerServices.InterceptsLocation(@"{{interceptorData.filePath}}", {{interceptorData.line.Line + 1}}, {{interceptorData.line.Character + 1}})]
                    {{methodAccessibility}} static partial {{interceptorData.methodSignature.ReturnType}} {{interceptorData.methodSignature.Name}}(this {{string.Join(", ", interceptorData.methodSignature.Parameters)}});
                }
                """;

            sourceProductionContext.AddSource("Intercept.g.cs", source);
        });
    }

    //private static string GetInterceptorFilePath(SyntaxTree tree, Compilation compilation)
    //{
    //    return compilation.Options.SourceReferenceResolver?.NormalizePath(tree.FilePath, baseFilePath: null) ?? tree.FilePath;
    //}
}
