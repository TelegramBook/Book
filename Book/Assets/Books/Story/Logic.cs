using Cysharp.Threading.Tasks;
using Shared.Disposable;
using System;
using System.Collections.Generic;
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
            public string[] Names { get; }
            public LogicAttribute(params string[] names) => Names = names;
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
            var logics = GetDelegats<Func<string, string, string, UniTask>>();

            _story = new Ink.Runtime.Story(_ctx.StoryText);

            _story.Continue();

            _mainCharacter = string.Empty;
            var storyInProgress = true;
            while (storyInProgress)
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

                var waitChoice = true;
                var buttons = _story.currentChoices.Select(c => (c.text, c.index)).ToArray();

                _ctx.Bubble.UpdateText(side, header, body, idx => 
                {
                    if (idx >= 0) _story.ChooseChoiceIndex(idx);
                    waitChoice = false;
                }, buttons);

                while (waitChoice) await UniTask.NextFrame();

                await UniTask.Delay(100);
            }
        }

        private Dictionary<string, T> GetDelegats<T>() where T : Delegate
        {
            var flags = BindingFlags.NonPublic | BindingFlags.Instance;
            return typeof(Logic).GetMethods(flags).Where(m => m.GetCustomAttribute<LogicAttribute>() != null)
                .SelectMany(m =>
                {
                    var attr = m.GetCustomAttribute<LogicAttribute>();
                    var del = (T)Delegate.CreateDelegate(typeof(T), this, m);
                    return attr.Names.Select(n => (n, del));
                }).ToDictionary(m => m.n, m => m.del);
        }
    }
}

