using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Inara
{
    public class InaraStatus
    {
        public string Timestamp { get; set; }

        public bool ShallUpdate { get; set; }

        public int Credits { get; set; }

        public int Assets { get; set; }

        public int Loan { get; set; }

        #region ranks

        public int CombatRank { get; set; }

        public int CombatProgress { get; set; }

        public int TradeRank { get; set; }

        public int TradeProgress { get; set; }

        public int ExploreRank { get; set; }

        public int ExploreProgress { get; set; }
        
        public int FederationRank { get; set; }

        public int FederationProgress { get; set; }

        public int EmpireRank { get; set; }

        public int EmpireProgress { get; set; }

        public int CQCRank { get; set; }

        public int CQCProgress { get; set; }

        public string PowerPlayName { get; set; }

        public int PowerPlayRankValue { get; set; }

        #endregion

        #region reputation

        public float ReputationEmpire { get; set; }

        public float ReputationFederation { get; set; }

        public float ReputationAlliance { get; set; }
        #endregion


    }
}
