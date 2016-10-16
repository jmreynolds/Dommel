using System;

namespace Dommel
{
    public partial class DommelMapper
    {
        /// <summary>
        ///     Adds a custom implementations of SQL Query Builders
        ///     for the specified ADO.NET connection type.
        /// </summary>
        /// <param name="connectionType">
        ///     The ADO.NET conncetion type to use the <paramref name="sqlBuilder" /> and <paramref name="updateBuilder" /> with.
        ///     Example: <c>typeof(SqlConnection)</c>.
        /// </param>
        /// <param name="sqlBuilder">An implementation of the <see cref="T:DommelMapper.ISqlBuilder interface" />.</param>
        /// <param name="updateBuilder">An implementation of the <see cref="T:DommelMapper.IUpdateBuilder interface" />.</param>
        public static void AddBuilders(Type connectionType, ISqlBuilder sqlBuilder, IUpdateBuilder updateBuilder)
        {
            AddSqlBuilder(connectionType, sqlBuilder);
            AddSqlUpdateBuilder(connectionType, updateBuilder);
        }
    }
}