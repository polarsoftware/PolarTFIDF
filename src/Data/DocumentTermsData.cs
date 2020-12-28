using System;
using System.Collections.Generic;

namespace Polar.ML.TfIdf
{
    public class DocumentTermsData
    {     
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        /// <summary>
        /// Identificator of document. Mostly it ia a foringer key from database of user of this library.
        /// </summary>
        public string Document { get; set; }

        /// <summary>
        /// List of TermData object: {Term, Count}
        /// </summary>
        public List<TermData> Terms { get; set; }
    }
}