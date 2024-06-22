using Bing.CodeGenerator.Configuration;
using SmartCode.Configuration;

namespace Bing.CodeGenerator.Extensions
{
    /// <summary>
    /// 项目扩展
    /// </summary>
    public static class ProjectExtensions
    {
        /// <summary>
        /// 获取工作单元名称
        /// </summary>
        /// <param name="project">项目</param>
        public static string GetUnitOfWorkName(this Project project) =>
            project.Parameters["UnitOfWork"].ToString();

        /// <summary>
        /// 获取工作单元
        /// </summary>
        /// <param name="project">项目</param>
        public static string GetUnitOfWork(this Project project) => $"{project.Parameters["UnitOfWork"]}UnitOfWork";

        /// <summary>
        /// 获取架构过滤
        /// </summary>
        /// <param name="project">项目</param>
        public static SchemaFilter GetSchemaFilter(this Project project)
        {
            if (project.Parameters.ContainsKey(nameof(SchemaFilter)))
            {
                var filter = (SchemaFilter)project.Parameters[nameof(SchemaFilter)];
                return filter;
            }

            return null;
        }
    }
}
