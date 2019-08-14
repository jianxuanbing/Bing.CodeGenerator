using System.Diagnostics;

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
                if (entity.ClassName == originName)
                    entity.ClassName = newName;
                foreach (var relationship in entity.Relationships)
                {
                    if (relationship.ThisEntity == originName)
                        relationship.ThisEntity = newName;
                    if (relationship.OtherEntity == originName)
                        relationship.OtherEntity = newName;
                }
            }
        }

        /// <summary>
        /// 重命名属性
        /// </summary>
        /// <param name="entityName">实体名称</param>
        /// <param name="originalName">源名称</param>
        /// <param name="newName">新名称</param>
        public void RenameProperty(string entityName, string originalName, string newName)
        {
            if (originalName == newName)
                return;
            Debug.WriteLine("Rename Property '{0}' to '{1}' in Entity '{2}'.", originalName, newName, entityName);
            foreach (var entity in Entities)
            {
                if (entity.ClassName == entityName)
                {
                    var property = entity.Properties.ByProperty(originalName);
                    if (property != null)
                        property.PropertyName = newName;
                }

                foreach (var relationship in entity.Relationships)
                {
                    if (relationship.ThisEntity == entityName)
                    {
                        for (int i = 0; i < relationship.ThisProperties.Count; i++)
                        {
                            if (relationship.ThisProperties[i] == originalName)
                                relationship.ThisProperties[i] = newName;
                        }
                    }

                    if (relationship.OtherEntity == entityName)
                    {
                        for (int i = 0; i < relationship.OtherProperties.Count; i++)
                        {
                            if (relationship.OtherProperties[i] == originalName)
                                relationship.OtherProperties[i] = newName;
                        }
                    }
                }
            }
        }
    }
}
