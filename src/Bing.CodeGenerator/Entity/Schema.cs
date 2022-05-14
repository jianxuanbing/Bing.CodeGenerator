﻿using System.Collections.Generic;
using SmartCode.Generator.Entity;

namespace Bing.CodeGenerator.Entity
{
    /// <summary>
    /// 架构
    /// </summary>
    public class Schema : IConvertedName
    {
        /// <summary>
        /// 架构标识
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 名称
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

        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault => Name == "dbo";

        /// <summary>
        /// 路径
        /// </summary>
        public string Path => IsDefault ? "" : $"{Name}";
    }
}
