using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bing.CodeGenerator.Entity;
using Bing.CodeGenerator.Helpers;

namespace Bing.CodeGenerator.Core
{
    /// <summary>
    /// 解决方案信息
    /// </summary>
    public class SlnInfo
    {
        /// <summary>
        /// 解决方案标识
        /// </summary>
        private static string _solutionId = "2150E333-8FDC-42A3-9474-1A3956D46DE8";

        /// <summary>
        /// 项目方案标识
        /// </summary>
        private static string _projectId = "9A19103F-16F7-4668-BE54-9A1E7A4F7556";

        /// <summary>
        /// 解决方案标识
        /// </summary>
        public static string SolutionId => _solutionId ?? (_solutionId = Guid.NewGuid().ToString().ToUpper());

        /// <summary>
        /// 项目方案标识
        /// </summary>
        public static string ProjectId => _projectId ?? (_projectId = Guid.NewGuid().ToString().ToUpper());

        /// <summary>
        /// 标识
        /// </summary>
        public string Id { get; private set; } = Guid.NewGuid().ToString().ToUpper();

        /// <summary>
        /// 方案类型
        /// </summary>
        public SlnType Type { get; private set; } = SlnType.Root;

        /// <summary>
        /// VS解决方案项目类型ID
        /// </summary>
        public string VsProjectTypeId
        {
            get
            {
                switch (Type)
                {
                    case SlnType.Dir:
                        return VsProjectType.SolutionFolder;
                    case SlnType.Project:
                        return VsProjectType.CSharpSdk;
                }
                return string.Empty;
            }
        }

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
        /// 解决方案信息列表，用于存放解决方案所有关系应用
        /// </summary>
        public static List<SlnInfo> SlnInfos { get; } = new List<SlnInfo>();

        /// <summary>
        /// 添加解决方案目录
        /// </summary>
        /// <param name="uuid">解决方案目录ID</param>
        /// <param name="name">解决方案目录名称</param>
        /// <param name="parentId">父解决方案目录ID</param>
        public void AddDir(string uuid, string name, string parentId = "")
        {
            if (string.IsNullOrWhiteSpace(uuid))
                throw new ArgumentNullException(nameof(uuid));
            if (!string.IsNullOrWhiteSpace(parentId) && !Exists(parentId)) 
                throw new ArgumentException($"指定父解决方案目录ID[{parentId}]不存在");

            var sln = new SlnInfo
            {
                Id = uuid,
                Type = SlnType.Dir,
                Name = name,
                RelativePath = name,
                ParentId = parentId
            };
            AddItem(sln);
        }

        /// <summary>
        /// 添加项目并自定义路径
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <param name="path">项目路径</param>
        /// <param name="parentId">父解决方案目录ID</param>
        public void AddProjectWithPath(string name, string path, string parentId = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(path))
                throw new ArgumentNullException(nameof(path));
            if (!string.IsNullOrWhiteSpace(parentId) && !Exists(parentId)) 
                throw new ArgumentException($"指定父解决方案目录ID[{parentId}]不存在");
            var sln = new SlnInfo
            {
                Id = Guid.NewGuid().ToString().ToUpperInvariant(),
                Type = SlnType.Project,
                Name = name,
                RelativePath = path,
                ParentId = parentId,
            };
            AddItem(sln);
        }

        /// <summary>
        /// 添加项目
        /// </summary>
        /// <param name="name">项目名称</param>
        /// <param name="parentId">父解决方案目录ID</param>
        public void AddProject(string name, string parentId = "") => AddProjectWithPath(name, $"src\\{name}\\{name}.csproj", parentId);

        /// <summary>
        /// 添加项
        /// </summary>
        /// <param name="info">解决方案信息</param>
        private void AddItem(SlnInfo info)
        {
            if (SlnInfos.Exists(x => x.Id == info.Id))
                return;
            SlnInfos.Add(info);
        }

        /// <summary>
        /// 是否存在指定GUID的解决方案目录ID
        /// </summary>
        /// <param name="uuid">解决方案目录ID</param>
        private bool Exists(string uuid) => SlnInfos.Exists(x => x.Id == uuid);

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="schemas">架构集合</param>
        public void InitData(string module, IEnumerable<Schema> schemas)
        {
            // 初始化解决方案目录
            InitSolutionFolder();
            InitProject(module, schemas);
            //AddProject(test, $"{module}.Tests", $"test\\{module}.Tests\\{module}.Tests.csproj");
            //AddProject(test, $"{module}.Tests.Integration", $"test\\{module}.Tests.Integration\\{module}.Tests.Integration.csproj");
        }

        /// <summary>
        /// 初始化解决方案目录
        /// </summary>
        private void InitSolutionFolder()
        {
            // src
            AddDir(VsProjectId.Src, "src");
            // src -> 01-Presentation
            AddDir(VsProjectId.Presentation, "01-Presentation", VsProjectId.Src);
            // src -> 02-Service
            AddDir(VsProjectId.Service, "02-Service", VsProjectId.Src);
            // src -> 03-Domain
            AddDir(VsProjectId.Domain, "03-Domain", VsProjectId.Src);
            // src -> 04-Infrastructure
            AddDir(VsProjectId.Infrastructure, "04-Infrastructure", VsProjectId.Src);
            // test
            AddDir(VsProjectId.Test, "test");
        }

        /// <summary>
        /// 初始化项目
        /// </summary>
        /// <param name="module">模块</param>
        /// <param name="schemas">架构集合</param>
        private void InitProject(string module, IEnumerable<Schema> schemas)
        {
            // 01-Presentation
            AddProject(module, VsProjectId.Presentation);
            // 02-Service
            AddProject($"{module}.Service", VsProjectId.Presentation);
            // 03-Domain
            if (!schemas.Any())
            {
                AddProject($"{module}.Domain", VsProjectId.Domain);
            }
            else
            {
                foreach (var schema in schemas)
                {
                    if(!schema.Tables.Any())
                        continue;
                    var domainName = schema.Name.ToLowerInvariant().Equals("dbo")
                        ? $"{module}.Domain"
                        : $"{module}.{schema.Name}.Domain";
                    AddProject(domainName, VsProjectId.Domain);
                }
            }
            // 04-Infrastructure
            AddProject($"{module}.Infrastructure", VsProjectId.Infrastructure);
            AddProject($"{module}.Data", VsProjectId.Infrastructure);
        }

        /// <summary>
        /// 获取项目字符串
        /// </summary>
        public string GetProjectString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var sln in SlnInfos)
            {
                // 格式（解决方案目录）：
                // Project("{2150E333-8FDC-42A3-9474-1A3956D46DE8}") = "src", "src", "{sln.Id}"
                // EndProject
                // 格式（项目）：
                // Project("{9A19103F-16F7-4668-BE54-9A1E7A4F7556}") = "Bing.Admin", "src\Bing.Admin\Bing.Admin.csproj", "{sln.Id}"
                // EndProject
                sb.AppendLine($"Project(\"{{{sln.VsProjectTypeId}}}\") = \"{sln.Name}\", \"{sln.RelativePath}\", \"{{{sln.Id}}}\"");
                sb.AppendLine("EndProject");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取项目列表
        /// </summary>
        public IEnumerable<SlnInfo> GetProjects() => SlnInfos.Where(x => x.Type == SlnType.Project);

        /// <summary>
        /// 获取解决方案关系
        /// </summary>
        public IDictionary<string, string> GetSlnRelation()
        {
            var dict = new Dictionary<string, string>();
            foreach (var item in SlnInfos.Where(x => !string.IsNullOrWhiteSpace(x.ParentId)))
                dict[item.Id] = item.ParentId;
            return dict;
        }
    }
}
