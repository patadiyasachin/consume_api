using loc_api_consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace loc_api_consume.Controllers
{
    public class CountryController : Controller
    {

        Uri baseAddress = new Uri("https://localhost:44337/api/Country");
        //https://localhost:44337/api/Country/countryID?countryID=8
        //https://localhost:44337/api/Country/countryID?countryID=4   edit
        //https://localhost:44337/api/Country/1

        private readonly HttpClient _Client;
        public CountryController()
        {
            this._Client = new HttpClient();
            this._Client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CountryGetAll()
        {
            List<CountryModel> countries = new List<CountryModel>();
            HttpResponseMessage response = _Client.GetAsync($"{baseAddress}").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                countries = JsonConvert.DeserializeObject<List<CountryModel>>(data);
            }
            return View("CountryGetAll", countries);
        }

        public async Task<IActionResult> Delete(int countryID)
        {
            var response = await _Client.DeleteAsync(baseAddress+"/countryID?countryID="+countryID);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("CountryGetAll");
            }

            TempData["Error"] = "Failed to delete the city. Please try again.";
            return RedirectToAction("CountryGetAll");
        }

        public async Task<IActionResult> AddEdit(int? countryID)
        {
            //await LoadCountryList();
            if (countryID!=null)
            {
                var response = await _Client.GetAsync("https://localhost:44337/api/Country/"+countryID);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var country = JsonConvert.DeserializeObject<CountryModel>(data);
                    return View("AddEdit",country);
                }
            }
            return View("AddEdit");
        }

        [HttpPost]
        public async Task<IActionResult> Save(CountryModel countryModel)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(countryModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (countryModel.CountryID==null)
                    response = await _Client.PostAsync("https://localhost:44337/api/Country",content);
                else
                    response = await _Client.PutAsync(baseAddress + "/countryID?countryID=" + countryModel.CountryID,content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("CountryGetAll");
            }
            return View("AddEdit",countryModel);
        }

    }
}
