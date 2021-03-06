﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErpCore.Entities;
using ErpCore.Filters;
using HimsService.Repos.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HimsService.Controllers
{
    [Produces("application/json")]
    [Route("api/Biopsy")]
    public class BiopsyController : Controller
    {
        private readonly IBiopsyRepository _biopsyRepo;

        public BiopsyController(IBiopsyRepository biopsyRepo)
        {
            _biopsyRepo = biopsyRepo;
        }


        [HttpGet("GetBiopsies")]
        public IEnumerable<Biopsy> GetPatientBiopsies()
        {
            return _biopsyRepo.GetAll();
        }

        [HttpGet("GetBiopsiesByPatientId/{PatientId}")]
        public IEnumerable<Biopsy> GetBiopsiesByPatientId(long PatientId)
        {
            return _biopsyRepo.GetList(b => (b.PatientClinicalRecord?.PatientId == PatientId || b.PatientId == PatientId), b => b.PatientClinicalRecord, b => b.PatientClinicalRecord.Consultant);
        }


        [HttpGet("GetPatientBiopsyByClinicalRecordId/{id}")]
        public Biopsy GetPatientBiopsyByClinicalRecordId(long id)
        {
            return _biopsyRepo.GetFirst(p => p.PatientClinicalRecordId == id);
        }

        [HttpGet("GetBiopsyByCollectionDate")]
        public Biopsy GetBiopsyByCollectionDate(DateTime? date, long? patientid)
        {
            if (date != null)
            {
                date = date.Value.Date;
                return _biopsyRepo.GetFirst(s => s.CollectionDate != null && s.CollectionDate.Value.Date == date && s.PatientId == patientid);

            }

            return null;
        }

        [HttpGet("GetBiopsyById/{Id}")]
        public Biopsy GetBiopsyById(long Id)
        {
            return _biopsyRepo.GetFirst(b => b.BiopsyId == Id);
        }

        [HttpPut("UpdateBiopsy")]
        [ValidateModelAttribute]
        public IActionResult UpdateBiopsy([FromBody]Biopsy biopsy)
        {
            _biopsyRepo.Update(biopsy);

            return new OkObjectResult(new { BiopsyId = biopsy.BiopsyId });
        }

        [HttpPost("AddBiopsy")]
        [ValidateModelAttribute]
        public IActionResult AddBiopsy([FromBody]Biopsy biopsy)
        {
            _biopsyRepo.Add(biopsy);

            return new OkObjectResult(new { BiopsyId = biopsy.BiopsyId });
        }
    }
}