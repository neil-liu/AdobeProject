using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AttractionService.Models;

namespace AttractionService.Controllers
{
    public class SearchController : ApiController
    {
        /// <summary>
        /// Method: POST
        /// Route:  xxx/api/search
        /// This method uses the exact 'searchValue' to search against 'Name', 'Country' and 'State' fields of
        /// the attractions in the database and return those attractions back to web client so they
        /// can be displayed.
        /// 1- whole-word match
        /// 2- search is case-insensitive
        /// Example: 'Great Wall of China' as searchValue will return 'Great Wall of China';
        ///          'Italy' will return all attrctions whose country is 'Italy'
        /// </summary>
        public IEnumerable<Attraction> Post([FromBody]string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue))
                return null;

            List<Attraction> attractions = new List<Attraction>();
            searchValue = searchValue.ToLower();
            
            using (var db = new AttractionContext())
            {
                var query = from a in db.Attractions
                            where a.Country.ToLower() == searchValue ||
                                  a.State.ToLower() == searchValue ||
                                  a.Name.ToLower() == searchValue
                            orderby a.Name
                            select a;
                foreach (var record in query)
                    attractions.Add(new Attraction()
                    {
                        UID = record.UID,
                        Name = record.Name,
                        Description = record.Description,
                        State = record.State,
                        Country = record.Country,
                        ImagePath_s = record.ImagePath_s,
                        ImagePath_m = record.ImagePath_m,
                        ImagePath_l = record.ImagePath_l
                    });
            }

            return attractions;
        }

        /// <summary>
        /// To support preflight OPTIONS method call made by browser (mainly Chrome/Firefox) in CORS, making changes to 
        /// handler configuration in web.config didn't resolve the problem, hence adding the following 'hack' 
        /// to make it work. It fixed 'HTTP 405 Method Not Allowed' with the following message: 
        ///     "The requested resource does not support http method 'OPTIONS'."
        ///     
        /// pending: this is redundant. should have a base class implementation so this can be shared by
        /// multiple child controllers.
        /// </summary>
        /// <returns></returns>
        public HttpResponseMessage Options()
        {
            return new HttpResponseMessage { StatusCode = HttpStatusCode.OK };
        }
    }
}
