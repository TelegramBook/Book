using Cysharp.Threading.Tasks;
using Shared.Disposable;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Books.Story
{
    internal partial class Logic : BaseDisposable
    {
        [AttributeUsage(AttributeTargets.Method)]
        private class LogicAttribute : Attribute
        {
            public string Name { get; }
            public LogicAttribute(string name) => Name = name;
        }

        public struct Ctx
        {
            public View.IBubble Bubble;
            public string StoryText;
        }

        private readonly Ctx _ctx;

        private Ink.Runtime.Story _story;

        private string _mainCharacter;

        public Logic(Ctx ctx)
        {
            _ctx = ctx;

            _ctx.Bubble.SetActive(false); 
        }

        public async UniTask ShowStoryProcess()
        {
            var logics = typeof(Logic).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(m => m.GetCustomAttribute<LogicAttribute>() != null)
                .ToDictionary(m =>
                {
                    return m.GetCustomAttribute<LogicAttribute>().Name;
                }, m =>
                {
                    return (Func<string, string, string, UniTask>) Delegate.CreateDelegate(typeof(Func<string, string, string, UniTask>), this, m);
                });

            _story = new Ink.Runtime.Story(_ctx.StoryText);

            _story.Continue();

            _mainCharacter = string.Empty;
            var storyInProgress = true;
            while (storyInProgress)
            {
                _ctx.Bubble.SetActive(false);

                while (_story.canContinue)
                {
                    _ctx.Bubble.SetActive(false);

                    if (!_story.Continue().TryProcessLine(out var header, out var attributes, out var body)) continue;

                    var headerForLogic = header.ToLower();
                    if (logics.TryGetValue(headerForLogic, out var func))
                    {
                        await func.Invoke(header, attributes, body);
                        continue;
                    }

                    switch (headerForLogic)
                    {
                        case "background":
                            continue;
                        case "music":
                            continue;
                        case "sound":
                            continue;
                        case "push":
                            continue;
                    }

                    var side = View.Bubble.Side.Right;
                    if (headerForLogic == _mainCharacter.ToLower()) side = View.Bubble.Side.Left;
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
                    _ctx.Bubble.SetActive(false);
                    _story = new Ink.Runtime.Story(_ctx.StoryText);
                }
            }
        }
    }
}

