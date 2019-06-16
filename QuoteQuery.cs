using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace QuoteQuery {

	public class Quote {

		public string text {get; set;}
		public string author {get; set;}

	}

	public static class QuoteQuery {

		static QuoteQuery() {

			// Parsing quotes from json.
			using (StreamReader streamReader = new StreamReader("quotes.json")) {

				quotes = JsonConvert.DeserializeObject<Quote[]>(streamReader.ReadToEnd());

			}

			random = new Random();

		}

		[FunctionName("QuoteQuery")]
		public static IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)] HttpRequest req, ILogger log) {

			log.LogInformation("[START] Processing a request.");

			string response = "Welcome! ";

			int quoteNumber;

			// Obtaining parameter.
			string quoteQuery = req.Query["quote"];

			log.LogInformation("[INPUT] \"" + quoteQuery + "\"");

			// Checking parameter.
			if (string.IsNullOrEmpty(quoteQuery)) {

				quoteNumber = random.Next(quotes.Length) + 1;
				response += "Here's a random quote for you:";

			} else if (Int32.TryParse(quoteQuery, out quoteNumber) && quoteNumber > 0 && quoteNumber <= quotes.Length) {

				response += "Here's your requested quote:";

			} else {

				log.LogError("[ERROR] Invalid input.");

				return new BadRequestObjectResult(response + "Couldn't find specified quote, please enter a number between 1 and " + quotes.Length + ".");

			}

			// Finishing response.
			Quote selectedQuote = quotes[quoteNumber - 1];
			response += "\n\n\t[QUOTE " + quoteNumber + "] \"" + selectedQuote.text + "\" -" + selectedQuote.author;

			log.LogInformation("[OUTPUT] \"" + response + "\"");

			// Returning response.
			return new OkObjectResult(response);

		}

		private static Quote[] quotes;
		private static Random random;

	}

}