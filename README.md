# Dommel
A modified version of [Dommel](https://github.com/henkmollema/Dommel) - provides some extension hooks for Update Queries, 
similar to those provided by Dommel for Insert Queries.

Dommel is a Dapper extension which provides simple CRUD operations.

<hr>

Dommel provides a convenient API for CRUD operations using extension methods on the 
`IDbConnection` interface. The SQL queries are generated based on your POCO entities. 
Dommel also supports LINQ expressions which are being translated to SQL expressions. 
[Dapper](https://github.com/StackExchange/dapper-dot-net) is used for query execution 
and object mapping.

Dommel also provides extensibility points to change the bevahior of resolving table names, 
column names, the key property and POCO properties. 
See [Extensibility](https://github.com/henkmollema/Dommel#extensibility) for more details.

## Modifications

This repository hosts a modified version of Dommel. The primary trunk does not support 
OleDB \ Access databases (positional parameters). The ISqlBuilder provided a way to hook in
an Insert Query provider, but Update Queries were still unsupported.

### IUpdateBuilder

This project adds in an IUpdateBuilder, as an addition to the ISqlBuilder.

```csharp
        public interface IUpdateBuilder
        {
            string BuildUpdate(string tableName, PropertyInfo[] typeProperties, PropertyInfo keyProperty);
        }
```

If you **DON'T** implement this Interface, that's ok. 

```csharp
        private static IUpdateBuilder GetUpdateBuilder(IDbConnection connection)
        {
            var connectionName = connection.GetType().Name.ToLower();
            IUpdateBuilder builder;
            return _updateBuilders.TryGetValue(connectionName, out builder) ? builder : new DefaultUpdateBuilder();
        }
```

`GetUpdateBuilder` will return the `DefaultUpdateBuilder` if nothing else is defined or appropriate.

On the other hand, in your own project, you can implement something like this:

```csharp
    public sealed class AccessUpdateBuilder : DommelMapper.IUpdateBuilder
    {
        public string BuildUpdate(string tableName, PropertyInfo[] typeProperties, PropertyInfo keyProperty)
        {
            var columnNames = typeProperties.Select(p => $"{DommelMapper.Resolvers.Column(p)} = ?{p.Name}?").ToArray();
            return $"update {tableName} set {string.Join(", ", columnNames)} where {DommelMapper.Resolvers.Column(keyProperty)} = ?{keyProperty.Name}?";
        }
    }
```

### AddSqlUpdateBuilder

And then call `DommelMapper.AddSqlUpdateBuilder(typeof(AccessDbConnection), new AccessUpdateBuilder());`

This will register the Connection and the UpdateSqlBuilder.

### AddBuilders

Finally - there is another method: `AddBuilders(Type connectionType, ISqlBuilder sqlBuilder, IUpdateBuilder updateBuilder)`

This is useful if you have implemented both an Insert *and* an Update (and that seems likely).

<hr>

### Download

Currently, I don't have this packaged as a Nuget file. 
I probably will soon... 
But there's a things I want to do first

1) Make sure I have the Dommel-Access package ready to go as well
2) Try to get my changes accepted into the main trunk 
(we don't **really** need two versions of the same thing floating around
3) If not accepted fairly quickly, then come up with a new project name - 
and we all know that [Naming Things is **HARD**](https://drupalize.me/blog/201301/naming-things-hard)

<hr>

## Credit

Obviously, the main credit here goes 
to [Henk Mollema](https://github.com/henkmollema) - He is the maintainer 
and author of the original Dommel library.
I'm just piggy-backing on his stuff.

The other big acknowledgement goes to StackExchange for the 
actual [Dapper](https://github.com/StackExchange/dapper-dot-net) library.
It's pretty freaking great.
