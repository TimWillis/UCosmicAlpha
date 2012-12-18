/// <reference path="../lib-ext.d.ts" />
/// <reference path="../jquery/jquery-1.8.d.ts" />
/// <reference path="../ko/knockout-2.2.d.ts" />

module ViewModels {
    export class PagedSearch {

        // paging observables
        pageSize: KnockoutObservableNumber = ko.observable();
        pageNumber: KnockoutObservableNumber = ko.observable();
        transitionedPageNumber: KnockoutObservableNumber = ko.observable();
        itemTotal: KnockoutObservableNumber = ko.observable();
        nextForceDisabled: KnockoutObservableBool = ko.observable(false);
        prevForceDisabled: KnockoutObservableBool = ko.observable(false);

        // paging computeds
        pageCount: KnockoutComputed;
        pageIndex: KnockoutComputed;
        firstIndex: KnockoutComputed;
        firstNumber: KnockoutComputed;
        lastNumber: KnockoutComputed;
        lastIndex: KnockoutComputed;
        nextEnabled: KnockoutComputed;
        prevEnabled: KnockoutComputed;

        // paging methods
        nextPage(): void {
            if (this.nextEnabled()) {
                var pageNumber = parseInt(this.pageNumber()) + 1;
                this.pageNumber(pageNumber);
            }
        }
        prevPage(): void {
            if (this.prevEnabled())
                history.back();
        }

        // results
        items: KnockoutObservableArray = ko.observableArray();
        hasItems: KnockoutComputed;
        hasNoItems: KnockoutComputed;
        hasManyItems: KnockoutComputed;
        hasManyPages: KnockoutComputed;
        showStatus: KnockoutComputed;

        // filtering
        orderBy: KnockoutObservableString = ko.observable();
        keyword: KnockoutObservableString =
            ko.observable($('input[type=hidden][data-bind="value: keyword"]').val());
        throttledKeyword: KnockoutComputed;

        // spinner observables
        spinnerDelay: number = 400;
        isSpinning: KnockoutObservableBool = ko.observable(true);
        showSpinner: KnockoutObservableBool = ko.observable(false);
        inTransition: KnockoutObservableBool = ko.observable(false);

        // spinner methods
        startSpinning(): void {
            this.isSpinning(true); // we are entering an ajax call
            if (this.spinnerDelay < 1)
                this.showSpinner(true);
            else
                setTimeout((): void => {
                    // only show spinner when load is still being processed
                    if (this.isSpinning() && !this.inTransition())
                        this.showSpinner(true);
                }, this.spinnerDelay);
        }
        stopSpinning(): void {
            this.inTransition(false);
            this.showSpinner(false);
            this.isSpinning(false);
        }

        constructor () {

            // paging computeds
            this.pageCount = ko.computed((): number => {
                return Math.ceil(this.itemTotal() / this.pageSize());
            });
            this.pageIndex = ko.computed((): number =>  {
                return parseInt(this.transitionedPageNumber()) - 1;
            });
            this.firstIndex = ko.computed((): number => {
                return this.pageIndex() * this.pageSize();
            });
            this.firstNumber = ko.computed((): number => {
                return this.firstIndex() + 1;
            });
            this.lastNumber = ko.computed((): number => {
                if (!this.items) return 0;
                return this.firstIndex() + this.items().length;
            });
            this.lastIndex = ko.computed((): number => {
                return this.lastNumber() - 1;
            });
            this.nextEnabled = ko.computed((): bool => {
                return this.pageNumber() < this.pageCount() && !this.nextForceDisabled();
            });
            this.prevEnabled = ko.computed((): bool => {
                return this.pageNumber() > 1 && !this.prevForceDisabled();
            });

            // paging subscriptions
            this.pageCount.subscribe((newValue: number) => {
                if (this.pageNumber() && this.pageNumber() > newValue)
                    this.pageNumber(1);
            });

            // result computeds
            this.hasItems = ko.computed((): bool => {
                return this.items() && this.items().length > 0;
            });
            this.hasManyItems = ko.computed((): bool => {
                return this.lastNumber() > this.firstNumber();
            });
            this.hasNoItems = ko.computed((): bool => {
                return !this.isSpinning() && !this.hasItems();
            });
            this.hasManyPages = ko.computed((): bool => {
                return this.pageCount() > 1;
            });
            this.showStatus = ko.computed((): bool => {
                return this.hasItems() && !this.showSpinner();
            });

            // filtering computeds
            this.throttledKeyword = ko.computed(this.keyword)
                .extend({ throttle: 400 });
        }

    }
}