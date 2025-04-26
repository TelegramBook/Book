using Cysharp.Threading.Tasks;

namespace Books.Story
{
    internal partial class Logic
    {
        [Logic(LogicIdx.Background)]
        private async UniTask<bool> RunBackground(string header, string attributes, string body)
        {
            await UniTask.NextFrame();
            return true;
        }
    }
}
