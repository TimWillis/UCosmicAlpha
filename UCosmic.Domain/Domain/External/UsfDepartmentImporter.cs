﻿using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using UCosmic.Domain.Employees;
using UCosmic.Domain.Establishments;

#pragma warning disable 649


namespace UCosmic.Domain.External
{
    /*
     * NOTE: This event handler is not thread-safe.  It's instantiation is controlled by a lock
     * in UsfFacultyImporter.  If you wish to raise UsfStaleEstablishmentHierarchy event
     * in a class other than UsfFacultyImporter, you will need to re-think synchronization.
     */
    public class UsfDepartmentImporter : IHandleEvents<UsfStaleEstablishmentHierarchy>
    {
        [DataContract]
        private class DepartmentRecord
        {
            [DataMember(Name = "DEPTID")] public string DeptId;
            [DataMember(Name = "INSTITUTION")] public string Institution;
            [DataMember(Name = "COLLEGE")] public string College;
            [DataMember(Name = "DEPARTMENT")] public string Department;
        }

        [DataContract]
        private class Record
        {
            [DataMember(Name = "LAST_ACTIVITY_DATE")] public string LastActivityDate;
            [DataMember(Name = "DEPARTMENTS")] public DepartmentRecord[] Departments;
        }

        private readonly ICommandEntities _entities;
        private readonly IHandleCommands<UsfCreateEstablishment> _createUsfEstablishment;
        private readonly IHandleCommands<UpdateEmployeeModuleSettings> _updateEmployeeModuleSettings;
        private readonly IHandleCommands<UpdateEstablishmentHierarchy> _hierarchy;
        private readonly IUnitOfWork _unitOfWork;
        
        private Establishment _usf;
        private StringDictionary _campuses;
        private EstablishmentType _campusEstablishmentType;
        private EstablishmentType _collegeEstablishmentType;
        private EstablishmentType _departmentEstablishmentType;

        public DateTime? LastDepartmentListActivityDate { get; set; }

        // ----------------------------------------------------------------------
        /*
        */
        // ----------------------------------------------------------------------
        public UsfDepartmentImporter( ICommandEntities entities,
                                      IHandleCommands<UsfCreateEstablishment> createUsfEstablishment,
                                      IHandleCommands<UpdateEmployeeModuleSettings> updateEmployeeModuleSettings,
                                      IHandleCommands<UpdateEstablishmentHierarchy> hierarchy,
                                      IUnitOfWork unitOfWork )
        {
            _entities = entities;
            _createUsfEstablishment = createUsfEstablishment;
            _updateEmployeeModuleSettings = updateEmployeeModuleSettings;
            _hierarchy = hierarchy;
            _unitOfWork = unitOfWork;
            _usf = null;
        }

        // ----------------------------------------------------------------------
        /*
        */
        // ----------------------------------------------------------------------
        public void UsfEstablishmentsSetup()
        {
            _usf = _entities.Get<Establishment>().SingleOrDefault(e => e.OfficialName == "University of South Florida");
            if (_usf == null) { throw new Exception("USF Establishment not found."); }

            _campuses = new StringDictionary();
            _campuses.Add("TAMPA", "USF Tampa Campus");
            _campuses.Add("USF Tampa", "USF Tampa Campus");
            _campuses.Add("ST.PETE", "USF St. Petersburg Campus");
            _campuses.Add("USF St. Petersburg", "USF St. Petersburg Campus");
            _campuses.Add("SARASOTA", "USF Sarasota-Manatee Campus");
            _campuses.Add("USF Sarasota", "USF Sarasota-Manatee Campus");

            string establishmentType = KnownEstablishmentType.UniversityCampus.AsSentenceFragment();
            _campusEstablishmentType =
                _entities.Get<EstablishmentType>().SingleOrDefault(t => t.EnglishName == establishmentType);
            if (_campusEstablishmentType == null) { throw new Exception("Campus EstablishmentType not found."); }

            establishmentType = KnownEstablishmentType.College.AsSentenceFragment();
            _collegeEstablishmentType =
                _entities.Get<EstablishmentType>().SingleOrDefault(t => t.EnglishName == establishmentType);
            if (_collegeEstablishmentType == null) { throw new Exception("College EstablishmentType not found."); }

            establishmentType = KnownEstablishmentType.Department.AsSentenceFragment();
            _departmentEstablishmentType =
                _entities.Get<EstablishmentType>().SingleOrDefault(t => t.EnglishName == establishmentType);
            if (_departmentEstablishmentType == null) { throw new Exception("Department EstablishmentType not found."); }

            /* Make sure campuses exist. */
            foreach (string officialName in _campuses.Values)
            {
                var campus = _entities.Get<Establishment>().SingleOrDefault(c => c.OfficialName == officialName);
                if (campus == null)
                {
                    var createCampus = new UsfCreateEstablishment()
                    {
                        OfficialName = officialName,
                        IsMember = true,
                        ParentId = _usf.RevisionId,
                        TypeId = _campusEstablishmentType.RevisionId
                    };

                    _createUsfEstablishment.Handle(createCampus);
                    _unitOfWork.SaveChanges();
                }
            }
        }

        // ----------------------------------------------------------------------
        /*
        */
        // ----------------------------------------------------------------------
        public void Import(Stream stream)
        {
#if DEBUG
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Debug.WriteLine(DateTime.Now + " USF: Department import start:");
#endif

            var serializer = new DataContractJsonSerializer(typeof(Record));
            var record = (Record)serializer.ReadObject(stream);

            LastDepartmentListActivityDate = null;

            Debug.WriteLine(DateTime.Now + " USF: Record last activity date: " + record.LastActivityDate);

            /* If the department list last activity date does not exists, error (?). */
            if (String.IsNullOrWhiteSpace(record.LastActivityDate))
            {
                return;
            }

            LastDepartmentListActivityDate = DateTime.Parse(record.LastActivityDate);

            Debug.WriteLine(DateTime.Now + " USF: Number of records: " + record.Departments.Length);

            /* If the department list is empty, leave. */
            if (record.Departments.Length == 0)
            {
                return;
            }

            for (int i = 0; i < record.Departments.Length; i += 1)
            {
                DepartmentRecord department = record.Departments[i];

                /* Attempt to find existing department by id */
                var existingDepartment =
                    _entities.Get<Establishment>().SingleOrDefault(e => e.ExternalId == department.DeptId);

                /* If not found, add. */
                if ((existingDepartment == null) &&
                    (!String.IsNullOrWhiteSpace(department.Department)) &&
                    (!String.IsNullOrWhiteSpace(department.College))
                   )
                {
                    /* Make sure we have USF and Campuses. */
                    if (_usf == null) { UsfEstablishmentsSetup(); }

                    /* Get the campus name. */
                    var campusOfficialName = _campuses[department.Institution];
                    if (campusOfficialName == null)
                    {
                        string message = String.Format("USF campus name {0} not found", department.Institution);
                        throw new Exception(message);
                    }

                    /* Get the campus. */
                    var campus = _entities.Get<Establishment>().SingleOrDefault(e => e.OfficialName == campusOfficialName);
                    if (campus == null)
                    {
                        string message = String.Format("USF campus {0} not found", campusOfficialName);
                        throw new Exception(message);
                    }

                    /* Does the college exist? If not, create it. */
                    var college = _entities.Get<Establishment>().FirstOrDefault(e => e.OfficialName == department.College);
                    if (college == null)
                    {
                        var createCollege = new UsfCreateEstablishment()
                        {
                            OfficialName = department.College,
                            IsMember = true,
                            ParentId = campus.RevisionId,
                            TypeId = _collegeEstablishmentType.RevisionId
                        };
                        _createUsfEstablishment.Handle(createCollege);
                        _unitOfWork.SaveChanges();

                        college = createCollege.CreatedEstablishment;
                    }

                    /* Create department/program. */
                    var createDepartment = new UsfCreateEstablishment()
                    {
                        OfficialName = department.Department,
                        ExternalId = department.DeptId,
                        IsMember = true,
                        ParentId = college.RevisionId,
                        TypeId = _departmentEstablishmentType.RevisionId
                    };
                    _createUsfEstablishment.Handle(createDepartment);
                    _unitOfWork.SaveChanges();

                }
            }

            _usf = _entities.Get<Establishment>()
                            .EagerLoad(_entities, new Expression<Func<Establishment, object>>[]
                            {
                                e => e.Offspring
                            })
                            .SingleOrDefault(e => e.OfficialName == "University of South Florida");

            _hierarchy.Handle(new UpdateEstablishmentHierarchy(_usf));

#if DEBUG
            stopwatch.Stop();
            Debug.WriteLine(DateTime.Now + " USF: Time to import departments: " + stopwatch.ElapsedMilliseconds.ToString() + " ms");
#endif
        }


        // --------------------------------------------------------------------------------
        /*
        */
        // --------------------------------------------------------------------------------
        public void Handle(UsfStaleEstablishmentHierarchy @event)
        {
            Debug.WriteLine(DateTime.Now + " USF: Begin establishment hierarchy update.");

            var usf = _entities.Get<Establishment>().SingleOrDefault(e => e.OfficialName == "University of South Florida");
            if (usf == null)
            {
                /* TBD - Better error handling. */
                return;
            }

            var employeeModuleSettings = _entities.Get<EmployeeModuleSettings>()
                    .SingleOrDefault(s => s.Establishment.RevisionId == usf.RevisionId);
            if (employeeModuleSettings == null)
            {
                /* TBD - Better error handling. */
                return;
            }

            try
            {
                UsfDepartmentImporter departmentImporter = null;

#if true
                string filePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory,
                                   @"..\UCosmic.Infrastructure\SeedData\SeedMediaFiles\USFDepartmentList.json");

                if (File.Exists(filePath))
                {
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        departmentImporter = new UsfDepartmentImporter( _entities,
                                                                        _createUsfEstablishment,
                                                                        _updateEmployeeModuleSettings,
                                                                        _hierarchy,
                                                                        _unitOfWork );
                        departmentImporter.Import(stream);
                    }
                }
#else
                var departmentImporter = new UsfDepartmentImporter( _entities,
                                                                    _createUsfEstablishment,
                                                                    _updateEmployeeModuleSettings,
                                                                    _hierarchy,
                                                                    _unitOfWork );

                departmentImporter.Import(stream);
#endif

                var updateSettings = new UpdateEmployeeModuleSettings(employeeModuleSettings.Id)
                {
                    EstablishmentsExternalSyncDate = departmentImporter.LastDepartmentListActivityDate,
                    EstablishmentsLastUpdateResult = "succeeded",
                    EstablishmentsUpdateFailCount = 0
                };

                _updateEmployeeModuleSettings.Handle(updateSettings);

                Debug.WriteLine(DateTime.Now + " USF: Establishment hierarchy update SUCCEEDED.");
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;

                var updateSettings = new UpdateEmployeeModuleSettings(employeeModuleSettings.Id)
                {
                    EstablishmentsLastUpdateResult = "failed",
                    EstablishmentsUpdateFailCount = employeeModuleSettings.EstablishmentsUpdateFailCount + 1
                };

                _updateEmployeeModuleSettings.Handle(updateSettings);

                int maxFailCount = Int32.Parse(ConfigurationManager.AppSettings["UsfDepartmentListUpdateMaxFailCountBeforeErrorMail"]);
                if (employeeModuleSettings.EstablishmentsUpdateFailCount >= maxFailCount)
                {
                    //MailMessage message = new MailMessage( fromAddress,
                    //                                       toAddress,
                    //                                       "UCosmic: Error updating USF Departments",
                    //                                       body );
                }

                Debug.WriteLine(DateTime.Now + " USF: Establishment hierarchy update FAILED. " + ex.Message);
            }
        }
    }
}
