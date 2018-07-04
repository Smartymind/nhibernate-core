﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.Generatedkeys.Select
{
	using System.Threading.Tasks;
	[TestFixture]
	public class SelectGeneratorTestAsync: TestCase
	{
		protected override string[] Mappings
		{
			get { return new[] { "Generatedkeys.Select.MyEntity.hbm.xml" }; }
		}

		protected override string MappingsAssembly
		{
			get { return "NHibernate.Test"; }
		}

		protected override bool AppliesTo(Dialect.Dialect dialect)
		{
			return dialect is Dialect.FirebirdDialect || dialect is Dialect.Oracle8iDialect;
		}

		[Test]
		public async Task GetGeneratedKeysSupportAsync()
		{
			ISession session = OpenSession();
			session.BeginTransaction();

			MyEntity e = new MyEntity("entity-1");
			await (session.SaveAsync(e));

			// this insert should happen immediately!
			Assert.AreEqual(1, e.Id, "id not generated through forced insertion");

			await (session.DeleteAsync(e));
			await (session.Transaction.CommitAsync());
			session.Close();
		}

	}
}