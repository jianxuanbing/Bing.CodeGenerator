namespace Bing.CodeGenerator.Entity
{
    /// <summary>
    /// 架构表信息
    /// </summary>
    internal class SchemaTable
    {
        /// <summary>
        /// 表标识
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 架构标识
        /// </summary>
        public string SchemaId { get; set; }
    }
}
