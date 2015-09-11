using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SweetUtils.LIb.Search;

namespace SweetUtils.Test.Search
{
    public class WeightedSearchTests
    {
        [Test]
        public void DoesApplyWeightsNonOptimized()
        {
            var list = new List<string>()
            {
                "Test A",
                "Test B",
                "Test C"
            };

            var weighted = list.WeightedSearch(new[] {"A", "B", "C"}, new List<Func<string, string, int>>()
            {
                ((value, token) =>
                {
                    var point = 0;

                    switch (token)
                    {
                        case "A":
                            point = 2;
                            break;
                        case "B":
                            point = 1;
                            break;
                        case "C":
                            point = 3;
                            break;                            
                    }

                    var result = value.Contains(token) ? point : 0;

                    return result;
                })
            }).OrderByDescending(value => value.Weight).ToList();

            Assert.That(weighted.ElementAt(0).Value, Is.EqualTo("Test C"));
            Assert.That(weighted.ElementAt(0).Weight, Is.EqualTo(3));
        }

        [Test]
        public void DoesApplyManyWeightsOptimized()
        {
            var list = new List<TestObject<int>>();

            for (var i = 0; i < 1000000; i++)
            {
                list.Add(new TestObject<int>() { Value = i });
            }

            var weighted = list.WeightedSearchOptimized(new List<Func<TestObject<int>, int>>()
            {
                i => i.Value >= 100 && i.Value <= 1000 ? 1 : 0,
                i => i.Value >= 500 && i.Value <= 500 ? 2 : 0 
            }, 32).ToList();

            var ordered = weighted.OrderByDescending(value => value.Weight);

            Assert.That(ordered.ElementAt(0).Value.Value, Is.EqualTo(500));
        }

        [Test]
        public void DoesApplyManyWeightsNonOptimized()
        {
            var list = new List<TestObject<int>>();

            for (var i = 0; i < 1000000; i++)
            {
                list.Add(new TestObject<int>() { Value = i });
            }

            var weighted = list.WeightedSearch(new List<Func<TestObject<int>, int>>()
            {
                i => i.Value >= 100 && i.Value <= 1000 ? 1 : 0,
                i => i.Value >= 500 && i.Value <= 500 ? 2 : 0
            }).ToList();

            var ordered = weighted.OrderByDescending(value => value.Weight);

            Assert.That(ordered.ElementAt(0).Value.Value, Is.EqualTo(500));
        }
    }

    class TestObject<TValue>
    {
        public TValue Value { get; set; }
    }
}