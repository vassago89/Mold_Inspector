using Mold_Inspector.Config;
using Mold_Inspector.Model.Algorithm;
using Mold_Inspector.Model.Algorithm.BinaryCount;
using Mold_Inspector.Model.Algorithm.PatternMatching;
using Mold_Inspector.Model.Algorithm.Profile;
using Mold_Inspector.Store.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mold_Inspector.Store
{
    class DefaultStore : ConfigStore<DefaultConfig>
    {
        public PatternMatchingAlgorithm Pattern { get; set; }
        public BinaryCountAlgorithm Binary { get; set; }
        public ProfileAlgorithm Profile { get; set; }

        protected override void CopyFrom(DefaultConfig config)
        {
            Pattern =  config.Pattern;
            Binary = config.Binary;
            Profile = config.Profile;
        }

        protected override void CopyTo(DefaultConfig config)
        {
            config.Pattern = Pattern;
            config.Binary = Binary;
            config.Profile = Profile;
        }

        protected override void Initialize()
        {
            
        }

        public IAlgorithm GetDefault(AlgorithmType type)
        {
            switch (type)
            {
                case AlgorithmType.Pattern:
                    return Pattern;
                case AlgorithmType.Binary:
                    return Binary;
                case AlgorithmType.Profile:
                    return Profile;
            }

            return null;
        }
    }
}
