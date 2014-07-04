using System;
using System.Collections.Generic;

namespace SkypeVoiceChanger.Effects
{
    public class EffectChain : IEnumerable<Effect>
    {
        private readonly List<Effect> effects;
        public event EventHandler<EventArgs> Modified;

        protected virtual void OnModified()
        {
            EventHandler<EventArgs> handler = Modified;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        public EffectChain()
        {
            effects = new List<Effect>();
        }

        public void Add(Effect effect)
        {
            effects.Add(effect);
            OnModified();
        }

        public bool MoveUp(Effect effect)
        {
            int index = effects.IndexOf(effect);
            if (index == -1)
            {
                throw new ArgumentException("The specified effect is not part of this effect chain");
            }
            if (index > 0)
            {
                effects.RemoveAt(index);
                effects.Insert(index - 1, effect);
                OnModified();
                return true;
            }
            return false;
        }

        public bool MoveDown(Effect effect)
        {
            int index = effects.IndexOf(effect);
            if (index == -1)
            {
                throw new ArgumentException("The specified effect is not part of this effect chain");
            }
            if (index < Count-1)
            {
                effects.RemoveAt(index);
                effects.Insert(index + 1, effect);
                OnModified();
                return true;
            }
            return false;
        }

        public void Clear()
        {
            effects.Clear();
            OnModified();
        }

        public bool Contains(Effect item)
        {
            return effects.Contains(item);
        }

        public int Count
        {
            get { return effects.Count; }
        }

        public bool Remove(Effect effect)
        {
            var removed = effects.Remove(effect);
            if (removed)
                OnModified();
            return removed;
        }

        public IEnumerator<Effect> GetEnumerator()
        {
            return effects.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return effects.GetEnumerator();
        }
    }
}
