﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using eTrackerInfrastructure.Repos.Interfaces;
using eTrackerCore.Entities;
using eTrackerInfrastructure.Filters;
using Microsoft.AspNetCore.Http;
using eTrackerInfrastructure.Helpers;
using System.IO;
using eTrackerInfrastructure.Models.JsonPostClasses;
using ErpCore.Entities.ETracker;

namespace eTrackerInfrastructure.Controllers
{
    [Route("api/[controller]")]
    public class StoreVisitController : Controller
    {
        private IStoreVisitRepository Repo { get; set; }

        private IStoreRepository StoreRepo { get; set; }

        public StoreVisitController(IStoreVisitRepository repo, IStoreRepository storeRepo)
        {
            Repo = repo;

            StoreRepo = storeRepo;
        }

        [HttpPost("AddStoreVisit")]
        [ValidateModelAttribute]
        public IActionResult AddStoreVisit([FromBody]StoreVisit visit)
        {
            Repo.Add(visit);

            return Ok(new { StoreVisitId = visit.StoreVisitId });
        }

        [HttpPost("UpdateStoreVisit")]
        [ValidateModelAttribute]
        public IActionResult UpdateStoreVisit([FromBody]StoreVisit visit)
        {
            Repo.Update(visit);

            return Ok(new { StoreVisitId = visit.StoreVisitId });
        }

        [HttpPost("AddOrder")]
        [ValidateModelAttribute]
        public IActionResult AddOrder([FromBody]OrderTaking order)
        {
            Repo.AddOrder(order);
            
            return Ok(new { OrderTakingId = order.OrderTakingId });
        }

        [HttpPost("AddMultipleOrders")]
        [ValidateModelAttribute]
        public IActionResult AddOrders([FromBody]Orders orders)
        {
            Repo.AddMultipleOrders(orders);

            return new OkObjectResult(new { response = "All Orders Successfully Added" });
        }

        [HttpPost("AddMerchendise")]
        [ValidateModelAttribute]
        public IActionResult AddMerchendise([FromBody]Merchandising merch)
        {
            Repo.AddMerchendize(merch);

            return Ok(new { MerchandisingId = merch.MerchandisingId });
        }

        [HttpPost("AddCompetativeStock")]
        [ValidateModelAttribute]
        public IActionResult AddCompetativeStock([FromBody]CompetatorStock stock)
        {
            Repo.AddCompetativeStock(stock);

            return Ok(new { CompetativeStockId = stock.CompetatorStockId });
        }

        [HttpPost("AddMultipleCompetativeStock")]
        [ValidateModelAttribute]
        public IActionResult AddMultipleCompetativeStock([FromBody]CompetativeStocks stocks)
        {
            Repo.AddMultipleCompetativeStock(stocks);

            return new OkObjectResult(new { response = "All Competative Stocks Successfully Added" });
        }

        [HttpPost("AddOutletStock")]
        [ValidateModelAttribute]
        public IActionResult AddOutletStock([FromBody]OutletStock stock)
        {
            Repo.AddOutletStock(stock);

            return Ok(new { OutletStockId = stock.OutletStockId });
        }

        [HttpPost("AddMultipleStock")]
        [ValidateModelAttribute]
        public IActionResult AddMultipleStocks([FromBody]Stocks stocks)
        {
            Repo.AddMultipleStockItems(stocks);

            return new OkObjectResult(new { response = "All Stocks Successfully Added" });
        }

        [HttpGet("GetOutletStocks/{storeVisitId}")]
        public IEnumerable<OutletStock> GetOutletStockList(long storeVisitId) => Repo.GetOutletStocks(storeVisitId);

        [HttpGet("GetOrders/{storeVisitId}")]
        public IEnumerable<OrderTaking> GetOrders(long storeVisitId) => Repo.GetOrders(storeVisitId);

        [HttpGet("CompetativeStocks/{storeVisitId}")]
        public IEnumerable<CompetatorStock> CompetativeStockList(long storeVisitId) => Repo.CompetativeStocks(storeVisitId);

        [HttpGet("GetMerchendiseList/{storeVisitId}")]
        public IEnumerable<Merchandising> GetMerchendiseList(long storeVisitId) => Repo.GetMerchendiseList(storeVisitId);

        [HttpGet("GetStoreVisit/{id}")]
        public StoreVisit GetStoreVisit(long id) => Repo.Find(id);

        [HttpGet("GetMostRecenetStoreVisit/{storeid}")]
        public StoreVisit GetMostRecenetStoreVisit(long storeid)
        {
            return Repo.MostRecenetStoreVisit(storeid);
        }

        [HttpGet("GetStoreVisitWithStore/{id}")]
        public StoreVisit GetStoreVisitWithStore(long id) => Repo.GetStoreVisitWithStore(id);

        [HttpGet("GetVisits/{id}")]
        public IEnumerable<StoreVisit> GetVisits(long id) => Repo.GetVisitsByStoreId(id);

        [HttpDelete("DeleteVisit/{id}")]
        public IActionResult DeleteVisit(long id)
        {
            var storeVisit = Repo.Find(id);
            if (storeVisit == null)
                return NotFound();

            Repo.Delete(storeVisit);

            return Ok();
        }

        [HttpPost("UploadStockImage/{OutletStockId}")]
        public async Task<ActionResult> UploadStockImage(IFormFile file, long OutletStockId)
        {

            // full path to file in temp location
            var stockObj = Repo.GetStockItemById(OutletStockId);
            if (stockObj == null)
                return new NotFoundResult();

            var filePath = "";
            string storePath = "Store_" + stockObj.StoreVisit.StoreId;
            string docFolder = Path.Combine("StoreImages", storePath, "Stock");

            try
            {
                if (file == null || file.Length == 0)
                    return Content("file not selected");

                var path = FileUploadHandler.GetFilePathForUpload(docFolder, file.FileName);

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception e)
            {
                return Json(new { result = "upload Failed", error = e.Message });
            }

            filePath = FileUploadHandler.FileReturnPath(docFolder, file.FileName);

            stockObj.ImageUrl = filePath;
            Repo.UpdateStock(stockObj);

            return Json(new { filepath = filePath, OutletStockId = OutletStockId });
        }
    }
}