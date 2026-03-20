using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA
{
    public interface IUraTask
    {
        string Name { get; }
        bool IsCompleted { get; }
        void Complete();
    }

    public interface IDishItem : IUraTask
    {
        int Price { get; }
        string Category { get; }
        bool IsSecretMenu { get; set; }
    }

    public interface ICommentItem : IUraTask
    {
        CommentPhase Phase { get; }
    }

    public interface IUraTaskProvider
    {
        void GetAllTasks(List<IUraTask> result);
    }

    [Obsolete("SO基底。今後はIUraTaskを直接実装すること")]
    public abstract class Norma : ScriptableObject, IUraTask, IUraTaskProvider
    {
        public string Name => name;
        public bool IsCompleted { get; private set; }

        protected virtual void OnComplete() { }
        public void Complete()
        {
            IsCompleted = true;
            OnComplete();
        }

        public void GetAllTasks(List<IUraTask> result)
        {
            result.Add(Instantiate(this));
        }
    }
}
