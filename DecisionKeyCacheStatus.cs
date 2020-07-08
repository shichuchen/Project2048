using System;
using System.Collections.Generic;
using System.Linq;

namespace Project2048
{
    class DecisionKeyCacheStatus<TDecision, TStatus>: ICacheDecision<TDecision>
        where TStatus: class, new()
    {   
        public DecisionKeyCacheStatus(TDecision initBestDecision)
        {
            this.initBestDecision = initBestDecision;
            BestDecision = initBestDecision;
        }
        private Dictionary<TDecision, TStatus> decisionStatusMap;
        private TDecision[] prevDecisions;

        public TDecision BestDecision { get; set; }
        private readonly TDecision initBestDecision;
        public void AddDecision(TDecision decision)
        {   
            if(decisionStatusMap is null)
            {
                decisionStatusMap = new Dictionary<TDecision, TStatus>();
            }
            decisionStatusMap[decision] = new TStatus();

        }
        public TStatus GetStatus(TDecision decision)
        {
            return decisionStatusMap[decision];
        }
        public TDecision[] GetDecisions()
        {
            if(Equals(initBestDecision, BestDecision))
            {
                if(prevDecisions is null)
                {
                    prevDecisions = decisionStatusMap.Keys.ToArray();
                }
            }
            else
            {
                SortDecisions();
            }
            return prevDecisions;
        }

        private void SortDecisions()
        {
            if(Equals(prevDecisions[0], BestDecision))
            {
                return;
            }
            int index = 1;
            for(; index < prevDecisions.Length; ++index)
            {
                if(Equals(prevDecisions[index], BestDecision))
                {
                    break;
                }
            }
            var temp = prevDecisions[index];
            prevDecisions[index] = prevDecisions[0];
            prevDecisions[0] = temp;
            
        }
    }
}
