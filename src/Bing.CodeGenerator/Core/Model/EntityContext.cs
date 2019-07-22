using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 实体上下文
    /// </summary>
    [DebuggerDisplay("Context: {ContextName}, Database: {DatabaseName}")]
    public class EntityContext : EntityBase
    {
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DatabaseName { get; set; }

        /// <summary>
        /// 实体集合
        /// </summary>
        public EntityCollection Entities { get; set; } = new EntityCollection();

        /// <summary>
        /// 重命名实体
        /// </summary>
        /// <param name="originName">源名称</param>
        /// <param name="newName">新名称</param>
        public void RenameEntity(string originName, string newName)
        {
            if (originName == newName)
                return;

            Debug.WriteLine($"Rename Entity '{originName}' to '{newName}'.");
            foreach (var entity in Entities)
            {

            }
        }
    }
}
