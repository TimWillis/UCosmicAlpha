module Agreements.ViewModels {

    export class PublicView {
        constructor(agreementId: number, agreementVisibility: string) {
            if (isNaN(agreementId)) {
                agreementId = 0;
            }
            this.agreementId = agreementId;
            this.agreementVisibility = agreementVisibility || 'Public';
            this.fileListPopulator = new Agreements.FileListPopulator();
            if (this.agreementId !== 0) {
                var dataBound = this._bindData();
                var mapCreated = this._createMap();
                $.when(dataBound, mapCreated).done((): void => {
                    this._bindMap();
                });
            }
            this.files = this.fileListPopulator.files;
            this.fileListPopulator.populate(this.agreementId);
            this.populateContacts();
            if (sessionStorage.getItem("agreementSaved") == "yes") {
                sessionStorage.setItem("agreementSaved", "no");
                App.flasher.flash("Agreement saved");
            }
        }
        //imported class instances
        fileListPopulator;

        agreementId: any;
        agreementVisibility: string;
        isBound = ko.observable(false);
        files = ko.observableArray();
        content = ko.observable();
        expiresOn = ko.observable<string>();
        isAutoRenew = ko.observable();
        status = ko.observable();
        isExpirationEstimated = ko.observable();
        name = ko.observable();
        notes = ko.observable();
        participants = ko.observableArray();
        startsOn = ko.observable<string>();
        type = ko.observable();
        umbrellaId = ko.observable();
        contacts = ko.mapping.fromJS([]);

        fileHref(parent, data): string {
            return '/api/agreements/' + parent.agreementId + '/files/' + data.id() + '/content/';
        }

        fileDownloadHref(parent, data): string {
            return '/api/agreements/' + parent.agreementId + '/files/' + data.id() + '/download/';
        }

        private _bindData(): JQueryDeferred<void> {
            var deferred = $.Deferred();
            $.get(App.Routes.WebApi.Agreements.get(this.agreementId))
                .done((response: any): void => {
                    var mapping = {
                        participants: { // don't need observables on participant properties
                            create: (options: KnockoutMappingCreateOptions): any => {
                                return options.data;
                            }
                        },
                    };
                    ko.mapping.fromJS(response, mapping, this);
                    this.participants(Enumerable.From(this.participants())
                        .OrderBy(function (x: ApiModels.Participant): boolean {
                            return x.isOwner;
                        })
                        .ThenBy(function (x: ApiModels.Participant): string {
                            return x.establishmentTranslatedName;
                        })
                        .ToArray());
                    this.isBound(true);
                    deferred.resolve();
                });
            return deferred;
        }

        startsOnDate = ko.computed((): string => {
            var value = this.startsOn();
            var myDate = new Date(value);
            if (myDate.getFullYear() < 1500) {
                return "unknown";
            }
            return (moment(value)).format('M/D/YYYY');
        });
        expiresOnDate = ko.computed((): string => {
            var value = this.expiresOn();
            var myDate = new Date(value);
            if (myDate.getFullYear() < 1500) {
                return "unknown";
            } else {
                return (moment(value)).format('M/D/YYYY');
            }
        });

        private _googleMap: google.maps.Map;
        private _createMap(): JQueryDeferred<void> {
            google.maps.visualRefresh = true;
            var options: google.maps.MapOptions = {
                mapTypeId: google.maps.MapTypeId.ROADMAP,
                center: new google.maps.LatLng(0, 0), // americas on left, australia on right
                zoom: 1, // zoom out
                draggable: true, // allow map panning
                scrollwheel: false, // prevent mouse wheel zooming
                streetViewControl: false,
                panControl: false,
                zoomControlOptions: {
                    style: google.maps.ZoomControlStyle.SMALL,
                },
            };
            this._googleMap = new google.maps.Map(document.getElementById("map-canvas"), options);
            var deferred = $.Deferred();
            google.maps.event.addListenerOnce(this._googleMap, 'idle', function () {
                deferred.resolve();
            });
            return deferred;
        }
        private _googleMarkers = ko.observableArray<google.maps.Marker>();
        private _bindMap(): void {
            // extract the partners from participants (they are non-owners)
            var partners = Enumerable.From(this.participants())
                .Where(function (x) { return !x.isOwner; }).ToArray();
            // collect together all of the plottable partner marker points
            var centers = Enumerable.From(partners)
            // note that centers.length may not be the same as partners.length
                .Where(function (x) { return x.center && x.center.hasValue; })
                .Select(function (x) { return x.center; }).ToArray();
            var latLngs = Enumerable.From(centers)
                .Select(function (x) { return new google.maps.LatLng(x.latitude, x.longitude); })
                .ToArray();

            var bounds = new google.maps.LatLngBounds();
            $.each(latLngs, (index: number, latLng: google.maps.LatLng): void => {
                // matching partner may have different index than this latLng
                var title = Enumerable.From(partners)
                    .Single(function (x) {
                        return x.center && x.center == centers[index];
                    }).establishmentTranslatedName;
                // plot the marker
                var options: google.maps.MarkerOptions = {
                    map: this._googleMap,
                    position: latLng,
                    title: title,
                };
                var marker = new google.maps.Marker(options);
                bounds.extend(latLng);
                this._googleMarkers.push(marker);
            });

            // when there is only 1 marker, center the map on it
            if (centers.length == 1) {
                this._googleMap.setCenter(latLngs[0]);
                // check to see if the centered partner has zoom
                var partner = Enumerable.From(partners)
                    .Single(function (x) {
                        return x.center && x.center == centers[0];
                    });
                var zoom = partner.googleMapZoomLevel;
                if (zoom) {
                    //this._animateMapZoom(zoom);
                    this._googleMap.setZoom(zoom);
                }
                // see if partner has bounding box without zoom
                else if (partner.boundingBox && partner.boundingBox.hasValue) {
                    bounds = new google.maps.LatLngBounds(
                        new google.maps.LatLng(partner.boundingBox.southWest.latitude,
                            partner.boundingBox.southWest.longitude),
                        new google.maps.LatLng(partner.boundingBox.northEast.latitude,
                            partner.boundingBox.northEast.longitude)
                        );
                    this._googleMap.fitBounds(bounds);
                    this._googleMap.setCenter(latLngs[0]);
                }
            }
            else if (centers.length > 0) {
                this._googleMap.fitBounds(bounds);
            }
        }

        private _animateMapZoom(zoom: number): void {
            var currentZoom = this._googleMap.getZoom();
            if (currentZoom == zoom) return;
            var difference = zoom - currentZoom;
            var direction = difference > 0 ? 1 : -1;
            var nextZoom = currentZoom + direction;
            this._googleMap.setZoom(nextZoom);
            if (nextZoom != currentZoom)
                setTimeout((): void => {
                    this._animateMapZoom(zoom);
                }, 100);
        }

        createMap(): void {
            var self = this;
            function initialize() {
                //https://developers.google.com/maps/documentation/javascript/markers#add
                var partners = Enumerable.From(self.participants())
                    .Where(function (x) {
                            return !x.isOwner
                        }).ToArray(),
                    centers = Enumerable.From(self.participants())
                        .Select(function (x) {
                            return x.center;
                        }).ToArray(),
                    LatLngList = Enumerable.From(centers)
                        .Select(function (x) {
                            return new google.maps.LatLng(x.latitude, x.longitude);
                        }).ToArray(), map;
                //  Make an array of the LatLng's of the markers you want to show
                //var LatLngList = new Array(new google.maps.LatLng(52.537, -2.061), new google.maps.LatLng(52.564, -2.017));
                //  Create a new viewpoint bound
                var bounds = new google.maps.LatLngBounds();
                //  Go through each...
                for (var i = 0, LtLgLen = LatLngList.length; i < LtLgLen; i++) {
                    //  And increase the bounds to take this point
                    bounds.extend(LatLngList[i]);
                }
                //  Fit these bounds to the map
                map = new google.maps.Map(document.getElementById("map-canvas"), {
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    mapMaker: true,
                })
                map.fitBounds(bounds);
            }
            google.maps.event.addDomListener(window, 'load', initialize);
        }

        populateContacts(): void {
            $.get(App.Routes.WebApi.Agreements.Contacts.get(this.agreementId))
                .done((response: any): void => {
                    ko.mapping.fromJS(response, this.contacts)
                });
        }
    }
}