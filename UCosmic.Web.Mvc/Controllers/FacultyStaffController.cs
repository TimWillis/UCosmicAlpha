﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using AttributeRouting.Web.Mvc;
using UCosmic.Domain.Establishments;
using UCosmic.Web.Mvc.Models;

namespace UCosmic.Web.Mvc.Controllers
{
    //[RestfulRouteConvention]
    [UserVoiceForum(UserVoiceForum.Employees)]
    public partial class FacultyStaffController : Controller
    {
        private readonly IProcessQueries _queryProcessor;
        private readonly IQueryEntities _entities;

        public FacultyStaffController(IProcessQueries queryProcessor, IQueryEntities entities)
        {
            _queryProcessor = queryProcessor;
            _entities = entities;
        }

        [GET("facultystaff")]
        public virtual ActionResult Index()
        {
            var model = new FacultyStaffInstitutionInfoModel
            {
                InstitutionHasCampuses = false,
                ActivityTypes = null
            };

            Establishment establishment = null;

            var tenancy = Request.Tenancy() ?? new Tenancy();

            if (!string.IsNullOrWhiteSpace(tenancy.StyleDomain) && !"default".Equals(tenancy.StyleDomain))
            {
                if (tenancy.TenantId.HasValue)
                {
                    establishment = _queryProcessor.Execute(new EstablishmentById(tenancy.TenantId.Value));
                }
                else if (!String.IsNullOrEmpty(tenancy.StyleDomain) && !"default".Equals(tenancy.StyleDomain))
                {
                    establishment = _queryProcessor.Execute(new EstablishmentByEmail(tenancy.StyleDomain));
                }
            }
            else if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                establishment =
                    _queryProcessor.Execute(new EstablishmentByEmail(User.Identity.Name.GetEmailDomain()));
            }

            if (establishment != null)
            {
                Establishment rootEstablishment = establishment;
                while (rootEstablishment.Parent != null)
                {
                    rootEstablishment = rootEstablishment.Parent;
                }

                model.InstitutionId = rootEstablishment.RevisionId;
                model.InstitutionOfficialName = rootEstablishment.OfficialName;

                var campusEstablishmentType =
                    _entities.Query<EstablishmentType>().Single(t => t.EnglishName == "University Campus");

                model.InstitutionHasCampuses = _entities.Query<Establishment>()
                                                        .Any(
                                                            e =>
                                                            (e.Parent.RevisionId == model.InstitutionId) &&
                                                            (e.Type.RevisionId == campusEstablishmentType.RevisionId));

                /* For now, return first level children of institution system. */
                ICollection<int> childIds = new Collection<int>();
                ICollection<string> childOfficialNames = new Collection<string>();
                var query = new EstablishmentChildren(rootEstablishment.RevisionId);
                var entities = _queryProcessor.Execute(query);
                if (entities != null)
                {
                    var orderedEstablishments = entities.OrderBy(e => e.VerticalRank);
                    foreach (var childEstablishment in orderedEstablishments)
                    {
                        childIds.Add(childEstablishment.RevisionId);
                        childOfficialNames.Add(childEstablishment.OfficialName);
                    }
                }
                model.InstitutionCampusIds = childIds.ToArray();
                model.InstitutionCampusOfficialNames = childOfficialNames.ToArray();
            }

            return View(model);
        }

        [GET("facultystaff/institution/{institutionId}")]
        public virtual ActionResult Institution(int institutionId)
        {
            var establishment = _queryProcessor.Execute(new EstablishmentById(institutionId));
            if (establishment != null)
            {
                var tenancy = new Tenancy
                {
                    StyleDomain = establishment.WebsiteUrl.GetUrlDomain(),
                    TenantId = institutionId
                };

                Response.Tenancy(tenancy);
            }

            return RedirectToAction(MVC.FacultyStaff.Index());
        }
    }
}
