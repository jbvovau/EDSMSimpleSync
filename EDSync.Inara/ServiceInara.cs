using Newtonsoft.Json;
using System.Collections.Generic;
using EDSMDomain.Models;
using EDSMDomain.Services;
using EDSync.Core;
using EDSync.Inara.Api;

namespace EDSync.Inara
{
    public class ServiceInara : IServiceJournal
    {
        public ServiceInara(ApiInara api)
        {
            this.Api = api;
            this.Status = new InaraStatus();
        }

        public ApiInara Api { get; private set; }

        public InaraStatus Status { get; private set; }

        public IList<string> ManagedEvents { get; private set; }

        public bool IsEventDiscarded(string name)
        {
            if (ManagedEvents == null)
            {
                ManagedEvents = new List<string> { "LoadGame","Rank","Progress", "LoadGame", "FSDJump" };
            }

            return !ManagedEvents.Contains(name);
        }

        public JournalResponse TestConnection()
        {
            var response = this.Api.GetCommanderProfile("Baalmeyer");

            var j = new JournalResponse();

            if (response?.Events?.Count > 0)
            {
                j.MessageNumber = (response.Events[0].Code == 200) ? 100 : 0;

            }

            return j;
        }


        public JournalResponse PostJournalEntry(string data)
        {
            var response = new JournalResponse();

            response.MessageNumber = 100;
            response.Message = this.manageEntry(data);

            return response;
        }

        /// <summary>
        /// Post batch
        /// </summary>
        /// <param name="lines"></param>
        /// <returns></returns>
        public JournalResponse PostJournalEntry(IList<string> lines)
        {
            var response = new JournalResponse();
            response.Message = "Added to Inara";
            response.MessageNumber = 100;
            foreach (var line in lines)
            {
                var detail = this.PostJournalEntry(line);
                response.Details.Add(detail);
            }

            return response;
        }

        #region commit result to Inara

        /// <summary>
        /// Commit all changes
        /// </summary>
        public IEnumerable<string> Commit()
        {
            if (this.Status.Timestamp == null || !this.Status.ShallUpdate)
            {
                yield return "No update";
            }
            else
            {
                // commit credits
                this.Api.SetCommanderCredits(this.Status.Timestamp, this.Status.Credits, this.Status.Assets,
                    this.Status.Loan);
                yield return "Credits Updated";

                // commit ranks
                this.Api.SetCommanderRankPilot(this.Status.Timestamp, "combat", this.Status.CombatRank,
                    this.Status.CombatProgress / 100f);
                this.Api.SetCommanderRankPilot(this.Status.Timestamp, "trade", this.Status.TradeRank,
                    this.Status.TradeProgress / 100f);
                this.Api.SetCommanderRankPilot(this.Status.Timestamp, "explore", this.Status.ExploreRank,
                    this.Status.ExploreProgress / 100f);
                this.Api.SetCommanderRankPilot(this.Status.Timestamp, "federation", this.Status.FederationRank,
                    this.Status.FederationProgress / 100f);
                this.Api.SetCommanderRankPilot(this.Status.Timestamp, "empire", this.Status.EmpireRank,
                    this.Status.EmpireProgress / 100f);
                this.Api.SetCommanderRankPilot(this.Status.Timestamp, "cqc", this.Status.CQCRank,
                    this.Status.CQCProgress / 100f);

                yield return "Ranks updated";

                // powerplay
                this.Api.SetCommanderRankPower(this.Status.Timestamp,this.Status.PowerPlayName, this.Status.PowerPlayRankValue);

                yield return "Power : " + Status.PowerPlayName + " - Rank : " + Status.PowerPlayRankValue;

                // reputation
                this.Api.SetCommanderReputationMajorFaction(this.Status.Timestamp, "empire", Status.ReputationEmpire / 100f);
                this.Api.SetCommanderReputationMajorFaction(this.Status.Timestamp, "federation", Status.ReputationFederation / 100f);
                this.Api.SetCommanderReputationMajorFaction(this.Status.Timestamp, "alliance", Status.ReputationAlliance / 100f);

                yield return string.Format("Reputation. Empire : {0}, Federation : {1}, Alliance : {2}",
                    Status.ReputationEmpire, Status.ReputationFederation, Status.ReputationAlliance);

                // commit
                var response = this.Api.Commit();




                this.Status.ShallUpdate = false;
            }
        }

        #endregion

        #region status analyzer

        private string manageEntry(string line)
        {
            const string no_message = "No action";
            string message = no_message;

            var name = Utils.GetName(line);

            switch (name)
            {
                case "LoadGame":
                    message = manageCredits(line);
                    break;
                case "Rank":
                    message = manageRank(line);
                    break;
                case "Progress":
                    message = manageProgress(line);
                    break;
                case "Reputation":
                    message = manageReputation(line);
                    break;
                case "FSDJump":
                    message = manageFSDJump(line);
                    break;
            }

            if (message != no_message)
            {
                this.Status.ShallUpdate = true;
            }

            return message;
        }

        /// <summary>
        /// Extract credits
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string manageCredits(string line)
        {
            dynamic json = JsonConvert.DeserializeObject(line);
            this.Status.Timestamp = json.timestamp;
            this.Status.Credits = json.Credits;
            this.Status.Loan = json.Loan;

            return string.Format("Credits : {0}, Loan : {1}", Status.Credits, Status.Loan);
        }

        private string manageRank(string line)
        {
            dynamic json = JsonConvert.DeserializeObject(line);
            this.Status.CombatRank = json.Combat;
            this.Status.TradeRank = json.Trade;
            this.Status.ExploreRank = json.Explore;
            this.Status.FederationRank = json.Federation;
            this.Status.EmpireRank = json.Empire;
            this.Status.CQCRank = json.CQC;

            return "Ranks updated";
        }

        private string manageProgress(string line)
        {
            dynamic json = JsonConvert.DeserializeObject(line);
            this.Status.CombatProgress = json.Combat;
            this.Status.TradeProgress = json.Trade;
            this.Status.ExploreProgress = json.Explore;
            this.Status.FederationProgress = json.Federation;
            this.Status.EmpireProgress = json.Empire;
            this.Status.CQCProgress = json.CQC;

            return "Progress updated";
        }

        private string manageReputation(string line)
        {
            dynamic json = JsonConvert.DeserializeObject(line);
            if (json.Empire != null)  this.Status.ReputationEmpire = json.Empire;
            if (json.Alliance != null) this.Status.ReputationAlliance = json.Alliance;
            if (json.Federation != null) this.Status.ReputationFederation = json.Federation;

            return "Reputation updated";
        }

        private string manageFSDJump(string line)
        {
            dynamic json = JsonConvert.DeserializeObject(line);

            string timestamp = json.timestamp;
            float jumpdist = (float) json.JumpDist;
            string starsystem = json.StarSystem;


            Api.AddCommanderTravelFSDJump(timestamp, starsystem, jumpdist, "", 0);

            return null;
        }

        #endregion

        #region status sender


        #endregion 
    }
}
