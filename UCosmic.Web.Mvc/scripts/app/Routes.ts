/// <reference path="../jquery/jquery-1.8.d.ts" />

module App.Routes {

    export var applicationPath: string = '/';

    function hasTrailingSlash(value: string): bool {
        return value.lastIndexOf('/') == value.length - 1;
    }

    export module WebApi {

        export var urlPrefix: string = 'api';

        function makeUrl(relativeUrl: string): string {
            var apiPrefix: string = WebApi.urlPrefix;
            if (!hasTrailingSlash(apiPrefix)) apiPrefix = apiPrefix + '/';
            var url = Routes.applicationPath + apiPrefix + relativeUrl;
            if (!hasTrailingSlash(url)) url = url + '/';
            return url;
        }

        export module Identity {

            export function signIn(): string {
                return makeUrl('sign-in');
            }

            export function signOut(): string {
                return makeUrl('sign-out');
            }

            export module Users {
                export function get (id?: number) {
                    var url = 'users';
                    if (id) url += '/' + id;
                    return makeUrl(url);
                }

                export function post () {
                    return makeUrl('users');
                }

                export function validateName (id?: number) {
                    id = id ? id : 0;
                    var url = 'users/' + id + '/validate-name';
                    return makeUrl(url);
                }

                export module Roles {
                    export function get (userId: number): string {
                        var url = 'users/' + userId + '/roles';
                        return makeUrl(url);
                    }

                    export function put(userId: number, roleId: number): string {
                        var url = 'users/' + userId + '/roles/' + roleId;
                        return makeUrl(url);
                    }

                    export function del(userId: number, roleId: number): string {
                        return put(userId, roleId);
                    }

                    export function validateGrant(userId: number, roleId: number) {
                        var url = 'users/' + userId + '/roles/' + roleId + '/validate-grant';
                        return makeUrl(url);
                    }

                    export function validateRevoke(userId: number, roleId: number) {
                        var url = 'users/' + userId + '/roles/' + roleId + '/validate-revoke';
                        return makeUrl(url);
                    }
                }
            }

            export module Roles {
                export function get (roleId?: number): string {
                    var url = 'roles';
                    if (roleId) url += '/' + roleId;
                    return makeUrl(url);
                }
            }
        }

        export module Languages {

            export function get (): string {
                return makeUrl('languages');
            }
        }

        export module Countries {

            export function get (): string {
                return makeUrl('countries');
            }
        }

        export module Places {

            export function get(placeId?: number): string {
                var url = 'places';
                if (placeId) url += '/' + placeId;
                return makeUrl(url);
            }

            export module ByCoordinates {
                export function get(latitude: number, longitude: number): string {
                    var url = 'places/by-coordinates/' + latitude + '/' + longitude;
                    return makeUrl(url);
                }
            }
        }

        export module Establishments {

            export function get (establishmentId?: number): string {
                var url = 'establishments';
                if (establishmentId) url += '/' + establishmentId;
                return makeUrl(url);
            }

            export function post(): string {
                return makeUrl('establishments');
            }

            export function put(establishmentId: number): string {
                return get(establishmentId);
            }

            export function validateCeebCode (establishmentId: number): string {
                    return makeUrl('establishments/' + establishmentId + '/validate-ceeb-code');
            }

            export function validateUCosmicCode (establishmentId: number): string {
                    return makeUrl('establishments/' + establishmentId + '/validate-ucosmic-code');
            }

            export class Names {

                static get (establishmentId: number, establishmentNameId?: number): string {
                    var url = 'establishments/' + establishmentId + '/names';
                    if (establishmentNameId) url += '/' + establishmentNameId;
                    return makeUrl(url);
                }

                static post(establishmentId: number): string {
                    return Names.get(establishmentId);
                }

                static put(establishmentId: number, establishmentNameId: number): string {
                    return makeUrl('establishments/' + establishmentId + '/names/'
                        + establishmentNameId);
                }

                static del(establishmentId: number, establishmentNameId: number): string {
                    return Names.put(establishmentId, establishmentNameId);
                }

                static validateText(establishmentId: number, establishmentNameId: number): string {
                    return makeUrl('establishments/' + establishmentId + '/names/'
                        + establishmentNameId + '/validate-text');
                }
            }

            export class Urls {

                static get (establishmentId: number, establishmentUrlId?: number): string {
                    var url = 'establishments/' + establishmentId + '/urls';
                    if (establishmentUrlId) url += '/' + establishmentUrlId;
                    return makeUrl(url);
                }

                static post(establishmentId: number): string {
                    return Urls.get(establishmentId);
                }

                static put(establishmentId: number, establishmentUrlId: number): string {
                    return makeUrl('establishments/' + establishmentId + '/urls/'
                        + establishmentUrlId);
                }

                static del(establishmentId: number, establishmentUrlId: number): string {
                    return Urls.put(establishmentId, establishmentUrlId);
                }

                static validateValue(establishmentId: number, establishmentUrlId: number): string {
                    return makeUrl('establishments/' + establishmentId + '/urls/'
                        + establishmentUrlId + '/validate-value');
                }
            }

            export class Locations {

                static get (establishmentId: number): string {
                    var url = 'establishments/' + establishmentId + '/location';
                    return makeUrl(url);
                }

                static put(establishmentId: number): string {
                    return get(establishmentId);
                }
            }

            export class Categories {

                static get (id?: number): string {
                    var url = 'establishment-categories';
                    if (id) url += '/' + id;
                    return makeUrl(url);
                }
            }
        }

        export module My {
            export module Profile {
                export function get (): string {
                    return makeUrl('my/profile');
                }
                export function put(): string {
                    return get();
                }
                export module Photo {
                    export function get(params?: any): string {
                        var url = post();
                        if (params) url += '?' + $.param(params);
                        return url;
                    }
                    export function post() {
                        return makeUrl('my/profile/photo');
                    }
                    export function del() {
                        return post();
                    }
                    export function kendoRemove() {
                        return makeUrl('my/profile/photo/kendo-remove');
                    }
                }
            }
        }

        export module People {
            export module Photo {

                export function get(personId: number, params?: any): string {
                    var url = 'people/' + personId + '/photo';
                    url = makeUrl(url);
                    if (params) url += '?' + $.param(params);
                    return url;
                }
            }
            export module Names {
                export class Salutations {
                    static get (): string {
                        return makeUrl('person-names/salutations');
                    }
                }
                export class Suffixes {
                    static get (): string {
                        return makeUrl('person-names/suffixes');
                    }
                }
                export class DeriveDisplayName {
                    static get (): string {
                        return makeUrl('person-names/derive-display-name');
                    }
                }
            }
        }

        export module Employees {
            export module ModuleSettings {
                export class FacultyRanks {
                    static get (): string {
                        return makeUrl('my/employee-module-settings/faculty-ranks');
                    }
                }
                export class ActivityTypes {
                    static get (): string {
                        return makeUrl('my/employee-module-settings/activity-types');
                    }
                }
            }
        }

        export module Activities {

            export function get (): string {
                return makeUrl('activities/page');
            }

            export function getDocProxy (): string {
                    return makeUrl('activities/docproxy');
            }

            export class Locations {
                static get (): string {
                    return makeUrl('activities/locations');
                }
            }

            export class Delete {
                static get (): string {
                    return makeUrl('activities/delete');
                }
            }


        }

        export module Activity {

            export function get (): string {
                return makeUrl('activity');
            }

            export function uploadDocument(): string {
                return makeUrl('activity/upload');
            }

            export function validateUploadFileTypeByExtension(activityId: number): string {
                return makeUrl('activity/' + activityId.toString() + '/validate-upload-filetype');
            }

            export function getDocuments(activityValuesId: number): string {
                return makeUrl('activity/' + activityValuesId.toString() + '/documents');
            }

            export function deleteDocument(activityDocumentId: number): string {
                return makeUrl('activity/' + activityDocumentId.toString() + '/document');
            }
        }
    }

    export module Mvc {

        function makeUrl(relativeUrl: string): string {
            var url = Routes.applicationPath + relativeUrl;
            if (!hasTrailingSlash(url)) url = url + '/';
            return url;
        }

        export module Establishments {
            export function show(establishmentId: number) {
                return makeUrl('establishments/' + establishmentId);
            }
            export function created(params?: any) {
                var url = makeUrl('establishments/created');
                if (params) url += '?' + $.param(params);
                return url;
            }
        }

        export module Identity {
            export module Users {
                export function created(params?: any) {
                    var url = makeUrl('users/created');
                    if (params) url += '?' + $.param(params);
                    return url;
                }
            }
        }
    }

    //export module Params {
    //    export class ImageResizeQuality {
    //        static THUMBNAIL: string = 'thumbnail';
    //        static HIGH: string = 'high';
    //    }
    //}
}
