namespace csharp
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Xunit;
    using static CommonExtensions;
    using static System.Linq.Enumerable;
    using System.Threading.Tasks;

    public class Common
    {
        // [Fact]
        public void Common_Test()
        {
            IEnumerable<int> yearsOfExperience = new[] { 8, 5, 2, 2, 10, 0 };

            bool IsSenior(int years) => years > 5;

            yearsOfExperience.Select(x => IsSenior(x));
            yearsOfExperience.Select(IsSenior);


            // level
            // .SelectMany(Skills)
            // .SelectMany(Skills)
            // .Select(Promote);

            // var a = "devops".Skills();

            // new[] {"devops", "backend", "frontend"}.SelectMany(Skills).Select(Upgrade);
            // Empty<string>().SelectMany(Skills).Select(Upgrade);


            // yearsOfExperience.Select(Level).SelectMany(Skills).Select(Category);


            // // new[] {"senior", "junior", "mid"}.SelectMany(Skills).Select(Category);

            // Skills("devops");
            // Skills("backend");

            // Func<string, IEnumerable<string>> skills = Skills;

            // var bindedSkills = Bind(skills);

            // var backops = bindedSkills(new[] {"backend", "devops"}); 

            // backops = new[] {"backend", "devops"}.SelectMany(Skills);

            // var upgradedBackops = new[] {"backend", "devops"}.SelectMany(Skills).Select(Upgrade);

            // IEnumerable<IEnumerable<string>> nestedBackops = new[] {"backend", "devops"}.Select(Skills);

            // new[] {"backend", "devops"}.SelectMany(Skills).Select(Upgrade);

            // new[] {"backend", "devops"}.Select(Skills);
            // [["docker", "azure"],["csharp", "visualstudio"]]

            // new[] {"backend", "devops"}.Select(Skills).Select(Upgrade);


            // Task<int> GetYearsOfExperience(string email) 
            //     => email switch {
            //         "bob@abax.no"   => Task.FromResult(3),
            //         "alice@abax.no" => Task.FromResult(4),
            //         _               => Task.FromResult(0),
            //     };

            // Maybe<int> GetYearsOfExperience(string email) 
            //     => email switch {
            //         "bob@abax.no"   => Maybe(3),
            //         "alice@abax.no" => Maybe(4),
            //         _               => Maybe.Nothing,
            //     };

            Nullable<int> GetYearsOfExperience(string email) 
                => email switch {
                    "bob@abax.no"   => 3,
                    "alice@abax.no" => 4,
                    _               => null,
                };
        }

        IEnumerable<TResult> SelectMany<TSource, TResult>(IEnumerable<TSource> source, Func<TSource, IEnumerable<TResult>> selector)
        {
            foreach (var element in source) {
                foreach (var subElement in selector(element)) {
                    yield return subElement;
                }
            }
        }

        async Task<TResult> SelectMany<TSource, TResult>(Task<TSource> source, Func<TSource, Task<TResult>> selector)
        {
            var x = await source;
            return await selector(x);
        }

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Bind<TSource, TResult>(Func<TSource, IEnumerable<TResult>> func)
            => source => source.SelectMany(func);

        Func<IEnumerable<TSource>, IEnumerable<TResult>> Map<TSource, TResult>(Func<TSource, TResult> func)
            => source => source.Select(func);

        T Log<T>(T result)
        {
            Console.WriteLine($"LOG {result}");
            return result;
        }
    }

    public static class CommonExtensions {
        public static string Upgrade(string skill)
            => skill switch
            {
                "scrum"  => "kanban",
                "fsharp" => "haskell",
                "csharp" => "fsharp",
                "docker" => "k8s",
                "js"     => "react",
                _        => ""
            };

        public static string Category(string skill)
            => skill switch
            {
                "fsharp"  => "backend",
                "csharp"  => "backend",
                "k8s"     => "devops",
                "docker"  => "devops",
                "js"      => "frontend",
                _         => ""
            };
    }
}