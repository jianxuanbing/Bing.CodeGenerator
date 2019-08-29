using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Bing.CodeGenerator.Extensions;

namespace Bing.CodeGenerator.Helpers
{
    /// <summary>
    /// 唯一名称器
    /// </summary>
    internal class UniqueNamer
    {
        /// <summary>
        /// 名称字典
        /// </summary>
        private readonly ConcurrentDictionary<string, HashSet<string>> _names;

        /// <summary>
        /// 比较器
        /// </summary>
        public IEqualityComparer<string> Comparer { get; set; }

        /// <summary>
        /// 初始化一个<see cref="UniqueNamer"/>类型的还顺利
        /// </summary>
        public UniqueNamer()
        {
            _names = new ConcurrentDictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);
            Comparer = StringComparer.CurrentCulture;

            UniqueContextName("ChangeTracker");
            UniqueContextName("Configuration");
            UniqueContextName("Database");
            UniqueContextName("InternalContext");
        }

        /// <summary>
        /// 唯一名称
        /// </summary>
        /// <param name="bucketName">桶名称</param>
        /// <param name="name">名称</param>
        public string UniqueName(string bucketName, string name)
        {
            var hashSet = _names.GetOrAdd(bucketName, k => new HashSet<string>(Comparer));
            string result = name.MakeUnique(hashSet.Contains);
            hashSet.Add(result);
            return result;
        }

        /// <summary>
        /// 唯一类名
        /// </summary>
        /// <param name="className">类名</param>
        public string UniqueClassName(string className)
        {
            const string globalClassName = "global::ClassName";
            return UniqueName(globalClassName, className);
        }

        /// <summary>
        /// 唯一上下文名称
        /// </summary>
        /// <param name="name">名称</param>
        public string UniqueContextName(string name)
        {
            const string globalContextName = "global::ContextName";
            return UniqueName(globalContextName, name);
        }

        /// <summary>
        /// 唯一关系名称
        /// </summary>
        /// <param name="name">关系名称</param>
        public string UniqueRelationshipName(string name)
        {
            const string globalRelationshipName = "global::RelationshipName";
            return UniqueName(globalRelationshipName, name);
        }
    }
}
