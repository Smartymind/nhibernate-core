﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;

namespace NHibernate.Test.Extralazy
{
	using System.Threading.Tasks;
	[TestFixture]
	public class ExtraLazyFixtureAsync : TestCase
	{
		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override string[] Mappings
		{
			get { return new[] {"Extralazy.UserGroup.hbm.xml"}; }
		}

		protected override string CacheConcurrencyStrategy
		{
			get { return null; }
		}

		[Test]
		public async Task ExtraLazyWithWhereClauseAsync()
		{
			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.CreateSQLQuery("insert into Users (Name,Password) values('gavin','secret')")
					.UniqueResultAsync());
				await (s.CreateSQLQuery("insert into Photos (Title,Owner) values('PRVaaa','gavin')")
					.UniqueResultAsync());
				await (s.CreateSQLQuery("insert into Photos (Title,Owner) values('PUBbbb','gavin')")
					.UniqueResultAsync());
				await (t.CommitAsync());
			}

			using (ISession s = OpenSession())
			{
				var gavin = await (s.GetAsync<User>("gavin"));
				Assert.AreEqual(1, gavin.Photos.Count);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
			}

			using (ISession s = OpenSession())
			using (ITransaction t = s.BeginTransaction())
			{
				await (s.CreateSQLQuery("delete from Photos")
					.UniqueResultAsync());
				await (s.CreateSQLQuery("delete from Users")
					.UniqueResultAsync());

				await (t.CommitAsync());
			}
			await (Sfi.EvictAsync(typeof (User)));
			await (Sfi.EvictAsync(typeof (Photo)));
		}

		[Test]
		public async Task OrphanDeleteAsync()
		{
			User gavin = null;
			Document hia = null;
			Document hia2 = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				hia = new Document("HiA", "blah blah blah", gavin);
				hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia); // NH: added ; I don't understand how can work in H3.2.5 without add
				gavin.Documents.Add(hia2); // NH: added 
				await (s.PersistAsync(gavin));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = await (s.GetAsync<User>("gavin"));
				Assert.AreEqual(2, gavin.Documents.Count);
				gavin.Documents.Remove(hia2);
				Assert.IsFalse(gavin.Documents.Contains(hia2));
				Assert.IsTrue(gavin.Documents.Contains(hia));
				Assert.AreEqual(1, gavin.Documents.Count);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = await (s.GetAsync<User>("gavin"));
				Assert.AreEqual(1, gavin.Documents.Count);
				Assert.IsFalse(gavin.Documents.Contains(hia2));
				Assert.IsTrue(gavin.Documents.Contains(hia));
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Documents));
				Assert.That(await (s.GetAsync<Document>("HiA2")), Is.Null);
				gavin.Documents.Clear();
				Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Documents));
				await (s.DeleteAsync(gavin));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task GetAsync()
		{
			User gavin = null;
			User turin = null;
			Group g = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				await (s.PersistAsync(g));
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = await (s.GetAsync<Group>("developers"));
				gavin = (User) g.Users["gavin"];
				turin = (User) g.Users["turin"];
				Assert.That(gavin, Is.Not.Null);
				Assert.That(turin, Is.Not.Null);
				Assert.That(g.Users.ContainsKey("emmanuel"), Is.False);
				Assert.IsFalse(NHibernateUtil.IsInitialized(g.Users));
				Assert.That(gavin.Session["foo"], Is.Not.Null);
				Assert.That(turin.Session.ContainsKey("foo"), Is.False);
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Session));
				Assert.IsFalse(NHibernateUtil.IsInitialized(turin.Session));
				await (s.DeleteAsync(gavin));
				await (s.DeleteAsync(turin));
				await (s.DeleteAsync(g));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task RemoveClearAsync()
		{
			User gavin = null;
			User turin = null;
			Group g = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				await (s.PersistAsync(g));
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = await (s.GetAsync<Group>("developers"));
				gavin = (User) g.Users["gavin"];
				turin = (User) g.Users["turin"];
				Assert.IsFalse(NHibernateUtil.IsInitialized(g.Users));
				g.Users.Clear();
				gavin.Session.Remove("foo");
				Assert.IsTrue(NHibernateUtil.IsInitialized(g.Users));
				Assert.IsTrue(NHibernateUtil.IsInitialized(gavin.Session));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = await (s.GetAsync<Group>("developers"));
				//Assert.IsTrue( g.Users.IsEmpty() );
				//Assert.IsFalse( NHibernateUtil.IsInitialized( g.getUsers() ) );
				gavin = await (s.GetAsync<User>("gavin"));
				Assert.IsFalse(gavin.Session.ContainsKey("foo"));
				Assert.IsFalse(NHibernateUtil.IsInitialized(gavin.Session));
				await (s.DeleteAsync(gavin));
				await (s.DeleteAsync(turin));
				await (s.DeleteAsync(g));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task IndexFormulaMapAsync()
		{
			User gavin = null;
			User turin = null;
			Group g = null;
			IDictionary<string, SessionAttribute> smap = null;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				turin = new User("turin", "tiger");
				g = new Group("developers");
				g.Users.Add("gavin", gavin);
				g.Users.Add("turin", turin);
				await (s.PersistAsync(g));
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = await (s.GetAsync<Group>("developers"));
				Assert.AreEqual(2, g.Users.Count);
				g.Users.Remove("turin");
				smap = ((User) g.Users["gavin"]).Session;
				Assert.AreEqual(2, smap.Count);
				smap.Remove("bar");
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = await (s.GetAsync<Group>("developers"));
				Assert.AreEqual(1, g.Users.Count);
				smap = ((User) g.Users["gavin"]).Session;
				Assert.AreEqual(1, smap.Count);
				gavin = (User) g.Users["gavin"]; // NH: put in JAVA return the previous value
				g.Users["gavin"] = turin;
				await (s.DeleteAsync(gavin));
				Assert.AreEqual(0, await (s.CreateQuery("select count(*) from SessionAttribute").UniqueResultAsync<long>()));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				g = await (s.GetAsync<Group>("developers"));
				Assert.AreEqual(1, g.Users.Count);
				turin = (User) g.Users["turin"];
				smap = turin.Session;
				Assert.AreEqual(0, smap.Count);
				Assert.AreEqual(1L, await (s.CreateQuery("select count(*) from User").UniqueResultAsync<long>()));
				await (s.DeleteAsync(g));
				await (s.DeleteAsync(turin));
				Assert.AreEqual(0, await (s.CreateQuery("select count(*) from User").UniqueResultAsync<long>()));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task SQLQueryAsync()
		{
			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				User gavin = new User("gavin", "secret");
				User turin = new User("turin", "tiger");
				gavin.Session.Add("foo", new SessionAttribute("foo", "foo bar baz"));
				gavin.Session.Add("bar", new SessionAttribute("bar", "foo bar baz 2"));
				await (s.PersistAsync(gavin));
				await (s.PersistAsync(turin));
				await (s.FlushAsync());
				s.Clear();

				IList results = await (s.GetNamedQuery("UserSessionData").SetParameter("uname", "%in").ListAsync());
				Assert.AreEqual(2, results.Count);
				// NH Different behavior : NH1612, HHH-2831
				gavin = (User) results[0];
				Assert.AreEqual("gavin", gavin.Name);
				Assert.AreEqual(2, gavin.Session.Count);
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				await (s.DeleteAsync("from SessionAttribute"));
				await (s.DeleteAsync("from User"));
				await (t.CommitAsync());
			}
		}

		[Test]
		public async Task AddToUninitializedSetWithLaterLazyLoadAsync()
		{
			User gavin;

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = new User("gavin", "secret");
				var hia = new Document("HiA", "blah blah blah", gavin);
				gavin.Documents.Add(hia);
				await (s.PersistAsync(gavin));
				await (t.CommitAsync());
			}

			using (var s = OpenSession())
			using (var t = s.BeginTransaction())
			{
				gavin = await (s.GetAsync<User>("gavin"));
				var hia2 = new Document("HiA2", "blah blah blah blah", gavin);
				gavin.Documents.Add(hia2);

				foreach (var _ in gavin.Documents)
				{
					//Force Iteration
				}

				Assert.That(gavin.Documents.Contains(hia2));
				await (s.DeleteAsync(gavin));
				await (t.CommitAsync());
			}
		}
	}
}