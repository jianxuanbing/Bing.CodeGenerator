using System.Collections.Generic;
using SmartCode.Generator.Entity;

namespace Bing.CodeGenerator.Entity
{
    /// <summary>
    /// 架构
    /// </summary>
    public class Schema : IConvertedName
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int? Id { get; set; }

        /// <summary>
        /// 架构名
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 转换名称
        /// </summary>
        public string ConvertedName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 表集合
        /// </summary>
        public IEnumerable<Table> Tables { get; set; }
    }
}
