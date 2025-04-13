using Cysharp.Threading.Tasks;

namespace Books.Story 
{
    internal partial class Logic
    {
        [Logic("await")]
        private async UniTask RunAwake(string header, string attributes, string body) 
        {
            if (int.TryParse(body, out var waitTime))
            {
                await UniTask.Delay(waitTime * 1000);
            }
        }
    }
}

