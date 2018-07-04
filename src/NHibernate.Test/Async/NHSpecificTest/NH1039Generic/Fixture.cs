﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System;
using System.Collections.Generic;
using NUnit.Framework;


namespace NHibernate.Test.NHSpecificTest.NH1039Generic
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				s.Delete("from Person");
				tx.Commit();
			}
		}

		[Test]
		public async Task testAsync()
		{
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person person = new Person("1");
				person.Name = "John Doe";
				var set = new HashSet<object>();
				set.Add("555-1234");
				set.Add("555-4321");
				person.Properties.Add("Phones", set);

				await (s.SaveAsync(person));
				await (tx.CommitAsync());
			}
			using (ISession s = OpenSession())
			using (ITransaction tx = s.BeginTransaction())
			{
				Person person = (Person)await (s.CreateCriteria(typeof(Person)).UniqueResultAsync());

				Assert.AreEqual("1", person.ID);
				Assert.AreEqual("John Doe", person.Name);
				Assert.AreEqual(1, person.Properties.Count);
				Assert.That(person.Properties["Phones"], Is.InstanceOf<ISet<object>>());
				Assert.IsTrue(((ISet<object>) person.Properties["Phones"]).Contains("555-1234"));
				Assert.IsTrue(((ISet<object>) person.Properties["Phones"]).Contains("555-4321"));
			}
		}
	}
}
