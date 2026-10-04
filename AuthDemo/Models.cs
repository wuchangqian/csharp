namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 用户记录: 密码只存 bcrypt 哈希, 永不保存明文.
/// </summary>
public record User(int Id, string Username, string PasswordHash);

/// <summary>
/// 学习资源分类: parent_id 为 null 表示一级分类.
/// </summary>
public record Category(int Id, string Name, int? ParentId);

/// <summary>
/// 树节点: 持有一个分类及其子节点, 供 TreeView 层级绑定.
/// </summary>
public record CategoryNode(Category Category, List<CategoryNode> Children);

/// <summary>
/// 分类下的知识点条目, 显示在 ListView.
/// </summary>
public record KnowledgeItem(int Id, int CategoryId, string Title, string Description, string Difficulty);
