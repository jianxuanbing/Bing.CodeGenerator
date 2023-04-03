# Bing.CodeGenerator
## 生成可执行文件
```
# 集成环境
dotnet publish -c release -r win-x64 --self-contained false /p:PublishSingleFile=true
# 独立环境
dotnet publish -c release -r win-x64 --self-contained true /p:PublishSingleFile=true
```