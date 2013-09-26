var __extends = this.__extends || function (d, b) {
    for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p];
    function __() { this.constructor = d; }
    __.prototype = b.prototype;
    d.prototype = new __();
};
var ViewModels;
(function (ViewModels) {
    /// <reference path="../../typings/jquery/jquery.d.ts" />
    /// <reference path="../../typings/jqueryui/jqueryui.d.ts" />
    /// <reference path="../../typings/knockout/knockout.d.ts" />
    /// <reference path="../../typings/knockout.mapping/knockout.mapping.d.ts" />
    /// <reference path="../../typings/knockout.validation/knockout.validation.d.ts" />
    /// <reference path="../../typings/kendo/kendo.all.d.ts" />
    /// <reference path="../../typings/moment/moment.d.ts" />
    /// <reference path="../../typings/googlemaps/google.maps.d.ts" />
    /// <reference path="../../typings/sammyjs/sammyjs.d.ts" />
    /// <reference path="../../app/Routes.ts" />
    /// <reference path="../../app/Spinner.ts" />
    /// <reference path="../activities/ServiceApiModel.d.ts" />
    (function (Employees) {
        var FacultyAndStaffSelect = (function () {
            function FacultyAndStaffSelect() {
            }
            FacultyAndStaffSelect.prototype.load = function () {
                var _this = this;
                var deferred = $.Deferred();

                //this.loadSpinner.start();
                $.ajax({
                    type: "GET",
                    async: true,
                    dataType: 'json',
                    url: App.Routes.WebApi.Establishments.get(),
                    data: {
                        pageNumber: 1,
                        pageSize: App.Constants.int32Max,
                        typeEnglishNames: ['University', 'University System'],
                        orderBy: 'name-asc'
                    },
                    success: function (data, textStatus, jqXhr) {
                        _this.institutions = ko.mapping.fromJS(data);
                        deferred.resolve();
                    },
                    error: function (jqXhr, textStatus, errorThrown) {
                        deferred.reject(errorThrown);
                    },
                    complete: function (jqXhr, textStatus) {
                        //this.loadSpinner.stop();
                    }
                });

                return deferred;
            };

            FacultyAndStaffSelect.prototype.selectInstitutionUrl = function (institutionId) {
                return App.Routes.Mvc.FacultyStaff.Institution.select(institutionId);
            };
            return FacultyAndStaffSelect;
        })();
        Employees.FacultyAndStaffSelect = FacultyAndStaffSelect;

        var FacultyAndStaff = (function () {
            function FacultyAndStaff(institutionInfo) {
                /* If you add or remove from this list, also look at _getHeatmapActivityDataTable()
                and _getHeatmapPeopleDataTable() to update the custom place tooltips text. */
                this.geochartCustomPlaces = [
                    {
                        name: 'Antarctica',
                        id: 'antarctica',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Southern Ocean',
                        id: 'southernOcean',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Indian Ocean',
                        id: 'indianOcean',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Pacific Ocean',
                        id: 'pacificOcean',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Atlantic Ocean',
                        id: 'atlanticOcean',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Gulf of Mexico',
                        id: 'gulfOfMexico',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Caribbean',
                        id: 'caribbean',
                        activityCount: 0,
                        peopleCount: 0
                    },
                    {
                        name: 'Arctic Ocean',
                        id: 'arcticOcean',
                        activityCount: 0,
                        peopleCount: 0
                    }
                ];
                this._initialize(institutionInfo);
            }
            FacultyAndStaff.prototype._initialize = function (institutionInfo) {
                this.sammy = Sammy();
                this.initialLocations = [];
                this.selectedLocationValues = [];
                this.fromDate = ko.observable();
                this.toDate = ko.observable();
                this.includeUndated = ko.observable(true);
                this.establishmentId = ko.observable(null);
                this.establishmentOfficialName = ko.observable(null);
                this.establishmentCountryOfficialName = ko.observable(null);
                this.institutionHasCampuses = ko.observable(false);
                this.institutionDropListData = [];
                this.locations = ko.observableArray();
                this.activityTypes = ko.observableArray();

                //this.selectedActivityIds = ko.observableArray();
                this.isHeatmapVisible = ko.observable(true);
                this.isPointmapVisible = ko.observable(false);
                this.isExpertVisible = ko.observable(false);

                //this.isTableVisible = ko.observable(false);
                this.mapType = ko.observable('heatmap');
                this.searchType = ko.observable('activities');
                this.selectedPlace = ko.observable(null);
                this.mapRegion = ko.observable('world');
                this.isGlobalView = ko.observable(true);

                this.degrees = ko.observable();
                this.tags = ko.observable();

                this.loadSpinner = new App.Spinner(new App.SpinnerOptions(200));

                this.globalActivityCountData = null;
                this.placeActivityCountData = null;
                this.globalPeopleCountData = null;
                this.placePeopleCountData = null;
                this.globalActivityTrendData = null;
                this.placeActivityTrendData = null;
                this.globalPeopleTrendData = null;
                this.placePeopleTrendData = null;
                this.heatmapActivityDataTable = null;
                this.heatmapPeopleDataTable = null;
                this.activityResults = ko.mapping.fromJS({
                    placeResults: [
                        {
                            officialName: '',
                            results: []
                        }
                    ]
                });
                this.peopleResults = ko.mapping.fromJS({
                    placeResults: [
                        {
                            officialName: '',
                            results: []
                        }
                    ]
                });
                this.pointmapActivityMarkers = null;
                this.pointmapPeopleMarkers = null;

                this.totalCount = ko.observable(0);
                this.totalPlaceCount = ko.observable(0);

                this.degreeCount = ko.observable(0);

                this.selectSearchType('activities');

                this.lenses = ko.observableArray([
                    { text: 'Map', value: 'map' },
                    { text: 'Table', value: 'table' }
                ]);
                this.lens = ko.observable('map');

                if (institutionInfo != null) {
                    if (institutionInfo.InstitutionId != null) {
                        this.establishmentId(Number(institutionInfo.InstitutionId));

                        this.institutionDropListData.push({
                            "officialName": institutionInfo.InstitutionOfficialName,
                            "id": institutionInfo.InstitutionId
                        });
                    }

                    if (institutionInfo.InstitutionHasCampuses != null) {
                        this.institutionHasCampuses(Boolean(institutionInfo.InstitutionHasCampuses));
                    }

                    if ((institutionInfo.InstitutionCampusIds != null) && (institutionInfo.InstitutionCampusIds.length > 0)) {
                        for (var i = 0; i < institutionInfo.InstitutionCampusIds.length; i += 1) {
                            this.institutionDropListData.push({
                                "officialName": institutionInfo.InstitutionCampusOfficialNames[i],
                                "id": institutionInfo.InstitutionCampusIds[i]
                            });
                        }
                    }
                }
            };

            FacultyAndStaff.prototype.setupWidgets = function (locationSelectorId, fromDatePickerId, toDatePickerId, establishmentDropListId, campusDropListId, collegeDropListId, departmentDropListId) {
                var _this = this;
                this.locationSelectorId = locationSelectorId;

                /*
                There appears to be a number of bugs/undocumented behaviors associated
                with the KendoUI Multiselect when using a dataSource that gets data
                from service via ajax.
                
                1) The control will query the service as soon as focus us obtained.  Event
                with minLength at three, it will query the server with no keyword
                and the service will return ALL Places (quite large).  See note in
                GraphicExpertiseEdit.cshtml on how this problem was circumvented.
                (Note: autoBind: false did NOT fix this problem.)
                
                2) Setting the initial values (dataItems) does not work as expected when
                we started using the ajax datasource.  To solve the problem, we use
                the initial Places AS the datasource and then change the datasource
                later to the ajax service.
                */
                var me = this;
                $("#" + locationSelectorId).kendoMultiSelect({
                    autoBind: true,
                    dataTextField: "officialName",
                    dataValueField: "id",
                    minLength: 3,
                    dataSource: me.initialLocations,
                    value: me.selectedLocationValues,
                    change: function (event) {
                        _this.updateLocations(event.sender.dataItems());
                    },
                    placeholder: "[Select Country/Location]"
                });

                $("#" + fromDatePickerId).kendoDatePicker({
                    /* If user clicks date picker button, reset format */
                    open: function (e) {
                        this.options.format = "MM/dd/yyyy";
                    }
                });

                $("#" + toDatePickerId).kendoDatePicker({
                    open: function (e) {
                        this.options.format = "MM/dd/yyyy";
                    }
                });

                this.establishmentDropListId = establishmentDropListId;
                this.campusDropListId = campusDropListId;
                this.collegeDropListId = collegeDropListId;
                this.departmentDropListId = departmentDropListId;

                //$("#" + establishmentDropListId).kendoAutoComplete({
                //    minLength: 3,
                //    filter: "contains",
                //    ignoreCase: true,
                //    placeholder: "[Enter Institution]",
                //    dataTextField: "officialName",
                //    dataSource: new kendo.data.DataSource({
                //        serverFiltering: true,
                //        transport: {
                //            read: (options: any): void => {
                //                $.ajax({
                //                    url: App.Routes.WebApi.Establishments.get(),
                //                    data: {
                //                        typeEnglishNames: ['University', 'University System']
                //                    },
                //                    success: (results: any): void => {
                //                        options.success(results.items);
                //                    }
                //                });
                //            }
                //        }
                //    }),
                //    change: (e: any): void => {
                //        this.checkInstitutionForNull();
                //    },
                //    select: (e: any): void => {
                //        var me = $("#" + establishmentDropListId).data("kendoAutoComplete");
                //        var dataItem = me.dataItem(e.item.index());
                //        this.establishmentOfficialName(dataItem.officialName);
                //        this.establishmentId(dataItem.id);
                //        if ((dataItem.countryName != null) && (dataItem.countryName.length > 0)) {
                //            this.establishmentCountryOfficialName(dataItem.countryName);
                //        }
                //        else {
                //            this.establishmentCountryOfficialName(null);
                //        }
                //    }
                //});
                $("#" + establishmentDropListId).kendoDropDownList({
                    dataTextField: "officialName",
                    dataValueField: "id",
                    dataSource: this.institutionDropListData,
                    change: function (e) {
                        var item = this.dataItem(e.sender.selectedIndex);
                        me.clearCachedData();
                        me.establishmentId(item.id);
                        me.establishmentOfficialName(item.officialName);
                        me.selectMap(me.mapType());

                        me.drawPointmap(true);
                    }
                });

                $("#" + departmentDropListId).kendoDropDownList({
                    dataTextField: "officialName",
                    dataValueField: "id",
                    optionLabel: { officialName: "[All Departments]", id: 0 },
                    change: function (e) {
                        me.drawPointmap(true);
                    },
                    dataBound: function (e) {
                        if ((this.selectedIndex != null) && (this.selectedIndex != -1)) {
                            var item = this.dataItem(this.selectedIndex);
                            if (item == null) {
                                this.text("");
                                $("#departmenDiv").hide();
                            } else {
                                $("#departmenDiv").show();
                            }
                        } else {
                            $("#departmenDiv").hide();
                        }
                    }
                });

                var collegeDropListDataSource = null;

                if (!this.institutionHasCampuses()) {
                    collegeDropListDataSource = new kendo.data.DataSource({
                        transport: {
                            read: {
                                url: App.Routes.WebApi.Establishments.getChildren(this.establishmentId()),
                                data: { orderBy: ['rank-asc', 'name-asc'] }
                            }
                        }
                    });
                }

                $("#" + collegeDropListId).kendoDropDownList({
                    dataTextField: "officialName",
                    dataValueField: "id",
                    optionLabel: { officialName: "[All Colleges]", id: 0 },
                    dataSource: collegeDropListDataSource,
                    change: function (e) {
                        var selectedIndex = e.sender.selectedIndex;
                        if (selectedIndex != -1) {
                            var item = this.dataItem(selectedIndex);
                            if ((item != null) && (item.id != 0)) {
                                var dataSource = new kendo.data.DataSource({
                                    transport: {
                                        read: {
                                            url: App.Routes.WebApi.Establishments.getChildren(item.id),
                                            data: { orderBy: ['rank-asc', 'name-asc'] }
                                        }
                                    }
                                });

                                $("#" + departmentDropListId).data("kendoDropDownList").setDataSource(dataSource);
                            }

                            if (selectedIndex == 0) {
                                $("#" + departmentDropListId).data("kendoDropDownList").setDataSource(new kendo.data.DataSource());
                            }

                            me.drawPointmap(true);
                        }
                    },
                    dataBound: function (e) {
                        if ((this.selectedIndex != null) && (this.selectedIndex != -1)) {
                            var item = this.dataItem(this.selectedIndex);
                            if ((item != null) && (item.id != 0)) {
                                var collegeId = item.id;
                                if (collegeId != null) {
                                    var dataSource = new kendo.data.DataSource({
                                        transport: {
                                            read: {
                                                url: App.Routes.WebApi.Establishments.getChildren(collegeId),
                                                data: { orderBy: ['rank-asc', 'name-asc'] }
                                            }
                                        }
                                    });

                                    $("#" + departmentDropListId).data("kendoDropDownList").setDataSource(dataSource);
                                }
                            }
                        }
                    }
                });

                if (this.institutionHasCampuses()) {
                    $("#" + campusDropListId).kendoDropDownList({
                        dataTextField: "officialName",
                        dataValueField: "id",
                        optionLabel: { officialName: "[All Institutions]", id: 0 },
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: App.Routes.WebApi.Establishments.getChildren(this.establishmentId()),
                                    data: { orderBy: ['rank-asc', 'name-asc'] }
                                }
                            }
                        }),
                        change: function (e) {
                            var selectedIndex = e.sender.selectedIndex;
                            if ((selectedIndex != null) && (selectedIndex != -1)) {
                                var item = this.dataItem(selectedIndex);
                                if ((item != null) && (item.id != 0)) {
                                    var dataSource = new kendo.data.DataSource({
                                        transport: {
                                            read: {
                                                url: App.Routes.WebApi.Establishments.getChildren(item.id),
                                                data: { orderBy: ['rank-asc', 'name-asc'] }
                                            }
                                        }
                                    });

                                    $("#" + collegeDropListId).data("kendoDropDownList").setDataSource(dataSource);
                                }

                                if (selectedIndex == 0) {
                                    $("#" + departmentDropListId).data("kendoDropDownList").setDataSource(new kendo.data.DataSource());
                                    $("#" + collegeDropListId).data("kendoDropDownList").setDataSource(new kendo.data.DataSource());

                                    $("#" + collegeDropListId).data("kendoDropDownList").text("");
                                }

                                me.drawPointmap(true);
                            }
                        },
                        dataBound: function (e) {
                            if ((this.selectedIndex != null) && (this.selectedIndex != -1)) {
                                var item = this.dataItem(this.selectedIndex);
                                if ((item != null) && (item.id != 0)) {
                                    var campusId = item.id;
                                    if (campusId != null) {
                                        var dataSource = new kendo.data.DataSource({
                                            transport: {
                                                read: {
                                                    url: App.Routes.WebApi.Establishments.getChildren(campusId),
                                                    data: { orderBy: ['rank-asc', 'name-asc'] }
                                                }
                                            }
                                        });

                                        $("#" + collegeDropListId).data("kendoDropDownList").setDataSource(dataSource);
                                    }
                                } else {
                                    $("#" + collegeDropListId).data("kendoDropDownList").setDataSource(new kendo.data.DataSource());
                                }
                            }
                        }
                    });
                }
                //$("#heatmapActivityDropList").kendoDropDownList({
                //    dataTextField: "type",
                //    dataValueField: "selected",
                //    dataSource: activities
                //});
            };

            FacultyAndStaff.prototype.setupValidation = function () {
                //ko.validation.rules['atLeast'] = {
                //    validator: (val: any, otherVal: any): boolean => {
                //        return val.length >= otherVal;
                //    },
                //    message: 'At least {0} must be selected.'
                //};
                //ko.validation.registerExtenders();
                //this.locations.extend({ atLeast: 1 });
                //this.institutions.extend({ required: true, maxLength: 200 });
                //this.from.extend({ required: true });
                //ko.validation.group(this);
            };

            FacultyAndStaff.prototype.setupSubscriptions = function () {
                var _this = this;
                //this.from.subscribe((newValue: any): void => { this.dirtyFlag(true); });
                //this.to.subscribe((newValue: any): void => { this.dirtyFlag(true); });
                //this.onGoing.subscribe((newValue: any): void => { this.dirtyFlag(true); });
                //this.institutions.subscribe((newValue: any): void => { this.dirtyFlag(true); });
                this.selectedPlace.subscribe(function (newValue) {
                    _this.selectMap('heatmap');
                });
                this.mapRegion.subscribe(function (newValue) {
                    _this.heatmapOptions["region"] = newValue;
                });

                this.searchType.subscribe(function (newValue) {
                    if (_this.mapType() === 'pointmap') {
                        //this.activitySearch();
                    }
                });

                for (var i = 0; i < this.activityTypes().length; i += 1) {
                    this.activityTypes()[i].checked.subscribe(function (newValue) {
                        if (_this.mapType() === 'pointmap') {
                            _this.drawPointmap(true);
                        }
                    });
                }

                this.degrees.subscribe(function (newValue) {
                    if (_this.mapType() === 'pointmap') {
                        _this.drawPointmap(true);
                    }
                });

                this.tags.subscribe(function (newValue) {
                    if (_this.mapType() === 'pointmap') {
                        _this.drawPointmap(true);
                    }
                });

                this.fromDate.subscribe(function (newValue) {
                    if (_this.mapType() === 'pointmap') {
                        _this.drawPointmap(true);
                    }
                });

                this.toDate.subscribe(function (newValue) {
                    if (_this.mapType() === 'pointmap') {
                        _this.drawPointmap(true);
                    }
                });

                this.includeUndated.subscribe(function (newValue) {
                    if (_this.mapType() === 'pointmap') {
                        _this.drawPointmap(true);
                    }
                });
            };

            FacultyAndStaff.prototype.setupRouting = function () {
                var _this = this;
                this.sammy.get('#/summary', function () {
                    _this.selectMap('heatmap');
                    $('#pageTitle').text("Professional Engagement Summary");
                });

                this.sammy.get('#/search', function () {
                    _this.selectMap('pointmap');
                    $('#pageTitle').text("Advanced Search");
                });

                this.sammy.get('#/expert', function () {
                    _this.selectMap('expert');
                    $('#pageTitle').text("Find an Expert");
                });

                this.sammy.run('#/summary');
            };

            FacultyAndStaff.prototype.setupMaps = function () {
                this.google.maps.visualRefresh = true;

                /* ----- Setup Pointmap ----- */
                this.pointmapOptions = {
                    mapTypeId: google.maps.MapTypeId.ROADMAP,
                    center: new google.maps.LatLng(0, 0),
                    zoom: 1,
                    draggable: true,
                    scrollwheel: false
                };

                /* ----- Setup Heatmap ----- */
                this.heatmapOptions = {
                    //is3D: true,
                    width: 680,
                    height: 500,
                    //magnifyingGlass: { enable: true, zoomFactor: 7.5 },
                    region: 'world',
                    //backgroundColor: 'lightBlue',
                    keepAspectRatio: false,
                    colorAxis: { colors: ['#FFFFFF', 'darkgreen'] },
                    backgroundColor: { fill: 'transparent' },
                    datalessRegionColor: 'FFFFFF'
                };

                if (this.activityTypes() != null) {
                    this.barchartActivityOptions = {
                        //title: 'Global Activities',
                        //titlePosition: 'out',
                        //titleTextStyle: {
                        //    color: 'black',
                        //    fontSize: 14,
                        //    bold: true
                        //},
                        hAxis: {
                            textPosition: 'none'
                        },
                        vAxis: {
                            textPosition: 'none'
                        },
                        chartArea: {
                            top: 16,
                            left: 10,
                            width: '100%',
                            height: '75%'
                        },
                        legend: { position: 'none' },
                        isStacked: true,
                        series: [
                            {
                                type: 'bars',
                                color: 'green'
                            },
                            {
                                type: 'line',
                                color: 'black',
                                lineWidth: 0,
                                pointSize: 0,
                                visibleInLegend: false
                            }
                        ]
                    };
                }

                this.barchartPeopleOptions = {
                    //title: 'Global People',
                    //titlePosition: 'out',
                    //titleTextStyle: {
                    //    color: 'black',
                    //    fontSize: 14,
                    //    bold: true
                    //},
                    hAxis: {
                        textPosition: 'none'
                    },
                    vAxis: {
                        textPosition: 'none'
                    },
                    chartArea: {
                        top: 16,
                        left: 10,
                        width: '100%',
                        height: '75%'
                    },
                    legend: { position: 'none' },
                    isStacked: true,
                    series: [
                        {
                            type: 'bars',
                            color: 'green'
                        },
                        {
                            type: 'line',
                            color: 'black',
                            lineWidth: 0,
                            pointSize: 0,
                            visibleInLegend: false
                        }
                    ]
                };

                this.barchart = new this.google.visualization.ColumnChart($('#facultystaff-summary-barchart')[0]);

                /* ----- Setup LineChart ----- */
                this.linechartActivityOptions = {
                    chartArea: {
                        top: 8,
                        left: 40,
                        width: '85%',
                        height: '60%'
                    },
                    //title: 'Global Activity Trend',
                    //titlePosition: 'out',
                    //titleTextStyle: {
                    //    color: 'black',
                    //    fontSize: 14,
                    //    bold: true
                    //},
                    colors: ['green'],
                    legend: { position: 'none' },
                    vAxis: { minValue: 0 }
                };

                this.linechartPeopleOptions = {
                    chartArea: {
                        top: 8,
                        left: 40,
                        width: '85%',
                        height: '60%'
                    },
                    //title: 'Global People Trend',
                    //titlePosition: 'out',
                    //titleTextStyle: {
                    //    color: 'black',
                    //    fontSize: 14,
                    //    bold: true
                    //},
                    colors: ['green'],
                    legend: { position: 'none' },
                    vAxis: { minValue: 0 }
                };

                this.linechart = new this.google.visualization.LineChart($('#facultystaff-summary-linechart')[0]);
            };

            FacultyAndStaff.prototype.getHeatmapActivityDataTable = function () {
                var _this = this;
                var deferred = $.Deferred();

                if (this.globalActivityCountData == null) {
                    //this.loadSpinner.start();
                    this.getActivityDataTable(null).done(function () {
                        deferred.resolve(_this._getHeatmapActivityDataTable());
                        //this.loadSpinner.stop();
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        deferred.reject(jqXHR, textStatus, errorThrown);
                    });
                } else {
                    deferred.resolve(this._getHeatmapActivityDataTable());
                }

                return deferred;
            };

            FacultyAndStaff.prototype._getHeatmapActivityDataTable = function () {
                if (this.heatmapActivityDataTable == null) {
                    var dataTable = new this.google.visualization.DataTable();

                    var colNames = new Array();
                    dataTable.addColumn('string', 'Country');
                    dataTable.addColumn('number', 'Total Activities');

                    var placeCounts = (this.globalActivityCountData).placeCounts;
                    if ((placeCounts != null) && (placeCounts.length > 0)) {
                        for (var i = 0; i < this.geochartCustomPlaces.length; i += 1) {
                            this.geochartCustomPlaces[i].activityCount = 0;
                        }

                        for (var i = 0; i < placeCounts.length; i += 1) {
                            var rowData = new Array();
                            rowData.push(placeCounts[i].officialName);
                            rowData.push(placeCounts[i].count);
                            dataTable.addRow(rowData);

                            var officialName = placeCounts[i].officialName;
                            var count = placeCounts[i].count;
                            var j = -1;

                            if ((officialName === "North Atlantic Ocean") || (officialName === "South Atlantic Ocean")) {
                                j = this.getCustomPlaceIndexByName("Atlantic Ocean");
                            } else if ((officialName === "North Pacific Ocean") || (officialName === "Pacific Ocean") || (officialName === "South Pacific Ocean")) {
                                j = this.getCustomPlaceIndexByName("Pacific Ocean");
                            } else if (officialName === "Arctic Ocean") {
                                j = this.getCustomPlaceIndexByName("Arctic Ocean");
                            } else if (officialName === "Gulf of Mexico") {
                                j = this.getCustomPlaceIndexByName("Gulf of Mexico");
                            } else if (officialName === "Caribbean Ocean") {
                                j = this.getCustomPlaceIndexByName("Caribbean");
                            } else if (officialName === "Indian Ocean") {
                                j = this.getCustomPlaceIndexByName("Indian Ocean");
                            } else if (officialName === "Southern Ocean") {
                                j = this.getCustomPlaceIndexByName("Southern Ocean");
                            } else if (officialName === "Antarctica") {
                                j = this.getCustomPlaceIndexByName("Antarctica");
                            }

                            if (j >= 0) {
                                this.geochartCustomPlaces[j].activityCount += count;
                            }
                        }
                    }

                    this.heatmapActivityDataTable = dataTable;
                }

                return this.heatmapActivityDataTable;
            };

            FacultyAndStaff.prototype.getHeatmapPeopleDataTable = function () {
                var _this = this;
                var deferred = $.Deferred();

                if (this.globalPeopleCountData == null) {
                    //this.loadSpinner.start();
                    this.getPeopleDataTable(null).done(function () {
                        deferred.resolve(_this._getHeatmapPeopleDataTable());
                        //this.loadSpinner.stop();
                    });
                } else {
                    deferred.resolve(this._getHeatmapPeopleDataTable());
                }

                return deferred;
            };

            FacultyAndStaff.prototype._getHeatmapPeopleDataTable = function () {
                if (this.heatmapPeopleDataTable == null) {
                    var dataTable = new this.google.visualization.DataTable();

                    var colNames = new Array();
                    dataTable.addColumn('string', 'Country');
                    dataTable.addColumn('number', 'Total People');

                    var placeCounts = (this.globalPeopleCountData).placeCounts;
                    if ((placeCounts != null) && (placeCounts.length > 0)) {
                        for (var i = 0; i < this.geochartCustomPlaces.length; i += 1) {
                            this.geochartCustomPlaces[i].peopleCount = 0;
                        }

                        for (var i = 0; i < placeCounts.length; i += 1) {
                            var rowData = new Array();
                            rowData.push(placeCounts[i].officialName);
                            rowData.push(placeCounts[i].count);
                            dataTable.addRow(rowData);

                            var officialName = placeCounts[i].officialName;
                            var count = placeCounts[i].count;
                            var j = -1;

                            if ((officialName === "North Atlantic Ocean") || (officialName === "South Atlantic Ocean")) {
                                j = this.getCustomPlaceIndexByName("Atlantic Ocean");
                            } else if ((officialName === "North Pacific Ocean") || (officialName === "Pacific Ocean") || (officialName === "South Pacific Ocean")) {
                                j = this.getCustomPlaceIndexByName("Pacific Ocean");
                            } else if (officialName === "Arctic Ocean") {
                                j = this.getCustomPlaceIndexByName("Arctic Ocean");
                            } else if (officialName === "Gulf of Mexico") {
                                j = this.getCustomPlaceIndexByName("Gulf of Mexico");
                            } else if (officialName === "Caribbean Ocean") {
                                j = this.getCustomPlaceIndexByName("Caribbean");
                            } else if (officialName === "Indian Ocean") {
                                j = this.getCustomPlaceIndexByName("Indian Ocean");
                            } else if (officialName === "Southern Ocean") {
                                j = this.getCustomPlaceIndexByName("Southern Ocean");
                            } else if (officialName === "Antarctica") {
                                j = this.getCustomPlaceIndexByName("Antarctica");
                            }

                            if (j >= 0) {
                                this.geochartCustomPlaces[j].peopleCount += count;
                            }
                        }
                    }

                    this.heatmapPeopleDataTable = dataTable;
                }

                return this.heatmapPeopleDataTable;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getGlobalActivityCounts = function () {
                var _this = this;
                var deferred = $.Deferred();

                if (this.globalActivityCountData == null) {
                    $.ajax({
                        type: "GET",
                        async: true,
                        data: { 'establishmentId': this.establishmentId(), 'placeId': null },
                        dataType: 'json',
                        url: App.Routes.WebApi.FacultyStaff.getActivityCount(),
                        success: function (data, textStatus, jqXhr) {
                            _this.globalActivityCountData = data;
                            deferred.resolve(_this.globalActivityCountData);
                        },
                        error: function (jqXhr, textStatus, errorThrown) {
                            deferred.reject(errorThrown);
                        },
                        complete: function (jqXhr, textStatus) {
                        }
                    });
                } else {
                    deferred.resolve(this.globalActivityCountData);
                }

                return deferred;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getActivityDataTable = function (placeOfficialName) {
                var _this = this;
                var deferred = $.Deferred();

                if (placeOfficialName == null) {
                    this.getGlobalActivityCounts().done(function (counts) {
                        deferred.resolve(_this._getActivityDataTable(null));
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        deferred.reject(jqXHR, textStatus, errorThrown);
                    });
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        if ((this.placeActivityCountData == null) || ((this.placeActivityCountData).placeId != placeId)) {
                            //this.loadSpinner.start();
                            $.ajax({
                                type: "GET",
                                async: true,
                                data: { 'establishmentId': this.establishmentId(), 'placeId': placeId },
                                dataType: 'json',
                                url: App.Routes.WebApi.FacultyStaff.getActivityCount(),
                                success: function (data, textStatus, jqXhr) {
                                    _this.placeActivityCountData = data;
                                    deferred.resolve(_this._getActivityDataTable(placeOfficialName));
                                },
                                error: function (jqXhr, textStatus, errorThrown) {
                                    deferred.reject(errorThrown);
                                },
                                complete: function (jqXhr, textStatus) {
                                    // this.loadSpinner.stop();
                                }
                            });
                        } else {
                            deferred.resolve(this._getActivityDataTable(placeOfficialName));
                        }
                    } else {
                        deferred.reject("Unknown PlaceId");
                    }
                }

                return deferred;
            };

            FacultyAndStaff.prototype._getActivityDataTable = function (placeOfficialName) {
                var view = null;
                var dt = null;

                if (placeOfficialName == null) {
                    dt = new this.google.visualization.DataTable();

                    dt.addColumn('string', 'Activity');
                    dt.addColumn('number', 'Count');
                    dt.addColumn({ type: 'number', role: 'annotation' });

                    for (var i = 0; i < (this.globalActivityCountData).typeCounts.length; i += 1) {
                        var activityType = (this.globalActivityCountData).typeCounts[i].type;
                        var count = (this.globalActivityCountData).typeCounts[i].count;
                        dt.addRow([activityType, count, count]);
                    }
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        dt = new this.google.visualization.DataTable();

                        dt.addColumn('string', 'Activity');
                        dt.addColumn('number', 'Count');
                        dt.addColumn({ type: 'number', role: 'annotation' });

                        for (var i = 0; i < (this.placeActivityCountData).typeCounts.length; i += 1) {
                            var activityType = (this.placeActivityCountData).typeCounts[i].type;
                            var count = (this.placeActivityCountData).typeCounts[i].count;
                            dt.addRow([activityType, count, count]);
                        }
                    }
                }

                view = new this.google.visualization.DataView(dt);
                view.setColumns([0, 1, 1, 2]);

                return view;
            };

            FacultyAndStaff.prototype.getGlobalPeopleCounts = function () {
                var _this = this;
                var deferred = $.Deferred();
                if (this.globalPeopleCountData == null) {
                    $.ajax({
                        type: "GET",
                        async: true,
                        data: { 'establishmentId': this.establishmentId(), 'placeId': null },
                        dataType: 'json',
                        url: App.Routes.WebApi.FacultyStaff.getPeopleCount(),
                        success: function (data, textStatus, jqXhr) {
                            _this.globalPeopleCountData = data;
                            deferred.resolve(_this._getPeopleDataTable(null));
                        },
                        error: function (jqXhr, textStatus, errorThrown) {
                            deferred.reject(errorThrown);
                        },
                        complete: function (jqXhr, textStatus) {
                        }
                    });
                } else {
                    deferred.resolve(this._getPeopleDataTable(null));
                }
                return deferred;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getPeopleDataTable = function (placeOfficialName) {
                var _this = this;
                var deferred = $.Deferred();

                if (placeOfficialName == null) {
                    this.getGlobalPeopleCounts().done(function (counts) {
                        deferred.resolve(_this._getPeopleDataTable(null));
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        deferred.reject(jqXHR, textStatus, errorThrown);
                    });
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        if ((this.placePeopleCountData == null) || ((this.placePeopleCountData).placeId != placeId)) {
                            //this.loadSpinner.start();
                            $.ajax({
                                type: "GET",
                                async: true,
                                data: { 'establishmentId': this.establishmentId(), 'placeId': placeId },
                                dataType: 'json',
                                url: App.Routes.WebApi.FacultyStaff.getPeopleCount(),
                                success: function (data, textStatus, jqXhr) {
                                    _this.placePeopleCountData = data;
                                    deferred.resolve(_this._getPeopleDataTable(placeOfficialName));
                                },
                                error: function (jqXhr, textStatus, errorThrown) {
                                    deferred.reject(errorThrown);
                                },
                                complete: function (jqXhr, textStatus) {
                                    //this.loadSpinner.stop();
                                }
                            });
                        } else {
                            deferred.resolve(this._getPeopleDataTable(placeOfficialName));
                        }
                    } else {
                        deferred.reject("Unknown placeId.");
                    }
                }

                return deferred;
            };

            FacultyAndStaff.prototype._getPeopleDataTable = function (placeOfficialName) {
                var view = null;
                var dt = new this.google.visualization.DataTable();

                dt.addColumn('string', 'Activity');
                dt.addColumn('number', 'Count');
                dt.addColumn({ type: 'number', role: 'annotation' });

                if (placeOfficialName == null) {
                    for (var i = 0; i < (this.globalPeopleCountData).typeCounts.length; i += 1) {
                        var activityType = (this.globalPeopleCountData).typeCounts[i].type;
                        var count = (this.globalPeopleCountData).typeCounts[i].count;
                        dt.addRow([activityType, count, count]);
                    }
                } else {
                    for (var i = 0; i < (this.placePeopleCountData).typeCounts.length; i += 1) {
                        var activityType = (this.placePeopleCountData).typeCounts[i].type;
                        var count = (this.placePeopleCountData).typeCounts[i].count;
                        dt.addRow([activityType, count, count]);
                    }
                }

                view = new this.google.visualization.DataView(dt);
                view.setColumns([0, 1, 1, 2]);

                return view;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getActivityTrendDataTable = function (placeOfficialName) {
                var _this = this;
                var deferred = $.Deferred();

                if (placeOfficialName == null) {
                    if (this.globalActivityTrendData == null) {
                        //this.loadSpinner.start();
                        $.ajax({
                            type: "GET",
                            async: true,
                            data: { 'establishmentId': this.establishmentId(), 'placeId': null },
                            dataType: 'json',
                            url: App.Routes.WebApi.FacultyStaff.getActivityTrend(),
                            success: function (data, textStatus, jqXhr) {
                                _this.globalActivityTrendData = data;
                                deferred.resolve(_this._getActivityTrendDataTable(null));
                            },
                            error: function (jqXhr, textStatus, errorThrown) {
                                deferred.reject(errorThrown);
                            },
                            complete: function (jqXhr, textStatus) {
                                //this.loadSpinner.stop();
                            }
                        });
                    } else {
                        deferred.resolve(this._getActivityTrendDataTable(null));
                    }
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        if ((this.placeActivityTrendData == null) || ((this.placeActivityTrendData).placeId != placeId)) {
                            //this.loadSpinner.start();
                            $.ajax({
                                type: "GET",
                                async: true,
                                data: { 'establishmentId': this.establishmentId(), 'placeId': placeId },
                                dataType: 'json',
                                url: App.Routes.WebApi.FacultyStaff.getActivityTrend(),
                                success: function (data, textStatus, jqXhr) {
                                    _this.placeActivityTrendData = data;
                                    deferred.resolve(_this._getActivityTrendDataTable(placeOfficialName));
                                },
                                error: function (jqXhr, textStatus, errorThrown) {
                                    deferred.reject(errorThrown);
                                },
                                complete: function (jqXhr, textStatus) {
                                    //this.loadSpinner.stop();
                                }
                            });
                        } else {
                            deferred.resolve(this._getActivityTrendDataTable(placeOfficialName));
                        }
                    } else {
                        deferred.reject();
                    }
                }

                return deferred;
            };

            FacultyAndStaff.prototype._getActivityTrendDataTable = function (placeOfficialName) {
                var dt = new this.google.visualization.DataTable();

                dt.addColumn('string', 'Year');
                dt.addColumn('number', 'Count');

                if (placeOfficialName == null) {
                    for (var i = 0; i < (this.globalActivityTrendData).trendCounts.length; i += 1) {
                        var year = (this.globalActivityTrendData).trendCounts[i].year.toString();
                        var count = (this.globalActivityTrendData).trendCounts[i].count;
                        dt.addRow([year, count]);
                    }
                } else {
                    for (var i = 0; i < (this.placeActivityTrendData).trendCounts.length; i += 1) {
                        var year = (this.placeActivityTrendData).trendCounts[i].year.toString();
                        var count = (this.placeActivityTrendData).trendCounts[i].count;
                        dt.addRow([year, count]);
                    }
                }

                return dt;
            };

            FacultyAndStaff.prototype.getPeopleTrendDataTable = function (placeOfficialName) {
                var _this = this;
                var deferred = $.Deferred();

                if (placeOfficialName == null) {
                    if (this.globalPeopleTrendData == null) {
                        //this.loadSpinner.start();
                        $.ajax({
                            type: "GET",
                            async: true,
                            data: { 'establishmentId': this.establishmentId(), 'placeId': null },
                            dataType: 'json',
                            url: App.Routes.WebApi.FacultyStaff.getPeopleTrend(),
                            success: function (data, textStatus, jqXhr) {
                                _this.globalPeopleTrendData = data;
                                deferred.resolve(_this._getPeopleTrendDataTable(null));
                            },
                            error: function (jqXhr, textStatus, errorThrown) {
                                deferred.reject(errorThrown);
                            },
                            complete: function (jqXhr, textStatus) {
                                //this.loadSpinner.stop();
                            }
                        });
                    } else {
                        deferred.resolve(this._getPeopleTrendDataTable(null));
                    }
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        if ((this.placePeopleTrendData == null) || ((this.placePeopleTrendData).placeId != placeId)) {
                            //this.loadSpinner.start();
                            $.ajax({
                                type: "GET",
                                async: true,
                                data: { 'establishmentId': this.establishmentId(), 'placeId': placeId },
                                dataType: 'json',
                                url: App.Routes.WebApi.FacultyStaff.getPeopleTrend(),
                                success: function (data, textStatus, jqXhr) {
                                    _this.placePeopleTrendData = data;
                                    deferred.resolve(_this._getPeopleTrendDataTable(placeOfficialName));
                                },
                                error: function (jqXhr, textStatus, errorThrown) {
                                    deferred.reject(errorThrown);
                                },
                                complete: function (jqXhr, textStatus) {
                                    //this.loadSpinner.stop();
                                }
                            });
                        } else {
                            deferred.resolve(this._getPeopleTrendDataTable(placeOfficialName));
                        }
                    } else {
                        deferred.reject("Unkown placeId");
                    }
                }

                return deferred;
            };

            FacultyAndStaff.prototype._getPeopleTrendDataTable = function (placeOfficialName) {
                var dt = new this.google.visualization.DataTable();

                dt.addColumn('string', 'Year');
                dt.addColumn('number', 'Count');

                if (placeOfficialName == null) {
                    for (var i = 0; i < (this.globalPeopleTrendData).trendCounts.length; i += 1) {
                        var year = (this.globalPeopleTrendData).trendCounts[i].year.toString();
                        var count = (this.globalPeopleTrendData).trendCounts[i].count;
                        dt.addRow([year, count]);
                    }
                } else {
                    for (var i = 0; i < (this.placePeopleTrendData).trendCounts.length; i += 1) {
                        var year = (this.placePeopleTrendData).trendCounts[i].year.toString();
                        var count = (this.placePeopleTrendData).trendCounts[i].count;
                        dt.addRow([year, count]);
                    }
                }

                return dt;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getDegreeCount = function (placeOfficialName) {
                var deferred = $.Deferred();

                if (placeOfficialName == null) {
                    //this.loadSpinner.start();
                    $.ajax({
                        type: "GET",
                        async: true,
                        data: { 'establishmentId': this.establishmentId(), 'placeId': null },
                        dataType: 'json',
                        url: App.Routes.WebApi.FacultyStaff.getDegreeCount(),
                        success: function (data, textStatus, jqXhr) {
                            deferred.resolve(data.count);
                        },
                        error: function (jqXhr, textStatus, errorThrown) {
                            deferred.reject(errorThrown);
                        },
                        complete: function (jqXhr, textStatus) {
                            //this.loadSpinner.stop();
                        }
                    });
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        // this.loadSpinner.start();
                        $.ajax({
                            type: "GET",
                            async: true,
                            data: { 'establishmentId': this.establishmentId(), 'placeId': placeId },
                            dataType: 'json',
                            url: App.Routes.WebApi.FacultyStaff.getDegreeCount(),
                            success: function (data, textStatus, jqXhr) {
                                deferred.resolve(data.count);
                            },
                            error: function (jqXhr, textStatus, errorThrown) {
                                deferred.reject(errorThrown);
                            },
                            complete: function (jqXhr, textStatus) {
                                //this.loadSpinner.stop();
                            }
                        });
                    } else {
                        deferred.reject("Unknown PlaceId");
                    }
                }

                return deferred;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getDegreePeopleCount = function (placeOfficialName) {
                var deferred = $.Deferred();

                if (placeOfficialName == null) {
                    //this.loadSpinner.start();
                    $.ajax({
                        type: "GET",
                        async: true,
                        data: { 'establishmentId': this.establishmentId(), 'placeId': null },
                        dataType: 'json',
                        url: App.Routes.WebApi.FacultyStaff.getDegreePeopleCount(),
                        success: function (data, textStatus, jqXhr) {
                            deferred.resolve(data.count);
                        },
                        error: function (jqXhr, textStatus, errorThrown) {
                            deferred.reject(errorThrown);
                        },
                        complete: function (jqXhr, textStatus) {
                            //this.loadSpinner.stop();
                        }
                    });
                } else {
                    var placeId = this.getPlaceId(placeOfficialName);
                    if (placeId != null) {
                        //this.loadSpinner.start();
                        $.ajax({
                            type: "GET",
                            async: true,
                            data: { 'establishmentId': this.establishmentId(), 'placeId': placeId },
                            dataType: 'json',
                            url: App.Routes.WebApi.FacultyStaff.getDegreePeopleCount(),
                            success: function (data, textStatus, jqXhr) {
                                deferred.resolve(data.count);
                            },
                            error: function (jqXhr, textStatus, errorThrown) {
                                deferred.reject(errorThrown);
                            },
                            complete: function (jqXhr, textStatus) {
                                //this.loadSpinner.stop();
                            }
                        });
                    } else {
                        deferred.reject("Unknown PlaceId");
                    }
                }

                return deferred;
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getPointmapActivityMarkers = function (refresh) {
                var _this = this;
                var deferred = $.Deferred();
                if (refresh) {
                    this.hidePointmapActivityMarkers();
                    this.pointmapActivityMarkers = null;
                }
                if (this.pointmapActivityMarkers == null) {
                    this.activitySearch().done(function (results) {
                        deferred.resolve(_this._getPointmapActivityMarkers(results));
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        deferred.reject(jqXHR, textStatus, errorThrown);
                    });
                } else {
                    deferred.resolve(this.pointmapActivityMarkers);
                }
                return deferred;
            };

            FacultyAndStaff.prototype._getPointmapActivityMarkers = function (activityResults) {
                var markers = new Array();
                var placeResults = activityResults.placeResults;
                if ((placeResults != null) && (placeResults.length > 0)) {
                    for (var i = 0; i < placeResults.length; i += 1) {
                        if (placeResults[i].results.length > 0) {
                            var iconURL = "/api/graphics/circle" + "?side=18&opacity=" + "&strokeColor=" + $("#mapMarkerColor").css("background-color") + "&fillColor=" + $("#mapMarkerColor").css("background-color") + "&textColor=" + $("#mapMarkerColor").css("color") + "&text=" + placeResults[i].results.length.toString();

                            //var marker = new MarkerWithLabel({
                            var marker = new google.maps.Marker({
                                position: new google.maps.LatLng(placeResults[i].lat, placeResults[i].lng),
                                map: null,
                                title: placeResults[i].officialName,
                                icon: iconURL
                            });

                            markers.push(marker);
                        }
                    }
                }
                this.pointmapActivityMarkers = markers;

                return this.pointmapActivityMarkers;
            };

            FacultyAndStaff.prototype.showPointmapActivityMarkers = function () {
                if (this.pointmapActivityMarkers != null) {
                    for (var i = 0; i < this.pointmapActivityMarkers.length; i += 1) {
                        this.pointmapActivityMarkers[i].setMap(this.pointmap);
                    }
                }
            };

            FacultyAndStaff.prototype.hidePointmapActivityMarkers = function () {
                if (this.pointmapActivityMarkers != null) {
                    for (var i = 0; i < this.pointmapActivityMarkers.length; i += 1) {
                        this.pointmapActivityMarkers[i].setMap(null);
                    }
                }
            };

            /*
            *
            */
            FacultyAndStaff.prototype.getPointmapPeopleMarkers = function (refresh) {
                var _this = this;
                var deferred = $.Deferred();
                if (refresh) {
                    this.hidePointmapPeopleMarkers();
                    this.pointmapPeopleMarkers = null;
                }
                if (this.pointmapPeopleMarkers == null) {
                    this.getGlobalPeopleCounts().done(function (counts) {
                        deferred.resolve(_this._getPointmapPeopleMarkers());
                    }).fail(function (jqXHR, textStatus, errorThrown) {
                        deferred.reject(jqXHR, textStatus, errorThrown);
                    });
                } else {
                    deferred.resolve(this._getPointmapPeopleMarkers());
                }
                return deferred;
            };

            FacultyAndStaff.prototype._getPointmapPeopleMarkers = function () {
                if (this.pointmapPeopleMarkers == null) {
                    var markers = new Array();
                    var placeCounts = (this.globalPeopleCountData).placeCounts;
                    if ((placeCounts != null) && (placeCounts.length > 0)) {
                        for (var i = 0; i < placeCounts.length; i += 1) {
                            if (placeCounts[i].count > 0) {
                                var iconURL = "/api/graphics/circle" + "?side=18&opacity=" + "&strokeColor=" + $("#mapMarkerColor").css("background-color") + "&fillColor=" + $("#mapMarkerColor").css("background-color") + "&textColor=" + $("#mapMarkerColor").css("color") + "&text=" + placeCounts[i].results.length.toString();

                                //var marker = new MarkerWithLabel({
                                var marker = new google.maps.Marker({
                                    position: new google.maps.LatLng(placeCounts[i].lat, placeCounts[i].lng),
                                    map: null,
                                    title: placeCounts[i].officialName,
                                    icon: iconURL
                                });
                                markers.push(marker);
                            }
                        }
                    }
                    this.pointmapPeopleMarkers = markers;
                }
                return this.pointmapPeopleMarkers;
            };

            FacultyAndStaff.prototype.showPointmapPeopleMarkers = function () {
                if (this.pointmapPeopleMarkers != null) {
                    for (var i = 0; i < this.pointmapPeopleMarkers.length; i += 1) {
                        this.pointmapPeopleMarkers[i].setMap(this.pointmap);
                    }
                }
            };

            FacultyAndStaff.prototype.hidePointmapPeopleMarkers = function () {
                if (this.pointmapPeopleMarkers != null) {
                    for (var i = 0; i < this.pointmapPeopleMarkers.length; i += 1) {
                        this.pointmapPeopleMarkers[i].setMap(null);
                    }
                }
            };

            FacultyAndStaff.prototype.makeActivityTooltip = function (name, count) {
                return "<b>" + name + "</b><br/>Total Activities: " + count.toString();
            };

            FacultyAndStaff.prototype.makePeopleTooltip = function (name, count) {
                return "<b>" + name + "</b><br/>Total People: " + count.toString();
            };

            FacultyAndStaff.prototype.updateCustomGeochartPlaceTooltips = function (selector) {
                var id = "";
                var name = "";
                var count = 0;

                for (var i = 0; i < this.geochartCustomPlaces.length; i += 1) {
                    if (selector === 'activities') {
                        id = this.geochartCustomPlaces[i].id;
                        name = this.geochartCustomPlaces[i].name;
                        count = this.geochartCustomPlaces[i].activityCount;
                        $("#" + id).tooltip({
                            //position: {
                            //    my: "bottom+10 right+10"
                            //},
                            track: true,
                            tooltipClass: "geochartTooltip",
                            items: "#" + id,
                            content: this.makeActivityTooltip(name, count)
                        });
                    } else {
                        id = this.geochartCustomPlaces[i].id;
                        name = this.geochartCustomPlaces[i].name;
                        count = this.geochartCustomPlaces[i].activityCount;
                        $("#" + id).tooltip({
                            //position: {
                            //    my: "bottom+10 right+10"
                            //},
                            track: true,
                            tooltipClass: "geochartTooltip",
                            items: "#" + id,
                            content: this.makePeopleTooltip(name, count)
                        });
                    }
                }
            };

            FacultyAndStaff.prototype.load = function () {
                var _this = this;
                var me = this;
                var deferred = $.Deferred();

                this.loadSpinner.start();

                var typesPact = $.Deferred();
                $.get(App.Routes.WebApi.Employees.ModuleSettings.ActivityTypes.get()).done(function (data, textStatus, jqXHR) {
                    typesPact.resolve(data);
                }).fail(function (jqXHR, textStatus, errorThrown) {
                    typesPact.reject(jqXHR, textStatus, errorThrown);
                });

                var placesPact = $.Deferred();
                $.ajax({
                    type: "GET",
                    data: { isCountry: true },
                    dataType: 'json',
                    url: App.Routes.WebApi.Places.get(),
                    success: function (data, textStatus, jqXhr) {
                        placesPact.resolve(data);
                    },
                    error: function (jqXhr, textStatus, errorThrown) {
                        placesPact.reject(jqXhr, textStatus, errorThrown);
                    }
                });

                var watersPact = $.Deferred();
                $.ajax({
                    type: "GET",
                    data: { isWater: true },
                    dataType: 'json',
                    url: App.Routes.WebApi.Places.get(),
                    success: function (data, textStatus, jqXhr) {
                        watersPact.resolve(data);
                    },
                    error: function (jqXhr, textStatus, errorThrown) {
                        watersPact.reject(jqXhr, textStatus, errorThrown);
                    }
                });

                // only process after all requests have been resolved
                $.when(typesPact, placesPact, watersPact).done(function (types, places, waters) {
                    _this.activityTypes = ko.mapping.fromJS(types);

                    for (var i = 0; i < _this.activityTypes().length; i += 1) {
                        //this.activityTypes()[i].checked = ko.computed(this.defHasActivityTypeCallback(i));
                        //this.activityTypes()[i].checked(true);
                        _this.activityTypes()[i].checked = ko.observable(true);
                    }

                    //ko.mapping.fromJS(data, {}, this);
                    ///* Initialize the list of selected locations with current locations in values. */
                    //for (var i = 0; i < this.locations().length; i += 1) {
                    //    this.initialLocations.push({
                    //        officialName: this.locations()[i].placeOfficialName(),
                    //        id: this.locations()[i].placeId()
                    //    });
                    //    this.selectedLocationValues.push(this.locations()[i].placeId());
                    //}
                    _this.places = places.concat(waters);

                    deferred.resolve();
                }).fail(function (xhr, textStatus, errorThrown) {
                    deferred.reject(xhr, textStatus, errorThrown);
                }).always(function () {
                    //this.loadSpinner.stop();
                });

                return deferred;
            };

            FacultyAndStaff.prototype.checkInstitutionForNull = function () {
                var me = $("#" + this.establishmentDropListId).data("kendoAutoComplete");
                var value = (me.value() != null) ? me.value().toString() : null;
                if (value != null) {
                    value = $.trim(value);
                }
                if ((value == null) || (value.length == 0)) {
                    me.value(null);
                    this.establishmentOfficialName(null);
                    this.establishmentId(null);
                }
            };

            FacultyAndStaff.prototype.updateLocations = function (items) {
                if (this.locations != null) {
                    this.locations.removeAll();
                    for (var i = 0; i < items.length; i += 1) {
                        var location = ko.mapping.fromJS({ id: 0, placeId: items[i].id, version: "" });
                        this.locations.push(location);
                    }
                    this.drawPointmap(true);
                }
            };

            // --------------------------------------------------------------------------------
            /*
            *
            */
            // --------------------------------------------------------------------------------
            FacultyAndStaff.prototype.selectMap = function (type) {
                var _this = this;
                this.mapType(type);

                $('#heatmapText').css("font-weight", "normal");
                this.isHeatmapVisible(false);

                $('#pointmapText').css("font-weight", "normal");
                this.isPointmapVisible(false);

                $('#expertText').css("font-weight", "normal");
                this.isExpertVisible(false);

                //$('#resultstableText').css("font-weight", "normal");
                //this.isTableVisible(false);
                $("#bib-faculty-staff-summary").removeClass("current");
                $("#bib-faculty-staff-search").removeClass("current");
                $("#bib-faculty-staff-expert").removeClass("current");

                if (type === "heatmap") {
                    $('#heatmapText').css("font-weight", "bold");
                    this.isHeatmapVisible(true);

                    if (this.searchType() === 'activities') {
                        $('#activitiesButton').css("font-weight", "bold");
                        $('#peopleButton').css("font-weight", "normal");
                    } else {
                        $('#activitiesButton').css("font-weight", "normal");
                        $('#peopleButton').css("font-weight", "bold");
                    }

                    if (this.heatmap == null) {
                        this.heatmap = new this.google.visualization.GeoChart($('#heatmap')[0]);
                        this.google.visualization.events.addListener(this.heatmap, 'select', function () {
                            _this.heatmapSelectHandler();
                        });
                    }

                    this.loadSpinner.start();

                    if (this.searchType() === 'activities') {
                        this.getActivityDataTable(this.selectedPlace()).done(function (dataTable) {
                            _this.barchart.draw(dataTable, _this.barchartActivityOptions);
                            if (_this.selectedPlace() != null) {
                                _this.totalCount(_this.placeActivityCountData.count);
                                _this.totalPlaceCount(_this.placeActivityCountData.countOfPlaces);
                            }

                            _this.getHeatmapActivityDataTable().done(function (dataTable) {
                                _this.heatmap.draw(dataTable, _this.heatmapOptions);
                                if (_this.selectedPlace() == null) {
                                    _this.totalCount(_this.globalActivityCountData.count);
                                    _this.totalPlaceCount(_this.globalActivityCountData.countOfPlaces);
                                }
                                _this.updateCustomGeochartPlaceTooltips(_this.searchType());
                            });

                            _this.loadSpinner.stop();
                        });

                        this.getActivityTrendDataTable(this.selectedPlace()).done(function (dataTable) {
                            _this.linechart.draw(dataTable, _this.linechartActivityOptions);
                        });

                        this.getDegreeCount(this.selectedPlace()).done(function (count) {
                            _this.degreeCount(count);
                        });
                    } else {
                        this.getPeopleDataTable(this.selectedPlace()).done(function (dataTable) {
                            _this.barchart.draw(dataTable, _this.barchartPeopleOptions);
                            if (_this.selectedPlace() != null) {
                                _this.totalCount(_this.placePeopleCountData.count);
                                _this.totalPlaceCount(_this.placePeopleCountData.countOfPlaces);
                            }

                            _this.getHeatmapPeopleDataTable().done(function (dataTable) {
                                _this.heatmap.draw(dataTable, _this.heatmapOptions);
                                if (_this.selectedPlace() == null) {
                                    _this.totalCount(_this.globalPeopleCountData.count);
                                    _this.totalPlaceCount(_this.globalPeopleCountData.countOfPlaces);
                                }
                                _this.updateCustomGeochartPlaceTooltips(_this.searchType());
                            });

                            _this.loadSpinner.stop();
                        });

                        this.getPeopleTrendDataTable(this.selectedPlace()).done(function (dataTable) {
                            _this.linechart.draw(dataTable, _this.linechartPeopleOptions);
                        });

                        this.getDegreePeopleCount(this.selectedPlace()).done(function (count) {
                            _this.degreeCount(count);
                        });
                    }

                    $("#bib-faculty-staff-summary").addClass("current");
                } else if (type === "pointmap") {
                    $('#pointmapText').css("font-weight", "bold");
                    this.isPointmapVisible(true);
                    $('#pointmap').css("display", "inline-block");

                    if (this.pointmap == null) {
                        var pointmapElement = $('#pointmap')[0];
                        this.pointmap = new this.google.maps.Map(pointmapElement, this.pointmapOptions);
                    }

                    this.drawPointmap(false);

                    $("#bib-faculty-staff-search").addClass("current");
                    //} else if (type === "resultstable") {
                    //    $('#resultstableText').css("font-weight", "bold");
                    //    this.isTableVisible(true);
                    //    $("#bib-faculty-staff-search").addClass("current");
                } else if (type === "expert") {
                    $('#expertText').css("font-weight", "bold");
                    this.isExpertVisible(true);
                    $('#expert').css("display", "inline-block");
                    $("#bib-faculty-staff-expert").addClass("current");
                }
            };

            FacultyAndStaff.prototype.selectSearchType = function (type) {
                if (type === 'activities') {
                    this.setActivitiesSearch();
                } else {
                    this.setPeopleSearch();
                }
                if (this.heatmap != null) {
                    this.selectMap("heatmap");
                }
            };

            FacultyAndStaff.prototype.setActivitiesSearch = function () {
                $('#activitiesButton').css("font-weight", "bold");
                $('#peopleButton').css("font-weight", "normal");
                this.searchType('activities');
            };

            FacultyAndStaff.prototype.setPeopleSearch = function () {
                $('#activitiesButton').css("font-weight", "normal");
                $('#peopleButton').css("font-weight", "bold");
                this.searchType('people');
            };

            FacultyAndStaff.prototype.heatmapSelectHandler = function () {
                var selection = this.heatmap.getSelection();

                if ((selection != null) && (selection.length > 0)) {
                    var officialName = '';
                    var countryCode = null;

                    if (this.searchType() === 'activities') {
                        officialName = this.heatmapActivityDataTable.getFormattedValue(selection[0].row, 0);

                        var i = 0;
                        while ((i < this.globalActivityCountData.placeCounts.length) && (countryCode == null)) {
                            if (this.globalActivityCountData.placeCounts[i].officialName === officialName) {
                                countryCode = this.globalActivityCountData.placeCounts[i].countryCode;
                            }

                            i += 1;
                        }
                    } else {
                        officialName = this.heatmapPeopleDataTable.getFormattedValue(selection[0].row, 0);

                        var i = 0;
                        while ((i < this.globalPeopleCountData.placeCounts.length) && (countryCode == null)) {
                            if (this.globalPeopleCountData.placeCounts[i].officialName === officialName) {
                                countryCode = this.globalPeopleCountData.placeCounts[i].countryCode;
                            }

                            i += 1;
                        }
                    }

                    this.selectedPlace(officialName);
                    this.mapRegion((countryCode != null) ? countryCode : 'world');
                }
            };

            FacultyAndStaff.prototype.expertClickHandler = function (item, event) {
                this.selectMap('expert');
            };

            FacultyAndStaff.prototype.globalViewClickHandler = function (item, event) {
                this.selectedPlace(null);
                this.mapRegion('world');
                this.selectMap('heatmap');
            };

            FacultyAndStaff.prototype.getPlaceId = function (officialName) {
                var i = 0;
                while ((i < this.places.length) && (officialName !== this.places[i].officialName)) {
                    i += 1;
                }
                return (i < this.places.length) ? this.places[i].id : null;
            };

            FacultyAndStaff.prototype.getCustomPlaceIndexByName = function (officialName) {
                var i = 0;
                while ((i < this.geochartCustomPlaces.length) && (officialName !== this.geochartCustomPlaces[i].name)) {
                    i += 1;
                }
                return (i < this.geochartCustomPlaces.length) ? i : -1;
            };

            FacultyAndStaff.prototype.clearCachedData = function () {
                this.globalActivityCountData = null;
                this.placeActivityCountData = null;
                this.globalPeopleCountData = null;
                this.placePeopleCountData = null;
                this.globalActivityTrendData = null;
                this.placeActivityTrendData = null;
                this.globalPeopleTrendData = null;
                this.placePeopleTrendData = null;
                this.heatmapActivityDataTable = null;
                this.heatmapPeopleDataTable = null;
            };

            FacultyAndStaff.prototype.customPlaceClick = function (event, item, officialName) {
                this.selectedPlace(officialName);
            };

            // --------------------------------------------------------------------------------
            /*
            */
            // --------------------------------------------------------------------------------
            FacultyAndStaff.prototype.activitySearch = function () {
                var _this = this;
                var deferred = $.Deferred();

                var locationIds = new Array();
                for (var i = 0; i < this.locations().length; i += 1) {
                    locationIds.push(this.locations()[i].placeId());
                }

                var activityTypeIds = new Array();
                for (var i = 0; i < this.activityTypes().length; i += 1) {
                    if (this.activityTypes()[i].checked()) {
                        activityTypeIds.push(this.activityTypes()[i].id());
                    }
                }

                var degrees = null;
                if ((this.degrees() != null) && (this.degrees().length > 0)) {
                    degrees = this.degrees().split(',');
                }

                var tags = null;
                if ((this.tags() != null) && (this.tags().length > 0)) {
                    tags = this.tags().split(',');
                }

                var fromDate = null;
                if (this.fromDate() != null) {
                    fromDate = this.fromDate().toString();
                }

                var toDate = null;
                if (this.toDate() != null) {
                    toDate = this.toDate().toString();
                }

                var dataItem = $("#" + this.campusDropListId).data("kendoDropDownList").dataItem();
                var campusId = ((dataItem != null) && (dataItem.id != 0)) ? dataItem.id : null;
                dataItem = $("#" + this.collegeDropListId).data("kendoDropDownList").dataItem();
                var collegeId = ((dataItem != null) && (dataItem.id != 0)) ? dataItem.id : null;
                dataItem = $("#" + this.departmentDropListId).data("kendoDropDownList").dataItem();
                var departmentId = ((dataItem != null) && (dataItem.id != 0)) ? dataItem.id : null;

                var filterOptions = {
                    establishmentId: this.establishmentId(),
                    filterType: this.searchType(),
                    locationIds: locationIds,
                    activityTypes: activityTypeIds,
                    degrees: degrees,
                    tags: tags,
                    fromDate: fromDate,
                    toDate: toDate,
                    noUndated: !this.includeUndated(),
                    campusId: campusId,
                    collegeId: collegeId,
                    departmentId: departmentId
                };

                $.ajax({
                    type: "POST",
                    data: ko.toJSON(filterOptions),
                    contentType: 'application/json',
                    dataType: 'json',
                    url: App.Routes.WebApi.FacultyStaff.postSearch(),
                    success: function (data, textStatus, jqXhr) {
                        ko.mapping.fromJS(data, {}, _this.activityResults);
                        deferred.resolve(data);
                    },
                    error: function (jqXhr, textStatus, errorThrown) {
                        deferred.reject(errorThrown);
                    }
                });

                return deferred;
            };

            FacultyAndStaff.prototype.selectLens = function (lens) {
                if (lens === 'map') {
                    this.lens('map');
                } else {
                    this.lens('table');
                }
            };

            FacultyAndStaff.prototype.getActivityTypeIconName = function (typeId) {
                var i = 0;
                while ((i < this.activityTypes().length) && (this.activityTypes()[i].id() != typeId)) {
                    i += 1;
                }
                return (i < this.activityTypes().length) ? this.activityTypes()[i].iconName() : null;
            };

            FacultyAndStaff.prototype.getActivityTypeToolTip = function (typeId) {
                var i = 0;
                while ((i < this.activityTypes().length) && (this.activityTypes()[i].id() != typeId)) {
                    i += 1;
                }
                return (i < this.activityTypes().length) ? this.activityTypes()[i].type() : null;
            };

            FacultyAndStaff.prototype.drawPointmap = function (updateMarkers) {
                var _this = this;
                this.loadSpinner.start();
                if (this.searchType() === 'activities') {
                    this.getPointmapActivityMarkers(updateMarkers).done(function () {
                        _this.hidePointmapPeopleMarkers();
                        _this.showPointmapActivityMarkers();
                    }).always(function () {
                        _this.loadSpinner.stop();
                    });
                } else {
                    this.getPointmapPeopleMarkers(updateMarkers).done(function () {
                        _this.hidePointmapActivityMarkers();
                        _this.showPointmapPeopleMarkers();
                    }).always(function () {
                        _this.loadSpinner.stop();
                    });
                }
            };
            return FacultyAndStaff;
        })();
        Employees.FacultyAndStaff = FacultyAndStaff;
    })(ViewModels.Employees || (ViewModels.Employees = {}));
    var Employees = ViewModels.Employees;
})(ViewModels || (ViewModels = {}));
