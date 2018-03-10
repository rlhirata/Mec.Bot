using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mec.Bot.Serialization
{
    public class LUISIntent
    {
        public class ResultLUISIntent
        {
            public string query { get; set; }
            public TopScoringIntent TopScoringIntent { get; set; }
            public Intent[] intents { get; set; }
            public Entity[] entities { get; set; }
        }

        public class TopScoringIntent
        {
            public string intent { get; set; }
            public float score { get; set; }
        }

        public class Intent
        {
            public string intent { get; set; }
            public float score { get; set; }
        }

        public class Entity
        {
            public string entity { get; set; }
            public string type { get; set; }
            public int startIndex { get; set; }
            public int endIndex { get; set; }
            public float score { get; set; }
        }
    }
}