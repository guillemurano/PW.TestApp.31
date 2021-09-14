using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PW.TestApp._31.Services;
using PW.TestApp._31.Filters;
using PW.TestApp._31.Entities;

namespace PW.TestApp._31
{
    public class PetFunctions
    {

        private readonly IPetService _petService;

        public PetFunctions(IPetService petService)
        {
            _petService = petService ?? throw new ArgumentNullException(nameof(petService));
        }

        [FunctionName(nameof(GetPetByIdAsync))]
        [Authorize]
        public async Task<IActionResult> GetPetByIdAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pet/{id}")] HttpRequest req,
            ILogger log, int id)
        {
            log.LogInformation($"Start {nameof(GetPetByIdAsync)} function execution.");

            var pet = await _petService.SingleOrDefaultAsync(p => p.Id == id);

            if (pet == null)
                return new NotFoundObjectResult(pet);

            log.LogInformation($"End {nameof(GetPetByIdAsync)} function execution.");

            return new OkObjectResult(pet);
        }

        [FunctionName(nameof(GetPetsAsync))]
        [Authorize]
        public async Task<IActionResult> GetPetsAsync(
            [HttpTrigger(AuthorizationLevel.Function, "get", Route = "pets")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Start {nameof(GetPetsAsync)} function execution.");

            var pets = await _petService.GetAsync();

            if (pets == null)
                return new NotFoundObjectResult(ErrorMessage.NotFound);

            log.LogInformation($"End {nameof(GetPetsAsync)} function execution.");

            return new OkObjectResult(pets);
        }

        [FunctionName(nameof(AddPetAsync))]
        [Authorize]
        public async Task<IActionResult> AddPetAsync(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = "pet")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Start {nameof(AddPetAsync)} function execution.");

            string requestBody = string.Empty;
            using (StreamReader streamReader = new StreamReader(req.Body))
            {
                requestBody = await streamReader.ReadToEndAsync();
            }

            var pet = JsonConvert.DeserializeObject<Pet>(requestBody);

            if (pet == null)
                throw new InvalidOperationException();

            pet = await _petService.AddAsync(pet);

            log.LogInformation($"End {nameof(AddPetAsync)} function execution.");

            return new OkObjectResult(pet);
        }

        [FunctionName(nameof(TickEveryTenSeconds))]
        public void TickEveryTenSeconds([TimerTrigger("*/10 * * * * *", RunOnStartup = true)] TimerInfo timer,
        ILogger log)
        {
            log.LogInformation($"Start {nameof(TickEveryTenSeconds)} function execution.");
            log.LogInformation($"Tic-Tac -------> { DateTime.UtcNow.ToString("HH:mm:ss") } <-----------------");
            log.LogInformation($"End {nameof(TickEveryTenSeconds)} function execution.");
        }
    }
}
