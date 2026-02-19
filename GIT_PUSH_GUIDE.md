# GitHub 推送指南

## 第一步：安装 Git
1. 访问 https://git-scm.com/download/win
2. 下载 Windows 版 Git
3. 运行安装程序，使用默认设置即可

## 第二步：配置 Git 用户信息
打开 Git Bash 或命令提示符，执行：
```bash
git config --global user.name "你的GitHub用户名"
git config --global user.email "你的GitHub邮箱"
```

## 第三种方式：使用批处理脚本推送
直接双击运行项目根目录下的 `push_to_github.bat` 文件

## 手动推送命令
在项目根目录下依次执行：
```bash
# 查看当前状态
git status

# 添加所有文件
git add .

# 提交更改
git commit -m "Initial commit: Minecraft launcher core implementation"

# 添加远程仓库
git remote add origin https://github.com/AhaTeam-CL/KataCore.git

# 推送到 GitHub
git push -u origin main
```

## 注意事项
- `.gitignore` 文件已配置好，会自动忽略 `.minecraft/` 和 `.idea/` 目录
- 确保你有该 GitHub 仓库的推送权限
- 如果推送失败，可能需要先在 GitHub 上创建空仓库

## 验证推送成功
推送完成后，访问 https://github.com/AhaTeam-CL/KataCore 查看代码是否已上传