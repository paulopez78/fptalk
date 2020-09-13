namespace csharp
{
    using Xunit;

    public class Composition
    {
        [Fact]
        public void Composition_Test()
        {
            Assert.Equal(Add5(Add2(2)), Add2Then5(2));

            Assert.Equal(IsEven(Add5(Add2(2))), IsEventIfAdd2Then5(2));

            Assert.Equal(
                2.Add2().Add5().IsEven(), 
                IsEventIfAdd2Then5(2)
            );

            bool IsEven(int x) => x % 2 == 0;

            int Add2 (int x) => x + 2;

            int Add5 (int x) => x + 5;

            int Add2Then5 (int x) => Add5(Add2(x));

            bool IsEventIfAdd2Then5 (int x) => IsEven(Add5(Add2(x)));
        }
    }

    public static class CompositionExtensions {
        public static bool IsEven(this int x) => x % 2 == 0;
        public static int Add2 (this int x) => x + 2;
        public static int Add5 (this int x) => x + 5;
    }
}