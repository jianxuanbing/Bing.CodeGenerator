using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Bing.CodeGenerator.Entity;
using Microsoft.Extensions.Logging;
using SmartCode.Configuration;
using SmartCode.Db;
using SmartCode.Generator.Entity;
using SmartSql;

namespace Bing.CodeGenerator.Db
{
    /// <summary>
    /// 架构仓储
    /// </summary>
    public class SchemaTableRepository : DbRepository, ISchemaRepository
    {
        /// <summary>
        /// 日志
        /// </summary>
        private readonly ILogger<SchemaTableRepository> _logger;

        /// <summary>
        /// 作用域
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 初始化一个<see cref="SchemaTableRepository"/>类型的实例
        /// </summary>
        /// <param name="dataSource">数据源</param>
        /// <param name="loggerFactory">日志工厂</param>
        public SchemaTableRepository(DataSource dataSource, ILoggerFactory loggerFactory) : base(dataSource, loggerFactory)
        {
            Scope = $"Database-{DbProviderName}";
            _logger = loggerFactory.CreateLogger<SchemaTableRepository>();
        }

        /// <summary>
        /// 查询架构
        /// </summary>
        /// <param name="tables">数据表集合</param>
        public async Task<IList<Schema>> QuerySchema(IList<Table> tables)
        {
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName}, QuerySchema Start! ----");
            IList<Schema> schemas;
            try
            {
                SqlMapper.SessionStore.Open();
                schemas = await SqlMapper.QueryAsync<Schema>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "QuerySchema",
                    Request = new { DbName, DbSchema }
                });
                foreach (var schema in schemas)
                {
                    if (string.IsNullOrWhiteSpace(schema.Id))
                        continue;
                    schema.Tables = await HandleSchemaTableRelation(schema.Id, tables);
                }
            }
            finally
            {
                SqlMapper.SessionStore.Dispose();
            }

            _logger.LogInformation(
                $"----Db:{DbName} Provider:{DbProviderName},Schems:{schemas.Count} QuerySchema End! ----");
            return schemas;
        }

        /// <summary>
        /// 处理架构表关系
        /// </summary>
        /// <param name="schemaId">架构标识</param>
        /// <param name="sourceTables">原始表集合</param>
        private async Task<IList<Table>> HandleSchemaTableRelation(string schemaId, IList<Table> sourceTables)
        {
            var schemaTableRelations = await SqlMapper.QueryAsync<SchemaTable>(new RequestContext
            {
                Scope = Scope,
                SqlId = "QuerySchemaTable",
                Request = new { DbName, DbSchema, SchemaId = schemaId }
            });
            var tableIds = schemaTableRelations.Select(x => x.Id);
            return sourceTables.Where(x => tableIds.Contains(x.Id)).ToList();
        }

        /// <summary>
        /// 查询表
        /// </summary>
        public async Task<IList<Table>> QueryTable()
        {
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName}, QueryTable Start! ----");
            IList<Table> tables;
            try
            {
                SqlMapper.SessionStore.Open();
                tables = await SqlMapper.QueryAsync<Table>(new RequestContext
                {
                    Scope = Scope,
                    SqlId = "QueryTable",
                    Request = new { DbName, DbSchema }
                });
                foreach (var table in tables)
                {
                    table.Columns = await SqlMapper.QueryAsync<Column>(new RequestContext
                    {
                        Scope = Scope,
                        SqlId = "QueryColumn",
                        Request = new { DbName, DbSchema, TableId = table.Id, TableName = table.Name }
                    });
                }
            }
            finally
            {
                SqlMapper.SessionStore.Dispose();
            }
            _logger.LogInformation($"----Db:{DbName} Provider:{DbProviderName},Tables:{tables.Count()} QueryTable End! ----");
            return tables;
        }
    }
}
