using Cysharp.Threading.Tasks;
using Shared.Disposable;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Books.Story 
{
    internal class Logic : BaseDisposable
    {
        public struct Ctx
        {
            public Data Data;
            public string StoryText;
        }

        private readonly Ctx _ctx;

        private readonly Stack<GameObject> _units;

        private Ink.Runtime.Story _story;

        public Logic(Ctx ctx)
        {
            _ctx = ctx;

            _units = new();

            _ctx.Data.StoryBubble.gameObject.SetActive(false);
        }

        public async UniTask ShowStoryProcess()
        {
            _story = new Ink.Runtime.Story(_ctx.StoryText);

            _story.Continue();

            var mainCharacter = string.Empty;
            var storyInProgress = true;
            while (storyInProgress)
            {
                ClearAll();
                View.Bubble storyBubble = null;
                while (_story.canContinue)
                {
                    ClearAll();
                    storyBubble = null;

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

                    storyBubble = CreateStoryBubble(side, header, body);

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

                    if (storyBubble == null)
                        storyBubble = CreateStoryBubble(View.Bubble.Side.Center, string.Empty, string.Empty);

                    var buttons = _story.currentChoices.Select(c => (c.text, c.index)).ToArray();
                    storyBubble.UpdateButtons(idx =>
                    {
                        _story.ChooseChoiceIndex(idx);
                        waitChoice = false;
                    }, buttons);

                    if (buttons.Length == 1 && buttons[0].text.Trim().ToLower() == "играть")
                    {
                        _story.ChooseChoiceIndex(buttons[0].index);
                        waitChoice = false;
                    }
                    else
                    {
                        while (waitChoice)
                            await UniTask.NextFrame();

                        await UniTask.Delay(100);
                    }
                }
                else
                {
                    ClearAll();
                    _story = new Ink.Runtime.Story(_ctx.StoryText);
                }
            }
        }

        private View.Bubble CreateStoryBubble(View.Bubble.Side side, string header, string body)
        {
            var storyBubble = UnityEngine.Object.Instantiate(_ctx.Data.StoryBubble);
            storyBubble.transform.SetParent(_ctx.Data.StoryBubble.transform.parent, false);
            storyBubble.gameObject.SetActive(true);
            storyBubble.UpdateText(side, header, body);

            _units.Push(storyBubble.gameObject);

            return storyBubble;
        }

        protected override void OnDispose()
        {
            ClearAll();
            base.OnDispose();
        }

        private void ClearAll()
        {
            while (_units.TryPop(out var unitGO))
                UnityEngine.Object.Destroy(unitGO);
        }
    }
}

