using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace csharp
{
    public class Samples
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
        }

        [Fact]
        public void IEnumerableSelect_Tests()
        {
            IEnumerable<int> yearsOfExperience = new[] { 8, 5, 2, 2, 10, 0 };

            Assert.Equal(IsSenior(yearsOfExperience), yearsOfExperience.Select(x => IsSenior(x)));
            Assert.Equal(IsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));

            Print(yearsOfExperience.Select(IsSenior));
            Print(IsSenior(yearsOfExperience));

            Func<IEnumerable<int>, IEnumerable<bool>> liftedIsSenior = Lift<int,bool>(IsSenior);

            Assert.Equal(liftedIsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));
            Assert.Equal(liftedIsSenior(yearsOfExperience), IsSenior(yearsOfExperience));

            Print(liftedIsSenior(yearsOfExperience));

            8.Level().Promote().Years().IsSenior(); // true

            Print(yearsOfExperience.Select(Level).Select(Promote).Select(Years).Select(IsSenior));
            yearsOfExperience.Select(Log).Select(IsSenior).Select(Log).ToArray();
        }

        [Fact]
        public async Task Task_Select_Tests()
        {
            Task<int> yearsOfExperience = Task.FromResult(6);

            Assert.Equal(IsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));
            
            Task<bool> result1 = yearsOfExperience.Select(IsSenior);
            bool isSenior = await result1;
            Assert.True(isSenior);

            Task<bool> result2 = yearsOfExperience.Select(Level).Select(Promote).Select(Years).Select(IsSenior);
            isSenior = await result2;
            Assert.True(isSenior);

            var liftedIsSenior = LiftTask<int,bool>(IsSenior);
            Assert.Equal(liftedIsSenior(yearsOfExperience), yearsOfExperience.Select(IsSenior));

            await yearsOfExperience.Select(Log).Select(IsSenior).Select(Log);
        }

        [Fact]
        public void Nullable_Select_Tests()
        {
            Nullable<int> GetYearsOfExperience(string email) 
                => email switch {
                    bob   => 3,
                    alice => 4,
                    _    => null,
                };

            HttpStatusCode IsAuthorized(string email)
            {
                var years = GetYearsOfExperience(email);
                if (years is null) {
                    return HttpStatusCode.NotFound;
                }

                var isSenior = years.Value.Add2().IsSenior();
                return isSenior ? HttpStatusCode.OK : HttpStatusCode.Forbidden;
            }

            HttpStatusCode IsAuthorizedWithMatch(string email) 
                => GetYearsOfExperience(email)
                .Select(Add2).Select(IsSenior)
                .Match(
                    nothing: HttpStatusCode.NotFound,
                    something: x => x ? HttpStatusCode.OK : HttpStatusCode.Forbidden
                );

            Nullable<bool> isBobSenior = GetYearsOfExperience(bob).Select(IsSenior);
            Assert.False(isBobSenior);

            Nullable<bool> isAliceSenior = GetYearsOfExperience(alice).Select(IsSenior);
            Assert.False(isAliceSenior);

            Nullable<bool> isSomeoneSenior = GetYearsOfExperience(gary).Select(IsSenior);
            Assert.Null(isSomeoneSenior);

            Assert.Equal(
                IsAuthorized(alice), 
                IsAuthorizedWithMatch(alice)
            );

            Assert.Equal(
                IsAuthorized(bob), 
                IsAuthorizedWithMatch(bob)
            );

            Assert.Equal(
                IsAuthorized(gary), 
                IsAuthorizedWithMatch(gary)
            );

            // does not compile, nullable only works for structs
            GetYearsOfExperience(bob);//.Select(Level).Select(Promote).Select(Years).Select(IsSenior); 
        }

        [Fact]
        public void IEnumerable_SelectMany_Tests()
        {
            IEnumerable<string> groups = new [] { "devops", "backend" };
            IEnumerable<string> empty = Enumerable.Empty<string>();

            Assert.Equal(
                new [] { new[] { "docker", "azure" } , new[] { "csharp", "fsharp"} },
                groups.Select(Skills)
            );

            Assert.Equal(
                new [] { "docker", "azure", "csharp", "fsharp" },
                groups.SelectMany(Skills)
            );

            Assert.Equal(
                empty,
                empty.SelectMany(Skills)
            );

            // does not compile
            groups.Select(Skills);//.Select(Upgrade);


            groups.Select(Skills);

            IEnumerable<string> backOps = groups.SelectMany(Skills).Select(Upgrade);

            Assert.Equal(
                new [] { "k8s", "gcp", "fsharp", "haskell" },
                backOps
            );

            var bindedSkills = Bind<string, string>(Skills);

            Assert.Equal(
                new[] { "csharp", "fsharp" },
                Skills("backend")
            );

            Assert.Equal(
                new[] { "csharp", "fsharp", "docker", "azure" },
                bindedSkills(new [] { "backend", "devops"})
            );

            string Group(string email) 
                => email switch {
                    bob   => "devops",
                    alice => "backend",
                    _     =>  "",
                };


            Assert.Equal(
                new[] { "fsharp", "haskell", "k8s", "gcp" },
                new[] { alice, bob }.Select(Group).SelectMany(Skills).Select(Upgrade)
            );
        }

        [Fact]
        public async Task Tasks_SelectMany_Tests()
        {
            Task<string> PromoteAsync(string level) 
                => level switch {
                "Ninja"   => Task.FromResult("Rockstar"),
                "Senior"  => Task.FromResult("Ninja"),
                "Mid"     => Task.FromResult("Senior"),
                "Junior"  => Task.FromResult("Mid"),
            _         => Task.FromResult(level)
            };

            Task<int> yearsOfExperience = Task.FromResult(4);

            await yearsOfExperience.Select(Level).Select(PromoteAsync);//.Select(Years).Select(IsSenior);

            var isSenior = await yearsOfExperience.Select(Level).SelectMany(PromoteAsync).Select(Years).Select(IsSenior);
            Assert.True(isSenior);

            isSenior = await Bind<string, string>(PromoteAsync).Invoke(yearsOfExperience.Select(Level)).Select(Years).Select(IsSenior);
            Assert.True(isSenior);
        }

        [Fact]
        public async Task Life_Without_Monads_Tests()
        {
            int? GetYearsOfExperience(string email) => email switch { bob => 3, alice => 4, _ => null, };

            string Level(int years) => years switch { > 10 => "Rockstar", > 8 => "Ninja", > 5 => "Senior", > 3  => "Mid", > 0  => "Junior", _ => null };

            string Promote(string level) => level switch { "Ninja" => "Rockstar", "Senior" => "Ninja", "Mid" => "Senior", "Junior" => "Mid", _ => null };

            int? Years(string level) => level switch { "Rockstar" => 12, "Ninja" => 10, "Senior" => 8, "Mid" => 6, "Junior" => 2, _ => null };

            bool? IsSenior(int? years) => years.HasValue ? years > 6 : null;

            bool TryGetYearsOfExperience(string email, out int years)
            {
                var result = GetYearsOfExperience(email);
                years = result.HasValue ? result.Value : default;
                return result.HasValue;
            }

            bool TryLevel(int years, out string level)
            {
                var result = Level(years);
                var hasValue = !string.IsNullOrEmpty(result);

                level = hasValue ? result : null;
                return hasValue;
            }

            bool TryPromote(string level, out string promotion)
            {
                var result = Promote(level);
                var hasValue = !string.IsNullOrEmpty(result);

                promotion = hasValue ? result : null;
                return hasValue;
            } 

            bool TryYears(string level, out int years)
            {
                var result = Years(level);
                years = result.HasValue ? result.Value : default;
                return result.HasValue;
            }

            bool TryIsSenior(int? years, out bool isSenior)
            {
                var result = IsSenior(years);
                isSenior = result.HasValue ? result.Value : default;
                return result.HasValue;
            }

            HttpStatusCode IsAuthorized(string email)
            {
                var years = GetYearsOfExperience(email);

                if (years.HasValue) {
                    var level = Level(years.Value);

                    if(!string.IsNullOrEmpty(level)){
                        var newLevel = Promote(level);

                        if(!string.IsNullOrEmpty(newLevel)){
                            var newYears = Years(newLevel);

                            if (newYears.HasValue) {
                                var senior = IsSenior(newYears);

                                if (senior.HasValue){
                                    return senior.Value 
                                        ? HttpStatusCode.OK 
                                        : HttpStatusCode.Forbidden;
                                }
                            }
                        }
                    }
                }

                return HttpStatusCode.NotFound;
            }

            HttpStatusCode TryIsAuthorized(string email) 
             => TryGetYearsOfExperience(email, out var years)
                    ? TryLevel(years, out var level)
                        ? TryPromote(level, out var promotion)
                            ? TryYears(promotion, out var newYears)
                                ? TryIsSenior(newYears, out var isSenior)
                                    ? isSenior 
                                        ? HttpStatusCode.OK 
                                        : HttpStatusCode.Forbidden
                                    : HttpStatusCode.NotFound
                                : HttpStatusCode.NotFound
                            : HttpStatusCode.NotFound
                        : HttpStatusCode.NotFound
                    : HttpStatusCode.NotFound;

            Assert.Equal(
                IsAuthorized(alice),
                TryIsAuthorized(alice)
            );

            Assert.Equal(
                IsAuthorized(bob),
                TryIsAuthorized(bob)
            );

            Assert.Equal(
                IsAuthorized(gary),
                TryIsAuthorized(gary)
            );
        }

        const string bob   = "bob@abax.no";
        const string alice = "alice@abax.no";
        const string gary  = "gary@abax.no";

        bool IsEven(int x) => x.IsEven();

        int Add2 (int x) => x.Add2();

        int Add5 (int x) => x.Add5();

        int Add2Then5 (int x) => Add5(Add2(x));

        bool IsEventIfAdd2Then5 (int x) => IsEven(Add5(Add2(x)));

        bool IsSenior(int years) => years.IsSenior(); // int -> bool

        IEnumerable<bool> IsSenior(IEnumerable<int> years) => years.Select(IsSenior); // IEnumerable<int> -> IEnumerable<bool>

        Task<bool> IsSenior(Task<int> years) => years.Select(IsSenior); // Task<int> -> Task<bool>

        void Print<T>(IEnumerable<T> list) => Console.WriteLine(string.Join(",", list));

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Lift<TSource, TResult>(Func<TSource, TResult> func)
            => source => source.Select(func);

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func)
            => Lift(func);

        Func<Task<TSource>, Task<TResult>> LiftTask<TSource, TResult>(Func<TSource, TResult> func)
            => source => source.Select(func);

        Func<Task<TSource>, Task<TResult>> MapTask<TSource, TResult>(Func<TSource, TResult> func)
            => LiftTask(func);

        Func<Nullable<TSource>, Nullable<TResult>> LiftNullable<TSource, TResult>(Func<TSource, TResult> func)
            where TSource: struct
            where TResult: struct
            => source => source.Select(func);

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Bind<TSource, TResult>(Func<TSource, IEnumerable<TResult>> func) 
            => func.Bind();

        Func<Task<TSource>, Task<TResult>> Bind<TSource, TResult>(Func<TSource, Task<TResult>> func) 
            => func.Bind();

        string Level(int years) => years.Level();

        string Promote(string level) => level.Promote();

        int Years(string level) => level.Years();

        IEnumerable<string> Skills (string group) => group.Skills();

        string Upgrade (string skill) => skill.Upgrade();

        T Log<T>(T result)
        {
            Console.Write($"LOG {result},");
            return result;
        }
    }

    public static class Extensions
    {
        public static bool IsEven(this int x) => x % 2 == 0;

        public static int Add2 (this int x) => x + 2;

        public static int Add5 (this int x) => x + 5;

        public static bool IsSenior(this int years) => years > 5; // int -> bool

        public static string Level(this int years) 
        => years switch
        {
            > 10 => "Rockstar",
            > 8  => "Ninja",
            > 5  => "Senior",
            > 3  => "Mid",
            > 0  => "Junior",
            _    => "No idea what I'm doing",
        };

        public static string Promote(this string level) 
        => level switch
        {
            "Ninja"  => "Rockstar",
            "Senior" => "Ninja",
            "Mid"    => "Senior",
            "Junior" => "Mid",
            _        => level
        };

        public static int Years(this string level) 
        => level switch
        {
            "Rockstar" => 12,
            "Ninja"    => 10,
            "Senior"   => 8,
            "Mid"      => 6,
            "Junior"   => 2,
            _          => 0
        };

        public static IEnumerable<string> Skills(this string group)
            => group switch
            {
                "devops"    => new[] { "docker", "azure" },
                "backend"   => new[] { "csharp", "fsharp" },
                "fullstack" => new[] { "js" },
                "frontend"  => new[] { "js", "css" },
                _           => new string[] {}
            };

        public static string Upgrade(this string skill)
            => skill switch
            {
                "fsharp" => "haskell",
                "csharp" => "fsharp",
                "docker" => "k8s",
                "azure"  => "gcp",
                _        => skill
            };

        public static IEnumerable<TResult> Select<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> func)
        {
            foreach (var item in source)
            {
                yield return func(item);
            }
        }

        public static IEnumerable<TResult> SelectMany<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (var element in source) {
                foreach (var subElement in selector(element)) {
                    yield return subElement;
                }
            }
        }

        public static async Task<TResult> Select<TSource, TResult>(this Task<TSource> source, Func<TSource, TResult> selector)
        {
            var x = await source;
            return selector(x);
        }

        public static async Task<TResult> SelectMany<TSource, TResult>(this Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            var x = await source;
            return await selector(x);
        }

        public static Nullable<TResult> Select<TSource, TResult> (this Nullable<TSource> source, Func<TSource, TResult> func) 
            where TSource : struct 
            where TResult : struct 
            => source.HasValue ? func(source.Value) : null;

        public static TResult Match<TSource,TResult>(this Nullable<TSource> source, TResult nothing, Func<TSource,TResult> something) 
            where TSource: struct
            => source.HasValue ? something(source.Value) : nothing;

        public static Func<IEnumerable<TSource>, IEnumerable<TResult>> Bind<TSource, TResult>(this Func<TSource, IEnumerable<TResult>> func) 
            => source => source.SelectMany(func);

        public static Func<Task<TSource>, Task<TResult>> Bind<TSource, TResult>(this Func<TSource, Task<TResult>> func) 
            => source => source.SelectMany(func);
    }
}