using Microsoft.Data.Sqlite;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 分类树与知识点条目的数据访问. 一次查全表后内存组装树, 减少连接往返.
/// </summary>
internal static class CategoryRepository
{
    /// <summary>
    /// 加载所有分类并按 parent_id 组装成树, 返回一级分类节点列表.
    /// </summary>
    public static List<CategoryNode> LoadTree()
    {
        var all = new List<Category>();

        using (var connection = new SqliteConnection(Database.ConnectionString))
        {
            connection.Open();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT id, name, parent_id FROM categories ORDER BY id";
            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                all.Add(new Category(
                    Id: reader.GetInt32(0),
                    Name: reader.GetString(1),
                    ParentId: reader.IsDBNull(2) ? null : reader.GetInt32(2)));
            }
        }

        return BuildTree(all, parentId: null);
    }

    /// <summary>
    /// 递归组装: 找出 parent_id 等于指定值的分类作为子节点, 再向下递归.
    /// </summary>
    private static List<CategoryNode> BuildTree(List<Category> all, int? parentId)
    {
        var nodes = new List<CategoryNode>();
        foreach (var cat in all)
        {
            if (cat.ParentId == parentId)
                nodes.Add(new CategoryNode(cat, BuildTree(all, cat.Id)));
        }
        return nodes;
    }

    /// <summary>
    /// 加载某分类下的所有知识点条目. 参数化查询.
    /// </summary>
    public static List<KnowledgeItem> LoadItemsByCategory(int categoryId)
    {
        var items = new List<KnowledgeItem>();

        using var connection = new SqliteConnection(Database.ConnectionString);
        connection.Open();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT id, category_id, title, description, difficulty FROM items WHERE category_id = @cid ORDER BY id";
        cmd.Parameters.AddWithValue("@cid", categoryId);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            items.Add(new KnowledgeItem(
                Id: reader.GetInt32(0),
                CategoryId: reader.GetInt32(1),
                Title: reader.GetString(2),
                Description: reader.GetString(3),
                Difficulty: reader.GetString(4)));
        }

        return items;
    }
}
