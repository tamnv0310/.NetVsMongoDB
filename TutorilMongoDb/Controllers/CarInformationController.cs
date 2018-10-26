using Core.Interfaces;
using Infrastructure.Data;
using MongoDB.Bson;
using System.Configuration;
using System.Web.Mvc;
using TutorilMongoDb.Models;

namespace TutorilMongoDb.Controllers
{
    public class CarInformationController : Controller
    {
        private ICarInformation<CarModel> _CarInformationServices;
        
        public CarInformationController()
        {
            _CarInformationServices = new CarInformationServices<CarModel>(new MongoDbContext<CarModel>());
        }
        public CarInformationController(ICarInformation<CarModel> CarInformationServices)
        {
            _CarInformationServices = CarInformationServices;
        }
        // GET: CarInformation
        public ActionResult Index()
        {
            return View(_CarInformationServices.GetAll());
        }

        // GET: CarInformation/Details/5
        public ActionResult Details(string id)
        {
            return View(_CarInformationServices.Get(id));
        }

        // GET: CarInformation/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CarInformation/Create
        [HttpPost]
        public ActionResult Create(CarModel carmodel)
        {
            try
            {
                // TODO: Add insert logic here
                var document = new CarModel
                {
                    CarName = carmodel.CarName,
                    Chassisno = carmodel.Chassisno,
                    Color = carmodel.Color,
                    Engineno = carmodel.Engineno,
                    Model = carmodel.Model,
                    Price = carmodel.Price,
                    Registrationno = carmodel.Registrationno
                };
                _CarInformationServices.Create(document);
                TempData["Message"] = "Carname ALready Exist";
                return View("Create", carmodel);
            }
            catch
            {
                return View();
            }
        }

        // GET: CarInformation/Edit/5
        public ActionResult Edit(string id)
        {
            return View(_CarInformationServices.Get(id));
        }

        // POST: CarInformation/Edit/5
        [HttpPost]
        public ActionResult Edit(string id, CarModel carmodel)
        {
            try
            {
                carmodel.Id = new ObjectId(id);
                _CarInformationServices.Update(carmodel);
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: CarInformation/Delete/5
        public ActionResult Delete(string id)
        {
            return View(_CarInformationServices.Get(id));
        }

        // POST: CarInformation/Delete/5
        [HttpPost]
        public ActionResult Delete(string id, CarModel carmodel)
        {
            try
            {
                // TODO: Add delete logic here
                _CarInformationServices.Delete(id);
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}