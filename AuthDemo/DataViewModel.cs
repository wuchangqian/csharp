using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CSharpLearningProject.ViewModels;

namespace CSharpLearningProject.AuthDemo;

/// <summary>
/// 数据窗口视图模型: 左树右列表, 选中树节点联动加载对应条目.
/// </summary>
public partial class DataViewModel : ViewModelBase
{
    public ObservableCollection<CategoryNode> Roots { get; } = new();

    // ListView 不能直接绑定 record 属性, 用显式属性集合.
    public ObservableCollection<KnowledgeItem> Items { get; } = new();

    [ObservableProperty]
    private CategoryNode? _selectedNode;

    public DataViewModel()
    {
        // 在 UI 线程构造, 建库已完成, 直接同步加载树.
        var tree = CategoryRepository.LoadTree();
        foreach (var node in tree)
            Roots.Add(node);

        // 默认选中第一个一级分类, 触发列表加载.
        if (Roots.Count > 0)
            SelectedNode = Roots[0];
    }

    partial void OnSelectedNodeChanged(CategoryNode? value)
    {
        Items.Clear();
        if (value is null)
            return;

        var items = CategoryRepository.LoadItemsByCategory(value.Category.Id);
        foreach (var item in items)
            Items.Add(item);
    }
}
