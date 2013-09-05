/// <reference path="../../typings/knockout/knockout.d.ts" />
/// <reference path="../../typings/knockout.mapping/knockout.mapping.d.ts" />
/// <reference path="../../typings/jquery/jquery.d.ts" />
/// <reference path="../../app/Routes.ts" />
/// <reference path="../../typings/kendo/kendo.all.d.ts" />
var agreements;
(function (agreements) {
    var phones = (function () {
        function phones(agreementId, establishmentItemViewModel, contactId) {
            this.selectConstructor = function (name, id) {
                this.name = name;
                this.id = id;
            };
            //phone vars
            this.contactPhoneTextValue = ko.observable("");
            this.contactPhoneType = ko.observable();
            this.contactPhones = ko.observableArray();
            this.phoneTypes = ko.mapping.fromJS([]);
            this.$phoneTypes = ko.observable();
            this.agreementId = agreementId;
            this.contactId = contactId;
            this.establishmentItemViewModel = establishmentItemViewModel;

            this.removePhone = this.removePhone.bind(this);
            this.addPhone = this.addPhone.bind(this);

            this.phoneTypes = ko.mapping.fromJS([
                new this.selectConstructor("[None]", ""),
                new this.selectConstructor("home", "home"),
                new this.selectConstructor("work", "work"),
                new this.selectConstructor("mobile", "mobile")
            ]);
            this.bindJquery();
        }
        phones.prototype.removePhone = function (me, e) {
            var _this = this;
            var url = App.Routes.WebApi.Agreements.Contacts.Phones.del(this.agreementId.val, me.contactId, me.id);
            $.ajax({
                url: url,
                type: 'DELETE',
                success: function () {
                    //this.files.remove(me);
                    _this.contactPhones.remove(me);
                    $("body").css("min-height", ($(window).height() + $("body").height() - ($(window).height() * 1.1)));
                }
            });
            this.contactPhones.remove(me);
            e.preventDefault();
            e.stopPropagation();
        };

        phones.prototype.addPhone = function (me, e) {
            if (this.contactPhoneTextValue().length > 0) {
                //var context = ko.contextFor($("#contactPhoneTextValue")[0])
                this.contactPhones.push({ type: '', contactId: '', value: this.contactPhoneTextValue() });
                this.contactPhoneTextValue("");
                $(".phoneTypes").kendoDropDownList({
                    dataTextField: "name",
                    dataValueField: "id",
                    dataSource: new kendo.data.DataSource({
                        data: ko.mapping.toJS(this.phoneTypes())
                    })
                });
            }
        };

        phones.prototype.bindJquery = function () {
            var _this = this;
            var self = this;

            this.contactPhoneTextValue.subscribe(function (me) {
                if (_this.contactPhoneTextValue().length > 0) {
                    if (_this.contactId()) {
                        var url = App.Routes.WebApi.Agreements.Contacts.Phones.post(_this.agreementId.val, _this.contactId());
                        var data = { id: "0", type: '', contactId: _this.contactId(), value: _this.contactPhoneTextValue() };
                        $.post(url, data).done(function (response, statusText, xhr) {
                            var myUrl = xhr.getResponseHeader('Location');
                            data.id = myUrl.substring(myUrl.lastIndexOf("/") + 1);
                            _this.contactPhones.push(data);
                            _this.contactPhoneTextValue("");

                            $(".phoneTypes").kendoDropDownList({
                                dataTextField: "name",
                                dataValueField: "id",
                                dataSource: new kendo.data.DataSource({
                                    data: ko.mapping.toJS(_this.phoneTypes())
                                })
                            });
                        }).fail(function (xhr, statusText, errorThrown) {
                            if (xhr.status === 400) {
                                _this.establishmentItemViewModel.$genericAlertDialog.find('p.content').html(xhr.responseText.replace('\n', '<br /><br />'));
                                _this.establishmentItemViewModel.$genericAlertDialog.dialog({
                                    title: 'Alert Message',
                                    dialogClass: 'jquery-ui',
                                    width: 'auto',
                                    resizable: false,
                                    modal: true,
                                    buttons: {
                                        'Ok': function () {
                                            _this.establishmentItemViewModel.$genericAlertDialog.dialog('close');
                                        }
                                    }
                                });
                            }
                        });
                    } else {
                        _this.contactPhones.push({ id: '', type: '', contactId: '', value: _this.contactPhoneTextValue() });
                        _this.contactPhoneTextValue("");

                        $(".phoneTypes").kendoDropDownList({
                            dataTextField: "name",
                            dataValueField: "id",
                            dataSource: new kendo.data.DataSource({
                                data: ko.mapping.toJS(_this.phoneTypes())
                            })
                        });
                    }
                }
            });
        };
        return phones;
    })();
    agreements.phones = phones;
})(agreements || (agreements = {}));
