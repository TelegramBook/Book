using Cysharp.Threading.Tasks;
using Shared.Disposable;
using System.Linq;
using UnityEngine;

namespace Books.Story 
{
    internal class Logic : BaseDisposable
    {
        public struct Ctx
        {
            public View.IBubble Bubble;
            public string StoryText;
        }

        private readonly Ctx _ctx;

        private Ink.Runtime.Story _story;

        public Logic(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.Bubble.GameObject.SetActive(false);
        }

        public async UniTask ShowStoryProcess()
        {
            _story = new Ink.Runtime.Story(_ctx.StoryText);

            _story.Continue();

            var mainCharacter = string.Empty;
            var storyInProgress = true;
            while (storyInProgress)
            {
                _ctx.Bubble.GameObject.SetActive(false);

                while (_story.canContinue)
                {
                    _ctx.Bubble.GameObject.SetActive(false);

                    if (!_story.Continue().TryProcessLine(out var header, out var attributes, out var body)) continue;

                    var headerForLogic = header.ToLower();
                    switch (headerForLogic)
                    {
                        case "аннотация":
                            continue;
                        case "камера":
                            continue;
                        case "жанры":
                            continue;
                        case "статы":
                            continue;
                        case "локация":
                            continue;
                        case "музыка":
                            continue;
                        case "звук":
                            continue;
                        case "звуки окружения":
                            continue;
                        case "уведомление":
                            continue;
                        case "ожидание":
                            if (int.TryParse(body, out var waitTime))
                            {
                                await UniTask.Delay(waitTime * 1000);
                            }
                            continue;
                        case "кат-сцена":
                            continue;
                        case "клавиатура":
                            mainCharacter = body.Trim();
                            continue;
                    }

                    var side = View.Bubble.Side.Right;
                    if (headerForLogic == mainCharacter.ToLower()) side = View.Bubble.Side.Left;
                    else if (headerForLogic == "...") side = View.Bubble.Side.Center;

                    _ctx.Bubble.UpdateText(side, header, body);

                    if (_story.canContinue)
                    {
                        while (!Input.GetMouseButtonUp(0))
                            await UniTask.NextFrame();

                        await UniTask.Delay(100);
                    }
                }

                if (_story.currentChoices.Count > 0)
                {
                    var waitChoice = true;

                    var buttons = _story.currentChoices.Select(c => (c.text, c.index)).ToArray();
                    _ctx.Bubble.UpdateButtons(idx =>
                    {
                        _story.ChooseChoiceIndex(idx);
                        waitChoice = false;
                    }, buttons);

                    while (waitChoice)
                        await UniTask.NextFrame();

                    await UniTask.Delay(100);
                }
                else
                {
                    _ctx.Bubble.GameObject.SetActive(false);
                    _story = new Ink.Runtime.Story(_ctx.StoryText);
                }
            }
        }
    }
}

