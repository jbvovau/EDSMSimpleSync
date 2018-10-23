using EDSMDomain.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace EDSync.Inara.Api
{
    public class ApiInara : RestApi
    {
        public const string API_JOURNAL = "https://inara.cz/inapi/v1/";

        private IList<InaraEvent> _events;

        private InaraRequest _request;

        public ApiInara()
        {
            this._events = new List<InaraEvent>();
        }

        /// <summary>
        /// Always call commit to send all events
        /// </summary>
        /// <returns></returns>
        public InaraResponse Commit()
        {
            int index = 0;

            InaraResponse result = new InaraResponse();

            while (index < this._events.Count)
            {
                var list = new List<InaraEvent>();

                while (list.Count < 20 && index < _events.Count)
                {
                    list.Add(_events[index++]);
                }

                var posted = this.PostEvents(list);

                result = buildResponse(posted);

                if (_request != null)
                {
                    foreach (var inaraResponseEvent in result.Events)
                    {
                        _request.SetCustomResponse(inaraResponseEvent.CustomID, inaraResponseEvent.eventStatus);
                    }
                }

            }

            this._events.Clear();

            return result;
        }

        /// <summary>
        /// Set commander credits
        /// </summary>
        /// <param name="credits"></param>
        /// <param name="assets"></param>
        /// <param name="loan"></param>
        /// <returns></returns>
        public void SetCommanderCredits(string timestamp, int credits, int assets = 0, int loan = 0)
        {
            var evt = new InaraEvent("setCommanderCredits");
            evt.Timestamp = timestamp;
            evt.AddData("commanderCredits", credits);
            if (assets > 0) evt.AddData("commanderAssets", assets);
            if (loan > 0) evt.AddData("commanderLoan", loan);

            this.Add(evt);
        }

        /// <summary>
        /// Set rank piulot
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="rankName"></param>
        /// <param name="rankValue"></param>
        /// <param name="rankProgress"></param>
        /// <returns></returns>
        public void SetCommanderRankPilot(string timestamp, string rankName, int rankValue, float rankProgress)
        {
            var evt = new InaraEvent("setCommanderRankPilot");
            evt.Timestamp = timestamp;
            evt.AddData("rankName", rankName);
            evt.AddData("rankValue", rankValue);
            evt.AddData("rankProgress", rankProgress);

            this.Add(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="powerName"></param>
        /// <param name="rankValue"></param>
        public void SetCommanderRankPower(string timestamp, string powerName, int rankValue)
        {
            var evt = new InaraEvent("setCommanderRankPower");
            evt.Timestamp = timestamp;
            evt.AddData("powerName", powerName);
            evt.AddData("rankValue", rankValue);

            this.Add(evt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="majorfactionName"></param>
        /// <param name="reputation"></param>
        public void SetCommanderReputationMajorFaction(string timestamp, string majorfactionName, float reputation)
        {
            var evt = new InaraEvent("setCommanderReputationMajorFaction");
            evt.Timestamp = timestamp;
            evt.AddData("majorfactionName", majorfactionName);
            evt.AddData("majorfactionReputation", reputation);

            this.Add(evt);
        }

        /// <summary>
        /// Add Jump travel
        /// </summary>
        /// <param name="timestamp"></param>
        /// <param name="starsystemName"></param>
        /// <param name="jumpDistance"></param>
        /// <param name="shipType"></param>
        /// <param name="shipGameID"></param>
        public void AddCommanderTravelFSDJump(string timestamp, string starsystemName, float jumpDistance,
            string shipType, int shipGameID)
        {
            var evt = new InaraEvent("addCommanderTravelFSDJump");
            evt.Timestamp = timestamp;
            evt.AddData("starsystemName", starsystemName);
            evt.AddData("jumpDistance", jumpDistance);
            if (!string.IsNullOrEmpty(shipType)) evt.AddData("shipType", shipType);
            if (shipGameID > 0) evt.AddData("shipGameID", shipGameID);

            this.Add(evt);
        }

        #region private

        private void Add(InaraEvent inaraEvent)
        {
            this._events.Add(inaraEvent);
        }
        


        private InaraResponse buildResponse(string data)
        {
            if (data == null) return null;
            var result = JsonConvert.DeserializeObject<InaraResponse>(data);
            return result;
        }

        /// <summary>
        /// Post a list of events
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        private string PostEvents(IList<InaraEvent> events)
        {
            var result = "";

            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, API_JOURNAL))
            {
                var input = new InaraRequest();
                input.Header = new Header
                {
                    appName = this.FromSoftware,
                    appVersion = this.FromSoftwareVersion,
                    isDeveloped = true,
                    APIkey = this.ApiKey,
                    commanderName = this.CommanderName
                };

                input.Events = events;

                this._request = input;
                this._request.Events = events;

                var json = JsonConvert.SerializeObject(input, Formatting.Indented);

                requestMessage.Content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = this.Client.SendAsync(requestMessage);
                result = response.Result.Content.ReadAsStringAsync().Result;
            }

            return result;
        }


        #endregion

    }
}
