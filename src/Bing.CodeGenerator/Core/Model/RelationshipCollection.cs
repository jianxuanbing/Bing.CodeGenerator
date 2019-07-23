using System.Collections.ObjectModel;
using System.Linq;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 关系集合
    /// </summary>
    public class RelationshipCollection : ObservableCollection<Relationship>
    {
        /// <summary>
        /// 是否已处理
        /// </summary>
        public bool IsProcessed { get; set; }

        //public Relationship ByName(string name)
        //{
        //}
    }
}
