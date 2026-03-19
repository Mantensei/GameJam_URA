using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA
{
    public interface INormaProvider
    {
        void GetAllNormas(List<Norma> result);
    }

    public abstract class Norma : ScriptableObject, INormaProvider
    {
        public bool IsCompleted { get; private set; }

        protected virtual void OnComplete() { }
        public void CompleteNorma()
        {
            IsCompleted = true;
            OnComplete();
        }

        public void GetAllNormas(List<Norma> result)
        {
            result.Add(Instantiate(this));
        }
    }

}
