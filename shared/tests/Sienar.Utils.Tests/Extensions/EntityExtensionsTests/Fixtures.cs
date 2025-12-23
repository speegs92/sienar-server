using System;
using Sienar.Data;

namespace Sienar.Utils.Tests.Extensions.EntityExtensionsTests;

[EntityName(Singular = SingularName, Plural = PluralName)]
internal class NamedEntity : IEntity
{
	public int Id { get; set; }
	public Guid ConcurrencyStamp { get; set; }
	public const string SingularName = "Name";
	public const string PluralName = "Names";
}

internal class UnnamedEntity : IEntity
{
	public int Id { get; set; }
	public Guid ConcurrencyStamp { get; set; }
}