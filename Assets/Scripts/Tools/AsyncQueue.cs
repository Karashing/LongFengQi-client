using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;

namespace ToolI {
    public class AsyncQueue<T> {
        private readonly ConcurrentQueue<T> _queue = new ConcurrentQueue<T>();
        private readonly SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void Enqueue(T item) {
            _queue.Enqueue(item);
            _signal.Release();
        }

        public async Task<T> Dequeue() {
            await _signal.WaitAsync();
            _queue.TryDequeue(out T item);
            return item;
        }
    }
}

// 使用示例
public class Program {
    public static async Task Main(string[] args) {
        var queue = new ToolI.AsyncQueue<int>();

        // 生产者任务
        var producer = Task.Run(async () => {
            for (int i = 0; i < 5; i++) {
                queue.Enqueue(i);
                Console.WriteLine($"Enqueued: {i}");
                await Task.Delay(1000); // 模拟生产延迟
            }
        });

        // 消费者任务
        var consumer = Task.Run(async () => {
            while (true) {
                var item = await queue.Dequeue();
                Console.WriteLine($"Dequeued: {item}");
                // 为了示例，这里只消费5个项目然后退出
                if (item == 4) break;
            }
        });

        // 等待生产者和消费者任务完成
        await Task.WhenAll(producer, consumer);

        Console.WriteLine("All tasks completed.");
    }
}
