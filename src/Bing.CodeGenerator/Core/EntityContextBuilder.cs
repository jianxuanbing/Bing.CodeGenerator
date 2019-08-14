using SmartCode;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 实体上下文构建器
    /// </summary>
    public class EntityContextBuilder
    {
        /// <summary>
        /// 构建
        /// </summary>
        /// <param name="context">构建上下文</param>
        public EntityContext Build(BuildContext context)
        {
            var entityContext = new EntityContext();
            entityContext.DatabaseName = context.GetItem<string>("UnitOfWork");
            return entityContext;
        }
    }
}
