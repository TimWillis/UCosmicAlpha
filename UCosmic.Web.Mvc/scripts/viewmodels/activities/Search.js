var Activities;
(function (Activities) {
    (function (ViewModels) {
        (function (DataGraphPivot) {
            DataGraphPivot[DataGraphPivot["activities"] = 1] = "activities";
            DataGraphPivot[DataGraphPivot["people"] = 2] = "people";
        })(ViewModels.DataGraphPivot || (ViewModels.DataGraphPivot = {}));
        var DataGraphPivot = ViewModels.DataGraphPivot;

        var Search = (function () {
            function Search(settings) {
                var _this = this;
                this.settings = settings;
                this.countryOptions = ko.observableArray(this.settings.countryOptions);
                this.countryCode = ko.observable(this.settings.input.countryCode);
                this.orderBy = ko.observable(this.settings.input.orderBy);
                this.keyword = ko.observable(this.settings.input.keyword);
                this.pager = new App.Pager(this.settings.input.pageNumber.toString(), this.settings.input.pageSize.toString());
                this.pivot = ko.observable(this.settings.input.pivot);
                this.isActivitiesChecked = ko.computed(function () {
                    return _this.pivot() != 2 /* people */;
                });
                this.isPeopleChecked = ko.computed(function () {
                    return _this.pivot() == 2 /* people */;
                });
                this.loadingSpinner = new App.Spinner();
                this.pager.apply(this.settings.output);
            }
            Search.prototype.applyBindings = function (element) {
                ko.applyBindings(this, element);

                this._applyKendo();
                this._applySubscriptions();
            };

            Search.prototype._applyKendo = function () {
                var _this = this;
                var inputInitialized = false;
                var emptyDataItem = {
                    officialName: '[Begin typing to see options]',
                    placeId: undefined
                };
                var emptyDataSource = new kendo.data.DataSource({ data: [emptyDataItem] });
                var serverDataSource = new kendo.data.DataSource({
                    serverFiltering: true,
                    transport: {
                        read: {
                            url: '/api/places/names/autocomplete'
                        },
                        parameterMap: function (data, action) {
                            if (action == 'read' && data && data.filter && data.filter.filters && data.filter.filters.length) {
                                return {
                                    terms: data.filter.filters[0].value
                                };
                            }
                            return data;
                        }
                    }
                });
                var hasPlace = (this.settings.input.placeIds && this.settings.input.placeIds.length && this.settings.input.placeNames && this.settings.input.placeNames.length && this.settings.input.placeIds[0] && this.settings.input.placeNames[0]) ? true : false;
                var dataSource = hasPlace ? 'server' : 'empty';
                var checkDataSource = function (widget) {
                    var inputVal = $.trim(widget.input.val());
                    if (!inputVal && dataSource == 'empty')
                        return;
                    if (inputVal && dataSource == 'server')
                        return;
                    if (!inputVal && dataSource != 'empty') {
                        dataSource = 'empty';
                        widget.setDataSource(emptyDataSource);
                        return;
                    }
                    if (inputVal && dataSource != 'server') {
                        dataSource = 'server';
                        widget.setDataSource(serverDataSource);
                        return;
                    }
                };
                this.$location.kendoComboBox({
                    animation: false,
                    dataTextField: 'officialName',
                    dataValueField: 'placeId',
                    filter: 'contains',
                    dataSource: hasPlace ? serverDataSource : emptyDataSource,
                    select: function (e) {
                        if (e.item.text() == emptyDataItem.officialName) {
                            e.sender.input.val('');
                            e.sender.search('');
                            return;
                        }

                        setTimeout(function () {
                            if (!_this.settings.input.placeIds || !_this.settings.input.placeIds.length || _this.settings.input.placeIds[0] != parseInt(e.sender.value())) {
                                _this._submitForm();
                            }
                        }, 0);
                    },
                    dataBound: function (e) {
                        var widget = e.sender;
                        var input = widget.input;
                        var inputVal = $.trim(input.val());

                        if (!inputInitialized) {
                            input.attr('name', 'placeNames');
                            input.on('keydown', function () {
                                setTimeout(function () {
                                    checkDataSource(widget);
                                }, 0);
                            });
                            if (hasPlace && inputVal) {
                                widget.search(inputVal);
                            }
                            inputInitialized = true;
                        } else if (hasPlace) {
                            widget.select(function (dataItem) {
                                return dataItem.placeId == this.settings.input.placeIds[0];
                            });
                            widget.close();
                            input.blur();
                            hasPlace = false;
                            setTimeout(function () {
                                _this.$location.val(_this.settings.input.placeIds[0]);
                            }, 0);
                        }
                    }
                });
            };

            Search.prototype._applySubscriptions = function () {
                var _this = this;
                this.pager.input.pageSizeText.subscribe(function (newValue) {
                    _this._submitForm();
                });
                this.pager.input.pageNumberText.subscribe(function (newValue) {
                    _this._submitForm();
                });
                this.countryCode.subscribe(function (newValue) {
                    _this._submitForm();
                });
                this.orderBy.subscribe(function (newValue) {
                    _this._submitForm();
                });
            };

            Search.prototype._submitForm = function () {
                this.loadingSpinner.start();
                this.$form.submit();
            };

            Search.prototype.onKeywordInputSearchEvent = function (viewModel, e) {
                if ($.trim(this.keyword()) && !$.trim($(e.target).val()) && this.$form)
                    this.$form.submit();
            };
            return Search;
        })();
        ViewModels.Search = Search;
    })(Activities.ViewModels || (Activities.ViewModels = {}));
    var ViewModels = Activities.ViewModels;
})(Activities || (Activities = {}));
