﻿function WebApiRoutes(applicationPath) {
    var self = this;

    function getAbsoluteUrl(url) {
        return applicationPath + url;
    }

    self.identity = {
        signIn: function() {
            return getAbsoluteUrl('api/sign-in/');
        },
        signOut: function() {
            return getAbsoluteUrl('api/sign-out/');
        }
    };

    self.countries = {
        get: function () {
            return getAbsoluteUrl('api/countries/');
        }
    };

    self.languages = {
        get: function () {
            return getAbsoluteUrl('api/languages/');
        }
    };

    self.establishments = {
        get: function() {
            return getAbsoluteUrl('api/establishments/');
        }
    };

    self.establishmentNames = {
        get: function (establishmentId, establishmentNameId) {
            var url = 'api/establishments/' + establishmentId + '/names/';
            if (establishmentNameId)
                url += establishmentNameId + '/';
            return getAbsoluteUrl(url);
        },
        post: function (establishmentId) {
            return this.get(establishmentId);
        },
        put: function (establishmentId, establishmentNameId) {
            return getAbsoluteUrl('api/establishments/' + establishmentId + '/names/' + establishmentNameId + '/');
        },
        del: function (establishmentId, establishmentNameId) {
            return this.put(establishmentId, establishmentNameId);
        },
        validateText: function(establishmentId, establishmentNameId) {
            return getAbsoluteUrl('api/establishments/' + establishmentId + '/names/' + establishmentNameId + '/validate-text/');
        }
    };
}

//function MvcRoutes(applicationPath) {
//    var self = this;
//
//    function getAbsoluteUrl(url) {
//        return applicationPath + url;
//    }
//}

function Routes(applicationPath) {
    var self = this;

    self.webApi = new WebApiRoutes(applicationPath);
    //self.mvc = new MvcRoutes(applicationPath);
}
