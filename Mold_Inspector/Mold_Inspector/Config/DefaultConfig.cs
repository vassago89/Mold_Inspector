using Mold_Inspector.Config.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Model.Algorithm.BinaryCount;
using Mold_Inspector.Model.Algorithm.Profile;

namespace Mold_Inspector.Config
{
    [Serializable]
    class DefaultConfig : BinaryConfig<DefaultConfig>
    {
        public PatternMatchingAlgorithm Pattern { get; set; }
        public BinaryCountAlgorithm Binary { get; set; }
        public ProfileAlgorithm Profile { get; set; }

        public DefaultConfig()
        {
            Pattern = new PatternMatchingAlgorithm();
            Binary = new BinaryCountAlgorithm();
            Profile = new ProfileAlgorithm();
        }
    }
}
