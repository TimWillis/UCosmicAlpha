module ViewModels.InternationalAffiliations {

    export class InternationalAffiliation {
        /* True if any field changes. */
        dirtyFlag: KnockoutObservable<boolean> = ko.observable( false );

        /* Locations for multiselect. */
        locationSelectorId: string;
        initialLocations: any[] = [];        // Bug - To overcome bug in Multiselect.
        selectedLocationValues: any[] = [];

        /* IObservableDegree implemented */
        id: KnockoutObservable<number>;           // if 0, new expertise
        version: KnockoutObservable<string>;      // byte[] converted to base64
        personId: KnockoutObservable<number>;
        from: KnockoutObservable<number>;
        to: KnockoutObservable<any>;              // nullable
        onGoing: KnockoutObservable<boolean>;
        institution: KnockoutObservable<string>;
        position: KnockoutObservable<string>;
        locations: KnockoutObservableArray<any>;
        whenLastUpdated: KnockoutObservable<string>;
        whoLastUpdated: KnockoutObservable<string>;

        errors: KnockoutValidationErrors;
        isValid: () => boolean;
        isAnyMessageShown: () => boolean;

        years: number[];

        _initialize( affiliationId: number ): void {

            var fromToYearRange: number = 80;
            var thisYear: number = Number(moment().format('YYYY'));
            this.years = new Array();
            for (var i: number = 0; i < fromToYearRange; i += 1)
            {
                this.years[i] = thisYear - i;
            }

            this.id = ko.observable(affiliationId);
        }

        setupWidgets( locationSelectorId: string ): void {
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
            $( "#" + locationSelectorId ).kendoMultiSelect( {
                autoBind: true,
                dataTextField: "officialName",
                dataValueField: "id",
                minLength: 3,
                dataSource: me.initialLocations,
                value: me.selectedLocationValues,
                change: ( event: any ) => {
                    this.updateLocations( event.sender.dataItems() );
                },
                placeholder: "[Select Country/Location]"
            } );


            $("#fromDate").kendoDropDownList({
            dataSource: this.years,
            value: me.from().toString(),
            optionLabel: " ",
            change: function (e) {
                var toDateDropList = $("#toDate").data("kendoDropDownList");
                if (toDateDropList.value() < this.value()) {
                    toDateDropList.value( this.value() );
                }
                me.from( this.value() );
            }
            });

            $("#toDate").kendoDropDownList({
            dataSource: this.years,
            value: me.to(),
            optionLabel: " ",
            change: function (e) {
                var fromDateDropList = $("#fromDate").data("kendoDropDownList");
                if (fromDateDropList.value() > this.value()) {
                    fromDateDropList.value( this.value() );
                }
                me.to( this.value() );
            }
            });
        }

        setupValidation(): void {
            ko.validation.rules['atLeast'] = {
                validator: ( val: any, otherVal: any ): boolean => {
                    return val.length >= otherVal;
                },
                message: 'At least {0} must be selected.'
            };

            ko.validation.registerExtenders();
            
            this.locations.extend( { atLeast: 1 } );
            this.institution.extend( { required: true, maxLength: 200 } );
            this.position.extend({ required: true, maxLength: 100 });
            this.from.extend({ required: true });

            ko.validation.group( this );
        }

        setupSubscriptions(): void {
            this.from.subscribe((newValue: any): void => { this.dirtyFlag(true); });
            this.to.subscribe((newValue: any): void => { this.dirtyFlag(true); });
            this.onGoing.subscribe((newValue: any): void => { this.dirtyFlag(true); });
            this.institution.subscribe((newValue: any): void => { this.dirtyFlag(true); });
            this.position.subscribe((newValue: any): void => { this.dirtyFlag(true); });
        }

        constructor( affiliationId: number ) {
            this._initialize( affiliationId );
        }

        load(): JQueryPromise<any> {
            var me = this;
            var deferred: JQueryDeferred<void> = $.Deferred();

            if ( this.id() == 0 ) {
                this.version = ko.observable( null );
                this.personId = ko.observable( 0 );
                this.from = ko.observable( 0 );
                this.to = ko.observable( 0 );
                this.onGoing = ko.observable( false );
                this.institution = ko.observable( null );
                this.position = ko.observable( null );
                this.locations = ko.observableArray();
                this.whenLastUpdated = ko.observable( null );
                this.whoLastUpdated = ko.observable( null );

                deferred.resolve();
            }
            else {
                var dataPact = $.Deferred();

                $.ajax( {
                    type: "GET",
                    url: App.Routes.WebApi.InternationalAffiliations.get( this.id() ),
                    success: function ( data: any, textStatus: string, jqXhr: JQueryXHR ): void
                    { dataPact.resolve( data ); },
                    error: function ( jqXhr: JQueryXHR, textStatus: string, errorThrown: string ): void
                    { dataPact.reject( jqXhr, textStatus, errorThrown ); },
                } );

                // only process after all requests have been resolved
                $.when( dataPact )
                              .done( ( data: any ): void => {

                                  ko.mapping.fromJS( data, {}, this );

                                  /* Initialize the list of selected locations with current locations in values. */
                                  for ( var i = 0; i < this.locations().length; i += 1 ) {

                                      this.initialLocations.push( {
                                          officialName: this.locations()[i].placeOfficialName(),
                                          id: this.locations()[i].placeId()
                                      } );

                                      this.selectedLocationValues.push( this.locations()[i].placeId() );
                                  }

                                  deferred.resolve();
                              } )
                              .fail( ( xhr: JQueryXHR, textStatus: string, errorThrown: string ): void => {
                                  deferred.reject( xhr, textStatus, errorThrown );
                              } );
            }

            return deferred;
        }

        save( viewModel: any, event: any ): void {

            if (!this.isValid()) {
                // TBD - need dialog here. 
                this.errors.showAllMessages();
                return;
            }

            var mapSource = {
                id : this.id,
                version : this.version,
                personId : this.personId,
                whenLastUpdated : this.whenLastUpdated,
                whoLastUpdated : this.whoLastUpdated,
                from: this.from,
                to: this.to,
                onGoing: this.onGoing,
                institution: this.institution,
                position: this.position,
                locations : ko.observableArray()
            };

            for(var i = 0; i < this.locations().length; i += 1) {
                mapSource.locations.push({
                    id : this.locations()[i].id,
                    version : this.locations()[i].version,
                    whenLastUpdated : this.locations()[i].whenLastUpdated,
                    whoLastUpdated : this.locations()[i].whoLastUpdated,
                    affiliationId : this.locations()[i].affiliationId,
                    placeOfficialName : this.locations()[i].placeOfficialName,
                    placeId : this.locations()[i].placeId
                });
            }
               
            var model = ko.mapping.toJS(mapSource);

            var url = (viewModel.id() == 0) ?
                        App.Routes.WebApi.InternationalAffiliations.post() :
                        App.Routes.WebApi.InternationalAffiliations.put( viewModel.id() );
            var type = (viewModel.id() == 0) ?  "POST" : "PUT";

            $.ajax( {
                type: type,
                async: false,
                url: url,
                data: model,
                success: ( data: any, textStatus: string, jqXhr: JQueryXHR ): void => {
                },
                error: ( jqXhr: JQueryXHR, textStatus: string, errorThrown: string ): void => {
                    alert( textStatus + " | " + errorThrown );
                },
                complete: ( jqXhr: JQueryXHR, textStatus: string ): void => {
                    location.href = App.Routes.Mvc.My.Profile.get( "international-affiliation" );
                }
            } );
        }

        cancel( item: any, event: any, mode: string ): void {
            if ( this.dirtyFlag() == true ) {
                $( "#cancelConfirmDialog" ).dialog( {
                    modal: true,
                    resizable: false,
                    width: 450,
                    buttons: {
                        "Do not cancel": function () {
                            $( this ).dialog( "close" );
                        },
                        "Cancel and lose changes": function () {
                            $( this ).dialog( "close" );
                            location.href = App.Routes.Mvc.My.Profile.get( "international-affiliation" );
                        }
                    }
                } );
            }
            else { 
                location.href = App.Routes.Mvc.My.Profile.get( "international-affiliation" );
            }
        }

        updateLocations( items: any[] ): void {
            this.locations.removeAll();
            for ( var i = 0; i < items.length; i += 1 ) {
                var location = ko.mapping.fromJS( { id: 0, placeId: items[i].id, version: "" } );
                this.locations.push( location );
            }
            this.dirtyFlag(true);
        }
    }
}
