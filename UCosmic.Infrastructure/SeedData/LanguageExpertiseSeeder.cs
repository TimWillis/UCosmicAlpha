﻿using System;
using System.Linq;
using System.Security.Principal;
using UCosmic.Domain.Identity;
using UCosmic.Domain.LanguageExpertise;
using UCosmic.Domain.Languages;
using UCosmic.Domain.People;

namespace UCosmic.SeedData
{
    public class LanguageExpertiseEntitySeeder : ISeedData
    {
        private readonly ICommandEntities _entities;
        private readonly IHandleCommands<CreateLanguageExpertise> _createLanguageExpertise;
        private readonly IUnitOfWork _unitOfWork;

        public LanguageExpertiseEntitySeeder(  ICommandEntities entities
                                    , IHandleCommands<CreateLanguageExpertise> createLanguageExpertise
                                    , IUnitOfWork unitOfWork
        )
        {
            _createLanguageExpertise = createLanguageExpertise;
            _unitOfWork = unitOfWork;
            _entities = entities;
        }

        public void Seed()
        {
            /* ----- USF People LanguageExpertises ----- */

            { // Douglas Corarito
                Person person = _entities.Get<Person>().SingleOrDefault(x => x.FirstName == "Douglas" && x.LastName == "Corarito");
                if (person == null) throw new Exception("USF person Douglas Corarito not found");

                User user = _entities.Get<User>().SingleOrDefault(x => x.Person.RevisionId == person.RevisionId);
                if (user == null) throw new Exception("USF person Douglas Corarito has no User.");

                var developerRoles = new[]
                    {
                        RoleName.AuthorizationAgent,
                        RoleName.EstablishmentLocationAgent,
                        RoleName.EstablishmentAdministrator,
                        RoleName.ElmahViewer,
                        RoleName.AgreementManager,
                        RoleName.AgreementSupervisor,
                        RoleName.EmployeeProfileManager,
                    };
                var identity = new GenericIdentity(user.Name);
                var principal = new GenericPrincipal(identity, developerRoles);

                CreateLanguageExpertise createLanguageExpertiseCommand;
                LanguageName languageName;

                // LANGUAGE EXPERTISE 1
                var entityId = new Guid("53ECE165-A1BC-4A20-9982-C0FF5E752085");
                bool expertiseExists = _entities.Get<LanguageExpertise>().Count(x => x.EntityId == entityId) > 0;
                if (!expertiseExists)
                {
                    languageName = _entities.Get<LanguageName>().FirstOrDefault(x => x.Text == "Spanish");
                    if (languageName == null) { throw new Exception("Language not found.");}

                    createLanguageExpertiseCommand = new CreateLanguageExpertise(
                        principal,
                        (int)LanguageProficiency.Proficiency.Elementary,
                        (int) LanguageProficiency.Proficiency.FunctionallyNative,
                        (int) LanguageProficiency.Proficiency.LimitedWorking,
                        (int) LanguageProficiency.Proficiency.Elementary)
                    {
                        EntityId = entityId,
                        LanguageId = languageName.LanguageId
                    };

                    _createLanguageExpertise.Handle(createLanguageExpertiseCommand);

                    _unitOfWork.SaveChanges();
                } // LANGUAGE EXPERTISE 1

                // LANGUAGE EXPERTISE 2
                entityId = new Guid("897269C7-5869-4077-BABB-FA463F140555");
                expertiseExists = _entities.Get<LanguageExpertise>().Count(x => x.EntityId == entityId) > 0;
                if (!expertiseExists)
                {
                    languageName = _entities.Get<LanguageName>().FirstOrDefault(x => x.Text == "Chinese");
                    if (languageName == null) { throw new Exception("Language not found."); }

                    createLanguageExpertiseCommand = new CreateLanguageExpertise(
                        principal,
                        (int) LanguageProficiency.Proficiency.Elementary,
                        (int) LanguageProficiency.Proficiency.LimitedWorking,
                        (int) LanguageProficiency.Proficiency.None,
                        (int) LanguageProficiency.Proficiency.None)
                    {
                        LanguageId = languageName.LanguageId,
                        Dialect = "Mandarin",
                        EntityId = entityId
                    };

                    _createLanguageExpertise.Handle(createLanguageExpertiseCommand);

                    _unitOfWork.SaveChanges();
                } // LANGUAGE EXPERTISE 2

                // LANGUAGE EXPERTISE 3
                entityId = new Guid("2C017B93-AD6D-4547-9866-61807EE1C853");
                expertiseExists = _entities.Get<LanguageExpertise>().Count(x => x.EntityId == entityId) > 0;
                if (!expertiseExists)
                {
                    createLanguageExpertiseCommand = new CreateLanguageExpertise(
                        principal,
                        (int) LanguageProficiency.Proficiency.FunctionallyNative,
                        (int) LanguageProficiency.Proficiency.FunctionallyNative,
                        (int) LanguageProficiency.Proficiency.GeneralProfessional,
                        (int) LanguageProficiency.Proficiency.GeneralProfessional)
                    {
                        Other = "Franco-Germanic",
                        Dialect = "Walser",
                        EntityId = entityId
                    };

                    _createLanguageExpertise.Handle(createLanguageExpertiseCommand);

                    _unitOfWork.SaveChanges();
                } // LANGUAGE EXPERTISE 3

            } // Douglas Corarito
        }
    }

}