using loc_api_consume.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;

namespace loc_api_consume.Controllers
{
    public class StateController : Controller
    {
        Uri baseAddress = new Uri("https://localhost:44337/api");
        //https://localhost:44337/api/State/stateID?stateID=2
        //https://localhost:44337/api/State

        private readonly HttpClient _client;
        public StateController() {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult StateList()
        {
            List<StateModel> state=new List<StateModel>();
            HttpResponseMessage response=_client.GetAsync($"{_client.BaseAddress}/State").Result;
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                state = JsonConvert.DeserializeObject<List<StateModel>>(data);
            }
            return View("StateList", state);
        }

        public async Task<IActionResult> Delete(int stateID)
        {
            var response = await _client.DeleteAsync(baseAddress + "/State/stateID?stateID=" + stateID);
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("StateList");
            }

            TempData["Error"] = "Failed to delete the city. Please try again.";
            return RedirectToAction("StateList");
        }

        public async Task<IActionResult> AddEdit(int? StateID)
        {
            await LoadCountryList();
            if (StateID.HasValue)
            {
                var response = await _client.GetAsync("https://localhost:44337/api/State/"+StateID);
                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadAsStringAsync();
                    var state = JsonConvert.DeserializeObject<StateModel>(data);
                    return View("AddEdit",state);
                }
            }
            return View("AddEdit");
        }

        [HttpPost]
        public async Task<IActionResult> Save(StateModel stateModel)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(stateModel);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response;

                if (stateModel.stateID == null)
                    response = await _client.PostAsync(baseAddress + "/State", content);
                else
                    response = await _client.PutAsync("https://localhost:44337/api/State/stateID?stateID="+stateModel.stateID,content);

                if (response.IsSuccessStatusCode)
                    return RedirectToAction("StateList");
            }
            await LoadCountryList();
            return View("AddEdit", stateModel);
        }

        private async Task LoadCountryList()
        {
            var response = await _client.GetAsync(baseAddress + "/City/GetAllCountries");
            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadAsStringAsync();
                var countries = JsonConvert.DeserializeObject<List<CountryDropDownModel>>(data);
                ViewBag.CountryList = countries;
            }
        }
    }
}
