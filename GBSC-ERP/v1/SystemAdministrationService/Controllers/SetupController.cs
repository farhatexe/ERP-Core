﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ErpCore.Entities;
using ErpCore.Entities.HRSetup;
using ErpCore.Filters;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SystemAdministrationService.Repos.HrSetupRepos.HrSetupInterfaces;
using SystemAdministrationService.Repos.Interfaces;
using SystemAdministrationService.ViewModels;

namespace SystemAdministrationService.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    //[EnableCors("MyPolicy")]
    public class SetupController : Controller
    {
        private ICompanyRepository com_repo;
        private IBranchRepository bra_repo;
        private IDepartmentRepository dep_repo;
        private IRoleRepository role_repo;
        private IFeatureRepository fea_repo;
        private IModuleRepository _module_repo;
        private ICountryRepository Country_repo;
        private ICityRepository City_repo;
        private IPermissionRepository per_repo;

        public SetupController(ICompanyRepository comrepo,
                                IBranchRepository brarepo,
                                IDepartmentRepository deprepo,
                                IRoleRepository rolrepo,
                                IFeatureRepository fearepo,
                                IModuleRepository module_repo,
                                ICountryRepository countryrepo,
                                ICityRepository cityrepo,
                                IPermissionRepository perrepo)
        {
            com_repo = comrepo;
            bra_repo = brarepo;
            dep_repo = deprepo;
            role_repo = rolrepo;
            fea_repo = fearepo;
            _module_repo = module_repo;
            Country_repo = countryrepo;
            City_repo = cityrepo;
            per_repo = perrepo;

        }

        [HttpGet("GetCompanies", Name = "GetCompanies")]
        public IEnumerable<Company> GetCompanies()
        {
            IEnumerable<Company> co = com_repo.GetList(c => c.CompanyId != null);
            co = co.OrderByDescending(a => a.CompanyId);
            return co;
        }

        [HttpGet("GetBranches", Name = "GetBranches")]
        public IEnumerable<Branch> GetBranches()
        {
            IEnumerable<Branch> br = bra_repo.GetList(c => c.BranchId != null);
            br = br.OrderByDescending(a => a.BranchId);
            return br;
        }

        [HttpGet("GetBranchesByCompanyId/{compid}", Name = "GetBranchesByCompanyId")]
        public IEnumerable<Branch> GetBranchesByCompanyId([FromRoute]long compid)
        {
            return bra_repo.GetList(c => c.CompanyId != null && c.CompanyId == compid).OrderByDescending(a => a.BranchId);
        }

        [HttpGet("GetDepartments", Name = "GetDepartments")]
        public IEnumerable<Department> GetDepartments()
        {
            IEnumerable<Department> de = dep_repo.GetAll();
            de = de.OrderByDescending(a => a.DepartmentId);
            return de;
        }

        [HttpGet("GetDepartmentsByCompanyId/{compid}", Name = "GetDepartmentsByCompanyId")]
        public IEnumerable<Department> GetDepartmentsByCompanyId([FromRoute]long compid)
        {
            return dep_repo.GetList(a => a.CompanyId != null && a.CompanyId == compid).OrderByDescending(a => a.DepartmentId);
        }

        [HttpGet("GetRoles", Name = "GetRoles")]
        public IEnumerable<Role> GetRoles()
        {
            return role_repo.GetAll();
        }

        [HttpGet("GetDropdownRolesByCompany/{CompanyId}")]
        public IEnumerable<Role> GetDropdownRolesByCompany(long CompanyId)
        {
            return role_repo.GetList(c => c.CompanyId != null && c.CompanyId == CompanyId);
        }

        [HttpGet("GetRolesByCompany/{CompanyId}")]
        public IEnumerable<RolesWithModulefeaturePermissionsViewModel> GetRolesByCompanyId(long CompanyId)
        {
            return role_repo.GetRolesByCompany(CompanyId);
        }

        [HttpGet("GetModulesByRole/{RoleId}")]
        public IEnumerable<ModuleViewModel> GetModulesByRole(long RoleId)
        {
            return role_repo.GetModulesByRole(RoleId);
        }

        [HttpGet("GetFeatures", Name = "GetFeatures")]
        public IEnumerable<Feature> GetFeatures()
        {
            IEnumerable<Feature> fe = fea_repo.GetAll(b => b.Permissions, c => c.Module);
            fe = fe.OrderByDescending(a => a.FeatureId);
            return fe;
        }

        [HttpGet("GetFeaturesByCompany/{CompanyId}")]
        public IEnumerable<Feature> GetFeaturesByCompany(long CompanyId)
        {
            return fea_repo.GetList(f => f.CompanyId != null && f.CompanyId == CompanyId, f => f.Module);
        }

        [HttpPost("GetFeaturesByModules", Name = "GetFeaturesByModules")]
        public IEnumerable<Feature> GetFeaturesByModules([FromBody]IEnumerable<long> moduleids)
        {
            List<Feature> Features = new List<Feature>();

            foreach (long moduleid in moduleids)
            {
                Features.AddRange(fea_repo.GetList(a => a.ModuleId != null && a.ModuleId == moduleid));
            }
            return Features.OrderBy(a => a.ModuleId);
        }

        [HttpGet("GetFeaturesByRoleId/{RoleId}")]
        public IEnumerable<FeaturesAndPermissionViewModel> GetFeaturesByRoleId(long RoleId)
        {
            return fea_repo.GetFeaturesByRoleId(RoleId);
        }

        [HttpGet("GetPermissions", Name = "GetPermissions")]
        public IEnumerable<Permission> GetPermissions()
        {
            IEnumerable<Permission> fp = per_repo.GetAll(b => b.Feature, c => c.Role, d => d.User);
            fp = fp.OrderByDescending(a => a.PermissionId);
            return fp;
        }

        [HttpGet("GetPermissionsByCompany/{companyid}", Name = "GetPermissionsByCompany")]
        public IEnumerable<Permission> GetPermissionsByCompany([FromRoute]long companyid)
        {
            return per_repo.GetList(a => a.CompanyId != null && a.CompanyId == companyid, b => b.Feature, c => c.Role, d => d.User).OrderByDescending(a => a.PermissionId);
        }


        [HttpGet("GetModules", Name = "GetModules")]
        public IEnumerable<Module> GetModules(long CompanyId)
        {
            if (CompanyId > 0)
                return _module_repo.GetList(r => r.CompanyId != null && r.CompanyId == CompanyId, a => a.Features, b => b.RoleModules);
            else
                return _module_repo.GetAll(a => a.Features, b => b.RoleModules);
        }

        [HttpGet("GetModulesByCompany/{companyId}", Name = "GetModulesByCompany")]
        public IEnumerable<Module> GetModulesByCompany([FromRoute]long CompanyId)
        {
            return _module_repo.GetList(r => r.CompanyId != null && r.CompanyId == CompanyId);
        }

        [HttpGet("GetCompany/{id}", Name = "GetCompany")]
        public Company GetCompany(long id) => com_repo.GetFirst(a => a.CompanyId == id);

        [HttpGet("GetBranch/{id}", Name = "GetBranch")]
        public Branch GetBranch(long id) => bra_repo.GetFirst(a => a.BranchId == id);

        [HttpGet("GetDepartment/{id}", Name = "GetDepartment")]
        public Department GetDepartment(long id) => dep_repo.GetFirst(a => a.DepartmentId == id);

        [HttpGet("GetRole/{id}", Name = "GetRole")]
        public Role GetRole(long id) => role_repo.GetFirst(a => a.RoleId == id, b => b.RoleFeatures, c => c.RoleModules, d => d.Permissions, e => e.Department, f => f.Users);

        [HttpGet("GetFeature/{id}", Name = "GetFeature")]
        public Feature GetFeature(long id) => fea_repo.GetFirst(a => a.FeatureId == id, b => b.Permissions, c => c.Module);

        [HttpGet("GetPermission/{id}", Name = "GetPermission")]
        public Permission GetPermission(long id) => per_repo.GetFirst(a => a.PermissionId == id, b => b.Feature, c => c.Role, d => d.User);

        [HttpGet("GetModule/{id}", Name = "GetModule")]
        public Module GetModule(long id) => _module_repo.GetFirst(a => a.ModuleId == id, b => b.Features, c => c.RoleModules);

        [HttpPut("UpdateCompany", Name = "UpdateCompany")]
        [ValidateModelAttribute]
        public IActionResult UpdateCompany([FromBody]Company model)
        {
            com_repo.Update(model);
            return new OkObjectResult(new { CompanyID = model.CompanyId });
        }

        [HttpPut("UpdateBranch", Name = "UpdateBranch")]
        [ValidateModelAttribute]
        public IActionResult UpdateBranch([FromBody]Branch model)
        {
            bra_repo.Update(model);
            return new OkObjectResult(new { BranchID = model.BranchId });
        }

        [HttpPut("UpdateDepartment", Name = "UpdateDepartment")]
        [ValidateModelAttribute]
        public IActionResult UpdateDepartment([FromBody]Department model)
        {
            dep_repo.Update(model);
            return new OkObjectResult(new { DepartmentID = model.DepartmentId });
        }

        [HttpPut("UpdateFeature", Name = "UpdateFeature")]
        [ValidateModelAttribute]
        public IActionResult UpdateFeature([FromBody]Feature model)
        {
            fea_repo.Update(model);
            return new OkObjectResult(new { FeatureID = model.FeatureId });
        }

        [HttpPut("UpdatePermission", Name = "UpdatePermission")]
        [ValidateModelAttribute]
        public IActionResult UpdatePermission([FromBody]Permission model)
        {
            per_repo.Update(model);
            return new OkObjectResult(new { PermissionID = model.PermissionId });
        }

        [HttpPut("UpdateModule", Name = "UpdateModule")]
        [ValidateModelAttribute]
        public IActionResult UpdateModule([FromBody]Module model)
        {
            _module_repo.Update(model);
            return new OkObjectResult(new { ModuleID = model.ModuleId });
        }

        [HttpPost("AddCompany", Name = "AddCompany")]
        [ValidateModelAttribute]
        public IActionResult AddCompany([FromBody]Company model)
        {
            com_repo.Add(model);
            return new OkObjectResult(new { CompanyID = model.CompanyId });
        }

        [HttpPost("AddBranch", Name = "AddBranch")]
        [ValidateModelAttribute]
        public IActionResult AddBranch([FromBody]Branch model)
        {
            bra_repo.Add(model);
            return new OkObjectResult(new { BranchID = model.BranchId });
        }

        [HttpPost("AddDepartment", Name = "AddDepartment")]
        [ValidateModelAttribute]
        public IActionResult AddDepartment([FromBody]Department model)
        {
            dep_repo.Add(model);
            return new OkObjectResult(new { DepartmentID = model.DepartmentId });
        }

        [HttpPost("AddRole", Name = "AddRole")]
        [ValidateModelAttribute]
        public IActionResult AddRole([FromBody]SaveRoleviewModel model)
        {
            var role = new Role
            {
                CompanyId = model.CompanyId,
                Name = model.roleName
            };

            role_repo.Add(role);
            model.RoleId = role.RoleId;

            var roleId = role_repo.AddRole(model);
            return new OkObjectResult(new { RoleId = roleId });
        }

        [HttpPut("UpdateRole", Name = "UpdateRole")]
        [ValidateModelAttribute]
        public IActionResult UpdateRole([FromBody]SaveRoleviewModel model)
        {
            role_repo.UpdateRole(model);
            return new OkObjectResult(new { RoleId = model.RoleId });
        }

        [HttpPost("AddFeature", Name = "AddFeature")]
        [ValidateModelAttribute]
        public IActionResult AddFeature([FromBody]Feature model)
        {
            fea_repo.Add(model);
            return new OkObjectResult(new { FeatureID = model.FeatureId });
        }

        [HttpPost("AddPermission", Name = "AddPermission")]
        [ValidateModelAttribute]
        public IActionResult AddPermission([FromBody]Permission model)
        {
            per_repo.Add(model);
            return new OkObjectResult(new { PermissionID = model.PermissionId });
        }

        [HttpPost("AddModule", Name = "AddModule")]
        [ValidateModelAttribute]
        public IActionResult AddModule([FromBody]Module model)
        {
            _module_repo.Add(model);
            return new OkObjectResult(new { ModuleID = model.ModuleId });
        }


        #region City
        [HttpGet("GetCities", Name = "GetCities")]
        public IEnumerable<City> GetCities()
        {
            return City_repo.GetList(c => c.CityId != null).OrderByDescending(a => a.CityId);
        }

        [HttpGet("GetCitiesByCompanyId/{compid}", Name = "GetCitiesByCompanyId")]
        public IEnumerable<City> GetCitiesByCompanyId([FromRoute]long compid)
        {
            return City_repo.GetList(c => c.CompanyId == compid).OrderByDescending(a => a.CityId);
        }

        [HttpGet("GetCity/{id}", Name = "GetCity")]
        public City GetCity(long id) => City_repo.GetFirst(a => a.CityId == id);

        [HttpPost("AddCity", Name = "AddCity")]
        [ValidateModelAttribute]
        public IActionResult AddCity([FromBody]City model)
        {
            City_repo.Add(model);
            return new OkObjectResult(new { CityID = model.CityId });
        }

        [HttpPut("UpdateCity", Name = "UpdateCity")]
        [ValidateModelAttribute]
        public IActionResult UpdateCity([FromBody]City model)
        {
            City_repo.Update(model);
            return new OkObjectResult(new { CityID = model.CityId });
        }

        [HttpDelete("DeleteCity/{id}")]
        public IActionResult CityBank(long id)
        {
            City a = City_repo.Find(id);
            if (a == null)
            {
                return NotFound();
            }
            City_repo.Delete(a);
            return Ok();
        }
        #endregion

        #region Country
        [HttpGet("GetCountries", Name = "GetCountries")]
        public IEnumerable<Country> GetCountries()
        {

            return Country_repo.GetList(c => c.CountryId != null).OrderByDescending(a => a.CountryId);
        }

        [HttpGet("GetCountriesByCompanyId/{compid}", Name = "GetCountriesByCompanyId")]
        public IEnumerable<Country> GetCountriesByCompanyId([FromRoute]long compid)
        {
            return Country_repo.GetList(c => c.CompanyId == compid).OrderByDescending(a => a.CountryId);
        }

        [HttpGet("GetCountry/{id}", Name = "GetCountry")]
        public Country GetCountry(long id) => Country_repo.GetFirst(a => a.CountryId == id);

        [HttpPost("AddCountry", Name = "AddCountry")]
        [ValidateModelAttribute]
        public IActionResult AddCountry([FromBody]Country model)
        {
            Country_repo.Add(model);
            return new OkObjectResult(new { CountryID = model.CountryId });
        }

        [HttpPut("UpdateCountry", Name = "UpdateCountry")]
        [ValidateModelAttribute]
        public IActionResult UpdateCountry([FromBody]Country model)
        {
            Country_repo.Update(model);
            return new OkObjectResult(new { CountryID = model.CountryId });
        }

        [HttpDelete("DeleteCountry/{id}")]
        public IActionResult DeleteCountry(long id)
        {
            Country a = Country_repo.Find(id);
            if (a == null)
            {
                return NotFound();
            }
            Country_repo.Delete(a);
            return Ok();
        }
        #endregion

        [HttpDelete("DeleteCompany/{id}")]
        public IActionResult DeleteCompany(long id)
        {
            Company com = com_repo.Find(id);
            if (com == null)
            {
                return NotFound();
            }

            com_repo.Delete(com);
            return Ok();
        }

        [HttpDelete("DeleteBranch/{id}")]
        public IActionResult DeleteBranch(long id)
        {
            Branch bra = bra_repo.Find(id);
            if (bra == null)
            {
                return NotFound();
            }

            bra_repo.Delete(bra);
            return Ok();
        }

        [HttpDelete("DeleteDepartment/{id}")]
        public IActionResult DeleteDepartment(long id)
        {
            Department dep = dep_repo.Find(id);
            if (dep == null)
            {
                return NotFound();
            }

            dep_repo.Delete(dep);
            return Ok();
        }

        [HttpDelete("DeleteRole/{id}")]
        public IActionResult DeleteRole(long id)
        {
            Role role = role_repo.Find(id);
            if (role == null)
            {
                return NotFound();
            }

            role_repo.Delete(role);
            return Ok();
        }

        [HttpDelete("DeleteFeature/{id}")]
        public IActionResult DeleteFeature(long id)
        {
            Feature fe = fea_repo.Find(id);
            if (fe == null)
            {
                return NotFound();
            }

            fea_repo.Delete(fe);
            return Ok();
        }

        [HttpDelete("DeletePermission/{id}")]
        public IActionResult DeletePermission(long id)
        {
            Permission fp = per_repo.Find(id);
            if (fp == null)
            {
                return NotFound();
            }

            per_repo.Delete(fp);
            return Ok();
        }

        [HttpDelete("DeleteModule/{id}")]
        public IActionResult DeleteModule(long id)
        {
            Module module = _module_repo.Find(id);
            if (module == null)
            {
                return NotFound();
            }
            _module_repo.Delete(module);
            return Ok();
        }

    }
}