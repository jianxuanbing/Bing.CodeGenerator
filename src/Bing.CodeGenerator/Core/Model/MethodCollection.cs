using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 方法集合
    /// </summary>
    public class MethodCollection : ObservableCollection<Method>
    {
        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsProcessed { get; set; }
    }
}
