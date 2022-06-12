using System;
using System.Collections;

namespace Gmap.ABLG 
{
    interface ILevelGenAlgo
    {
        IEnumerator Run(Level l, System.Action<Level> updateVis=null);
    }

    public class LevelGenAlgoEmpty : ILevelGenAlgo
    {
        public IEnumerator Run(Level l, Action<Level> updateVis = null)
        {
            yield return null;
        }
    }
}