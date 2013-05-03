var App;
(function (App) {
    (function (Routes) {
        Routes.applicationPath = '/';
        function hasTrailingSlash(value) {
            return value.lastIndexOf('/') == value.length - 1;
        }
        (function (WebApi) {
            WebApi.urlPrefix = 'api';
            function makeUrl(relativeUrl) {
                var apiPrefix = WebApi.urlPrefix;
                if(!hasTrailingSlash(apiPrefix)) {
                    apiPrefix = apiPrefix + '/';
                }
                var url = Routes.applicationPath + apiPrefix + relativeUrl;
                if(!hasTrailingSlash(url)) {
                    url = url + '/';
                }
                return url;
            }
            (function (Identity) {
                function signIn() {
                    return makeUrl('sign-in');
                }
                Identity.signIn = signIn;
                function signOut() {
                    return makeUrl('sign-out');
                }
                Identity.signOut = signOut;
                (function (Users) {
                    function get(id) {
                        var url = 'users';
                        if(id) {
                            url += '/' + id;
                        }
                        return makeUrl(url);
                    }
                    Users.get = get;
                    function post() {
                        return makeUrl('users');
                    }
                    Users.post = post;
                    function validateName(id) {
                        id = id ? id : 0;
                        var url = 'users/' + id + '/validate-name';
                        return makeUrl(url);
                    }
                    Users.validateName = validateName;
                    (function (Roles) {
                        function get(userId) {
                            var url = 'users/' + userId + '/roles';
                            return makeUrl(url);
                        }
                        Roles.get = get;
                        function put(userId, roleId) {
                            var url = 'users/' + userId + '/roles/' + roleId;
                            return makeUrl(url);
                        }
                        Roles.put = put;
                        function del(userId, roleId) {
                            return put(userId, roleId);
                        }
                        Roles.del = del;
                        function validateGrant(userId, roleId) {
                            var url = 'users/' + userId + '/roles/' + roleId + '/validate-grant';
                            return makeUrl(url);
                        }
                        Roles.validateGrant = validateGrant;
                        function validateRevoke(userId, roleId) {
                            var url = 'users/' + userId + '/roles/' + roleId + '/validate-revoke';
                            return makeUrl(url);
                        }
                        Roles.validateRevoke = validateRevoke;
                    })(Users.Roles || (Users.Roles = {}));
                    var Roles = Users.Roles;
                })(Identity.Users || (Identity.Users = {}));
                var Users = Identity.Users;
                (function (Roles) {
                    function get(roleId) {
                        var url = 'roles';
                        if(roleId) {
                            url += '/' + roleId;
                        }
                        return makeUrl(url);
                    }
                    Roles.get = get;
                })(Identity.Roles || (Identity.Roles = {}));
                var Roles = Identity.Roles;
            })(WebApi.Identity || (WebApi.Identity = {}));
            var Identity = WebApi.Identity;
            (function (Languages) {
                function get() {
                    return makeUrl('languages');
                }
                Languages.get = get;
            })(WebApi.Languages || (WebApi.Languages = {}));
            var Languages = WebApi.Languages;
            (function (Countries) {
                function get() {
                    return makeUrl('countries');
                }
                Countries.get = get;
            })(WebApi.Countries || (WebApi.Countries = {}));
            var Countries = WebApi.Countries;
            (function (Places) {
                function get(placeId) {
                    var url = 'places';
                    if(placeId) {
                        url += '/' + placeId;
                    }
                    return makeUrl(url);
                }
                Places.get = get;
                (function (ByCoordinates) {
                    function get(latitude, longitude) {
                        var url = 'places/by-coordinates/' + latitude + '/' + longitude;
                        return makeUrl(url);
                    }
                    ByCoordinates.get = get;
                })(Places.ByCoordinates || (Places.ByCoordinates = {}));
                var ByCoordinates = Places.ByCoordinates;
            })(WebApi.Places || (WebApi.Places = {}));
            var Places = WebApi.Places;
            (function (Establishments) {
                function get(establishmentId) {
                    var url = 'establishments';
                    if(establishmentId) {
                        url += '/' + establishmentId;
                    }
                    return makeUrl(url);
                }
                Establishments.get = get;
                function post() {
                    return makeUrl('establishments');
                }
                Establishments.post = post;
                function put(establishmentId) {
                    return get(establishmentId);
                }
                Establishments.put = put;
                function validateCeebCode(establishmentId) {
                    return makeUrl('establishments/' + establishmentId + '/validate-ceeb-code');
                }
                Establishments.validateCeebCode = validateCeebCode;
                function validateUCosmicCode(establishmentId) {
                    return makeUrl('establishments/' + establishmentId + '/validate-ucosmic-code');
                }
                Establishments.validateUCosmicCode = validateUCosmicCode;
                var Names = (function () {
                    function Names() { }
                    Names.get = function get(establishmentId, establishmentNameId) {
                        var url = 'establishments/' + establishmentId + '/names';
                        if(establishmentNameId) {
                            url += '/' + establishmentNameId;
                        }
                        return makeUrl(url);
                    };
                    Names.post = function post(establishmentId) {
                        return Names.get(establishmentId);
                    };
                    Names.put = function put(establishmentId, establishmentNameId) {
                        return makeUrl('establishments/' + establishmentId + '/names/' + establishmentNameId);
                    };
                    Names.del = function del(establishmentId, establishmentNameId) {
                        return Names.put(establishmentId, establishmentNameId);
                    };
                    Names.validateText = function validateText(establishmentId, establishmentNameId) {
                        return makeUrl('establishments/' + establishmentId + '/names/' + establishmentNameId + '/validate-text');
                    };
                    return Names;
                })();
                Establishments.Names = Names;                
                var Urls = (function () {
                    function Urls() { }
                    Urls.get = function get(establishmentId, establishmentUrlId) {
                        var url = 'establishments/' + establishmentId + '/urls';
                        if(establishmentUrlId) {
                            url += '/' + establishmentUrlId;
                        }
                        return makeUrl(url);
                    };
                    Urls.post = function post(establishmentId) {
                        return Urls.get(establishmentId);
                    };
                    Urls.put = function put(establishmentId, establishmentUrlId) {
                        return makeUrl('establishments/' + establishmentId + '/urls/' + establishmentUrlId);
                    };
                    Urls.del = function del(establishmentId, establishmentUrlId) {
                        return Urls.put(establishmentId, establishmentUrlId);
                    };
                    Urls.validateValue = function validateValue(establishmentId, establishmentUrlId) {
                        return makeUrl('establishments/' + establishmentId + '/urls/' + establishmentUrlId + '/validate-value');
                    };
                    return Urls;
                })();
                Establishments.Urls = Urls;                
                var Locations = (function () {
                    function Locations() { }
                    Locations.get = function get(establishmentId) {
                        var url = 'establishments/' + establishmentId + '/location';
                        return makeUrl(url);
                    };
                    Locations.put = function put(establishmentId) {
                        return Locations.get(establishmentId);
                    };
                    return Locations;
                })();
                Establishments.Locations = Locations;                
                var Categories = (function () {
                    function Categories() { }
                    Categories.get = function get(id) {
                        var url = 'establishment-categories';
                        if(id) {
                            url += '/' + id;
                        }
                        return makeUrl(url);
                    };
                    return Categories;
                })();
                Establishments.Categories = Categories;                
            })(WebApi.Establishments || (WebApi.Establishments = {}));
            var Establishments = WebApi.Establishments;
            (function (My) {
                (function (Profile) {
                    function get() {
                        return makeUrl('my/profile');
                    }
                    Profile.get = get;
                    function put() {
                        return get();
                    }
                    Profile.put = put;
                    (function (Photo) {
                        function get(params) {
                            var url = post();
                            if(params) {
                                url += '?' + $.param(params);
                            }
                            return url;
                        }
                        Photo.get = get;
                        function post() {
                            return makeUrl('my/profile/photo');
                        }
                        Photo.post = post;
                        function del() {
                            return post();
                        }
                        Photo.del = del;
                        function kendoRemove() {
                            return makeUrl('my/profile/photo/kendo-remove');
                        }
                        Photo.kendoRemove = kendoRemove;
                    })(Profile.Photo || (Profile.Photo = {}));
                    var Photo = Profile.Photo;
                })(My.Profile || (My.Profile = {}));
                var Profile = My.Profile;
            })(WebApi.My || (WebApi.My = {}));
            var My = WebApi.My;
            (function (People) {
                (function (Photo) {
                    function get(personId, params) {
                        var url = 'people/' + personId + '/photo';
                        url = makeUrl(url);
                        if(params) {
                            url += '?' + $.param(params);
                        }
                        return url;
                    }
                    Photo.get = get;
                })(People.Photo || (People.Photo = {}));
                var Photo = People.Photo;
                (function (Names) {
                    var Salutations = (function () {
                        function Salutations() { }
                        Salutations.get = function get() {
                            return makeUrl('person-names/salutations');
                        };
                        return Salutations;
                    })();
                    Names.Salutations = Salutations;                    
                    var Suffixes = (function () {
                        function Suffixes() { }
                        Suffixes.get = function get() {
                            return makeUrl('person-names/suffixes');
                        };
                        return Suffixes;
                    })();
                    Names.Suffixes = Suffixes;                    
                    var DeriveDisplayName = (function () {
                        function DeriveDisplayName() { }
                        DeriveDisplayName.get = function get() {
                            return makeUrl('person-names/derive-display-name');
                        };
                        return DeriveDisplayName;
                    })();
                    Names.DeriveDisplayName = DeriveDisplayName;                    
                })(People.Names || (People.Names = {}));
                var Names = People.Names;
            })(WebApi.People || (WebApi.People = {}));
            var People = WebApi.People;
            (function (Employees) {
                (function (ModuleSettings) {
                    var FacultyRanks = (function () {
                        function FacultyRanks() { }
                        FacultyRanks.get = function get() {
                            return makeUrl('my/employee-module-settings/faculty-ranks');
                        };
                        return FacultyRanks;
                    })();
                    ModuleSettings.FacultyRanks = FacultyRanks;                    
                    var ActivityTypes = (function () {
                        function ActivityTypes() { }
                        ActivityTypes.get = function get() {
                            return makeUrl('my/employee-module-settings/activity-types');
                        };
                        return ActivityTypes;
                    })();
                    ModuleSettings.ActivityTypes = ActivityTypes;                    
                })(Employees.ModuleSettings || (Employees.ModuleSettings = {}));
                var ModuleSettings = Employees.ModuleSettings;
            })(WebApi.Employees || (WebApi.Employees = {}));
            var Employees = WebApi.Employees;
            (function (Activities) {
                function get(activityId) {
                    var url = makeUrl('activities');
                    if(activityId) {
                        url += '/' + activityId;
                    }
                    return url;
                }
                Activities.get = get;
                function post() {
                    return makeUrl('activities');
                }
                Activities.post = post;
                function getEdit(activityId) {
                    return makeUrl('activities/' + activityId + "/edit");
                }
                Activities.getEdit = getEdit;
                function getEditState(activityId) {
                    return makeUrl('activities/' + activityId + "/edit-state");
                }
                Activities.getEditState = getEditState;
                function put(activityId) {
                    return makeUrl('activities/' + activityId);
                }
                Activities.put = put;
                function putEdit(activityId) {
                    return makeUrl('activities/' + activityId + "/edit");
                }
                Activities.putEdit = putEdit;
                function del(activityId) {
                    return makeUrl('activities/' + activityId);
                }
                Activities.del = del;
                (function (Documents) {
                    function get(activityId, documentId, activityMode) {
                        var url = makeUrl('activities/' + activityId + '/documents');
                        if(documentId) {
                            url += '/' + documentId;
                        } else if(activityId) {
                            url += '/?activityMode=' + activityMode;
                        }
                        return url;
                    }
                    Documents.get = get;
                    function post(activityId, activityMode) {
                        var url = makeUrl('activities/' + activityId + '/documents/');
                        url += '?activityMode=' + activityMode;
                        return url;
                    }
                    Documents.post = post;
                    function put(activityId, documentId) {
                        return makeUrl('activities/' + activityId + '/documents/' + documentId);
                    }
                    Documents.put = put;
                    function del(activityId, documentId) {
                        return makeUrl('activities/' + activityId + '/documents/' + documentId);
                    }
                    Documents.del = del;
                    function rename(activityId, documentId) {
                        return makeUrl('activities/' + activityId + '/documents/' + documentId + "/title");
                    }
                    Documents.rename = rename;
                    function validateFileExtensions(activityId) {
                        return makeUrl('activities/' + activityId + '/documents/validate-upload-filetype');
                    }
                    Documents.validateFileExtensions = validateFileExtensions;
                    (function (Thumbnail) {
                        function get(activityId, documentId) {
                            return makeUrl('activities/' + activityId + '/documents/' + documentId + '/thumbnail');
                        }
                        Thumbnail.get = get;
                    })(Documents.Thumbnail || (Documents.Thumbnail = {}));
                    var Thumbnail = Documents.Thumbnail;
                })(Activities.Documents || (Activities.Documents = {}));
                var Documents = Activities.Documents;
                (function (Locations) {
                    function get() {
                        return makeUrl('activity-locations');
                    }
                    Locations.get = get;
                })(Activities.Locations || (Activities.Locations = {}));
                var Locations = Activities.Locations;
            })(WebApi.Activities || (WebApi.Activities = {}));
            var Activities = WebApi.Activities;
            (function (GeographicExpertises) {
                function get(expertiseId) {
                    var url = makeUrl('geoexpertises');
                    if(expertiseId) {
                        url += '/' + expertiseId;
                    }
                    return url;
                }
                GeographicExpertises.get = get;
                function post() {
                    return makeUrl('geoexpertises');
                }
                GeographicExpertises.post = post;
                function getEdit(expertiseId) {
                    return makeUrl('geoexpertises/' + expertiseId + "/edit");
                }
                GeographicExpertises.getEdit = getEdit;
                function getEditState(expertiseId) {
                    return makeUrl('geoexpertises/' + expertiseId + "/edit-state");
                }
                GeographicExpertises.getEditState = getEditState;
                function put(expertiseId) {
                    return makeUrl('geoexpertises/' + expertiseId);
                }
                GeographicExpertises.put = put;
                function putEdit(expertiseId) {
                    return makeUrl('geoexpertises/' + expertiseId + "/edit");
                }
                GeographicExpertises.putEdit = putEdit;
                function del(expertiseId) {
                    return makeUrl('geoexpertises/' + expertiseId);
                }
                GeographicExpertises.del = del;
                (function (Locations) {
                    function get() {
                        return makeUrl('geoexpertises-locations');
                    }
                    Locations.get = get;
                })(GeographicExpertises.Locations || (GeographicExpertises.Locations = {}));
                var Locations = GeographicExpertises.Locations;
            })(WebApi.GeographicExpertises || (WebApi.GeographicExpertises = {}));
            var GeographicExpertises = WebApi.GeographicExpertises;
            (function (LanguageExpertises) {
                function get(expertiseId) {
                    var url = makeUrl('langexpertises');
                    if(expertiseId) {
                        url += '/' + expertiseId;
                    }
                    return url;
                }
                LanguageExpertises.get = get;
                function post() {
                    return makeUrl('langexpertises');
                }
                LanguageExpertises.post = post;
                function getEdit(expertiseId) {
                    return makeUrl('langexpertises/' + expertiseId + "/edit");
                }
                LanguageExpertises.getEdit = getEdit;
                function getEditState(expertiseId) {
                    return makeUrl('langexpertises/' + expertiseId + "/edit-state");
                }
                LanguageExpertises.getEditState = getEditState;
                function put(expertiseId) {
                    return makeUrl('langexpertises/' + expertiseId);
                }
                LanguageExpertises.put = put;
                function putEdit(expertiseId) {
                    return makeUrl('langexpertises/' + expertiseId + "/edit");
                }
                LanguageExpertises.putEdit = putEdit;
                function del(expertiseId) {
                    return makeUrl('langexpertises/' + expertiseId);
                }
                LanguageExpertises.del = del;
            })(WebApi.LanguageExpertises || (WebApi.LanguageExpertises = {}));
            var LanguageExpertises = WebApi.LanguageExpertises;
            (function (FormalEducations) {
                function get(educationId) {
                    var url = makeUrl('educations');
                    if(educationId) {
                        url += '/' + educationId;
                    }
                    return url;
                }
                FormalEducations.get = get;
                function post() {
                    return makeUrl('educations');
                }
                FormalEducations.post = post;
                function getEdit(educationId) {
                    return makeUrl('educations/' + educationId + "/edit");
                }
                FormalEducations.getEdit = getEdit;
                function getEditState(educationId) {
                    return makeUrl('educations/' + educationId + "/edit-state");
                }
                FormalEducations.getEditState = getEditState;
                function put(educationId) {
                    return makeUrl('educations/' + educationId);
                }
                FormalEducations.put = put;
                function putEdit(educationId) {
                    return makeUrl('educations/' + educationId + "/edit");
                }
                FormalEducations.putEdit = putEdit;
                function del(educationId) {
                    return makeUrl('educations/' + educationId);
                }
                FormalEducations.del = del;
            })(WebApi.FormalEducations || (WebApi.FormalEducations = {}));
            var FormalEducations = WebApi.FormalEducations;
            (function (Affiliations) {
                function get(affiliationId) {
                    var url = makeUrl('affiliations');
                    if(affiliationId) {
                        url += '/' + affiliationId;
                    }
                    return url;
                }
                Affiliations.get = get;
                function post() {
                    return makeUrl('affiliations');
                }
                Affiliations.post = post;
                function getEdit(affiliationId) {
                    return makeUrl('affiliations/' + affiliationId + "/edit");
                }
                Affiliations.getEdit = getEdit;
                function getEditState(affiliationId) {
                    return makeUrl('affiliations/' + affiliationId + "/edit-state");
                }
                Affiliations.getEditState = getEditState;
                function put(affiliationId) {
                    return makeUrl('affiliations/' + affiliationId);
                }
                Affiliations.put = put;
                function putEdit(affiliationId) {
                    return makeUrl('affiliations/' + affiliationId + "/edit");
                }
                Affiliations.putEdit = putEdit;
                function del(affiliationId) {
                    return makeUrl('affiliations/' + affiliationId);
                }
                Affiliations.del = del;
            })(WebApi.Affiliations || (WebApi.Affiliations = {}));
            var Affiliations = WebApi.Affiliations;
        })(Routes.WebApi || (Routes.WebApi = {}));
        var WebApi = Routes.WebApi;
        (function (Mvc) {
            function makeUrl(relativeUrl) {
                var url = Routes.applicationPath + relativeUrl;
                if(!hasTrailingSlash(url)) {
                    url = url + '/';
                }
                return url;
            }
            (function (Establishments) {
                function show(establishmentId) {
                    return makeUrl('establishments/' + establishmentId);
                }
                Establishments.show = show;
                function created(params) {
                    var url = makeUrl('establishments/created');
                    if(params) {
                        url += '?' + $.param(params);
                    }
                    return url;
                }
                Establishments.created = created;
            })(Mvc.Establishments || (Mvc.Establishments = {}));
            var Establishments = Mvc.Establishments;
            (function (Identity) {
                (function (Users) {
                    function created(params) {
                        var url = makeUrl('users/created');
                        if(params) {
                            url += '?' + $.param(params);
                        }
                        return url;
                    }
                    Users.created = created;
                })(Identity.Users || (Identity.Users = {}));
                var Users = Identity.Users;
            })(Mvc.Identity || (Mvc.Identity = {}));
            var Identity = Mvc.Identity;
            (function (My) {
                (function (Profile) {
                    function get() {
                        return makeUrl('my/profile');
                    }
                    Profile.get = get;
                    function activityEdit(activityId) {
                        var url = makeUrl('my/activity/');
                        return url + activityId;
                    }
                    Profile.activityEdit = activityEdit;
                    function geographicExpertiseEdit(expertiseId) {
                        var url = makeUrl('my/geoexpertise/');
                        return url + expertiseId;
                    }
                    Profile.geographicExpertiseEdit = geographicExpertiseEdit;
                    function languageExpertiseEdit(expertiseId) {
                        var url = makeUrl('my/langexpertise/');
                        return url + expertiseId;
                    }
                    Profile.languageExpertiseEdit = languageExpertiseEdit;
                    function formalEducationEdit(expertiseId) {
                        var url = makeUrl('my/education/');
                        return url + expertiseId;
                    }
                    Profile.formalEducationEdit = formalEducationEdit;
                    function affiliationEdit(expertiseId) {
                        var url = makeUrl('my/affiliation/');
                        return url + expertiseId;
                    }
                    Profile.affiliationEdit = affiliationEdit;
                })(My.Profile || (My.Profile = {}));
                var Profile = My.Profile;
            })(Mvc.My || (Mvc.My = {}));
            var My = Mvc.My;
        })(Routes.Mvc || (Routes.Mvc = {}));
        var Mvc = Routes.Mvc;
        (function (Content) {
            function makeUrl(relativeUrl) {
                var url = Routes.applicationPath + relativeUrl;
                if(!hasTrailingSlash(url)) {
                    url = url + '/';
                }
                return url;
            }
            function styles(relativePath) {
                var url = 'styles';
                url = makeUrl(url);
                url += relativePath;
                return url;
            }
            Content.styles = styles;
        })(Routes.Content || (Routes.Content = {}));
        var Content = Routes.Content;
    })(App.Routes || (App.Routes = {}));
    var Routes = App.Routes;
})(App || (App = {}));
