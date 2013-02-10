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

            export interface IFilterPlaces {
                parentId?: number;
                isContinent?: bool;
                isCountry?: bool;
                isAdmin1?: bool;
                isAdmin2?: bool;
                isAdmin3?: bool;
            }

            function getByFilterArgs(args: IFilterPlaces): string {
                var url = makeUrl('places');
                url = url.substr(0, url.length - 1); // strip trailing slash
                url += '?';
                if (args.parentId) url += 'parentId=' + args.parentId + '&';
                if (args.isContinent) url += 'isContinent=' + args.isContinent + '&';
                if (args.isCountry) url += 'isCountry=' + args.isCountry + '&';
                if (args.isAdmin1) url += 'isAdmin1=' + args.isAdmin1 + '&';
                if (args.isAdmin2) url += 'isAdmin2=' + args.isAdmin2 + '&';
                if (args.isAdmin3) url += 'isAdmin3=' + args.isAdmin3 + '&';
                if (url.lastIndexOf('&') === url.length - 1) // strip trailing amphersand
                    url = url.substr(0, url.length - 1);
                return url;
            }

            function getByLatLng(latitude: number, longitude: number): string {
                var url = 'places/by-coordinates/' + latitude + '/' + longitude;
                return makeUrl(url);
            }

            export function get (args: IFilterPlaces): string;
            export function get (latitude: number, longitude: number): string;
            export function get (arg1: any, arg2?: number): string {
                if (!arg2 || typeof arg2 !== 'number')
                    return getByFilterArgs(arg1);
                else
                    return getByLatLng(arg1, arg2);
            }
        }

        export module Establishments {

            export function get (establishmentId?: number): string {
                var url = 'establishments';
                if (establishmentId)
                    url += '/' + establishmentId;
                return makeUrl(url);
            }

            export class Names {

                static get (establishmentId: number, establishmentNameId?: number): string {
                    var url = 'establishments/' + establishmentId + '/names';
                    if (establishmentNameId)
                        url += '/' + establishmentNameId;
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
                    if (establishmentUrlId)
                        url += '/' + establishmentUrlId;
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
            }

        }

        export module My {
            export class Profile {
                static get (): string {
                    return makeUrl('my/profile');
                }
                static put (): string {
                    return get();
                }
            }
        }

        export module People {
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
            }
        }
    }
}


