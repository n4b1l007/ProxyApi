using System.Collections.Generic;
using System.Web.Http;
using ProxyApi.Sample.Models;

namespace ProxyApi.Sample.Controllers
{
	[ValidateHttpAntiForgeryToken(ExcludeAuthenticationTypes="Basic")]
    public class PersonController : ApiController
    {
		[HttpGet]
		public IEnumerable<Person> GetAllPeople()
		{
			var people = new List<Person>
			{
				new Person { Id = 1, FirstName = "John", LastName = "Smith" },
				new Person { Id = 2, FirstName = "James", LastName = "Bond" }
			};

			return people;
		}

		[HttpGet]
		public Person GetPerson([FromUri]Person person)
		{
			return person;
		}
    }
}
