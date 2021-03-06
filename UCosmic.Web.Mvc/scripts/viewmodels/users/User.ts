module ViewModels.Users {

    //class UserNameValidator implements KnockoutValidationAsyncRuleDefinition {
    //    private _ruleName: string = 'validUserName';
    //    private _isAwaitingResponse: boolean = false;
    //    async: boolean = true;
    //    message: string = 'error';
    //    validator(val: string, vm: User, callback: KnockoutValidationAsyncCallback) {
    //        if (!this._isAwaitingResponse) {
    //            var route = App.Routes.WebApi.Identity.Users.validateName(vm.id());
    //            this._isAwaitingResponse = true;
    //            $.post(route, { name: vm.name() })
    //            .always((): void => {
    //                this._isAwaitingResponse = false;
    //            })
    //            .done((): void => {
    //                callback(true);
    //            })
    //            .fail((xhr: JQueryXHR): void => {
    //                callback({ isValid: false, message: xhr.responseText });
    //            });
    //        }
    //    }
    //    constructor() {
    //        ko.validation.rules[this._ruleName] = this;
    //        ko.validation.addExtender(this._ruleName);
    //    }
    //}
    //new UserNameValidator();

    export class User implements KnockoutValidationGroup {

        id = ko.observable<number>();
        name = ko.observable<string>();

        saveSpinner = new App.Spinner({ delay: 200, });
        errorMessage = ko.observable<string>();

        isValid: () => boolean;
        errors: KnockoutValidationErrors;
        //isValidating: KnockoutComputed<boolean>;

        isWarned = ko.observable<boolean>(sessionStorage.getItem('UserCreateFormIsWarned') || false);

        acceptWarning(): void {
            this.isWarned(true);
            sessionStorage.setItem('UserCreateFormIsWarned', this.isWarned().toString());
        }

        constructor() {

            this.name.extend({
                required: {
                    message: 'Username is required.'
                },
                maxLength: 256,
                validUserName: this
            });

            //this.isValidating = ko.computed((): boolean => {
            //    return this.name.isValidating();
            //});

            ko.validation.group(this);
        }

        save(): boolean {

            if (!this.isValid()) { // validate
                this.errors.showAllMessages();
                return false;
            }

            this.saveSpinner.start();

            $.ajax({
                type: "POST",
                async: false,
                url: App.Routes.WebApi.Identity.Users.validateName(this.id()),
                data: { name: this.name() },
                success: (data: any, textStatus: string, jqXhr: JQueryXHR): void =>
                { this.errorMessage(null); },
                error: (jqXhr: JQueryXHR, textStatus: string, errorThrown: string): void =>
                { this.errorMessage(jqXhr.responseText); },
            });

            if (this.errorMessage() != null) {
                this.saveSpinner.stop();
                return;
            }

            //if (this.isValidating()) {
            //    setTimeout((): boolean => { this.save(); }, 50);
            //    return false;
            //}

            //var url = App.Routes.WebApi.Identity.Users.post();
            //var data = {
            //    name: this.name()
            //};

            //$.post(url, data)
            //.done((response: string, statusText: string, xhr: JQueryXHR): void => {
            //    // redirect to search
            //    window.location.href = App.Routes.Mvc.Identity.Users.created({ location: xhr.getResponseHeader('Location') });
            //})
            //.fail((xhr: JQueryXHR, statusText: string, errorThrown: string): void => {
            //    this.errorMessage('An unexpected error occurred while trying to create this user.');
            //});

            $.ajax({
                type: "POST",
                async: true,
                url: App.Routes.WebApi.Identity.Users.post(),
                data: { name: this.name() },
                success: (data: any, textStatus: string, jqXhr: JQueryXHR): void => {
                    window.location.href = App.Routes.Mvc.Identity.Users.created({ location: jqXhr.getResponseHeader('Location') });
                },
                error: (jqXhr: JQueryXHR, textStatus: string, errorThrown: string): void => {
                    this.errorMessage('An unexpected error occurred while trying to create this user.');
                },
                complete: (jqXhr: JQueryXHR, textStatus: string): void => {
                    this.saveSpinner.stop();
                }
            });
        }
    }
}