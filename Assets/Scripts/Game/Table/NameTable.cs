using System.Collections.Generic;
using UnityEngine;
using Util;
using Random = System.Random;

namespace Game.Table
{
    [CreateAssetMenu(fileName = "NameTable", menuName = "Nk/NameTable")]
    public class NameTable : ScriptableObject
    {
        static NameTable _instance;
        public static NameTable instance
        {
            get
            {
                if (_instance == null)
                    CreateInstance();

                return _instance;
            }
        }

        static public void CreateInstance()
        {
            _instance = Resources.Load("Table/NameTable") as NameTable;
        }

        [SerializeField] private List<string> nickNames = new List<string>();

        private List<string> temp = new List<string>();
        public List<string> ShuffleName()
        {
            temp.Clear();
            temp.AddRange(nickNames);
            temp.Shuffle(new Random(7));
            return temp;
        }
    }
}
