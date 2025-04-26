using Cysharp.Threading.Tasks;

namespace Books.Story
{
    internal partial class Logic
    {
        [Logic(LogicIdx.Hero)]
        private async UniTask RunHero(string header, string attributes, string body)
        {
            _mainCharacter = body.Trim();
        }
    }
}
