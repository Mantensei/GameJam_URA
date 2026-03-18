using System;
using System.Collections.Generic;
using UnityEngine;

namespace GameJam_URA
{
    public interface INormaProvider
    {
        void GetAllNormas(List<Norma> result);
    }

    [Serializable]
    public class MenuItem
    {
        [SerializeField] string itemName;
        [SerializeField] int price;

        public string ItemName => itemName;
        public int Price => price;

        public bool IsSecretMenu { get; set; }
    }

    [Serializable]
    public class Comment
    {
        [SerializeField] string text;

        public string Text => text;
    }

    public abstract class Norma : ScriptableObject, INormaProvider
    {
        public bool IsCompleted { get; set; }

        public void GetAllNormas(List<Norma> result)
        {
            result.Add(Instantiate(this));
        }
    }

}
