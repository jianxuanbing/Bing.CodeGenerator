using System.Collections.Generic;
using Bing.CodeGenerator.Entity;
using SmartCode;

namespace Bing.CodeGenerator.Extensions
{
    /// <summary>
    /// 构建上下文扩展
    /// </summary>
    public static class BuildContextExtensions
    {
        /// <summary>
        /// 当前全部架构
        /// </summary>
        public const string CurrentAllSchema = "CurrentAllSchema";

        /// <summary>
        /// 获取当前所有架构
        /// </summary>
        /// <param name="context">构建上下文</param>
        public static IEnumerable<Schema> GetCurrentAllSchema(this BuildContext context)
        {
            if (!context.Items.ContainsKey(CurrentAllSchema))
            {
                var schemas = context.GetDataSource<DbTableWithSchemaSource>().Schemas;
                context.SetItem(CurrentAllSchema, schemas);
                return schemas;
            }

            return context.GetItem<IEnumerable<Schema>>(CurrentAllSchema);
        }

    }
}
