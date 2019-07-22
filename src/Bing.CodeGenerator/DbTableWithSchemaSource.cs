using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SmartCode;
using SmartCode.Configuration;
using SmartCode.Db;
using SmartCode.Generator;
using SmartCode.Generator.Entity;

namespace Bing.CodeGenerator
{
    /// <summary>
    /// 带架构的数据表源
    /// </summary>
    public class DbTableWithSchemaSource: DbSource, ITableSource
    {
        /// <summary>
        /// 初始化一个<see cref="DbTableWithSchemaSource"/>类型的实例
        /// </summary>
        /// <param name="project">项目</param>
        /// <param name="loggerFactory">日志工厂</param>
        /// <param name="pluginManager">插件管理器</param>
        public DbTableWithSchemaSource(Project project, ILoggerFactory loggerFactory, IPluginManager pluginManager) :
            base(project, loggerFactory, pluginManager)
        {
        }


        public override string Name => "DbTableSchema";
        public IList<Table> Tables { get; private set; }

        public override async Task InitData()
        {
            var dbTableRepository = new DbTableRepository(Project.DataSource, LoggerFactory);
            DbRepository = dbTableRepository;
            Tables = await dbTableRepository.QueryTable();
            var dbTypeConvert = PluginManager.Resolve<IDbTypeConverter>();
            foreach (var table in Tables)
            {
                foreach (var col in table.Columns)
                {
                    if ((DbRepository.DbProvider == SmartCode.Db.DbProvider.MySql ||
                         DbRepository.DbProvider == SmartCode.Db.DbProvider.MariaDB)
                        && col.DbType == "char"
                        && col.DataLength == 36
                        && Project.Language == "CSharp")
                    {
                        col.LanguageType = "Guid";
                    }
                    else
                    {
                        col.LanguageType =
                            dbTypeConvert.LanguageType(DbRepository.DbProvider, Project.Language, col.DbType);
                    }
                }
            }
        }
    }
}
