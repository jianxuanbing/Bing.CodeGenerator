using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bing.CodeGenerator.Entity;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 解决方案信息
    /// </summary>
    public class SlnInfo
    {
        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; } = Guid.NewGuid().ToString().ToUpper();

        /// <summary>
        /// 方案类型
        /// </summary>
        public SlnType Type { get; private set; } = SlnType.Root;

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 相对路径
        /// </summary>
        public string RelativePath { get; private set; }

        /// <summary>
        /// 父标识
        /// </summary>
        public string ParentId { get; private set; }

        /// <summary>
        /// 父方案
        /// </summary>
        public SlnInfo Parent { get; private set; }

        /// <summary>
        /// 子方案列表
        /// </summary>
        public IList<SlnInfo> Childrens { get; } = new List<SlnInfo>();

        /// <summary>
        /// 添加解决方案文件夹
        /// </summary>
        /// <param name="parent">父目录</param>
        /// <param name="name"></param>
        /// <returns></returns>
        public SlnInfo AddDir(SlnInfo parent, string name)
        {
            if (parent.Type == SlnType.Project)
                throw new ArgumentException($"{parent.Type} 不能是项目类型");
            var sln = new SlnInfo
            {
                Type = SlnType.Dir,
                Name = name,
                RelativePath = name,
                ParentId = parent.Id,
                Parent = parent
            };
            parent.Childrens.Add(sln);
            return sln;
        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="parent">父目录</param>
        /// <param name="name">名称</param>
        /// <param name="path">路径</param>
        public SlnInfo AddProject(SlnInfo parent, string name, string path)
        {
            var sln = new SlnInfo
            {
                Type = SlnType.Project,
                Name = name,
                RelativePath = path,
                ParentId = parent.Id,
                Parent = parent
            };
            parent.Childrens.Add(sln);
            return sln;
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="schemas">架构集合</param>
        public void InitData(string module, IEnumerable<Schema> schemas)
        {
            var src = AddDir(this, "01-src");
            var presentation = AddDir(src, "01-Presentation");
            AddProject(presentation, $"{module}.Api", $"src\\{module}.Api\\{module}.Api.csproj");
            var service = AddDir(src, "02-Service");
            AddProject(service, $"{module}.Service", $"src\\{module}.Service\\{module}.Service.csproj");
            var domain = AddDir(src, "03-Domain");
            if (!schemas.Any())
            {
                AddProject(domain, $"{module}.Domain", $"src\\{module}.Domain\\{module}.Domain.csproj");
            }
            else
            {
                foreach (var schema in schemas)
                {
                    if (!schema.Tables.Any())
                        continue;
                    if (schema.Name.ToLower() == "dbo")
                    {
                        AddProject(domain, $"{module}.Domain", $"src\\{module}.Domain\\{module}.Domain.csproj");
                    }
                    else
                    {
                        AddProject(domain, $"{module}.{schema.Name}.Domain", $"src\\{module}.{schema.Name}.Domain\\{module}.{schema.Name}.Domain.csproj");
                    }
                }
            }
            var infrastructure = AddDir(src, "04-Infrastructure");
            AddProject(infrastructure, $"01-{module}.Infrastructure",
                $"src\\{module}.Infrastructure\\{module}.Infrastructure.csproj");
            AddProject(infrastructure, $"02-{module}.Data",
                $"src\\{module}.Data\\{module}.Data.csproj");
            var test = AddDir(this, "02-test");
            AddProject(test, $"{module}.Tests", $"test\\{module}.Tests\\{module}.Tests.csproj");
            AddProject(test, $"{module}.Tests.Integration", $"test\\{module}.Tests.Integration\\{module}.Tests.Integration.csproj");
        }

        /// <summary>
        /// 获取项目字符串
        /// </summary>
        /// <returns></returns>
        public string GetProjectString()
        {
            StringBuilder sb = new StringBuilder();
            BuildProjectString(this.Childrens, sb);
            return sb.ToString();
        }

        /// <summary>
        /// 构建项目字符串
        /// </summary>
        /// <param name="slns">解决方案信息集合</param>
        /// <param name="sb">拼接字符串</param>
        private void BuildProjectString(IList<SlnInfo> slns, StringBuilder sb)
        {
            foreach (var sln in slns)
            {
                sb.AppendLine($"Project(\"{{{sln.ParentId}}}\" = \"{sln.Name}\", \"{sln.RelativePath}\", \"{{{sln.Id}}}\"");
                sb.AppendLine("EndProject");
                BuildProjectString(sln.Childrens, sb);
            }
        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        public IEnumerable<SlnInfo> GetProjects()
        {
            var list = new List<SlnInfo>();
            HandleProject(list, this.Childrens);
            return list;
        }

        /// <summary>
        /// 处理项目
        /// </summary>
        /// <param name="list">列表</param>
        /// <param name="source">源项目信息</param>
        private void HandleProject(IList<SlnInfo> list, IList<SlnInfo> source)
        {
            foreach (var item in source)
            {
                if (item.Type == SlnType.Project)
                {
                    list.Add(item);
                    continue;
                }
                HandleProject(list, item.Childrens);
            }
        }

        /// <summary>
        /// 获取解决方案关系
        /// </summary>
        public IDictionary<string, string> GetSlnRelation()
        {
            var dict = new Dictionary<string, string>();
            HandleSlnRelation(dict, this.Childrens);
            return dict;
        }

        /// <summary>
        /// 处理解决方案关系
        /// </summary>
        /// <param name="dict">字典</param>
        /// <param name="source">源集合</param>
        private void HandleSlnRelation(IDictionary<string, string> dict, IList<SlnInfo> source)
        {
            foreach (var item in source)
            {
                if (item.Type == SlnType.Root)
                    continue;
                if (item.Type == SlnType.Dir)
                {
                    if (item.Parent.Type != SlnType.Root)
                    {
                        dict[item.Id] = item.ParentId;
                    }
                    HandleSlnRelation(dict, item.Childrens);
                    continue;
                }

                dict[item.Id] = item.ParentId;
            }
        }
    }
}
