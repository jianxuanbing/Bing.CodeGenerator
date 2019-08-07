using System.Collections.ObjectModel;
using System.Linq;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 实体集合
    /// </summary>
    public class EntityCollection : ObservableCollection<Entity>
    {
        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsProcessed { get; set; }

        /// <summary>
        /// 通过表全名获取实体
        /// </summary>
        /// <param name="fullName">全名</param>
        public Entity ByTable(string fullName) => this.FirstOrDefault(x => x.FullName == fullName);

        /// <summary>
        /// 通过表名获取实体
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="tableSchema">表架构</param>
        public Entity ByTable(string tableName, string tableSchema) =>
            this.FirstOrDefault(x => x.TableName == tableName && x.TableSchema == tableSchema);

        /// <summary>
        /// 通过类名获取实体
        /// </summary>
        /// <param name="className">类名</param>
        public Entity ByClass(string className) => this.FirstOrDefault(x => x.ClassName == className);
    }
}
