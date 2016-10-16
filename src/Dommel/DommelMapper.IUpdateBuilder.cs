using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        ///     Adds a custom implementation of <see cref="T:DommelMapper.IUpdateBuilder" />
        ///     for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        ///     The ADO.NET conncetion type to use the <paramref name="builder" /> with.
        ///     Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="builder">An implementation of the <see cref="T:DommelMapper.IUpdateBuilder interface" />.</param>
        public static void AddSqlUpdateBuilder(Type connectionType, IUpdateBuilder builder)
        {
            _updateBuilders[connectionType.Name.ToLower()] = builder;
        }

        private static IUpdateBuilder GetUpdateBuilder(IDbConnection connection)
        {
            var connectionName = connection.GetType().Name.ToLower();
            IUpdateBuilder builder;
            return _updateBuilders.TryGetValue(connectionName, out builder) ? builder : new DefaultUpdateBuilder();
        }

        /// <summary>
        ///     Defines methods for building specialized SQL Update queries.
        /// </summary>
        public interface IUpdateBuilder
        {
            /// <summary>
            ///     Builds an Update query using the specified table name, column names and parameter names.
            ///     A query to fetch the new id will be included as well.
            /// </summary>
            /// <param name="tableName">The name of the table to query.</param>
            /// <param name="typeProperties">The names of the columns in the table.</param>
            /// <param name="keyProperty">
            ///     The key property. This can be used to query a specific column for the new id. This is
            ///     optional.
            /// </param>
            /// <param name="paramNames">The names of the parameters in the database command.</param>
            /// <returns>An insert query including a query to fetch the new id.</returns>
            string BuildUpdate(string tableName, List<PropertyInfo> typeProperties, PropertyInfo keyProperty);
        }

        private class DefaultUpdateBuilder : IUpdateBuilder
        {
            public string BuildUpdate(string tableName, List<PropertyInfo> typeProperties, PropertyInfo keyProperty)
            {
                var columnNames = typeProperties.Select(p => $"{Resolvers.Column(p)} = @{p.Name}").ToArray();
                return $"update {tableName} set {string.Join(", ", columnNames)} where {Resolvers.Column(keyProperty)} = @{keyProperty.Name}";
            }
        }
    }
}