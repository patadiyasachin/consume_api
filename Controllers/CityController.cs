using loc_api_consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;

namespace loc_api_consume.Controllers
{
    public class CityController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44337/api");
        //https://localhost:44337/api/City/cityID?cityID=9

        //https://localhost:44337/api/City/countryID?countryID=2  getByID
        //https://localhost:44337/api/City



        private readonly HttpClient _client;
        public CityController(HttpClient client)
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        public IActionResult CityList()
        {
            List<CityModel> cities = new List<CityModel>();
            HttpResponseMessage response = _client.GetAsync($"{_client.BaseAddress}/City").Result;
            if (response.IsSuccessStatusCode) { 
                string data=response.Content.ReadAsStringAsync().Result;
                cities=JsonConvert.DeserializeObject<List<CityModel>>(data);
            }
            return View("CityList",cities);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int cityID)
        {
            var response = await _client.DeleteAsync(baseAddress+"/City/cityID?cityID="+cityID);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("CityList");
            }

            TempData["Error"] = "Failed to delete the city. Please try again.";
            return RedirectToAction("CityList");
        }

        public async Task<IActionResult> AddEdit(int? CityID)
        {
            await LoadCountryList();
            if (CityID.HasValue)
            {
                var response = await _client.GetAsync("https://localhost:44337/api/City/"+CityID);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var city = JsonConvert.DeserializeObject<CityModel>(data);
                    ViewBag.StateList = await GetStatesByCountryID(city.CountryID);
                    return View(city);
                }
            }
            return View("AddEdit");
        }

        [HttpPost]
        public async Task<IActionResult> Save(CityModel city)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(city);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (city.CityID == null)
                    response = await _client.PostAsync(baseAddress+"/City", content);
                else
                    response = await _client.PutAsync("https://localhost:44337/api/City/cityID?cityID="+city.CityID,content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("CityList");
            }
            await LoadCountryList();
            return View("AddEdit",city);
        }

        private async Task LoadCountryList()
        {
            var response = await _client.GetAsync(baseAddress+"/City/GetAllCountries");
            if (response.IsSuccessStatusCode)   
            {
                var data = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<CountryDropDownModel>>(data);
                ViewBag.CountryList = countries;
            }
        }

        [HttpPost]
        public async Task<JsonResult> GetStatesByCountry(int CountryID)
        {
            var states = await GetStatesByCountryID(CountryID);
            return Json(states);
        }

        private async Task<List<StateDropDownModel>> GetStatesByCountryID(int CountryID)
        {
            var response = await _client.GetAsync(baseAddress+ "/City/countryID?countryID="+CountryID);
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<List<StateDropDownModel>>(data);
            }
            return new List<StateDropDownModel>();
        }
    }
}


