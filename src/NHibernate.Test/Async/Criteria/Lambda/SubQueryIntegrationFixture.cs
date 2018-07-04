﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Linq;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.Criteria.Lambda
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SubQueryIntegrationFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] { "Criteria.Lambda.Mappings.hbm.xml" }; }
		}

		protected override void OnSetUp()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Save(new Person { Name = "Name 1", Age = 1 }
					.AddChild(new Child { Nickname = "Name 1.1", Age = 1 }));

				s.Save(new Person { Name = "Name 2", Age = 2 }
					.AddChild(new Child { Nickname = "Name 2.1", Age = 2 })
					.AddChild(new Child { Nickname = "Name 2.2", Age = 2 }));

				s.Save(new Person { Name = "Name 3", Age = 3 }
					.AddChild(new Child { Nickname = "Name 3.1", Age = 3 }));

				t.Commit();
			}
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				s.Delete("from Child");
				s.Delete("from Person");
				t.Commit();
			}
		}

		[Test]
		public async Task JoinQueryOverAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var persons = await (s.QueryOver<Person>()
					.JoinQueryOver(p => p.Children)
					.Where(c => c.Nickname == "Name 2.1")
					.ListAsync());

				Assert.That(persons.Count, Is.EqualTo(1));
				Assert.That(persons[0].Name, Is.EqualTo("Name 2"));
			}
		}

		[Test]
		public async Task JoinQueryOverProjectionAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				var simpleProjection = (await (s.QueryOver<Child>()
					.JoinQueryOver(c => c.Parent)
					.Where(p => p.Name == "Name 1" && p.Age == 1)
					.Select(c => c.Nickname, c => c.Age)
					.ListAsync<object[]>()))
					.Select(props => new
					{
						Name = (string) props[0],
						Age = (int) props[1],
					});

				Assert.That(simpleProjection.Count(), Is.EqualTo(1));
				Assert.That(simpleProjection.First().Name, Is.EqualTo("Name 1.1"));
				Assert.That(simpleProjection.First().Age, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task JoinQueryOverProjectionAliasAsync()
		{
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				Child childAlias = null;

				var listProjection = (await (s.QueryOver<Child>(() => childAlias)
					.JoinQueryOver(c => c.Parent)
					.Where(p => p.Name == "Name 1" && p.Age == 1)
					.SelectList(list => list
						.Select(c => childAlias.Nickname)
						.Select(c => c.Age))
					.ListAsync<object[]>()))
					.Select(props => new
					{
						Name = (string) props[0],
						Age = (int) props[1],
					});

				Assert.That(listProjection.Count(), Is.EqualTo(1));
				Assert.That(listProjection.First().Name, Is.EqualTo("Name 1.1"));
				Assert.That(listProjection.First().Age, Is.EqualTo(1));
			}
		}

		[Test]
		public async Task SubQueryAsync()
		{
			if (!Dialect.SupportsScalarSubSelects)
				Assert.Ignore(Dialect.GetType().Name + " does not support scalar sub-queries");

			using (var s = OpenSession())
			{
				Person personAlias = null;
				object childCountAlias = null;

				QueryOver<Child> averageChildAge = QueryOver.Of<Child>()
					.SelectList(list => list.SelectAvg(c => c.Age));

				QueryOver<Child> childCountQuery = QueryOver.Of<Child>()
					.Where(c => c.Parent.Id == personAlias.Id)
					.Select(Projections.RowCount());

				var nameAndChildCount = (await (s.QueryOver<Person>(() => personAlias)
					.WithSubquery.Where(p => p.Age <= averageChildAge.As<int>())
					.SelectList(list => list
						.Select(p => p.Name)
						.SelectSubQuery(childCountQuery).WithAlias(() => childCountAlias))
					.OrderByAlias(() => childCountAlias).Desc
					.ListAsync<object[]>()))
					.Select(props => new
					{
						Name = (string) props[0],
						ChildCount = (int) props[1],
					})
					.ToList();

				Assert.That(nameAndChildCount.Count, Is.EqualTo(2));

				Assert.That(nameAndChildCount[0].Name, Is.EqualTo("Name 2"));
				Assert.That(nameAndChildCount[0].ChildCount, Is.EqualTo(2));

				Assert.That(nameAndChildCount[1].Name, Is.EqualTo("Name 1"));
				Assert.That(nameAndChildCount[1].ChildCount, Is.EqualTo(1));
			}
		}
	}
}
