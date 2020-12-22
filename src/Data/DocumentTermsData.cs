using LiteDB;
using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    public class DocumentTermsData
    {
        //TODO: separate id generate from LiteDB implementation - 2020-12-22T08:45:44
        public string Id { get; set; } = ObjectId.NewObjectId().ToString();

        /// <summary>
        /// Document name.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// List of TermData object: {Term, Count}
        /// </summary>
        public List<TermData> Terms { get; set; }
    }
}