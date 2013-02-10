/// <reference path="../../jquery/jquery-1.8.d.ts" />
/// <reference path="../../jquery/jqueryui-1.9.d.ts" />
/// <reference path="../../ko/knockout-2.2.d.ts" />
/// <reference path="../../ko/knockout.mapping-2.0.d.ts" />
/// <reference path="../../ko/knockout.extensions.d.ts" />
/// <reference path="../../kendo/kendouiweb.d.ts" />
/// <reference path="../../datacontext/iemployee.ts" />
/// <reference path="../../app/Routes.ts" />

module ViewModels.My {

    export class Profile {

        private _isInitialized: bool = false;

        private _revisionId: number;
        private _employeeId: number;

        isDisplayNameDerived: KnockoutObservableBool = ko.observable();
        displayName: KnockoutObservableString = ko.observable();
        private _userDisplayName: string = '';

        salutation: KnockoutObservableString = ko.observable();
        firstName: KnockoutObservableString = ko.observable();
        middleName: KnockoutObservableString = ko.observable();
        lastName: KnockoutObservableString = ko.observable();
        suffix: KnockoutObservableString = ko.observable();

        facultyRanks: KnockoutObservableArray = ko.observableArray();
        facultyRankId: KnockoutObservableNumber = ko.observable();
        jobTitles: KnockoutObservableString = ko.observable();
        administrativeAppointments: KnockoutObservableString = ko.observable();

        gender: KnockoutObservableString = ko.observable();
        isActive: KnockoutObservableBool = ko.observable();

        $photo: KnockoutObservableJQuery = ko.observable();
        $facultyRanks: KnockoutObservableJQuery = ko.observable();
        $nameSalutation: KnockoutObservableJQuery = ko.observable();
        $nameSuffix: KnockoutObservableJQuery = ko.observable();

        constructor() {
            this._initialize();

            this._setupKendoWidgets();
            this._setupDisplayNameDerivation();
        }

        private _initialize() {

            // start both requests at the same time
            var facultyRanksPact = $.Deferred();
            $.get(App.Routes.WebApi.Employees.ModuleSettings.FacultyRanks.get())
                .done((data: any, textStatus: string, jqXHR: JQueryXHR): void => {
                    facultyRanksPact.resolve(data);
                })
                .fail((jqXHR: JQueryXHR, textStatus: string, errorThrown: string): void => {
                    facultyRanksPact.reject(jqXHR, textStatus, errorThrown);
                });

            var viewModelPact = $.Deferred();
            $.get(App.Routes.WebApi.My.Profile.get())
                .done((data: any, textStatus: string, jqXHR: JQueryXHR): void => {
                    viewModelPact.resolve(data);
                })
                .fail((jqXHR: JQueryXHR, textStatus: string, errorThrown: string): void => {
                    viewModelPact.reject(jqXHR, textStatus, errorThrown);
                });

            // only process after both requests have been resolved
            $.when(facultyRanksPact, viewModelPact).then(

                // all requests succeeded
                (facultyRanks: any, viewModel: any): void => {

                    this.facultyRanks(facultyRanks); // populate the faculty ranks menu

                    this._revisionId = viewModel.revisionId; // not an observable
                    this._employeeId = viewModel.employeeId; // not an observable
                    var viewModelMapping = { // options for viewmodel ko.mapping
                        ignore: ['revisionId', 'employeeId'] // do not map these to observables
                    };
                    ko.mapping.fromJS(viewModel, viewModelMapping, this); // populate the scalars

                    $(this).trigger('ready'); // ready to apply bindings
                    this._isInitialized = true; // bindings have been applied
                    this.$facultyRanks().kendoDropDownList(); // kendoui dropdown for faculty ranks
                },

                // one of the responses failed (never called more than once, even on multifailures)
                (xhr: JQueryXHR, textStatus: string, errorThrown: string): void => {
                    //alert('a GET API call failed :(');
                });
        }

        saveInfo(formElement: HTMLFormElement): void {
            var apiModel = ko.mapping.toJS(this);
            apiModel.revisionId = this._revisionId;
            apiModel.employeeId = this._employeeId;

            $.ajax({
                url: App.Routes.WebApi.My.Profile.put(),
                type: 'PUT',
                data: apiModel
            })
            .done(() => {
                // flash feedback message
            })
            .fail(() => {
                //alert('a PUT API call failed :(');
            });

            $("#accordion").accordion('activate', 1);	// Open next panel
        }

        savePicture(formElement: HTMLFormElement): void {
            $("#accordion").accordion('activate', 0);	// Open next panel
        }

        // comboboxes for salutation & suffix
        private _setupKendoWidgets(): void {
            // when the $element observables are bound, they will have length
            // use this opportinity to apply kendo extensions
            this.$nameSalutation.subscribe((newValue: JQuery): void => {
                if (newValue && newValue.length)
                    newValue.kendoComboBox({
                        dataTextField: "text",
                        dataValueField: "value",
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: App.Routes.WebApi.People.Names.Salutations.get()
                                }
                            }
                        })
                    });
            });
            this.$nameSuffix.subscribe((newValue: JQuery): void => {
                if (newValue && newValue.length)
                    newValue.kendoComboBox({
                        dataTextField: "text",
                        dataValueField: "value",
                        dataSource: new kendo.data.DataSource({
                            transport: {
                                read: {
                                    url: App.Routes.WebApi.People.Names.Suffixes.get()
                                }
                            }
                        })
                    });
            });

            this.$photo.subscribe((newValue: JQuery): void => {
                if (newValue && newValue.length) {
                    newValue.kendoUpload({
                        multiple: false,
                        localization: {
                            select: 'Choose a photo to upload...'
                        }
                        //async: {
                        //    saveUrl: 'saveit',
                        //    removeUrl: 'removeit'
                        //}
                    });
                }
            });
        }

        // logic to derive display name
        private _setupDisplayNameDerivation(): void {
            this.displayName.subscribe((newValue: string): void => {
                if (!this.isDisplayNameDerived()) {
                    // stash user-entered display name only when it is not derived
                    this._userDisplayName = newValue;
                }
            });

            ko.computed((): void => {
                // generate display name if it has been API-initialized
                if (this.isDisplayNameDerived() && this._isInitialized) {
                    var data = ko.mapping.toJS(this);
                    $.ajax({
                        url: App.Routes.WebApi.People.Names.DeriveDisplayName.get(),
                        type: 'GET',
                        cache: false,
                        data: data
                    }).done((result: string): void => {
                        this.displayName(result);
                    });
                }
                else if (this._isInitialized) {
                    // prevent user display name from being blank
                    if (!this._userDisplayName)
                        this._userDisplayName = this.displayName();
                    // restore user-entered display name
                    this.displayName(this._userDisplayName);
                }
            }).extend({ throttle: 400 }); // wait for observables to stop changing
        }
    }

}

