using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SkypeVoiceChanger.Effects
{
    public class EffectChain : IEnumerable<Effect>
    {
        public List<Effect> effects;

        public EffectChain()
        {
            effects = new List<Effect>();
        }

        public void Add(Effect effect)
        {
            effects.Add(effect);
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
                return true;
            }
            return false;
        }

        public void Clear()
        {
            effects.Clear();
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
            return effects.Remove(effect);
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
