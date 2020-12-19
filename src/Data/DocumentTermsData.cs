using LiteDB;
using System;
using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    public class DocumentTermsData
    {
        public string Id { get; set; } = ObjectId.NewObjectId().ToString();

        /// <summary>
        /// Document name.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// List of Term object 
        /// </summary>
        public List<TermData> Terms { get; set; }

    }
}