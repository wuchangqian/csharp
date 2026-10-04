namespace CSharpLearningProject.Lessons
{
    /// <summary>
    /// 模块 13: 异步编程
    /// 知识点: async/await、Task、ValueTask、CancellationToken、并发/并行、IAsyncEnumerable
    /// </summary>
    public static class L13_Async
    {
        public static async Task RunAsync()
        {
            L01_Basics.PrintHeader("模块 13: 异步编程");

            // ===== 1. 同步调用 (阻塞) =====
            Console.WriteLine("=== 同步调用 ===");
            var sw = System.Diagnostics.Stopwatch.StartNew();
            string result1 = SyncDownload("http://example.com/1");
            string result2 = SyncDownload("http://example.com/2");
            string result3 = SyncDownload("http://example.com/3");
            sw.Stop();
            Console.WriteLine($"  结果: {result1}, {result2}, {result3}");
            Console.WriteLine($"  同步耗时: {sw.ElapsedMilliseconds}ms (串行 3×100ms)");

            // ===== 2. 异步调用 (不阻塞) =====
            Console.WriteLine("\n=== 异步调用 (串行 await) ===");
            sw.Restart();
            string r1 = await AsyncDownload("http://example.com/1");
            string r2 = await AsyncDownload("http://example.com/2");
            string r3 = await AsyncDownload("http://example.com/3");
            sw.Stop();
            Console.WriteLine($"  结果: {r1}, {r2}, {r3}");
            Console.WriteLine($"  异步串行耗时: {sw.ElapsedMilliseconds}ms (await 串行)");

            // ===== 3. 并发异步 (Task.WhenAll) =====
            Console.WriteLine("\n=== 并发异步 (Task.WhenAll) ===");
            sw.Restart();
            // 三个 Task 同时启动, 不互相等待
            Task<string> t1 = AsyncDownload("http://example.com/1");
            Task<string> t2 = AsyncDownload("http://example.com/2");
            Task<string> t3 = AsyncDownload("http://example.com/3");
            string[] results = await Task.WhenAll(t1, t2, t3);
            sw.Stop();
            Console.WriteLine($"  结果: {string.Join(", ", results)}");
            Console.WriteLine($"  并发耗时: {sw.ElapsedMilliseconds}ms (3个同时跑, 只等100ms)");

            // ===== 4. Task.WhenAny (任一完成) =====
            Console.WriteLine("\n=== Task.WhenAny (先到先用) ===");
            var tasks = new[]
            {
                AsyncDownload("fast", 50),
                AsyncDownload("medium", 150),
                AsyncDownload("slow", 300)
            };
            var firstDone = await Task.WhenAny(tasks);
            Console.WriteLine($"  最先完成: {firstDone.Result}");

            // ===== 5. CancellationToken (取消) =====
            Console.WriteLine("\n=== CancellationToken ===");
            using var cts = new CancellationTokenSource();

            // 1秒后自动取消
            cts.CancelAfter(TimeSpan.FromMilliseconds(500));

            try
            {
                await LongRunningTask(cts.Token);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("  任务被取消");
            }

            // ===== 6. Task.Run (在线程池执行 CPU 密集任务) =====
            Console.WriteLine("\n=== Task.Run (CPU密集) ===");
            long fibResult = await Task.Run(() => SlowFib(35));
            Console.WriteLine($"  Fib(35) = {fibResult} (在后台线程计算)");

            // ===== 7. Parallel.For (数据并行) =====
            Console.WriteLine("\n=== Parallel.For ===");
            int[] data = Enumerable.Range(1, 100).ToArray();
            long total = 0;
            object lockObj = new();
            Parallel.For(0, data.Length, i =>
            {
                Interlocked.Add(ref total, data[i]);  // 线程安全累加
            });
            Console.WriteLine($"  1..100 并行求和 = {total}");

            // ===== 8. async/await 原理 =====
            // async 方法被编译器改写成状态机
            // await = "等这个 Task 完成, 完成后从这里继续"
            // 期间不阻塞线程 (线程可以去干别的)

            // ===== 9. ValueTask (避免 Task 分配开销) =====
            // 如果结果通常已就绪, ValueTask 更高效 (无堆分配)
            int value = await GetValueAsync();
            Console.WriteLine($"\n  ValueTask: {value}");

            // ===== 10. IAsyncEnumerable (异步流) =====
            Console.WriteLine("\n=== IAsyncEnumerable (异步流) ===");
            await foreach (var item in GenerateAsync(5))
                Console.WriteLine($"  产出: {item}");

            Console.WriteLine();
        }

        // 同步方法 (模拟网络下载, 阻塞 100ms)
        static string SyncDownload(string url, int delayMs = 100)
        {
            Thread.Sleep(delayMs);
            return $"[{url}→{delayMs}ms]";
        }

        // 异步方法 (模拟网络下载, 不阻塞线程)
        // async 修饰符 + Task<T> 返回类型
        static async Task<string> AsyncDownload(string url, int delayMs = 100)
        {
            await Task.Delay(delayMs);  // 异步等待, 不阻塞线程
            return $"[{url}→{delayMs}ms]";
        }

        // 可取消的异步任务
        static async Task LongRunningTask(CancellationToken token)
        {
            for (int i = 0; i < 10; i++)
            {
                token.ThrowIfCancellationRequested();  // 检查取消请求
                Console.WriteLine($"  工作中... ({i + 1}/10)");
                await Task.Delay(200, token);  // 可取消的延迟
            }
        }

        // CPU 密集计算 (斐波那契)
        static long SlowFib(int n) =>
            n <= 1 ? n : SlowFib(n - 1) + SlowFib(n - 2);

        // ValueTask: 同步完成时无堆分配
        static ValueTask<int> GetValueAsync()
        {
            // 如果已就绪, 直接返回, 不创建 Task 对象
            int value = 42;
            return new ValueTask<int>(value);
        }

        // IAsyncEnumerable: 异步迭代器
        static async IAsyncEnumerable<int> GenerateAsync(int count)
        {
            for (int i = 0; i < count; i++)
            {
                await Task.Delay(100);  // 模拟异步获取数据
                yield return i;          // 逐个产出
            }
        }
    }
}
