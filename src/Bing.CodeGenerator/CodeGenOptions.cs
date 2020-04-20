using System.Collections.Generic;

namespace Bing.CodeGenerator
{
    /// <summary>
    /// 代码生成选项配置
    /// </summary>
    public class CodeGenOptions : Dictionary<string, CodeGenItem>
    {
    }

    /// <summary>
    /// 代码生成项
    /// </summary>
    public class CodeGenItem
    {
        /// <summary>
        /// 解决方案名称
        /// </summary>
        public string SlnName { get; set; }

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }

        /// <summary>
        /// 数据库提供程序
        /// </summary>
        public string DbProvider { get; set; }

        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string DbConnectionString { get; set; }

        /// <summary>
        /// 输出路径
        /// </summary>
        public string OutputPath { get; set; }

        /// <summary>
        /// 工作单元名称
        /// </summary>
        public string UnitOfWorkName { get; set; }
    }
}
